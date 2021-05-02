using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace SourceCodePosterizer
{
    public class ImageHandler
    {
        public static Image CreateImage(string text, string title, string foregroundColor, string backgroundColor, int fontsize, int borderThickness)
        {
            var cc = new ColorConverter();
            var font = new Font("Lucida Console", fontsize, FontStyle.Regular);
            var bgColor = (Color)cc.ConvertFromString(backgroundColor);
            var fgColor = (Color)cc.ConvertFromString(foregroundColor);
            var hasTitle = title != string.Empty;
            var padding = 100;

            Image img = new Bitmap(1, 1);
            var drawing = Graphics.FromImage(img);
            var textSize = drawing.MeasureString(text, font);

            img.Dispose();
            drawing.Dispose();

            img = new Bitmap((int) textSize.Width + (padding * 2), (int) textSize.Height + (padding * (hasTitle ? 3:2)));
            drawing = Graphics.FromImage(img);
            drawing.Clear(bgColor);

            Brush textBrush = new SolidBrush(fgColor);
            drawing.DrawString(text, font, textBrush, padding, padding * (hasTitle ? 2 : 0));
            drawing.Save();

            if (hasTitle)
            {
                AddTitle(img, title, textBrush, fontsize);
            }

            if (borderThickness > 0)
            {
                AddBorder(img, borderThickness, fgColor);
            }

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }

        private static void AddBorder(Image img, int thickness, Color color)
        {
            using (var g = Graphics.FromImage(img))
            {
                g.DrawRectangle(new Pen(color, thickness), new Rectangle(0, 0, img.Width, img.Height));
            }
        }

        private static void AddTitle(Image img, string title, Brush brush, int fontsize)
        {
            using (var g = Graphics.FromImage(img))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center
                };
                g.DrawString(title, new Font("Bauhaus 93", fontsize * 3), brush, img.Width / 2, 100, sf);
            }
        }


        public static void SaveImage(string filePath, string filename, Image poster)
        {
            var codecInfo = GetEncoderInfo("image/png");
            var encoderParams = new EncoderParameters(1);
            var encoderParam = new EncoderParameter(Encoder.Quality, 100L);
            encoderParams.Param[0] = encoderParam;

            poster.Save(filePath + $"\\{filename}.png", codecInfo, encoderParams);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var encoders = ImageCodecInfo.GetImageEncoders();
            return encoders.FirstOrDefault(encoder => encoder.MimeType == mimeType);
        }
    }
}