param(
  [string]$PidFile = ".logs/app.pid",
  [switch]$Force
)

if (Test-Path $PidFile) {
  try {
    $pid = Get-Content -Raw $PidFile | ForEach-Object { $_.Trim() } | Select-Object -First 1
    if ($pid -and ($pid -as [int])) {
      Write-Host "Stopping app PID=$pid"
      Stop-Process -Id $pid -Force:$Force -ErrorAction SilentlyContinue
      Remove-Item $PidFile -Force -ErrorAction SilentlyContinue
      Write-Host "Stopped."
      exit 0
    }
  } catch {}
}

# Fallback: scan running dotnet processes by command line
$stopped = $false
try {
  $procs = Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'"
  foreach ($p in $procs) {
    if ($p.CommandLine -and $p.CommandLine -match 'SocksShoppingStore.dll') {
      Write-Host "Stopping SocksShoppingStore.dll (PID=$($p.ProcessId))"
      Stop-Process -Id $p.ProcessId -Force:$Force -ErrorAction SilentlyContinue
      $stopped = $true
    }
  }
} catch {}

if (-not $stopped) { Write-Warning "Could not find running app process."; exit 1 }
Write-Host "Stopped by scan."
exit 0
