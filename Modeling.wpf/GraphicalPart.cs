using Modeling.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Modeling.wpf
{
    public class GraphicalPart
    {
        private double Left1; // Первый отступ слева для надписей
        private double Left2; // Второй отступ слева для графика от оси
        private double Bottom1; // Первый отступ снизу для надписей
        private double Bottom2; // Второй отступ снизу для графика от оси
        private double Top; // Отступ сверху для графика от оси
        private double Right; // Отступ справа для графика от оси

        private double ActualH; // Длина в пикселях по Y
        private double ActualW; // Длина в пикселях по X

        private double minY; // Минимальное значение функции
        private double maxY; // Максимальное значение функции
        private double minX; // минимальное значение аргумента
        private double maxX; // Максимальное значение аргумента

        private double mulY;
        private double mulX;
        private int n;// Количество участков сетки

        public List<Line> Lines { get; set; }//Линии сетки
        public List<Label> Labels { get; set; }//Значения на графике
        private GraphicSpline spline;

        public GraphicalPart()
        {

        }
        public GraphicalPart(double left1 = 60, double left2 = 20,
            double bottom1 = 60, double bottom2 = 20, double top = 50, double right = 30,
            double actualH = 719.55, double actualW = 1435.5, int n = 20)
        {
            Left1 = left1;
            Left2 = left2;
            Bottom1 = bottom1;
            Bottom2 = bottom2;
            Top = top;
            Right = right;
            ActualH = actualH;
            ActualW = actualW;
            this.n = n;
        }

        public void CreateSpline(Data.Point[] points, double alpha, double beta)
        {
            spline = new GraphicSpline(points, alpha, beta);
            
        }

        public double GetFunctionValue(double x, char ch)
        {
            return spline.GetFunction(x, ch);
        }

        public void BuildCoordinateGrid(Data.Point[] points)
        {
            this.Lines = new List<Line>();
            this.Labels = new List<Label>();
            //Рассчитываем для всего графика

            // TODO Находим минимумы и максимумы массива, надо искать минимум и максимум самой функции
            minY = spline.MinArray(points, 'y');
            maxY = spline.MaxArray(points, 'y');
            minX = spline.MinArray(points, 'x');
            maxX = spline.MaxArray(points, 'x');

            maxY += (Math.Abs(maxY) / 8);
            minY -= (Math.Abs(minY) / 8);

            double lengthY = 0; // Разность между минимальным и максимальном по оси Y в значениях
            double lengthX = 0; // Разность между минимальным и максимальном по оси X в значениях

            #region Находим длину рабочей поверхности по X и Y в значениях
            if ((minY < 0 && maxY < 0) || (minY > 0 && maxY > 0) || (maxY == 0) || (minY == 0))
            {
                lengthY = Math.Abs(Math.Abs(minY) - Math.Abs(maxY));
            }
            if (maxY > 0 && minY < 0)
            {
                lengthY = Math.Abs(maxY + Math.Abs(minY));
            }

            if ((minX < 0 && maxX < 0) || (minX > 0 && maxX > 0) || (maxX == 0) || (minX == 0))
            {
                lengthX = Math.Abs(Math.Abs(minX) - Math.Abs(maxX));
            }
            if (maxX > 0 && minX < 0)
            {
                lengthX = Math.Abs(maxX + Math.Abs(minX));
            }
            #endregion

            //Длина в пикселях с учетом всех отступов будет
            double lengthYCanvas = ActualH - Left1 - Left2 - Right;
            double lengthXCanvas = ActualW - Bottom1 - Bottom2 - Top;

            //Делим длину в пикселях на нашу длину и это будет множитель по соответствующей оси
            //Для перевода реального значения в пиксели на графике
            mulY = lengthYCanvas / lengthY;
            mulX = lengthXCanvas / lengthX;

            #region Отрисовка сетки
            #region Построение нижних осевых линий
            Line VerticalLine = new Line();
            VerticalLine.X1 = Left1;
            VerticalLine.X2 = Left1;
            VerticalLine.Y1 = 0;
            VerticalLine.Y2 = ActualH - Bottom1;
            VerticalLine.Stroke = Brushes.Black;
            VerticalLine.StrokeThickness = 2;
            Lines.Add(VerticalLine);

            Line HorisontalLine = new Line();
            HorisontalLine.X1 = Left1;
            HorisontalLine.X2 = ActualW;
            HorisontalLine.Y1 = ActualH - Bottom1;
            HorisontalLine.Y2 = ActualH - Bottom1;
            HorisontalLine.Stroke = Brushes.Black;
            HorisontalLine.StrokeThickness = 2;
            Lines.Add(HorisontalLine);
            #endregion

            double StepHeight = lengthYCanvas / n; //Шаг по вертикальной сетке
            double StepWidth = lengthXCanvas / n; //Шаг по горизонтальной сетке

            //Построение сетки выделить в отдельные методы
            #region Построение вертикальной сетки
            double temp = Left1;
            double indent = Left1 - 20;//Отступ от левого края

            double minimumX = minX - Left2 / mulX;
            Label l0 = new Label { Content = Math.Round(minimumX, 2).ToString() };
            Canvas.SetLeft(l0, indent);
            Canvas.SetTop(l0, ActualH - Bottom1);
            Labels.Add(l0);

            for (int i = 0; i < n; i++)
            {
                temp += StepWidth;
                minimumX += (StepWidth / mulX);
                indent += StepWidth;

                Label l = new Label { Content = Math.Round(minimumX, 2).ToString() };
                Canvas.SetLeft(l, indent);
                Canvas.SetTop(l, ActualH - Bottom1);//Отступ снизу постоянный
                Labels.Add(l);

                Lines.Add(new Line { X1 = temp, X2 = temp, Y1 = 0, Y2 = ActualH - Bottom1, Stroke = Brushes.Gray });
            }
            #endregion
            #region Построение горизонтальной сетки
            temp = ActualH - Bottom1;
            double minimumY = minY - Bottom2 / mulY;
            indent = ActualH - Bottom1 - 20;//Отступ от верхнего края
            Label l1 = new Label { Content = Math.Round(minimumY, 2).ToString() };
            Canvas.SetLeft(l1, 10);//Постоянное значение
            Canvas.SetTop(l1, indent);
            Labels.Add(l1);

            for (int j = 0; j < n + 1; j++)
            {
                temp -= StepHeight;
                minimumY += (StepHeight / mulY);
                indent -= StepHeight;
                Label l = new Label { Content = Math.Round(minimumY, 2).ToString() };
                Canvas.SetLeft(l, 10);//
                Canvas.SetTop(l, indent);
                Labels.Add(l);

                Lines.Add(new Line { X1 = Left1, X2 = ActualW, Y1 = temp, Y2 = temp, Stroke = Brushes.Gray });
            }
            #endregion
            #endregion
        }

        public List<Line> CreateGraphofSplines(Data.Point[] points, Brush brush, char ch)
        {

            double exchangeY = 0; // Расчетные значения точки по оси Y
            double exchangeX = 0; // Расчетные значения точки по оси X

            //Берем отрезок делим его на 5 частей берем 4 части,1 и 5 оставляем           
            //int n = 10;

            int interval = (points.Length - 1) * (n - 1);
            double step = (points[points.Length - 1].X - points[0].X) / (interval);

            ExchangePoint(points[0].X, points[0].Y, ref exchangeY, ref exchangeX);

            double tempExchangeX = exchangeX;
            double tempExchangeY = exchangeY;

            double x = points[0].X;
            double y = 0;
            List<Line> GraphLines = new List<Line>();

            if (ch == 'f')
            {
                for (int i = 0; i < interval - 1; i++)
                {
                    x += step;
                    y = Math.Sin(x / 3);

                    ExchangePoint(x, y, ref exchangeY, ref exchangeX);
                    GraphLines.Add(new Line { X1 = tempExchangeX * mulX + Left1 + Left2, X2 = exchangeX * mulX + Left1 + Left2, Y1 = tempExchangeY * mulY + Top, Y2 = exchangeY * mulY + Top, Stroke = brush });
                    tempExchangeX = exchangeX;
                    tempExchangeY = exchangeY;
                }

                x = points[points.Length - 1].X;
                y = Math.Sin(x / 3);

                ExchangePoint(x, y, ref exchangeY, ref exchangeX);
                GraphLines.Add(new Line { X1 = tempExchangeX * mulX + Left1 + Left2, X2 = exchangeX * mulX + Left1 + Left2, Y1 = tempExchangeY * mulY + Top, Y2 = exchangeY * mulY + Top, Stroke = brush });
                return GraphLines;
            }

            //points
            for (int i = 0; i < interval - 1; i++)
            {
                x += step;
                y = GetFunctionValue(x, ch);

                ExchangePoint(x, y, ref exchangeY, ref exchangeX);

                GraphLines.Add(new Line { X1 = tempExchangeX * mulX + Left1 + Left2, X2 = exchangeX * mulX + Left1 + Left2, Y1 = tempExchangeY * mulY + Top, Y2 = exchangeY * mulY + Top, Stroke = brush });

                tempExchangeX = exchangeX;
                tempExchangeY = exchangeY;
            }

            x = points[points.Length - 1].X;
            y = GetFunctionValue(x, ch);


            ExchangePoint(x, y, ref exchangeY, ref exchangeX);

            GraphLines.Add(new Line { X1 = tempExchangeX * mulX + Left1 + Left2, X2 = exchangeX * mulX + Left1 + Left2, Y1 = tempExchangeY * mulY + Top, Y2 = exchangeY * mulY + Top, Stroke = brush });

            return GraphLines;
        }

        public List<Ellipse> CreateGraphofPoints(Data.Point[] points, Brush brush)
        {
            this.BuildCoordinateGrid(points);
            List<Ellipse> Ellipses = new List<Ellipse>();

            double exchangeY = 0; // Расчетные значения точки по оси Y
            double exchangeX = 0; // Расчетные значения точки по оси X

            for (int i = 0; i < points.Length; i++)
            {
                ExchangePoint(points[i].X, points[i].Y, ref exchangeY, ref exchangeX);

                //Отступ 60 + 20(на самом графике)
                Ellipse el = new Ellipse();
                el.Height = 6;
                el.Width = 6;
                el.Stroke = brush;
                el.StrokeThickness = 5;
                Canvas.SetLeft(el, exchangeX * mulX + Left1 + Left2 - 3);// Сдвигаем на радиус -3
                Canvas.SetTop(el, exchangeY * mulY + Top - 3);
                Ellipses.Add(el);
            }
            return Ellipses;
        }

        private void ExchangePoint(double x, double y, ref double exchangeY, ref double exchangeX)
        {

            if ((maxY >= 0 && y >= 0) || (maxY <= 0 && y <= 0))
            {

                //
                exchangeY = Math.Abs(maxY - y);//модуль не нужен
            }
            if (maxY >= 0 && y <= 0)
            {
                exchangeY = Math.Abs(maxY + Math.Abs(y));
            }

            if ((minX >= 0 && x >= 0) || (maxX <= 0 && x <= 0))
            {
                exchangeX = Math.Abs(x - minX);//модуль не нужен
            }
            if ((minX <= 0 && x >= 0) || (minX < 0 && x < 0))
            {
                exchangeX = Math.Abs(x + Math.Abs(minX));
            }
        }

        public Data.Point[] CreateArray(double start, double end, double step)
        {
            List<Data.Point> p = new List<Data.Point>();

            double temp = start;

            double length = end - start;
            int n = Convert.ToInt32(length / step);
            for (int i = 0; i < n; i++)
            {
                p.Add(new Data.Point(temp));
                temp += step;
            }
            p.Add(new Data.Point(end));

            return p.ToArray();
        }
    }
}
