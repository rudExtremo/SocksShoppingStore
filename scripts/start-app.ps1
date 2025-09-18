param(
  [string]$Project = "SocksShoppingStore/SocksShoppingStore.csproj",
  [string]$Url = "http://127.0.0.1:5123",
  [string]$Configuration = "Release",
  [int]$TimeoutSec = 30,
  [string]$LogPath = ".logs/app-local.log",
  [switch]$NoBuild
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

$argsList = @(
  'run','-c',$Configuration,
  '--project',$Project,
  '--urls',$Url,
  '--no-launch-profile'
)

Write-Host "Launching app in background..."
$proc = Start-Process -FilePath "dotnet" -ArgumentList $argsList -RedirectStandardOutput $LogPath -RedirectStandardError $LogPath -NoNewWindow -PassThru
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

