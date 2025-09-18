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

function Run-Cmd([string[]]$args) {
  Write-Host "dotnet $($args -join ' ')"
  & dotnet @args
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

switch ($Suite) {
  'dev-fast' {
    $env:USE_TEST_FACTORY = '1'
    $env:RUN_UI_TESTS = '0'
    $args = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=Unit|TestCategory=Integration|TestCategory=API-Smoke', '--logger', 'trx;LogFileName=dev.trx')
    if ($ResultsDir) { $args += @('--results-directory', $ResultsDir) }
    Run-Cmd $args
  }
  'main-full' {
    $env:RUN_UI_TESTS = '1'
    $env:IGNORE_CERT_ERRORS = ($IgnoreCertErrors ? '1' : '0')
    $env:USE_TEST_FACTORY = '1'
    $args = @('test', $Project, '-c', $Configuration, '--logger', 'trx;LogFileName=main.trx', '--settings', 'coverlet.runsettings', '--results-directory', 'SocksShoppingStore.Tests/TestResults/Coverage')
    Run-Cmd $args
  }
  'ui-smoke' {
    $env:RUN_UI_TESTS = '1'
    $env:IGNORE_CERT_ERRORS = ($IgnoreCertErrors ? '1' : '0')
    $env:USE_TEST_FACTORY = '0'
    $args = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=UI-Smoke', '--logger', 'trx;LogFileName=ui.trx')
    if ($ResultsDir) { $args += @('--results-directory', $ResultsDir) }
    Run-Cmd $args
  }
  'api-coverage' {
    $env:USE_TEST_FACTORY = '1'
    $env:RUN_UI_TESTS = '0'
    $args = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=API-Smoke', '--settings', 'coverlet.api.runsettings', '--results-directory', 'SocksShoppingStore.Tests/TestResults/CoverageApi')
    Run-Cmd $args
  }
  'integration-coverage' {
    $env:USE_TEST_FACTORY = ($UseTestFactory ? '1' : '1')
    $args = @('test', $Project, '-c', $Configuration, '--filter', 'TestCategory=Integration', '--settings', 'coverlet.integration.runsettings')
    if ($ResultsDir) { $args += @('--results-directory', $ResultsDir) }
    Run-Cmd $args
  }
}

Write-Host "Suite completed: $Suite"
exit 0

