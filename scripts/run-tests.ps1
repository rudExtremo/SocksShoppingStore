param(
  [ValidateSet('dev-fast','main-full','ui-smoke','api-coverage','integration-coverage')]
  [string]$Suite = 'dev-fast',
  [string]$Project = 'SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj',
  [string]$Configuration = 'Release',
  [string]$BaseUrl = 'http://127.0.0.1:5123',
  [switch]$IgnoreCertErrors,
  [switch]$UseTestFactory,
  [string]$ResultsDir
)

Write-Host "Running test suite: $Suite"
$env:BASE_URL = $BaseUrl

if (-not $ResultsDir) { $ResultsDir = ".logs/TestResults/$Suite" }
if (-not (Test-Path $ResultsDir)) { New-Item -ItemType Directory -Path $ResultsDir -Force | Out-Null }
$suiteLog = ".logs/test-$Suite.log"
if (-not (Test-Path (Split-Path -Parent $suiteLog))) { New-Item -ItemType Directory -Path (Split-Path -Parent $suiteLog) -Force | Out-Null }

function Run-Cmd([string[]]$cmdArgs) {
  Write-Host "dotnet $($cmdArgs -join ' ')"
  # capture both stdout/stderr into suite log and keep exit code
  & dotnet @cmdArgs 2>&1 | Tee-Object -FilePath $suiteLog -Append | Out-Host
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

switch ($Suite) {
  'dev-fast' {
    $env:USE_TEST_FACTORY = '1'
    $env:RUN_UI_TESTS = '0'
    $cmd = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=Unit|TestCategory=Integration|TestCategory=API-Smoke', '--logger', 'trx;LogFileName=dev.trx', '--results-directory', $ResultsDir)
    Run-Cmd $cmd
  }
  'main-full' {
    $env:RUN_UI_TESTS = '1'
    $env:IGNORE_CERT_ERRORS = ($IgnoreCertErrors ? '1' : '0')
    $env:USE_TEST_FACTORY = '1'
    $cmd = @('test', $Project, '-c', $Configuration, '--logger', 'trx;LogFileName=main.trx', '--settings', 'coverlet.runsettings', '--results-directory', $ResultsDir)
    Run-Cmd $cmd
  }
  'ui-smoke' {
    $env:RUN_UI_TESTS = '1'
    $env:IGNORE_CERT_ERRORS = ($IgnoreCertErrors ? '1' : '0')
    $env:USE_TEST_FACTORY = '0'
    $cmd = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=UI-Smoke', '--logger', 'trx;LogFileName=ui.trx', '--results-directory', $ResultsDir)
    Run-Cmd $cmd
  }
  'api-coverage' {
    $env:USE_TEST_FACTORY = '1'
    $env:RUN_UI_TESTS = '0'
    $cmd = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=API-Smoke', '--settings', 'coverlet.api.runsettings', '--results-directory', $ResultsDir)
    Run-Cmd $cmd
  }
  'integration-coverage' {
    $env:USE_TEST_FACTORY = ($UseTestFactory ? '1' : '1')
    $cmd = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=Integration', '--settings', 'coverlet.integration.runsettings', '--results-directory', $ResultsDir)
    Run-Cmd $cmd
  }
}

Write-Host "Suite completed: $Suite"
exit 0

