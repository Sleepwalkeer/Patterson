using Patterson.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Patterson.service.implementation
{
    internal class ImageProcessorService : IImageProcessorService
    {

        public Image originalImage;

        private double thetaEndX;

        private static readonly int OPTIMAL_DISPLAY_IMAGE_WIDTH = 900;
        private static readonly int OPTIMAL_DISPLAY_IMAGE_HEIGHT = 650;

        private static readonly int LINE_PIXEL_THRESHOLD = 40;
        private static readonly int BLACK_LINE_WIDTH = 2;
        public List<PeakData> ProcessImage(Image image, double minTheta, double maxTheta)
        {
            Bitmap bitmap = new Bitmap(800, 800);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawImage(image, 0, 0);
            graphics.Dispose();
            bitmap = TrimBottomSide(bitmap);
            bitmap = TrimLeftSide(bitmap);
            bitmap = SetBaseline(bitmap);
            return findPeaks(bitmap, minTheta, maxTheta);
            //обрезал границы, теперь зная thetaEndX всегда знаем где заканчивается график и от него же можем рассчитать theta
            //теперь нужно найти baseline  и потом сам расчет пиков
            return null;
        }

        private Bitmap SetBaseline(Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        private List<PeakData> findPeaks(Bitmap image, double minTheta, double maxTheta)
        {
            List<PeakData> peaks = new List<PeakData>();
            int startSearchPix = ConvertThetaToPix(minTheta);
            int endSearchPix = ConvertThetaToPix(maxTheta);

            //for (int y = 0; y < image.Width; y++)
            //{
            //    for (int x = 0; x < length; x++)
            //    {

            //    }
            //}


            return peaks;
        }

        private int ConvertThetaToPix(double theta)
        {
            return (int)(theta * thetaEndX / 120);
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

        public Bitmap TrimLeftSide(Bitmap image)
        {
            int lineStartX = -1;
            int linePixCounter = 0;

            for (int x = 0; x < image.Width; x++) // Outer loop for width
            {
                for (int y = 0; y < image.Height; y++) // Inner loop for height
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
                                while(pixelColor.R != 255 && pixelColor.G != 255 && pixelColor.B != 255)
                                {
                                    y++;
                                    pixelColor = image.GetPixel(x, y);
                                }
                                lineStartX+= BLACK_LINE_WIDTH;

                                int newWidth = image.Width - lineStartX;
                                int newHeight = y -2;


                                Bitmap trimmedImage = new Bitmap(newWidth, newHeight);

                                for (int height = 0; height < newHeight; height++)
                                {
                                    for (int width = 0; width < newWidth; width++)
                                    {
                                        trimmedImage.SetPixel(width, height, image.GetPixel(width + lineStartX, height));
                                    }
                                }

                                return trimmedImage;
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
            return image;
        }

        public Bitmap TrimBottomSide(Bitmap image)
        {
            int lineStartY = image.Height;
            int linePixCounter = 0;

            for (int y = image.Height-1; y >= 0; y--)
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
                                thetaEndX = x;

                                lineStartY += BLACK_LINE_WIDTH;                                

                                int newWidth = 800;
                                int newHeight = lineStartY;

                                Bitmap trimmedImage = new Bitmap(newWidth, newHeight);

                                for (int height = 0; height < newHeight; height++)
                                {
                                    for (int width = 0; width < newWidth; width++)
                                    {
                                        trimmedImage.SetPixel(width, height, image.GetPixel(width, height));
                                    }
                                }

                                return trimmedImage;
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
            return image;
        }




        public Image test(Image image)
        {
            Bitmap bitmap = new Bitmap(800,800);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawImage(image, 0, 0);
            graphics.Dispose();
            bitmap = TrimBottomSide(bitmap);
            bitmap = TrimLeftSide(bitmap);
            return bitmap;
        }
    }
}
