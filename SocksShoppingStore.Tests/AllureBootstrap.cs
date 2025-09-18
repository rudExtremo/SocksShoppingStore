using NUnit.Framework;
using System;
using System.IO;

namespace SocksShoppingStore.Tests
{
    // Runs once per test assembly to enrich Allure with env + categories
    [SetUpFixture]
    public class AllureBootstrap
    {
        [OneTimeSetUp]
        public void WriteAllureAuxFiles()
        {
            try
            {
                var workDir = TestContext.CurrentContext.WorkDirectory; // bin/<cfg>/net8.0
                var resultsDir = Path.Combine(workDir, "allure-results");
                Directory.CreateDirectory(resultsDir);

                // environment.properties for Allure "Environment" widget
                var envPath = Path.Combine(resultsDir, "environment.properties");
                var lines = new[]
                {
                    $"BASE_URL={TestSettings.BaseUrl}",
                    $"RUN_UI_TESTS={TestSettings.RunUi}",
                    $"IGNORE_CERT_ERRORS={TestSettings.IgnoreCertErrors}",
                    $"USE_TEST_FACTORY={TestSettings.UseTestFactory}",
                    $"OS={Environment.OSVersion}",
                    $"DOTNET_VERSION={Environment.Version}"
                };
                File.WriteAllLines(envPath, lines);

                // categories.json (defect taxonomy) -> copy if present
                var srcCategories = Path.Combine(workDir, "categories.json");
                var dstCategories = Path.Combine(resultsDir, "categories.json");
                if (File.Exists(srcCategories))
                {
                    File.Copy(srcCategories, dstCategories, overwrite: true);
                }
            }
            catch
            {
                // Best-effort only; do not fail tests due to reporting helpers
            }
        }
    }
}

