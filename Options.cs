using CommandLine;

namespace SourceCodePosterizer
{
    public class Options
    {
        [Option('p', "path", Required = true, HelpText = "The path to your source files.")]
        public string FilePath { get; set; }

        [Option('t', "title", Required = false, HelpText = "Adds a title to your Image.")]
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

        [Option('y', "filetypes", Required = false,
            HelpText = "Look up only the selected files. Multiple filetypes are possible. Usage: *.cs|*.css")]
        public string Filetypes { get; set; } =
            "*.cs,*.css,*.scss,*.js,*.json,*.ts,*.html,*.txt,*.md";

        [Option('u', "border", Required = false,
            HelpText = "Frames the image with a border in the same color as the foreground. Default width: 0")]
        public int BorderThickness { get; set; } = 0;

        [Option('a', "lock-aspect", Required = false,
            HelpText = "Locks the generated image in an 1:414 aspect ratio.")]
        public bool LockAspect { get; set; }
    }
}