using System.Text.Json;

namespace SocksShoppingStore.Tests
{
    public static class TestSettings
    {
        private static readonly object _lock = new();
        private static bool _loaded = false;
        private static string _baseUrl = "https://localhost:7068";
        private static bool _runUi = false;
        private static bool _ignoreCert = false;
        private static bool _useFactory = false;

        public static string BaseUrl { get { EnsureLoaded(); return _baseUrl; } }
        public static bool RunUi { get { EnsureLoaded(); return _runUi; } }
        public static bool IgnoreCertErrors { get { EnsureLoaded(); return _ignoreCert; } }
        public static bool UseTestFactory { get { EnsureLoaded(); return _useFactory; } }

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            lock (_lock)
            {
                if (_loaded) return;
                // Defaults
                string? envBase = Environment.GetEnvironmentVariable("BASE_URL");
                if (!string.IsNullOrWhiteSpace(envBase)) _baseUrl = envBase!;

                string? runUi = Environment.GetEnvironmentVariable("RUN_UI_TESTS");
                _runUi = string.Equals(runUi, "1", StringComparison.OrdinalIgnoreCase) || string.Equals(runUi, "true", StringComparison.OrdinalIgnoreCase);

                string? ign = Environment.GetEnvironmentVariable("IGNORE_CERT_ERRORS");
                _ignoreCert = string.Equals(ign, "1", StringComparison.OrdinalIgnoreCase) || string.Equals(ign, "true", StringComparison.OrdinalIgnoreCase);

                string? useFac = Environment.GetEnvironmentVariable("USE_TEST_FACTORY");
                _useFactory = string.Equals(useFac, "1", StringComparison.OrdinalIgnoreCase) || string.Equals(useFac, "true", StringComparison.OrdinalIgnoreCase);

                // Optional JSON config override if present
                try
                {
                    var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Test.json");
                    if (File.Exists(configPath))
                    {
                        var json = JsonDocument.Parse(File.ReadAllText(configPath));
                        var root = json.RootElement;
                        if (root.TryGetProperty("BaseUrl", out var b) && string.IsNullOrWhiteSpace(envBase))
                        {
                            _baseUrl = b.GetString() ?? _baseUrl;
                        }
                        if (root.TryGetProperty("RunUi", out var r) && runUi == null)
                        {
                            _runUi = r.GetBoolean();
                        }
                        if (root.TryGetProperty("IgnoreCertErrors", out var ic) && ign == null)
                        {
                            _ignoreCert = ic.GetBoolean();
                        }
                        if (root.TryGetProperty("UseTestFactory", out var uf) && useFac == null)
                        {
                            _useFactory = uf.GetBoolean();
                        }
                    }
                }
                catch { /* ignore */ }

                _loaded = true;
            }
        }
    }
}

