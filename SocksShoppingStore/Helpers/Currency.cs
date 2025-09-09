using System.Globalization;

namespace SocksShoppingStore.Helpers
{
    public static class Currency
    {
        private static CultureInfo? _fr;

        public static string Eur(decimal value)
        {
            try
            {
                _fr ??= new CultureInfo("fr-FR");
                return value.ToString("C", _fr);
            }
            catch (CultureNotFoundException)
            {
                var s = value.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',');
                return s + " \u20AC";
            }
        }
    }
}

