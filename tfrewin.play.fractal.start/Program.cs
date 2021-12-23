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
            int planeWidth = 400;
            int planeHeight = 300;
            double zoom = 1;
            double iterationFactor = 1;
            int colourOffset = 0;

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

            if (args.Any(a => a.ToLower().StartsWith("iterationfactor=")))
            {
                iterationFactor *= double.Parse(args.First(a => a.ToLower().StartsWith("iterationfactor=")).ToLower().Replace("iterationfactor=", string.Empty));
            }

            if (args.Any(a => a.ToLower().StartsWith("zoom=")))
            {
                zoom *= double.Parse(args.First(a => a.ToLower().StartsWith("zoom=")).ToLower().Replace("zoom=", string.Empty));
            }

            if (args.Any(a => a.ToLower().StartsWith("colouroffset=")))
            {
                colourOffset = int.Parse(args.First(a => a.ToLower().StartsWith("colouroffset=")).ToLower().Replace("colouroffset=", string.Empty));
            }

            new Program().PaintFile(setName, planeWidth, planeHeight, zoom, iterationFactor, colourOffset);
        }

        private Matrix GetMatrixForFormula(string setName, int planeWidth, int planeHeight, double zoom, int maximumIteration)
        {
            return new FormulaProcessorFactory().Create(setName).Process(planeWidth, planeHeight, zoom, maximumIteration);
        }

        public void PaintFile(string setName, int planeWidth, int planeHeight, double zoom, double iterationFactor, int colourOffset)
        {
            var colours = this.GenerateColourWheel().ToArray();

            Console.WriteLine("{0} - Processing for '{1}' ...", DateTime.UtcNow.ToString("o"), setName);

            var matrix = this.GetMatrixForFormula(setName, planeWidth, planeHeight, zoom, (int)(colours.Length * iterationFactor));

            Console.WriteLine("{0} Painting ...", DateTime.UtcNow.ToString("o"));

            var bitmap = new Bitmap(planeWidth, planeHeight);

            foreach (var point in matrix.Points)
            {
                int colourPoint = 0;

                if (point.IterationCount > 0)
                {
                    colourPoint = (int)(point.IterationCount / iterationFactor) + colourOffset;

                    while (colourPoint >= colours.Length)
                    {
                        colourPoint -= colours.Length;
                    }
                }

                bitmap.SetPixel(point.XAxisValue, point.YAxisValue, colours[colourPoint]);
            }

            var filename = string.Format("painted-{0}-{1}.output.png", setName, DateTime.UtcNow.ToString("o").Replace(":", string.Empty).Replace(".", string.Empty));
            Console.WriteLine(filename);
            bitmap.Save(filename);
        }

        public List<Color> GenerateColourWheel()
        {
            var returnThis = new List<Color>();

            // The range of RED
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(i, 0, 0));
            }

            // All RED AND The range of GREEN
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(255, i, 0));
            }

            // All GREEN and the reverse range of RED
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(255-i, 255, 0));
            }

            // All GREEN AND the range of BLUE
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(0, 255, i));
            }

            // All BLUE AND the reverse range of GREEN
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(0, 255-i, 255));
            }

            // All BLUE AND the range of RED
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(i, 0, 255));
            }

            // All GREEN AND All BLUE AND a range of RED
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(i, 255, 255));
            }

            // All RED AND All BLUE AND a range of GREEN
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(255, i, 255));
            }

            // All GREEN AND All RED AND a range of BLUE
            for (int i = 0; i < 256; i++)
            {
                returnThis.Add(Color.FromArgb(255, 255, i));
            }

            return returnThis; // 9 * 255 = 2295
        }
    }
}