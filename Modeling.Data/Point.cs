using System;

namespace Modeling.Data
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double df { get; set; }
        public double ddf { get; set; }
        public Point(double x)
        {
            X = x;
            Y = Fx(x);
        }
        public Point(double x,double y)
        {
            X = x;
            Y = y;
        }

        public double Fx(double x)
        {
            return Math.Sin(x);
        }
    }
}
