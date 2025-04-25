namespace Test_API.Services
{
    public class FormatterService
    {
        public string BioFormat(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
                
            return string.Join(" ", input.Split(' ')
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }
    }
}