using System.Globalization;

namespace Test_API.Services
{
    public class FormatterService
    {
        public string BioFormat(string inp)
        {
            if (string.IsNullOrWhiteSpace(inp))
                return string.Empty;

            var trimmed = inp.Trim().ToLower();
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            Console.WriteLine($"Formatter Service Completed {trimmed} ✍️");
            return textInfo.ToTitleCase(trimmed);
        }
    }
}
