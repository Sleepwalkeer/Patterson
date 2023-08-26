using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.service
{
    internal interface IImageProcessorService
    {
        void ProcessImage(string imagePath);

        Image RenderImage(Image prescaledImage);
    }
}
