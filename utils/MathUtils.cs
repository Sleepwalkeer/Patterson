using System;

namespace Patterson.utils
{
    internal class MathUtils
    {
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
