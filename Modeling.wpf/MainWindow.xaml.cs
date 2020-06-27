using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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


    public partial class MainWindow : Window
    {
        //Данные
        private Modeling.Data.Point[] Points;//массив, который хранит точки табличной функции
        private GraphicalPart graphicalPart;
        
        public MainWindow()
        {
            InitializeComponent();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Для построения графика
            graphicalPart = new GraphicalPart(60, 20, 60, 20, 30, 30, 719.55, 1435.5, 20);

            Initial();
        }

        //Данные на форме при запуске приложения
        private void Initial()
        {
            //Создать массив рассчитать и разместить на экране данные взять с формы
            this.CreateArray(Convert.ToDouble(StartBox.Text), Convert.ToDouble(EndBox.Text), Convert.ToDouble(StepBox.Text));

            this.CreateTable();

            graphicalPart.CreateSpline(this.Points, Convert.ToDouble(AlphaBox.Text), Convert.ToDouble(BetaBox.Text));

            YBox.Content = graphicalPart.GetFunctionValue(Convert.ToDouble(XBox.Text),'n').ToString();
            SinBox.Content = Math.Sin(Convert.ToDouble(XBox.Text) / 3).ToString();
            //Построить график
            graphicalPart.BuildCoordinateGrid(Points);

            this.BuildGraphField();

            PrintGraph(Points, Brushes.Red);
        }

        private void CreateTable()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                TableFunc.Items.Add(new TablePoint { ValueX = Points[i].X, ValueY = Math.Round(Points[i].Y, 5) });
            }
        }

        private void Calculate()
        {
            TableFunc.Items.Clear();
            //Получить с формы данные, создать Spline, рассчитать и вывести на форму
            CreateArray(Convert.ToDouble(StartBox.Text), Convert.ToDouble(EndBox.Text), Convert.ToDouble(StepBox.Text));
            this.CreateTable();

            graphicalPart.CreateSpline(this.Points, Convert.ToDouble(AlphaBox.Text), Convert.ToDouble(BetaBox.Text));

            YBox.Content = graphicalPart.GetFunctionValue(Convert.ToDouble(XBox.Text),'n').ToString();
            SinBox.Content = Math.Sin(Convert.ToDouble(XBox.Text) / 3).ToString();
            //Построить график
            graphicalPart.BuildCoordinateGrid(Points);

            this.BuildGraphField();

            PrintGraph(Points, Brushes.Red);

            this.PrintSplineGraph(Points, Brushes.Blue);
        }

        private void CreateArray(double start, double end, double step)
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

            this.Points = p.ToArray();
        }
        private void CalculateY_Click(object sender, RoutedEventArgs e)
        {

            double x = Convert.ToDouble(XBox.Text);

            SinBox.Content = Math.Sin(x / 3).ToString();

            YBox.Content = graphicalPart.GetFunctionValue(x,'n').ToString();

        }
        private void BuildGraphField()
        {
            graphicalPart.BuildCoordinateGrid(Points);

            for (int i = 0; i < graphicalPart.Lines.Count; i++)
            {
                Canvas1.Children.Add(graphicalPart.Lines[i]);
            }

            for (int i = 0; i < graphicalPart.Labels.Count; i++)
            {
                Canvas1.Children.Add(graphicalPart.Labels[i]);
            }
        }

        //Точечный график
        private void PrintGraph(Data.Point[] points, Brush brush)
        {
            List<Ellipse> els = graphicalPart.CreateGraphofPoints(points, brush);
            for (int i = 0; i < els.Count; i++)
            {
                Canvas1.Children.Add(els[i]);
            }

        }

        //График сплайна
        private void PrintSplineGraph(Data.Point[] points, Brush brush)
        {
            List<Line> pLines = graphicalPart.CreateGraphofSplines(points, brush,'n');

            for (int i = 0; i < pLines.Count; i++)
            {
                Canvas1.Children.Add(pLines[i]);
            }
        }

        private void FirstCalculate_Click(object sender, RoutedEventArgs e)
        {
            Canvas1.Children.Clear();
            this.Calculate();
        }
    }
}
