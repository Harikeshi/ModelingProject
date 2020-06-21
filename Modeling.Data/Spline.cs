using System;
using System.Collections.Generic;

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
        public Spline(Point[] points)//подаем массив
        {
            _points = points;
            _splines = new SplineSubInterval[points.Length - 1];
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
        public List<Point> GetValuesForDrow()
        {
            //Разбить интервал на более мелкие
            //и рассчитать точки в этих интервалах
            //Вернуть массив этих точек.
            
            //Пройти циклом по всем интервалам и составить массив
            int length = _splines.Length;
            int n = 0;
            double step;
            int temp = 0;
            double x1 = _splines[0].GetX1();

            List<Point> array = new List<Point>();
            
            SplineSubInterval interval = null;

            for(int i = 0; i < length; i++)
            {
                interval = _splines[i];
                n = GetIntervalNum(interval);
                step = GetIntervalStep(interval, n);

                for(int j = 0; j < n - 1; j++)
                {
                    x1 += step;
                    ;
                    array.Add(new Point(x1, GetFunctionValue(x1)));
                    temp++;
                }
              
                array.Add(interval.GetLast());


                temp++;
                
            }


            return array;
        }
        public double GetIntervalStep(SplineSubInterval interval,int n)
        {
            return (interval.GetX2() - interval.GetX1()) / n;
        }
        public double GetFunctionValue(double x)//n- количество интервалов
        {
            SplineSubInterval interval = GetInterval(x);

            if (interval.GetX1() == x)
            {
                return interval.GetPoint().Y;
            }
            if (interval.GetX2() == x)
            {
                return interval.GetLast().Y;
            }

            bool check = false;//проверяет найдено ли значения во время работы цикла
            double result = 0;
            //Находим количество интервалов
            int n = GetIntervalNum(interval);

            //Находим шаг
            double step = GetIntervalStep(interval,n);

            //Ходить будем n-1 раз, а последний интервал берем, что осталось
            SplineSubInterval CurrentInterval = null;

            Point p1 = interval.GetPoint();

            for (int i = 0; i < n - 1; i++)
            {
                double tempX = p1.X + step;
                Point p2 = new Point(tempX, interval.F(tempX));

                CurrentInterval = new SplineSubInterval(p1, p2, p1.df, p1.ddf);

                if (CurrentInterval.GetX1() < x && CurrentInterval.GetX2() > x)//если X попадает в интервал выходим и вычисляем значение функции 
                {
                    check = true;
                    result = CurrentInterval.F(x);
                    break;
                }
                if (CurrentInterval.GetX1() == x)
                {
                    return CurrentInterval.GetPoint().Y;
                }
                p1 = p2;
            }

            //Если не нашли в цикле, то значит значение находится в последнем интервале
            if (!check)
            {
                CurrentInterval = new SplineSubInterval(p1, interval.GetLast(), p1.df, p1.ddf);
            }
            return result;
        }
        //Находим количество интервалов
        private static int GetIntervalNum(SplineSubInterval interval)
        {
            int n = Convert.ToInt32(Math.Round((interval.GetX2() - interval.GetX1()) / 2, 0));
            //Если получилось мало шагов
            if (Math.Abs(n) < 10)
            {
                n = 10;
            }

            return n;
        }
        //Находим нужный интервал
        public SplineSubInterval GetInterval(double x)
        {
            SplineSubInterval interval = null;

            for (int i = 0; i < _splines.Length; i++)
            {

                if (_splines[i].GetX1() < x && _splines[i].GetX2() > x)
                {
                    interval = _splines[i];
                }
                if (_splines[i].GetX1() == x)
                {
                    interval = _splines[i];
                }
                if (_splines[i].GetX2() == x)
                {
                    interval = _splines[i];
                }

            }
            return interval;
        }
    }
}

