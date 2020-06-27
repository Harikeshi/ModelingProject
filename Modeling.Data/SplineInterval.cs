using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Data
{
    public class SplineInterval
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }

        private readonly Point _p1;
        private readonly Point _p2;

        #region Accessors
        public double GetX1()
        {
            return _p1.X;
        }
        public double GetX2()
        {
            return _p2.X;
        }
        public Point GetFirst() { return _p1; }
        public Point GetLast() { return _p2; }
        #endregion

        public SplineInterval(Point p1, Point p2, double a, double b, double c, double d)
        {
            _p1 = p1;
            _p2 = p2;

            A = a;
            B = b;
            C = c;
            D = d;
        }

        public double F(double x)
        {
            return A + B * (x - _p1.X) + C * Math.Pow((x - _p1.X), 2) + D * Math.Pow((x - _p1.X), 3);
        }
    }
}

