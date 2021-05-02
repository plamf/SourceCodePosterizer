using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace SourceCodePosterizer
{
    public class ImageHandler
    {
        public static Image CreateImage(string text, string foregroundColor, string backgroundColor, int fontsize)
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