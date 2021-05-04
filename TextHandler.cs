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

            result = textcase switch
            {
                "Upper" => result.ToUpper(),
                "Lower" => result.ToLower(),
                _ => result
            };

            return result;
        }

        public static string FormatText(string minifiedText, int charsPerLine)
        {
            for (var i = 0; i < minifiedText.Length; i += charsPerLine) minifiedText = minifiedText.Insert(i, "\n");

            return minifiedText;
        }
    }
}