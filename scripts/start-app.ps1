param(
  [string]$Project = "SocksShoppingStore/SocksShoppingStore.csproj",
  [string]$Url = "http://127.0.0.1:5123",
  [string]$Configuration = "Release",
  [int]$TimeoutSec = 30,
  [string]$LogPath = ".logs/app-local.log",
  [switch]$NoBuild,
  [switch]$TestMode = $true,  # set env for stable local tests and use dotnet run
  [switch]$DisableHttpsRedirect,
  [switch]$TrustHttpsCert
)

Write-Host "Starting app: $Project -> $Url (config=$Configuration)"

if (-not (Test-Path (Split-Path -Parent $LogPath))) {
  New-Item -ItemType Directory -Path (Split-Path -Parent $LogPath) -Force | Out-Null
}

if (-not $NoBuild) {
  Write-Host "Building project..."
  dotnet build $Project -c $Configuration
  if ($LASTEXITCODE -ne 0) { Write-Error "Build failed"; exit 1 }
}

Write-Host "Launching app in background (dotnet run)..."
if (-not (Test-Path (Split-Path -Parent $LogPath))) {
  New-Item -ItemType Directory -Path (Split-Path -Parent $LogPath) -Force | Out-Null
}
$stdOut = $LogPath
$stdErr = if ($LogPath.ToLower().EndsWith('.log')) { $LogPath.Substring(0, $LogPath.Length-4) + '.err.log' } else { "$LogPath.err" }

# Environment for stable local tests
if ($TestMode) {
  $env:ASPNETCORE_ENVIRONMENT = 'Development'
  $env:DOTNET_ENVIRONMENT = 'Development'
  $env:RateLimiting__GlobalPerMinute = '1000'
  $env:RateLimiting__ApiPerMinute = '1000'
  $env:FreeTier__Enabled = 'false'
}

# If requested (or implied by URL scheme), disable HTTPS redirect so health checks over HTTP work
try {
  $uri = [Uri]$Url
  if ($DisableHttpsRedirect -or $uri.Scheme -eq 'http') {
    $env:HttpsRedirect__Enabled = 'false'
  }
  if ($uri.Scheme -eq 'https' -and $TrustHttpsCert) {
    Write-Host "Trusting local HTTPS dev certificate (dotnet dev-certs https --trust)"
    dotnet dev-certs https --trust | Out-Host
  }
} catch {
  # ignore URL parse errors
}

$argsList = @('run', '-c', $Configuration, '--project', $Project, '--urls', $Url, '--no-launch-profile')
if ($NoBuild) { $argsList += '--no-build' }

$proc = Start-Process -FilePath "dotnet" -ArgumentList $argsList -RedirectStandardOutput $stdOut -RedirectStandardError $stdErr -NoNewWindow -PassThru
"$($proc.Id)" | Set-Content -Path ".logs/app.pid"

# health wait
$health = "$Url/healthz"
$up = $false
for ($i=0; $i -lt $TimeoutSec; $i++) {
  try {
    $resp = Invoke-WebRequest -Uri $health -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
    if ($resp.StatusCode -eq 200) { $up = $true; break }
  } catch { }
  Start-Sleep -Seconds 1
}

if (-not $up) {
  Write-Error "App failed to become healthy at $health"
  if (Test-Path $LogPath) {
    Write-Host "--- $LogPath (tail) ---"
    Get-Content -Path $LogPath -Tail 200
  }
  exit 1
}

Write-Host "App is UP at $Url (PID=$($proc.Id))"
exit 0

