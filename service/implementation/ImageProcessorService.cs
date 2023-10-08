using Patterson.model;
using System.Collections.Generic;
using System.Drawing;

namespace Patterson.service.implementation
{
    internal class ImageProcessorService : IImageProcessorService
    {
        public Image originalImage;

        private int graphEndX;
        private int baseLineY;

        private static readonly int OPTIMAL_DISPLAY_IMAGE_WIDTH = 900;
        private static readonly int OPTIMAL_DISPLAY_IMAGE_HEIGHT = 650;

        private static readonly double TITLE_BLOCK_WIDTH_THRESHOLD_PERCENTAGE = 0.7;
        private static readonly double TITLE_BLOCK_HEIGHT_THRESHOLD_PERCENTAGE = 0.2;

        private static readonly double MIN_INTENSITY_THRESHOLD = 5;
        private static readonly double NOISE_PIXEL_THRESHOLD_PERCENTAGE = 0.1;
        private static readonly int LINE_PIXEL_THRESHOLD = 40;

        private static readonly int BLACK_LINE_WIDTH = 3;

        private static readonly int MIN_THETA = 20;
        private static readonly int MAX_THETA = 120;

        public List<PeakData> ProcessImage(double minTheta, double maxTheta)
        {
            Bitmap bitmap = new Bitmap(800, 800);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawImage(originalImage, 0, 0);
            graphics.Dispose();
            TrimBottomSide(bitmap);
            TrimLeftSide(ref bitmap);
            SetBaseline(bitmap, 33);
            TrimTitleBlock(bitmap);
            return findPeaks(bitmap, minTheta, maxTheta);
        }

        private void TrimTitleBlock(Bitmap image)
        {
            int startX = (int)(TITLE_BLOCK_WIDTH_THRESHOLD_PERCENTAGE * graphEndX);
            int endY = (int)(TITLE_BLOCK_HEIGHT_THRESHOLD_PERCENTAGE * image.Height);

            for (int x = startX; x < graphEndX; x++)
            {
                for (int y = 0; y < endY; y++)
                {
                    image.SetPixel(x, y, Color.White);
                }
            }
        }

        private void SetBaseline(Bitmap image, double minTheta)
        {
            int thetaStartX = ConvertThetaToPix(minTheta);

            int noiseThreshold = (int)((graphEndX - thetaStartX) * NOISE_PIXEL_THRESHOLD_PERCENTAGE);
            int baseLineHeight = -1;

            bool withinNoiseLevel = false;

            for (int y = image.Height - 1; y >= 0; y--)
            {
                int blackPixelCount = 0;

                for (int x = thetaStartX; x < graphEndX; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    if (pixelColor.R != 255 || pixelColor.G != 255 || pixelColor.B != 255)
                    {
                        blackPixelCount++;
                    }
                }

                if (blackPixelCount >= noiseThreshold)
                {
                    baseLineHeight = y;
                    withinNoiseLevel = true;
                }

                if (withinNoiseLevel && blackPixelCount < noiseThreshold)
                {
                    baseLineY = baseLineHeight;
                    break;
                }
            }
        }

        private List<PeakData> findPeaks(Bitmap image, double minTheta, double maxTheta)
        {
            List<PeakData> peaks = new List<PeakData>();
            int startSearchPixX = ConvertThetaToPix(minTheta);
            int endSearchPixX = ConvertThetaToPix(maxTheta);

            HashSet<int> ignoreXs = new HashSet<int>();

            for (int y = 0; y < baseLineY + 7; y++)
            {
                for (int x = startSearchPixX; x < endSearchPixX; x++)
                {
                    if (!ignoreXs.Contains(x))
                    {
                        Color pixelColor = image.GetPixel(x, y);

                        if (pixelColor.R != 255 && pixelColor.G != 255 && pixelColor.B != 255)
                        {
                            double theta = ConvertPixToTheta(x);
                            double intensity = baseLineY - y + 7;
                            if (intensity < MIN_INTENSITY_THRESHOLD)
                            {
                                continue;
                            }

                            for (int q = 0; q < image.Height; q++)
                            {
                                image.SetPixel(x, q, Color.Red);
                            }

                            peaks.Add(new PeakData(intensity, theta));
                            for (int i = x - 5; i < x + 5; i++)
                            {
                                ignoreXs.Add(i);
                            }
                        }
                    }
                }
            }
            return PeakData.SortByTheta(peaks);
        }

        private int ConvertThetaToPix(double theta)
        {
            return (int)(((theta - MIN_THETA) / (MAX_THETA - MIN_THETA)) * graphEndX);
        }

        private double ConvertPixToTheta(int pixel)
        {
            return ((double)pixel / graphEndX) * (MAX_THETA - MIN_THETA) + MIN_THETA;

        }

        public Image RenderImage(Image imageToRender)
        {
            originalImage = imageToRender;
            Image image = ScaleImage(imageToRender, OPTIMAL_DISPLAY_IMAGE_WIDTH, OPTIMAL_DISPLAY_IMAGE_HEIGHT);

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
                                graphEndX -= lineStartX;
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
                                graphEndX = x;

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
    }
}
