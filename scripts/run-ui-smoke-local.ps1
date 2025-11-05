param(
  [string]$Url = "http://127.0.0.1:5123",
  [string]$Configuration = "Release",
  [switch]$IgnoreCertErrors,
  [switch]$TrustHttpsCert,
  [switch]$NoBuild
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path '.logs')) { New-Item -ItemType Directory -Path '.logs' -Force | Out-Null }
$log = '.logs/ui-smoke-tests.log'
Remove-Item $log -Force -ErrorAction SilentlyContinue | Out-Null

Write-Host "[ui-smoke] Starting app at $Url (config=$Configuration)"

# Start app (disables HTTPS redirect automatically for http://)
& scripts/start-app.ps1 -Url $Url -Configuration $Configuration -NoBuild:$NoBuild -TestMode -TrustHttpsCert:$TrustHttpsCert | Tee-Object -FilePath $log -Append | Out-Host
if ($LASTEXITCODE -ne 0) {
  Write-Error "App failed to start. See $log."
  exit 1
}

try {
  Write-Host "[ui-smoke] Running UI smoke tests against $Url"
  $env:RUN_UI_TESTS = '1'
  $env:BASE_URL = $Url
  if ($IgnoreCertErrors) { $env:IGNORE_CERT_ERRORS = '1' } else { $env:IGNORE_CERT_ERRORS = '0' }

  & scripts/run-tests.ps1 -Suite ui-smoke -BaseUrl $Url -Configuration $Configuration -IgnoreCertErrors:$IgnoreCertErrors  | Tee-Object -FilePath $log -Append | Out-Host
  $code = $LASTEXITCODE

  if ($code -ne 0) {
    Write-Error "[ui-smoke] Tests failed with exit code $code. See $log for details."
    exit $code
  }
  Write-Host "[ui-smoke] Tests passed."
}
finally {
  Write-Host "[ui-smoke] Stopping app..."
  & scripts/stop-app.ps1 -Force | Tee-Object -FilePath $log -Append | Out-Host
}

Write-Host "[ui-smoke] Done. Log: $log"
exit 0

