using Patterson.model;
using System.Collections.Generic;
using System.Drawing;

namespace Patterson.service
{
    public interface IImageProcessorService
    {
        List<PeakData> ProcessImage(double minTheta, double maxTheta, Point start, Point end);

        Image RenderImage(Image prescaledImage);

        void CaptureSelectedImageWithoutRectangle(Point start, Point end);

        Image testProcessImage(double minTheta, double maxTheta, Point start, Point end);
    }
}
