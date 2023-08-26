using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.utils
{
    internal class MathUtils
    {
        public static double Pow(double a, double b)
        {
            int tmp = (int)(BitConverter.DoubleToInt64Bits(a) >> 32);
            int tmp2 = (int)(b * (tmp - 1072632447) + 1072632447);
            return BitConverter.Int64BitsToDouble(((long)tmp2) << 32);
        }

        public static double Squared(double a)
        {
            return a * a;
        }

        public static double ToRads(double degrees)
        {
            return (degrees * Math.PI) / 180.0;
        }
    }
}
