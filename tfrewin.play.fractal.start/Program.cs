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
            const int planeWidth = 800 * 4;
            const int planeHeight = 600 * 4;
            const int zoom = 1;
            const int maximumIteration = 255;

            new Program().PaintFile("JuliaSet", planeWidth, planeHeight, zoom, maximumIteration);
        }

        private Dictionary<int, List<Tuple<int,int>>> GetMatrixForFormula(string formula, int planeWidth, int planeHeight, int zoom, int maximumIteration)
        {
            IFormulaProcesor formulaProcessor;

            switch(formula)
            {
                case "JuliaSet" :
                    formulaProcessor = new JuliaSetProcessor();
                    break;
                default:
                    throw new ArgumentException(string.Format("Formula '{0}' is not supported."));
            }

            return formulaProcessor.Process(planeWidth, planeHeight, zoom, maximumIteration);
        }

        public void PaintFile(string formula, int planeWidth, int planeHeight, int zoom, int maximumIteration)
        {
            Console.WriteLine("Processing for '{0}' ...", formula);

            var matrix = this.GetMatrixForFormula(formula, planeWidth, planeHeight, zoom, maximumIteration);

            Console.WriteLine("Painting ...");
            var colors = (from c in Enumerable.Range(0, 256)
                          select Color.FromArgb((c >> 5) * 36, (c >> 3 & 7) * 36, (c & 3) * 85)).ToArray();
            var bitmap = new Bitmap(planeWidth, planeHeight);

            foreach (var y in matrix.Keys)
            {
                foreach(var xPositionAndColour in matrix[y])
                {
                    bitmap.SetPixel(xPositionAndColour.Item1, y, colors[xPositionAndColour.Item2]);
                }
            }

            var filename = string.Format("painted-{0}-{1}.output.png", formula, DateTime.UtcNow.ToString("o").Replace(":", string.Empty).Replace(".", string.Empty));
            Console.WriteLine(filename);
            bitmap.Save(filename);
        }
    }
}