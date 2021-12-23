using System;
using System.Drawing;
using System.Linq;

using tfrewin.play.fractal.start.processor;

namespace tfrewin.play.fractal.start
{
    class Program
    {
        static void Main(string[] args)
        {
            int planeWidth = 400;
            int planeHeight = 300;
            int zoom = 1;

            string setName = "mandelbrot";

            if (args.Any(a => a.ToLower().StartsWith("setname=")))
            {
                setName = args.First(a => a.ToLower().StartsWith("setname=")).ToLower().Replace("setname=", string.Empty);
            }

            if (args.Any(a => a.ToLower().StartsWith("widthmultiplier=")))
            {
                planeWidth *= int.Parse(args.First(a => a.ToLower().StartsWith("widthmultiplier=")).ToLower().Replace("widthmultiplier=", string.Empty));
            }

            if (args.Any(a => a.ToLower().StartsWith("heightmultiplier=")))
            {
                planeHeight *= int.Parse(args.First(a => a.ToLower().StartsWith("heightmultiplier=")).ToLower().Replace("heightmultiplier=", string.Empty));
            }

            new Program().PaintFile(setName, planeWidth, planeHeight, zoom);
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
}