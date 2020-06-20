using System;

namespace Modeling.Data
{
    class Program
    {

        //TODO Задать функцию
        //TODO Получить сетку
        //TODO Придумать значения производной и второй производной в начальной и конечной точках
        //TODO Для каждой точки (x) рассчитать полиномы

        //TODO Метод Гаусса

        //TODO Указать на графике точки табличные и график интерполяции


        static void Main(string[] args)
        {
            //Значения x
            //{-1,1,3,5,10,15}

            GridFunction func = new GridFunction();

            Spline spline = new Spline(func);

            spline.GenerateSplines();

            Console.WriteLine(new string('-', 20));

            double x = 12;
            Console.WriteLine($"Синус({x}) = {Math.Sin(x)}");
            Console.WriteLine($"Инерполяция: {spline.GetFunctionValue(x, 10)}");
            Console.WriteLine(new string('-', 20));

        }
    }
}
