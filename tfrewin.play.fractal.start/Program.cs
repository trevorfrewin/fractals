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

/*
        private Dictionary<int, List<Tuple<int,int>>> ProcessJulia(int planeWidth, int planeHeight, int zoom, int maximumIteration)
        {
            Dictionary<int, List<Tuple<int,int>>> returnThis = new Dictionary<int, List<Tuple<int,int>>>();

            const int moveX = 0;
            const int moveY = 0;
            const double cX = -0.7;
            const double cY = 0.27015;
            double zx, zy, tmp;
            int i;
 
            for (int x = 0; x < planeWidth; x++)
            {
                for (int y = 0; y < planeHeight; y++)
                {
                    if (!returnThis.ContainsKey(y))
                    {
                        returnThis.Add(y, new List<Tuple<int,int>>());
                    }

                    zx = 1.5 * (x - planeWidth / 2) / (0.5 * zoom * planeWidth) + moveX;
                    zy = 1.0 * (y - planeHeight / 2) / (0.5 * zoom * planeHeight) + moveY;
                    i = maximumIteration;
                    while (zx * zx + zy * zy < 4 && i > 1)
                    {
                        tmp = zx * zx - zy * zy + cX;
                        zy = 2.0 * zx * zy + cY;
                        zx = tmp;
                        i -= 1;
                    }

                    returnThis[y].Add(new Tuple<int,int>(x, i));
                }
            }

            return returnThis;
        }

        public void GetFile(int planeWidth, int planeHeight, int zoom, int maxiter)
        {
            const int moveX = 0;
            const int moveY = 0;
            const double cX = -0.7;
            const double cY = 0.27015;
            double zx, zy, tmp;
            int i;
 
            var colors = (from c in Enumerable.Range(0, 256)
                          select Color.FromArgb((c >> 5) * 36, (c >> 3 & 7) * 36, (c & 3) * 85)).ToArray();
            var bitmap = new Bitmap(planeWidth, planeHeight);
            
            for (int x = 0; x < planeWidth; x++)
            {
                for (int y = 0; y < planeHeight; y++)
                {
                    zx = 1.5 * (x - planeWidth / 2) / (0.5 * zoom * planeWidth) + moveX;
                    zy = 1.0 * (y - planeHeight / 2) / (0.5 * zoom * planeHeight) + moveY;
                    i = maxiter;
                    while (zx * zx + zy * zy < 4 && i > 1)
                    {
                        tmp = zx * zx - zy * zy + cX;
                        zy = 2.0 * zx * zy + cY;
                        zx = tmp;
                        i -= 1;
                    }
                    bitmap.SetPixel(x, y, colors[i]);
                }
            }

            var filename = string.Format("julia-set-{0}.png", DateTime.UtcNow.ToString("o").Replace(":", string.Empty).Replace(".", string.Empty));
            Console.WriteLine(filename);
            bitmap.Save(filename);
        }
*/
    }
}