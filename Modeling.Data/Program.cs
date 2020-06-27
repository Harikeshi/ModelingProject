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

            GraphicSpline sp = new GraphicSpline(func.Data,1,1);

            sp.BuildSubIntervals();
            

            sp.ShowMatrixNatural();
            sp.ShowMatrixClamped();
         
            Console.WriteLine(sp.GetFunction(12,'n'));
            Console.WriteLine(Math.Sin(12));
        }
    }
}
