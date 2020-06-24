using System;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Geared;
using Modeling.Data;

namespace Modeling.wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //Данные
        public Modeling.Data.Point[] Points;//массив, который хранит точки табличной функции
        public double[] Begin_array = { -1, 1, 3, 5, 10, 15 };
       
        public MainWindow()
        {
            InitializeComponent();


            Modeling.Data.Point[] Points = new Data.Point[Begin_array.Length];
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = new Data.Point(Begin_array[i]);
            }

            test1.Content = MaxArray(Points, 'x').ToString() + " " + MinArray(Points, 'x');
            test2.Content = MaxArray(Points, 'y').ToString() + " " + MinArray(Points, 'y');
            //1. заполнить таблицу

            //будет заполняться по функции синус 
            //Вычесляем значения Y и добавляем элементы в таблицу

            //Построить график

            //2. рассчитать значение  

            GetPoints(Points);
        }

        private void CalculateY_Click(object sender, RoutedEventArgs e)
        {
            GridFunction func = new GridFunction();
            Spline spline = new Spline(func.Data);

            spline.GenerateSplines();

            double x = Convert.ToDouble(valueX.Text);

            SinX.Content = Math.Sin(x).ToString();


            valueY.Content = spline.GetFunctionValue(x).ToString();

        }

        private void CalculateTable_Click(object sender, RoutedEventArgs e)
        {
            //Берем значения из формы таблицы
            //нажимаем рассчитать
            //Эти значения сохраняются в статических массивах Х и Y
            //далее эти массивы используются для рассчетов сплайнов
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            for (int i = 0; i < Begin_array.Length; i++)
            {
                TableFunc.Items.Add(new TablePoint { ValueX = Begin_array[i], ValueY = Math.Round(Math.Sin(Begin_array[i]), 5) });
            }
            GridFunction func = new GridFunction();
            Spline spline = new Spline(func.Data);

            spline.GenerateSplines();

            double x = Convert.ToDouble(valueX.Text);

            SinX.Content = Math.Sin(x).ToString();
            List<Modeling.Data.Point> points = spline.GetValuesForDrow();

            valueY.Content = spline.GetFunctionValue(x).ToString();
            
            CreateGraphic(Points);
        }
        private void CreateGraphic(Data.Point[] points)
        {
            //Отступы от осей 20          
            
        
        }




        private double MaxArray(Data.Point[] points, char ch)
        {
            double result = 0;
            //Минимум в зависимости ch это x или y
            if (ch == 'x')
            {
                result = points[0].X;
                for (int i = 1; i < points.Length; i++)
                {
                    if (result < points[i].X)
                    {
                        result = points[i].X;
                    }
                }
            }
            if (ch == 'y')
            {
                result = points[0].Y;
                for (int i = 1; i < points.Length; i++)
                {
                    if (result < points[i].Y)
                    {
                        result = points[i].Y;
                    }
                }
            }
            return result;
        }
        private double MinArray(Data.Point[] points, char ch)
        {
            double result = 0;

            //Максимум в зависимости ch это x или y
            if (ch == 'x')
            {
                result = points[0].X;
                for (int i = 1; i < points.Length; i++)
                {
                    if (result > points[i].X)
                    {
                        result = points[i].X;
                    }
                }
            }
            if (ch == 'y')
            {
                result = points[0].Y;
                for (int i = 1; i < points.Length; i++)
                {
                    if (result > points[i].Y)
                    {
                        result = points[i].Y;
                    }
                }
            }
            return result;
        }
        private Data.Point[] GetPoints(Data.Point[] points)
        {

            double Left1 = 60;
            double Left2 = 20;
            double Bottom1 = 60;
            double Bottom2 = 20;
            double Top = 30;
            double Right = 30;
            //Рассчитывать надо сразу для всего графика

            //надо найти значение середины
            //зачем?? найти 0 и от него считать
            double minY = MinArray(points, 'y');
            double maxY = MaxArray(points, 'y');
            double minX = MinArray(points, 'x');
            double maxX = MaxArray(points, 'x');

            double lengthY = 0;
            double lengthX = 0;
            //Находим длину рабочей поверхности по X и Y
            if ((minY <= 0 && maxY <= 0) || (minY > 0 && maxY > 0))
            {
                lengthY = Math.Abs(Math.Abs(minY) - Math.Abs(maxY));
            }
            if (maxY >= 0 && minY <= 0)
            {
                lengthY = Math.Abs(maxY + Math.Abs(minY));
            }

            if ((minX <= 0 && maxX <= 0) || (minX >= 0 && maxX >= 0))
            {
                lengthX = Math.Abs(Math.Abs(minX) - Math.Abs(maxX));
            }
            if (maxX >= 0 && minX <= 0)
            {
                lengthX = Math.Abs(maxX + Math.Abs(minX));
            }

            double ActualH = 719.55;
            double ActualW = 1435.5;
            //Длина в пикселях
            double lengthYCanvas = ActualH - Left1-Left2-Right;
            double lengthXCanvas = ActualW - Bottom1-Bottom2-Top;

            //делим длину в пикселях на нашу длину и это будет множитель
            double mulY = lengthYCanvas / lengthY;
            double mulX = lengthXCanvas / lengthX;

            #region Отрисовка сетки
            //Осевые линии
            Line VerticalLine = new Line();
            VerticalLine.X1 = Left1;
            VerticalLine.X2 = Left1;
            VerticalLine.Y1 = 0;
            VerticalLine.Y2 = ActualH - Bottom1;
            VerticalLine.Stroke = Brushes.Black;
            VerticalLine.StrokeThickness = 2;
            Canvas1.Children.Add(VerticalLine);

            Line HorisontalLine = new Line();
            HorisontalLine.X1 = Left1;
            HorisontalLine.X2 = ActualW;
            HorisontalLine.Y1 = ActualH - Bottom1;
            HorisontalLine.Y2 = ActualH - Bottom1;
            HorisontalLine.Stroke = Brushes.Black;
            HorisontalLine.StrokeThickness = 2;
            Canvas1.Children.Add(HorisontalLine);






            #endregion


            List<Data.Point> ps = new List<Data.Point>();
            //Берем за точку отсчета максимальное значение это 30 и к нему будем прибавлять разницу между этим значением и текущим умноженное на множитель
            double exchangeY = 0;
            double exchangeX = 0;

            for (int i = 0; i < points.Length; i++)
            {
                //В цикле для всего массива
                if ((maxY >= 0 && points[i].Y >= 0) || (maxY <= 0 && points[i].Y <= 0))
                {
                    exchangeY = Math.Abs(maxY - points[i].Y);//модуль не нужен
                }
                if (maxY >= 0 && points[i].Y <= 0)
                {
                    exchangeY = Math.Abs(maxY + Math.Abs(points[i].Y));
                }

                if ((minX >= 0 && points[i].X >= 0) || (maxX <= 0 && points[i].X <= 0))
                {
                    exchangeX = Math.Abs(points[i].X - minX);//модуль не нужен
                }
                if (minX <= 0 && points[i].X >= 0)
                {
                    exchangeX = Math.Abs(points[i].X + Math.Abs(minX));
                }
                //Отступ 20 + 10(на самом графике)
                ps.Add(new Data.Point(exchangeX * mulX + Left1+Left2, exchangeY * mulY + Top));
                Ellipse el = new Ellipse();
                el.Height = 5;
                el.Width = 5;
                el.Stroke = Brushes.Red;
                el.StrokeThickness = 5;
                Canvas.SetLeft(el, exchangeX * mulX + Left1+Left2);
                Canvas.SetTop(el, exchangeY * mulY + Top);
                Canvas1.Children.Add(el);
            }




            return ps.ToArray();
        }


        private void DrawTable(Modeling.Data.Point[] points)
        {

        }


    }
}
