using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.service.implementation
{
    internal class ImageProcessorService : IImageProcessorService
    {

        private static readonly int OPTIMAL_DISPLAY_IMAGE_WIDTH = 900;
        private static readonly int OPTIMAL_DISPLAY_IMAGE_HEIGHT = 650;
        public void ProcessImage(string imagePath)
        {
            throw new NotImplementedException();
        }

        public Image RenderImage(Image imageToRender)
        {
            Image image = ScaleImage(imageToRender, OPTIMAL_DISPLAY_IMAGE_WIDTH, OPTIMAL_DISPLAY_IMAGE_HEIGHT);

            return image;
        }

        private Image ScaleImage(Image imageToScale, int width, int height)
        {
            return new Bitmap(imageToScale, width, height);
        }
    }
}
