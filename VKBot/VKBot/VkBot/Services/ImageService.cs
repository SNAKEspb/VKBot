using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Text;

namespace VKBot.VkontakteBot.Services
{
    public class ImageService
    {
        private NLog.Logger _logger;
        public ImageService(NLog.Logger logger)
        {
            _logger = logger;
        }

        //public void addTextToImage(string text)
        //{
        //    var bitMapImage = new Bitmap("dotnetbot_thumb.png");
        //    var graphicImage = Graphics.FromImage(bitMapImage);
        //    graphicImage.DrawString("testing 1 2 3", new Font("Calibri", 20, FontStyle.Bold), SystemBrushes.WindowText, new Point(200, 200));
        //    bitMapImage.Save("d:\\dotnetbot_thumb2.png", ImageFormat.Png);
        //    graphicImage.Dispose();
        //    bitMapImage.Dispose();
        //}

        public byte[] addTextToImage(byte[] imageData, string text)
        {
            using (var inputStream = new MemoryStream(imageData))
            using (var bitMap = new Bitmap(inputStream))
            using (var graphic = Graphics.FromImage(bitMap))
            //using (var font = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Regular))
            using (var font = new Font("Calibri", (int)getSize(bitMap, text), FontStyle.Regular))
            {

                graphic.InterpolationMode = InterpolationMode.High;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                drawText(graphic, text, font);
                //bitMap.Save("d:\\output.jpg", ImageFormat.Jpeg);
                using (var outputStream = new MemoryStream())
                {
                    bitMap.Save(outputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return outputStream.ToArray();
                }
            }
            return new byte[0];
        }

        //public void drawText(Graphics graphic, string text, Font font)
        //{
        //    using (var graphicsPath = new GraphicsPath())
        //    {
        //        graphicsPath.AddString(
        //                            text,             // text to draw
        //                            font.FontFamily,  // or any other font family
        //                            (int)font.Style,      // font style (bold, italic, etc.)
        //                            graphic.DpiY * font.Size / 72,       // em size
        //                            new Point(0, 0),              // location where to draw text
        //                            new StringFormat());          // set options here (e.g. center alignment)
        //        graphic.DrawPath(Pens.White, graphicsPath);
        //        graphic.FillPath(Brushes.Black, graphicsPath);
        //    }
        //}

        public void drawText(Graphics graphics, string text, Font font) {

            var OutlineForeColor = Color.White;
            var OutlineWidth = 2;
            var ForeColor = Color.Black;

            using (GraphicsPath graphicsPath = new GraphicsPath())
            using (Pen outline = new Pen(OutlineForeColor, OutlineWidth) { LineJoin = LineJoin.Round })
            using (StringFormat stringFormat = new StringFormat())
            using (Brush foreBrush = new SolidBrush(ForeColor))
            {

                var point = getPoint(graphics, text, font);

                graphicsPath.AddString(text, font.FontFamily, (int)font.Style, graphics.DpiY * font.Size / 72, point, stringFormat);
                //graphic.ScaleTransform(1.3f, 1.35f);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawPath(outline, graphicsPath);
                graphics.FillPath(foreBrush, graphicsPath);
            }
        }

        public decimal getSize(Image img, string text)
        {
            return img.Width / text.Length;
        }

        public Point getPoint(Graphics graphics, string text, Font font)
        {
            var measureResult = graphics.MeasureString(text, font);

            var minWidth = graphics.VisibleClipBounds.Width * 0.05;
            var maxWidth = calculateMax(graphics.VisibleClipBounds.Width, minWidth, measureResult.Width);

            var minHeight = graphics.VisibleClipBounds.Height * 0.05;
            var maxHeight = calculateMax(graphics.VisibleClipBounds.Height, minHeight, measureResult.Height);

            Random rand = new Random();
            var pointX = rand.Next((int)minWidth, (int)maxWidth);
            var pointY = rand.Next((int)minHeight, (int)maxHeight);
            return new Point(pointX, pointY);
        }

        public double calculateMax(double total, double min, double size)
        {
            return total - min - size;
        }


        //public static byte[] imageToByte(Image img)
        //{
        //    using (var stream = new MemoryStream())
        //    {
        //        img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        //        return stream.ToArray();
        //    }
        //}

    }
}
