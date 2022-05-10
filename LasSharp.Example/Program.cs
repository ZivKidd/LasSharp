namespace LasSharp.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LasReader lasReader = new LasReader(@"D:\desktop\test\50.las");
            LasWriter lasWriter = new LasWriter(@"D:\desktop\test\501.las", pointDataFormat: 0, xScale: 0.001, yOffset: 100, versionMinor: 1);
            LasPoint lasPoint;
            while (lasReader.MoveToNextPoint())
            {
                lasPoint = lasReader.CurrentPoint;
                lasWriter.WritePoint(lasPoint);
            }
            lasReader.Close();
            lasWriter.Close();// very important, write the border and point num to las
        }
    }
}
