using System;

namespace Modeling.Data
{
    public class GridFunction
    {
        public Point[] Data = new Point[6]
        {
            new Point(-1),
            new Point(1),
            new Point(3),
            new Point(5),
            new Point(10),
            new Point(15)
        };
        public Point[] points;


        public GridFunction(double start, double end, double step)
        {
            int n = Convert.ToInt32(Math.Abs(end - start) / step);

            points = new Point[n];
            points[0] = new Point(start);
            for (int i = 1; i < n; i++)
            {
                start += step;
                points[i] = new Point(start);
            }
        }
        public GridFunction()
        {

        }
    }
}
