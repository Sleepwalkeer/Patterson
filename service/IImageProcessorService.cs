using Patterson.model;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Patterson.service
{
    internal interface IImageProcessorService
    {
        List<PeakData> ProcessImage(double minTheta, double maxTheta);

        Image RenderImage(Image prescaledImage);
    }
}
