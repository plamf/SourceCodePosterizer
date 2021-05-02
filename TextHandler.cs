namespace SourceCodePosterizer
{
    public class TextHandler
    {
        public static string MinifyText(string rawText, string textcase)
        {
            var result = rawText
                .Replace("\n", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("\r", string.Empty)
                .Replace(" ", string.Empty);

            if (textcase.Equals("Upper"))
                result = result.ToUpper();
            if (textcase.Equals("Lower"))
                result = result.ToLower();

            return result;
        }

        public static string FormatText(string minifiedText, int charsPerLine)
        {
            for (var i = 0; i < minifiedText.Length; i += charsPerLine) minifiedText = minifiedText.Insert(i, "\n");

            return minifiedText;
        }
    }
}