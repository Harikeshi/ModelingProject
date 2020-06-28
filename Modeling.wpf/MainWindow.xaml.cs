using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        private void Initial()
        {
            //Данные на форме при запуске приложения
            //Создать массив рассчитать и разместить на экране данные взять с формы
            Points = graphicalPart.CreateArray(Convert.ToDouble(StartBox.Text), Convert.ToDouble(EndBox.Text), Convert.ToDouble(StepBox.Text));

            this.CreateTable();

            graphicalPart.CreateSpline(this.Points, Convert.ToDouble(AlphaBox.Text), Convert.ToDouble(BetaBox.Text));

            YBoxNatural.Content = graphicalPart.GetFunctionValue(Convert.ToDouble(XBox.Text), 'n').ToString();
            YBoxClamped.Content = graphicalPart.GetFunctionValue(Convert.ToDouble(XBox.Text), 'c').ToString();
            SinBox.Content = Math.Sin(Convert.ToDouble(XBox.Text) / 3).ToString();
            //Построить график
            graphicalPart.BuildCoordinateGrid(Points);

            this.BuildGraphField();

            this.PrintSplineGraph(Points, Brushes.Green, 'c');
            this.PrintSplineGraph(Points, Brushes.Red, 'f');
            this.PrintSplineGraph(Points, Brushes.Blue, 'n');

            PrintGraph(Points, Brushes.Red);

            NaturalCheck.IsChecked = true;
            FuncCheck.IsChecked = true;
            ClampedCheck.IsChecked = true;
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
            Points = graphicalPart.CreateArray(Convert.ToDouble(StartBox.Text), Convert.ToDouble(EndBox.Text), Convert.ToDouble(StepBox.Text));
            this.CreateTable();

            graphicalPart.CreateSpline(this.Points, Convert.ToDouble(AlphaBox.Text), Convert.ToDouble(BetaBox.Text));

            YBoxNatural.Content = graphicalPart.GetFunctionValue(Convert.ToDouble(XBox.Text), 'n').ToString();
            YBoxClamped.Content = graphicalPart.GetFunctionValue(Convert.ToDouble(XBox.Text), 'c').ToString();

            SinBox.Content = Math.Sin(Convert.ToDouble(XBox.Text) / 3).ToString();
            //Построить график
            graphicalPart.BuildCoordinateGrid(Points);

            this.BuildGraphField();

            PrintGraph(Points, Brushes.Red);

            if (NaturalCheck.IsChecked == true)
                this.PrintSplineGraph(Points, Brushes.Blue, 'n');
            if (ClampedCheck.IsChecked == true)
                this.PrintSplineGraph(Points, Brushes.Green, 'c');
            if (FuncCheck.IsChecked == true)
                this.PrintSplineGraph(Points, Brushes.Red, 'f');
        }

        private void CalculateY_Click(object sender, RoutedEventArgs e)
        {
            double x = Convert.ToDouble(XBox.Text);

            SinBox.Content = Math.Sin(x / 3).ToString();

            YBoxNatural.Content = graphicalPart.GetFunctionValue(x, 'n').ToString();
            YBoxClamped.Content = graphicalPart.GetFunctionValue(x, 'c').ToString();
        }

        private void FirstCalculate_Click(object sender, RoutedEventArgs e)
        {
            Canvas1.Children.Clear();
            this.Calculate();
        }

        private void BuildGraphField()
        {
            //Создание сетки и надписей на форме
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

        private void PrintGraph(Data.Point[] points, Brush brush)
        {
            //Точечный график для таблицы
            List<Ellipse> els = graphicalPart.CreateGraphofPoints(points, brush);
            for (int i = 0; i < els.Count; i++)
            {
                Canvas1.Children.Add(els[i]);
            }
        }

        private void PrintSplineGraph(Data.Point[] points, Brush brush, char ch)
        {
            //Построение графика по линиям
            List<Line> Lines = graphicalPart.CreateGraphofSplines(points, brush, ch);
            for (int i = 0; i < Lines.Count; i++)
            {
                Canvas1.Children.Add(Lines[i]);
            }
        }

    }
}
