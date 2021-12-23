using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using tfrewin.play.fractal.start.processor;

namespace tfrewin.play.fractal.start
{
    class Program
    {
        static void Main(string[] args)
        {
            const int planeWidth = 400 * 16;
            const int planeHeight = 300 * 16;
            const int zoom = 1;

            if (args.Length > 0 && string.Equals(args[0].ToLower(), "j"))
            {
                new Program().PaintFile("JuliaSet", planeWidth, planeHeight, zoom);
                return;
            }

            new Program().PaintFile("MandelbrotSet", planeWidth, planeHeight, zoom);
        }

        private Matrix GetMatrixForFormula(string formulaName, int planeWidth, int planeHeight, int zoom)
        {
            return new FormulaProcessorFactory().Create(formulaName).Process(planeWidth, planeHeight, zoom);
        }

        public void PaintFile(string formulaName, int planeWidth, int planeHeight, int zoom)
        {
            Console.WriteLine("Processing for '{0}' ...", formulaName);

            var matrix = this.GetMatrixForFormula(formulaName, planeWidth, planeHeight, zoom);

            Console.WriteLine("Painting ...");
            var colors = (from c in Enumerable.Range(0, 256)
                          select Color.FromArgb((c >> 5) * 36, (c >> 3 & 7) * 36, (c & 3) * 85)).ToArray();
            var bitmap = new Bitmap(planeWidth, planeHeight);

            foreach (var point in matrix.Points)
            {
                bitmap.SetPixel(point.XAxisValue, point.YAxisValue, colors[point.IterationCount]);
            }

            var filename = string.Format("painted-{0}-{1}.output.png", formulaName, DateTime.UtcNow.ToString("o").Replace(":", string.Empty).Replace(".", string.Empty));
            Console.WriteLine(filename);
            bitmap.Save(filename);
        }
    }

    public class Matrix
    {
        public int MaximumIterations { get; private set; }

        public List<Point> Points { get; private set; }

        public Matrix(int maximumIterations)
        {
            this.MaximumIterations = maximumIterations;
            this.Points = new List<Point>();
        }
    }

    public class Point
    {
        public int XAxisValue { get; private set; }

        public int YAxisValue { get; private set; }

        public int IterationCount { get; private set; }

        public Point(int xAxisValue, int yAxisValue, int iterationCount)
        {
            this.XAxisValue = xAxisValue;
            this.YAxisValue = yAxisValue;
            this.IterationCount = iterationCount;
        }
    }
}