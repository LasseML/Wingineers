using System.Globalization;

namespace Wingineers.Services.WingineersApi
{
    public static class WingineersDateParser
    {
        private static readonly string[] Formats =
        [
            "yyyy-MM-dd HH:mm",
            "dd-MM-yyyy HH:mm",
            "yyyy-MM-dd",
            "dd-MM-yyyy"
        ];

        public static DateTime? Parse(string? value) =>
            DateTime.TryParseExact(value?.Trim(), Formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime date)
                ? date
                : null;
    }
}
