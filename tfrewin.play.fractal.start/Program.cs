using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using tfrewin.play.fractal.start.processor;
using tfrewin.play.fractal.start.processor.output;

namespace tfrewin.play.fractal.start
{
    class Program
    {
        private List<Color> ColorWheel;

        public Program()
        {
            this.ColorWheel = this.GenerateColourWheel();
        }

        static void Main(string[] args)
        {
            // int planeWidth = 1920;
            // int planeHeight = 1080;
            int planeWidth = 300;
            int planeHeight = 200;
            double zoom = 1;
            double moveX = 0;
            double moveY = 0;
            double iterationFactor = 1;
            int colourOffset = 0;

            string setName = "mandelbrot";

            if (args.Any(a => a.ToLower().StartsWith("setname=")))
            {
                setName = args.First(a => a.ToLower().StartsWith("setname=")).ToLower().Replace("setname=", string.Empty);
            }

            if (args.Any(a => a.ToLower().StartsWith("widthmultiplier=")))
            {
                planeWidth = (int)(planeWidth * double.Parse(args.First(a => a.ToLower().StartsWith("widthmultiplier=")).ToLower().Replace("widthmultiplier=", string.Empty)));
            }

            if (args.Any(a => a.ToLower().StartsWith("heightmultiplier=")))
            {
                planeHeight = (int)(planeHeight * double.Parse(args.First(a => a.ToLower().StartsWith("heightmultiplier=")).ToLower().Replace("heightmultiplier=", string.Empty)));
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

            if (args.Any(a => a.ToLower().StartsWith("movex=")))
            {
                moveX = double.Parse(args.First(a => a.ToLower().StartsWith("movex=")).ToLower().Replace("movex=", string.Empty));
            }

            if (args.Any(a => a.ToLower().StartsWith("movey=")))
            {
                moveY = double.Parse(args.First(a => a.ToLower().StartsWith("movey=")).ToLower().Replace("movey=", string.Empty));
            }

            new Program().PaintFile(new ImageParameters(DateTime.UtcNow, setName, planeWidth, planeHeight, zoom, moveX, moveY, iterationFactor, colourOffset));
        }

        private Matrix GetMatrixForFormula(string setName, int planeWidth, int planeHeight, double zoom, double moveX, double moveY, int maximumIteration)
        {
            return new FormulaProcessorFactory().Create(setName).Process(planeWidth, planeHeight, zoom, moveX, moveY, maximumIteration);
        }

        public void PaintFile(ImageParameters parameters)
        {
            var colours = this.ColorWheel.ToArray();

            Console.WriteLine("{0} - Processing for '{1}' ...", DateTime.UtcNow.ToString("o"), parameters.SetName);

            var processingStopWatch = new Stopwatch();
            processingStopWatch.Start();
            var matrix = this.GetMatrixForFormula(parameters.SetName, parameters.PlaneWidth, parameters.PlaneHeight, parameters.Zoom, parameters.MoveX, parameters.MoveY, (int)(colours.Length * parameters.IterationFactor));
            processingStopWatch.Stop();
            parameters.ProcessingMilliseconds = processingStopWatch.ElapsedMilliseconds;

            Console.WriteLine("{0} Painting ...", DateTime.UtcNow.ToString("o"));

            var paintingStopWatch = new Stopwatch();
            paintingStopWatch.Start();
            var bitmap = new Bitmap(parameters.PlaneWidth, parameters.PlaneHeight);

            foreach (var point in matrix.Points)
            {
                int colourPoint = 0;

                if (point.IterationCount > 0)
                {
                    colourPoint = (int)(point.IterationCount / parameters.IterationFactor) + parameters.ColourOffset;

                    while (colourPoint >= colours.Length)
                    {
                        colourPoint -= colours.Length;
                    }
                }

                bitmap.SetPixel(point.XAxisValue, point.YAxisValue, colours[colourPoint]);
            }
            paintingStopWatch.Stop();
            parameters.PaintMilliseconds = paintingStopWatch.ElapsedMilliseconds;

            var filename = string.Format("{0}-{1}.output", parameters.SetName, DateTime.UtcNow.ToString("o").Replace(":", string.Empty).Replace(".", string.Empty));
            var imageFilename = string.Concat(filename, ".png");
            Console.WriteLine("Saving {0} ...", imageFilename);
            bitmap.Save(imageFilename);

            var parametersFilename = string.Concat(filename, ".json");
            Console.WriteLine("Saving {0} ...", parametersFilename);

            parameters.ImageFilename = imageFilename;
            var parametersContent = JsonConvert.SerializeObject(parameters, Formatting.Indented);
            File.WriteAllText(parametersFilename, parametersContent);
        }

/// TODO: Move this to a single call on construction
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