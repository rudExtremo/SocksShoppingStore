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

function Run-Cmd([string[]]$cmdArgs, [switch]$AllowFail) {
  Write-Host "dotnet $($cmdArgs -join ' ')"
  # capture both stdout/stderr into suite log and keep exit code
  & dotnet @cmdArgs 2>&1 | Tee-Object -FilePath $suiteLog -Append | Out-Host
  $code = $LASTEXITCODE
  if (-not $AllowFail -and $code -ne 0) { exit $code }
  return $code
}

switch ($Suite) {
  'dev-fast' {
    $env:USE_TEST_FACTORY = '1'
    $env:RUN_UI_TESTS = '0'
    $cmd = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=Unit|TestCategory=Integration|TestCategory=API-Smoke', '--logger', 'trx;LogFileName=dev.trx', '--results-directory', $ResultsDir, '--settings', 'nunit.runsettings')
    Run-Cmd $cmd
  }
  'main-full' {
    # Prepare separate result folders for clarity
    $nonUiDir = Join-Path $ResultsDir 'nonui'
    $uiDir = Join-Path $ResultsDir 'ui'
    if (-not (Test-Path $nonUiDir)) { New-Item -ItemType Directory -Path $nonUiDir -Force | Out-Null }
    if (-not (Test-Path $uiDir)) { New-Item -ItemType Directory -Path $uiDir -Force | Out-Null }

    Write-Host "=== PASS 1/2: Non-UI with coverage ==="
    # Pass 1: non-UI with coverage (UI disabled via env)
    $env:RUN_UI_TESTS = '0'
    $env:USE_TEST_FACTORY = '1'
    $cmd1 = @('test', $Project, '-c', $Configuration, '--logger', 'trx;LogFileName=main.trx', '--settings', 'coverlet.runsettings', '--results-directory', $nonUiDir)
    $code1 = Run-Cmd $cmd1 -AllowFail

    Write-Host "=== PASS 2/2: UI-Smoke (no coverage) ==="
    # Pass 2: UI smoke only (no coverage)
    $env:RUN_UI_TESTS = '1'
    $env:IGNORE_CERT_ERRORS = ($IgnoreCertErrors ? '1' : '0')
    $env:USE_TEST_FACTORY = '0'
    $cmd2 = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=UI-Smoke', '--logger', 'trx;LogFileName=ui.trx', '--results-directory', $uiDir)
    $code2 = Run-Cmd $cmd2 -AllowFail

    if ($code1 -ne 0 -or $code2 -ne 0) { exit 1 }
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

