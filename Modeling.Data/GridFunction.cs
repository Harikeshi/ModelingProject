namespace Modeling.Data
{
    public class GridFunction
    {

        //TODO Получить сетку
        //TODO Придумать значения производной и второй производной в начальной и конечной точках
        // значения производных брать с формы

        public Point[] Data = new Point[6]
        {
            new Point(-1),
            new Point(1),
            new Point(3),
            new Point(5),
            new Point(10),
            new Point(15)
        };
        //public GridFunction(double df, double ddf)
        //{
        //    Data[0].df = df;
        //    Data[0].ddf = ddf;
        //}
    }
}
