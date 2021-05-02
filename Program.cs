using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace SourceCodePosterizer
{
    internal class Program
    {
        private static string _rawText = string.Empty;

        private static void Main(string[] args)
        {
#if DEBUG
            // Options used to generate the demo pictures
            var foreground = "#00947B";
            var background = "#FF7A45";
            args = new[] { 
                $"-p{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.Parent?.FullName}",
                $"-f{foreground}",
                $"-b{background}",
                $"-u{80}",
                "-y*.cs",
                "-l100"
            };
#endif

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        private static void RunOptions(Options opts)
        {
            if (Directory.Exists(opts.FilePath))
            {
                ProcessDirectory(opts.FilePath, opts.Filetypes);

                var minifiedText = TextHandler.MinifyText(_rawText, opts.TextCase);
                var formattedText = TextHandler.FormatText(minifiedText, opts.LineLength);
                var poster = ImageHandler.CreateImage(formattedText,opts.Title, opts.ForegroundColor, opts.BackgroundColor,
                    opts.FontSize, opts.Border);

                ImageHandler.SaveImage(opts.FilePath, opts.Title, poster);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", opts.FilePath);
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors lol
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
            var searchPatterns = searchPattern.Split(',');
            var files = new List<string>();
            foreach (var sp in searchPatterns)
                files.AddRange(Directory.GetFiles(path, sp));
            files.Sort();
            return files.ToArray();
        }
    }
}