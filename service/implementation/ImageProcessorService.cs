using Patterson.model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Patterson.service.implementation
{
    internal class ImageProcessorService : IImageProcessorService
    {
        private Image originalImage;
        private Image croppedImage;

        private List<Point> baseline = new List<Point>();

        private static readonly int OPTIMAL_DISPLAY_IMAGE_WIDTH = 900;
        private static readonly int OPTIMAL_DISPLAY_IMAGE_HEIGHT = 650;

        private static readonly double MIN_INTENSITY_THRESHOLD = 10;
        private static readonly int LINE_PIXEL_THRESHOLD = 40;

        private static readonly int BLACK_LINE_WIDTH = 3;

        private static readonly int PIXEL_BRIGHTNESS_THRESHOLD = 127;

        public List<PeakData> ProcessImage(double minTheta, double maxTheta, Point start, Point end)
        {
            CaptureSelectedImageWithoutRectangle(start, end);
            Bitmap bitmap = new Bitmap(croppedImage.Width, croppedImage.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawImage(croppedImage, 0, 0);
            graphics.Dispose();
            //TrimBottomSide(bitmap);
            //TrimLeftSide(ref bitmap);
            TrimTitleBlock(bitmap);
            SetBaseline(bitmap);
            return findPeaks(bitmap, minTheta, maxTheta);
        }

        public Image testProcessImage(double minTheta, double maxTheta, Point start, Point end)
        {
            CaptureSelectedImageWithoutRectangle(start, end);
            Bitmap bitmap = new Bitmap(croppedImage.Width, croppedImage.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawImage(croppedImage, 0, 0);
            graphics.Dispose();
            //TrimBottomSide(bitmap);
            //TrimLeftSide(ref bitmap)            
            TrimTitleBlock(bitmap);
            SetBaseline(bitmap);
            List<PeakData> peaks = findPeaks(bitmap, minTheta, maxTheta);
            //peaks.RemoveAt(11);
            //peaks.RemoveAt(10);
            //peaks.RemoveAt(9);
            //peaks.RemoveAt(6);
            //peaks.RemoveAt(4);
            //peaks.RemoveAt(3);
            //peaks.RemoveAt(0);
            return bitmap;
        }

        private void TrimTitleBlock(Bitmap image)
        {
            int width = image.Width / 2;
            int height = image.Height / 2;

            using (Graphics g = Graphics.FromImage(image))
            {
                Rectangle rectangle = new Rectangle(width, 0, width, height);
                g.SetClip(rectangle);
                g.Clear(Color.White);
                g.ResetClip();
            }
        }

        private void SetBaseline(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            int baselineHeightIncreaseThreshold = 10;
            int previousBaselinePoint = 0;

            for (int x = 0; x < width-1; x++)
            {
                int sum = 0;
                int count = 0;

                bool foundGraphStart = false;

                for (int y = height - 1; y >= 0; y--)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    int brightness = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);

                    if (brightness < PIXEL_BRIGHTNESS_THRESHOLD)
                    {
                        foundGraphStart = true;
                    }
                    else if (foundGraphStart)
                    {
                        break;
                    }

                    if (foundGraphStart)
                    {
                        sum += y;
                        count++;
                    }
                }

                int baselinePoint = (count == 0) ? 0 : sum / count;

                Color nextPixelColor = image.GetPixel(x + 1, baselinePoint);

                int nextPixelbrightness = getBrightness(nextPixelColor);

                if (previousBaselinePoint != 0 && nextPixelbrightness > PIXEL_BRIGHTNESS_THRESHOLD)
                {
                    baselinePoint = previousBaselinePoint;
                }
                else if (previousBaselinePoint != 0 && baselinePoint < previousBaselinePoint && (Math.Abs(baselinePoint - previousBaselinePoint) > baselineHeightIncreaseThreshold))
                {
                        baselinePoint = previousBaselinePoint;

                }

                image.SetPixel(x, baselinePoint, Color.Gold);
                baseline.Add(new Point(x, baselinePoint));
                previousBaselinePoint = baselinePoint;
            }
        }

        private List<PeakData> findPeaks(Bitmap image, double minTheta, double maxTheta)
        {
            List<PeakData> peaks = new List<PeakData>();

            HashSet<int> ignoreXs = new HashSet<int>();

            for (int y = 0; y < croppedImage.Height-1; y++)
            {
                for (int x = 0; x < image.Width-2; x++)
                {
                    if (!ignoreXs.Contains(x))
                    {
                        Color pixelColor = image.GetPixel(x, y);
                        int brightness = getBrightness(pixelColor);

                        if (brightness < PIXEL_BRIGHTNESS_THRESHOLD)
                        {
                            double theta = ConvertPixToTheta(x, minTheta, maxTheta);
                            double intensity = getPeakIntensity(x, y);
                            if (intensity < MIN_INTENSITY_THRESHOLD)
                            {
                                continue;
                            }
                            int testY = 0;
                            foreach (Point point in baseline)
                            {
                                if (point.X == x)
                                {
                                    testY = point.Y;
                                }
                            }
                            for (int q = y; q < testY; q++)
                            {
                                image.SetPixel(x, q, Color.Brown);
                            }

                            peaks.Add(new PeakData(intensity, theta));
                            for (int i = x - 3; i < x + 3; i++)
                            {
                                ignoreXs.Add(i);
                            }
                        }
                    }
                }
            }
            return PeakData.SortByTheta(peaks);
        }

        private double getPeakIntensity(int x, int y)
        {
            foreach (Point point in baseline)
            {
                if (point.X == x)
                {
                    return point.Y - y;
                }
            }
            return 0;
        }

        private int getBrightness(Color pixelColor)
        {
            return (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
        }

        private double ConvertPixToTheta(int pixel, double minTheta, double maxTheta)
        {
            double fraction = (double)pixel / croppedImage.Width;
            return minTheta + fraction * (maxTheta - minTheta);

        }

        public Image RenderImage(Image imageToRender)
        {
            Image image = ScaleImage(imageToRender, OPTIMAL_DISPLAY_IMAGE_WIDTH, OPTIMAL_DISPLAY_IMAGE_HEIGHT);
            originalImage = image;
            return image;
        }

        private Image ScaleImage(Image imageToScale, int width, int height)
        {
            return new Bitmap(imageToScale, width, height);
        }

        public void TrimLeftSide(ref Bitmap image)
        {
            int lineStartX = -1;
            int linePixCounter = 0;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    if (pixelColor.R != 255 && pixelColor.G != 255 && pixelColor.B != 255)
                    {
                        linePixCounter++;
                        if (lineStartX == -1)
                        {
                            lineStartX = x;
                        }
                        else
                        {
                            if (linePixCounter >= LINE_PIXEL_THRESHOLD)
                            {
                                while (pixelColor.R != 255 && pixelColor.G != 255 && pixelColor.B != 255)
                                {
                                    y++;
                                    pixelColor = image.GetPixel(x, y);
                                }
                                lineStartX += BLACK_LINE_WIDTH;

                                int newWidth = image.Width - lineStartX;
                                int newHeight = y - BLACK_LINE_WIDTH;

                                Bitmap trimmedImage = new Bitmap(newWidth, newHeight);

                                for (int height = 0; height < newHeight; height++)
                                {
                                    for (int width = 0; width < newWidth; width++)
                                    {
                                        trimmedImage.SetPixel(width, height, image.GetPixel(width + lineStartX, height));
                                    }
                                }
                                image = trimmedImage;
                                return;
                            }
                        }
                    }
                    else
                    {
                        linePixCounter = 0;
                        lineStartX = -1;
                    }
                }
            }
        }

        public void TrimBottomSide(Bitmap image)
        {
            int lineStartY = image.Height;
            int linePixCounter = 0;

            for (int y = image.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    if (pixelColor.R != 255 && pixelColor.G != 255 && pixelColor.B != 255)
                    {
                        linePixCounter++;
                        if (lineStartY == image.Height)
                        {
                            lineStartY = y;
                        }
                        else
                        {
                            if (linePixCounter >= LINE_PIXEL_THRESHOLD)
                            {
                                while (pixelColor.R != 255 && pixelColor.G != 255 && pixelColor.B != 255)
                                {
                                    x++;
                                    pixelColor = image.GetPixel(x, y);
                                }

                                lineStartY += BLACK_LINE_WIDTH;

                                int newWidth = 800;
                                int newHeight = lineStartY;

                                image = new Bitmap(newWidth, newHeight);

                                for (int height = 0; height < newHeight; height++)
                                {
                                    for (int width = 0; width < newWidth; width++)
                                    {
                                        image.SetPixel(width, height, image.GetPixel(width, height));
                                    }
                                }
                                return;
                            }
                        }
                    }
                    else
                    {
                        linePixCounter = 0;
                        lineStartY = image.Height;
                    }
                }
            }
        }
        private Bitmap CaptureSelectedAreaWithoutRectangle(Rectangle selectionRect)
        {
            Bitmap capturedImage = new Bitmap(selectionRect.Width, selectionRect.Height);

            using (Graphics g = Graphics.FromImage(capturedImage))
            {
                Rectangle sourceRect = new Rectangle(selectionRect.X,
                                                      selectionRect.Y,
                                                      selectionRect.Width,
                selectionRect.Height);

                g.DrawImage(originalImage, new Rectangle(0, 0, capturedImage.Width, capturedImage.Height), sourceRect, GraphicsUnit.Pixel);
            }

            return capturedImage;
        }

        public void CaptureSelectedImageWithoutRectangle(Point selectionStart, Point selectionEnd)
        {
            Rectangle selectionRect = new Rectangle(
                Math.Min(selectionStart.X, selectionEnd.X),
                Math.Min(selectionStart.Y, selectionEnd.Y),
                Math.Abs(selectionStart.X - selectionEnd.X),
                Math.Abs(selectionStart.Y - selectionEnd.Y));

            croppedImage = CaptureSelectedAreaWithoutRectangle(selectionRect);
        }
    }
}
