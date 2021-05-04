using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace SourceCodePosterizer
{
    public class ImageHandler
    {
        public static Image CreateImage(string text, Options options)
        {
            var cc = new ColorConverter();
            var font = new Font("Lucida Console", options.FontSize, FontStyle.Regular);
            var bgColor = (Color) cc.ConvertFromString(options.BackgroundColor);
            var fgColor = (Color) cc.ConvertFromString(options.ForegroundColor);
            var padding = 100;
            var textBrush = new SolidBrush(fgColor);

            // Prepare a dummy image
            var img = new Bitmap(1, 1);
            var drawing = Graphics.FromImage(img);
            var textSize = drawing.MeasureString(text, font);

            // Free up memory used by the dummy image
            img.Dispose();
            drawing.Dispose();

            // Create an image with the right size to fit the text
            img = options.LockAspect ? CreateBitmapByAspect(textSize, padding) : CreateBitmapByTextsize(textSize, padding);

            // Draw the text and colorize the image
            drawing = Graphics.FromImage(img);
            drawing.Clear(bgColor);
            drawing.DrawString(text, font, textBrush, padding, padding * 2);
            drawing.Save();

            AddTitle(img, options.Title, textBrush, options.FontSize);

            if (options.BorderThickness > 0) AddBorder(img, options.BorderThickness, fgColor);

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }

        private static Bitmap CreateBitmapByTextsize(SizeF textSize, int padding)
        {
            var width = (int) textSize.Width + padding * 2;
            var height = (int) textSize.Height + padding * 3;

            return new Bitmap(width, height);
        }

        private static Bitmap CreateBitmapByAspect(SizeF textSize, int padding)
        {
            const float aspectRatio = 1.414F;
            var width = (int)textSize.Width + padding * 2;
            var aspectCorrectedHeight = width * aspectRatio;

            return new Bitmap(width, (int)aspectCorrectedHeight);
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