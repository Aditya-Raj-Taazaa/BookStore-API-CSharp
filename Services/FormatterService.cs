using Test_API.Interfaces;
namespace Test_API.Services
{
    public class FormatterService : IFormatterService
    {
        public string BioFormat(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Trim().ToLower();
            char[] chars = input.ToCharArray();
            bool capitalizeNext = true;

            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsWhiteSpace(chars[i]))
                {
                    capitalizeNext = true;
                }
                else if (capitalizeNext && char.IsLetter(chars[i]))
                {
                    chars[i] = char.ToUpper(chars[i]);
                    capitalizeNext = false;
                }
            }

            string formatted = new string(chars);
            Console.WriteLine($"Formatter: {formatted} ✍️");
            return formatted;
        }
    }
}
