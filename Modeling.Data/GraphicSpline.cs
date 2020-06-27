using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Data
{
    public class GraphicSpline
    {
        private Point[] _points;
        private SplineInterval[] _splines;
        private SplineInterval[] _ClampedSplines;

        private double[,] A1;//Матрица для натурального сплайна
        private double[,] A2;//Матрица с дополнительными граничными условиями
        private double[] v1;//Для матрицы A1
        private double[] v2;//Для матрицы A2

        private double alpha = 0;//
        private double beta = 0;//Дополнительные граничные условия

        private int Count;//Размемрность

        public GraphicSpline(Point[] points, double a, double b)
        {
            _points = points;
            Count = _points.Length;
            A1 = new double[Count, Count];
            A2 = new double[Count, Count];
            v1 = new double[Count];
            v2 = new double[Count];
            _splines = new SplineInterval[Count - 1];
            _ClampedSplines = new SplineInterval[Count - 1];
            alpha = a;
            beta = b;

            this.CreateMatrixNatural();
            this.CreateMatrixClamped();
        }

        public void CreateMatrixNatural()
        {
            int n = Count;

            A1[0, 0] = 1;
            A1[n - 1, n - 1] = 1;

            for (int i = 1; i < n - 1; i++)
            {
                double h0 = _points[i].X - _points[i - 1].X;
                double h1 = _points[i + 1].X - _points[i].X;
                A1[i, i - 1] = h0;
                A1[i, i + 1] = h1;
                A1[i, i] = 2 * (h0 + h1);
                v1[i] = 3 * ((_points[i + 1].Y - _points[i].Y) / h1 - (_points[i].Y - _points[i - 1].Y) / h0);
            }
        }
        public void CreateMatrixClamped()
        {
            int n = Count;

            double h = _points[1].X - _points[0].X;
            double hn = _points[n - 1].X - _points[n - 2].X;
            A2[0, 0] = 2 * h;
            A2[0, 1] = h;
            A2[n - 1, n - 1] = 2 * (hn);
            A2[n - 1, n - 2] = hn;
            v2[0] = 3 * (((_points[1].Y - _points[0].Y) / h) - alpha);
            v2[n - 1] = 3 * (beta - (_points[n - 1].Y - _points[n - 2].Y) / hn);

            for (int i = 1; i < n - 1; i++)
            {
                double h0 = _points[i].X - _points[i - 1].X;
                double h1 = _points[i + 1].X - _points[i].X;

                A2[i, i] = 2 * (h0 + h1);
                A2[i, i - 1] = h0;
                A2[i, i + 1] = h1;
                v2[i] = 3 * (((_points[i + 1].Y - _points[i].Y) / h1) - ((_points[i].Y - _points[i - 1].Y) / h0));
            }
        }
        public double[] CreateCvector(double[,] A, double[] v, int n)
        {          
            double[,] a = new double[n, n];
            Array.Copy(A, a, A.Length);

            double[] c = new double[n];
            Array.Copy(v, c, v.Length);

            for (int k = 0; k < n; k++)
            {
                double d = a[k, k];
                for (int j = k; j < n; j++) a[k, j] /= d;
                c[k] /= d;
                for (int i = k + 1; i < n; i++)
                {
                    double r = a[i, k];
                    for (int j = k; j < n; j++)
                        a[i, j] -= a[k, j] * r;
                    c[i] -= c[k] * r;
                }
            }

            for (int k = 0; k < n; k++)
            {
                double d = a[k, k];
                for (int j = k; j < n; j++) a[k, j] /= d;
                c[k] /= d;
                for (int i = k + 1; i < n; i++)
                {
                    double r = a[i, k];
                    for (int j = k; j < n; j++)
                        a[i, j] -= a[k, j] * r;
                    c[i] -= c[k] * r;
                }
            }

            return c;
        }
        public void BuildSubIntervals()
        {
            double[] c = CreateCvector(A2, v2, Count);

            //{ -1,1,3,5,10,15}
            //Находим массив A            
            for (int i = 0; i < c.Length - 1; i++)
            {
                double hi = _points[i + 1].X - _points[i].X;
                _splines[i] = new SplineInterval(_points[i], _points[i + 1], _points[i].Y,
                    (_points[i + 1].Y - _points[i].Y) / hi - (hi * (2 * c[i] + c[i + 1])) / 3, c[i], (c[i + 1] - c[i]) / (3 * (_points[i + 1].X - _points[i].X)));
            }
        }
        public void BuildSubIntervalsForClamped()
        {
            double[] c = CreateCvector(A1, v1, Count);

            //{ -1,1,3,5,10,15}
            //Находим массив A            
            for (int i = 0; i < c.Length - 1; i++)
            {
                double hi = _points[i + 1].X - _points[i].X;
                _ClampedSplines[i] = new SplineInterval(_points[i], _points[i + 1], _points[i].Y,
                    (_points[i + 1].Y - _points[i].Y) / hi - (hi * (2 * c[i] + c[i + 1])) / 3, c[i], (c[i + 1] - c[i]) / (3 * (_points[i + 1].X - _points[i].X)));
            }
        }
        public double GetFunction(double x,char ch)
        {
            SplineInterval[] splines = new SplineInterval[Count - 1];
            if (ch == 'c') splines = _ClampedSplines;
            else splines = _splines;
            if (x == splines[0].GetX1())
            {
                return splines[0].F(x);
            }

            for (int i = 0; i < splines.Length; i++)
            {
                if (x <= splines[i].GetX2())
                {
                    return splines[i].F(x);
                }
            }
            return 0;
        }
        public void ShowMatrixNatural()
        {
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    Console.Write($"{A1[i, j]} ");
                }
                Console.WriteLine($"  {v1[i]}");
                Console.WriteLine();
            }
        }
        public void ShowMatrixClamped()
        {
            Console.WriteLine();
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    Console.Write($"{A2[i, j]} ");
                }
                Console.WriteLine($"  {v2[i]}");
                Console.WriteLine();
            }
        }

    }
}
