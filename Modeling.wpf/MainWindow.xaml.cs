using System;
using System.Collections.Generic;
using System.Windows;
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
        public Modeling.Data.Point[] Data;//массив, который хранит точки табличной функции
        public double[] Begin_array = { -1, 1, 3, 5, 10, 15 };
        

        public MainWindow()
        {
            InitializeComponent();

            
            //1. заполнить таблицу

            //будет заполняться по функции синус 
            //Вычесляем значения Y и добавляем элементы в таблицу

            //Построить график


            //2. рассчитать значение
            //3. 

            //1.


           
        }

        private void CalculateY_Click(object sender, RoutedEventArgs e)
        {
            GridFunction func = new GridFunction();
            Spline spline = new Spline(func.Data);

            spline.GenerateSplines();

            double x = Convert.ToDouble(valueX.Text);

            SinX.Content= Math.Sin(x).ToString();


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

            SinX.Content= Math.Sin(x).ToString();
            List<Modeling.Data.Point> points = spline.GetValuesForDrow();

            valueY.Content = spline.GetFunctionValue(x).ToString();
        }
    }
}
