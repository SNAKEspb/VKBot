using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;

namespace VKBot.VkontakteBot.Services
{
    public class ImageService
    {
        private NLog.Logger _logger;

        public ImageService(NLog.Logger logger)
        {
            _logger = logger;
        }

        public byte[] addTextToImage(byte[] imageData, string text)
        {
            IImageFormat format;
            using (var image = Image.Load(imageData, out format))
            {
                var font = SystemFonts.CreateFont("Calibri", getSize(image, text), FontStyle.Regular);

                TextOptions options = new TextOptions(font)
                {
                    HorizontalAlignment = HorizontalAlignment.Right // Right align
                };

                PointF point = getPoint(image, text, options);

                image.Mutate(x => x.DrawText(text, font, Brushes.Solid(Color.Black), Pens.Solid(Color.White, 1), point));

                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, format);
                    return outputStream.ToArray();
                }
            }

            return new byte[0];
        }

        public float getSize(Image img, string text)
        {
            return img.Width / text.Length;
        }

        public PointF getPoint(Image image, string text, TextOptions textOptions)
        {
            FontRectangle measureResult = TextMeasurer.Measure(text, textOptions);

            var minWidth = image.Width * 0.05;
            var maxWidth = calculateMax(image.Width, minWidth, measureResult.Width);

            var minHeight = image.Height * 0.05;
            var maxHeight = calculateMax(image.Height, minHeight, measureResult.Height);

            Random rand = new Random();
            var pointX = rand.Next((int)minWidth, (int)maxWidth);
            var pointY = rand.Next((int)minHeight, (int)maxHeight);
            return new PointF(pointX, pointY);
        }

        public double calculateMax(double total, double min, double size)
        {
            return total - min - size;
        }

    }
}
