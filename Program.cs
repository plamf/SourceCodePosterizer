using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CommandLine;

namespace SourceCodePosterizer
{
    internal class Program
    {
        private static string _rawText = string.Empty;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        private static void RunOptions(Options opts)
        {
            if (Directory.Exists(opts.FilePath))
            {
                ProcessDirectory(opts.FilePath, opts.Filetypes);

                var minifiedText = MinifyText(opts.TextCase);
                var formattedText = FormatText(minifiedText, opts.LineLength);
                var poster = CreateImage(formattedText, opts.ForegroundColor, opts.BackgroundColor, opts.FontSize);

                SaveImage(opts.FilePath, opts.Title, poster);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", opts.FilePath);
            }
        }

        private static void SaveImage(string filePath, string filename, Image poster)
        {
            var codecInfo = GetEncoderInfo("image/png");
            var encoderParams = new EncoderParameters(1);
            var encoderParam = new EncoderParameter(Encoder.Quality, 100L);
            encoderParams.Param[0] = encoderParam;

            poster.Save(filePath + $"\\{filename}.png", codecInfo, encoderParams);
        }

        private static string FormatText(string minifiedText, int charsPerLine)
        {
            for (var i = 0; i < minifiedText.Length; i += charsPerLine) minifiedText = minifiedText.Insert(i, "\n");

            return minifiedText;
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }

        private static void ProcessDirectory(string targetDirectory, string filetypes)
        {
            // Process the list of files found in the directory.
            var fileEntries = GetFiles(targetDirectory, filetypes);
            foreach (var fileName in fileEntries)
                _rawText += File.ReadAllText(fileName);

            // Recurse into subdirectories of this directory.
            var subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (var subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, filetypes);
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            string[] searchPatterns = searchPattern.Split(',');
            List<string> files = new List<string>();
            foreach (string sp in searchPatterns)
                files.AddRange(Directory.GetFiles(path, sp));
            files.Sort();
            return files.ToArray();
        }

        private static string MinifyText(string textcase)
        {
            var result = _rawText
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

        private static Image CreateImage(string text, string foregroundColor, string backgroundColor, int fontsize)
        {
            var cc = new ColorConverter();
            var font = new Font("Lucida Console", fontsize, FontStyle.Regular);

            Image img = new Bitmap(1, 1);
            var drawing = Graphics.FromImage(img);
            var textSize = drawing.MeasureString(text, font);

            img.Dispose();
            drawing.Dispose();

            img = new Bitmap((int) textSize.Width, (int) textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear((Color) cc.ConvertFromString(backgroundColor));

            Brush textBrush = new SolidBrush((Color) cc.ConvertFromString(foregroundColor));
            drawing.DrawString(text, font, textBrush, 0, 0);
            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var encoders = ImageCodecInfo.GetImageEncoders();
            return encoders.FirstOrDefault(encoder => encoder.MimeType == mimeType);
        }

        public class Options
        {
            [Option('p', "path", Required = true, HelpText = "The path to your source files.")]
            public string FilePath { get; set; }

            [Option('t', "title", Required = false, HelpText = "A title for your header/filename.")]
            public string Title { get; set; } = "SCP";

            [Option('l', "line-length", Required = false,
                HelpText = "The amount of characters in a single line before a linebreak. Default: 200")]
            public int LineLength { get; set; } = 200;

            [Option('f', "foreground-color", Required = false, HelpText = "The hexadecimal color of the code.")]
            public string ForegroundColor { get; set; } = "#000000";

            [Option('b', "background-color", Required = false, HelpText = "The hexadecimal color of the background.")]
            public string BackgroundColor { get; set; } = "#FFFFFF";

            [Option('c', "casing", Required = false, HelpText = "The casing is either Upper, Lower or Default")]
            public string TextCase { get; set; } = "Default";

            [Option('s', "size", Required = false, HelpText = "The fontsize. Default: 16")]
            public int FontSize { get; set; } = 16;

            [Option('y', "filetypes", Required = false, HelpText = "Look up only the selected files. Multiple filetypes are possible. Usage: *.cs|*.css")]
            public string Filetypes { get; set; } = "*.cs,*.css,*.scss,*.js,*.json,*.ts,*.html,*.xml,*.xaml,*.html,*.txt,*.md";
        }
    }
}