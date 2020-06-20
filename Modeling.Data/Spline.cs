using System;

namespace Modeling.Data
{
    public class Spline
    {
        private Point[] _points;
        private SplineSubInterval[] _splines;

        #region Производные
        public double Df1
        {
            get { return _points[0].df; }
            set { _points[0].df = value; }
        }
        public double Ddf1
        {
            get { return _points[0].ddf; }
            set { _points[0].ddf = value; }
        }
        public double Dfn
        {
            get { return _points[_points.Length - 1].df; }
            set { _points[_points.Length - 1].df = value; }
        }
        public double Ddfn
        {
            get { return _points[_points.Length - 1].ddf; }
            set { _points[_points.Length - 1].ddf = value; }
        }
        #endregion
        #region Конструктор
        public Spline(GridFunction gridFunction)
        {
            _points = gridFunction.Data;
            _splines = new SplineSubInterval[gridFunction.Data.Length - 1];
        }
        #endregion
        private double BuildSplines(double ddf1)
        {
            double df = _points[0].df, ddf = ddf1;
            for (var i = 0; i < _splines.Length; i++)
            {
                _splines[i] = new SplineSubInterval(_points[i], _points[i + 1], df, ddf);

                df = _splines[i].Df(_points[i + 1].X);
                ddf = _splines[i].Ddf(_points[i + 1].X);

                if (i < _splines.Length - 1)
                {
                    _points[i + 1].df = df;
                    _points[i + 1].ddf = ddf;
                }
            }
            return df - Dfn;//Dfn-последнего узла.
        }

        public void GenerateSplines()
        {
            const double x1 = 0;
            var y1 = BuildSplines(x1);
            const double x2 = 10;
            var y2 = BuildSplines(x2);

            //Высчитываем вторую производную в нулевой точке.
            _points[0].ddf = -y1 * (x2 - x1) / (y2 - y1);

            BuildSplines(_points[0].ddf);

            _points[_points.Length - 1].ddf = _splines[_splines.Length - 1].Ddf(_points[_points.Length - 1].X);
        }

        public double? GetFunctionValue(double x, int n)//n- количество интервалов
        {
            if (_splines[0].GetX() > x || _splines[_points.Length - 2].GetX2() < x)
            {
                return null;
            }

            for (int i = 0; i < _splines.Length; i++)
            {
                if (_splines[i].GetX() > x)
                {
                    return GetInterval(_splines[i - 1].GetX(), x, n, i - 1);
                }
                else if (_splines[i].GetX() == x)
                {
                    return _splines[i].F(x);
                }
            }
            if (x < _splines[_points.Length - 2].GetX2())
            {
                return GetInterval(_splines[_points.Length - 2].GetX(), x, n, _points.Length - 2);
            }
            return null;
        }
        public double GetInterval(double x0, double x, int n, int IntervalIndex)
        {
            SplineSubInterval interval = null;
            double number = GetIntervalStep(_splines[IntervalIndex].GetX(), x, n);
            Point point = _splines[IntervalIndex].GetPoint();

            double temp = point.X;
            for (int j = 0; j < n; j++)
            {

                //Рассчитываем значения на каждом интервале
                temp += number;

                Point p = new Point(temp);
                interval = new SplineSubInterval(point, p, point.df, point.ddf);

                point = p;
                //То что получилось на последнем шаге
            }
            return interval.F(point.X);
        }

        public double GetIntervalStep(double x0, double x1, int n)//n количество интервалов
        {
            return Math.Abs(x1 - x0) / n;
        }

    }
}

