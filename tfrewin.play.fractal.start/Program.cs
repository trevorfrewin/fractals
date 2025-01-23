using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using tfrewin.play.fractal.start.processor;
using tfrewin.play.fractal.start.processor.output;
using tfrewin.play.fractal.start.utilities;

/*
 Some sample commands:
 dotnet tfrewin.play.fractal.start.dll colourOffset=10 zoom=12000 moveX=-1.240128 moveY=0.12258 colourWheelName=First widthmultiplier=2 heightmultiplier=2
 dotnet tfrewin.play.fractal.start.dll colourOffset=10 zoom=12000 moveX=-1.240128 moveY=0.12258 colourWheelName=First widthmultiplier=16 heightmultiplier=16

 */

namespace tfrewin.play.fractal.start
{
    class Program
    {
        private List<ColourWheel> ColorWheels;

        public Program()
        {
            this.ColorWheels = new ColourWheelGenerator().GenerateColourWheels();
        }

        static void Main(string[] args)
        {
            int planeWidth = 300;
            int planeHeight = 200;
            double zoom = 1;
            double moveX = 0;
            double moveY = 0;
            double iterationFactor = 1;
            int colourOffset = 0;
            string colourWheelName = "Generated";

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

            if (args.Any(a => a.ToLower().StartsWith("colourwheelname=")))
            {
                colourWheelName = args.First(a => a.ToLower().StartsWith("colourwheelname=")).ToLower().Replace("colourwheelname=", string.Empty);
            }

            if (args.Any(a => a.ToLower().StartsWith("movex=")))
            {
                moveX = double.Parse(args.First(a => a.ToLower().StartsWith("movex=")).ToLower().Replace("movex=", string.Empty));
            }

            if (args.Any(a => a.ToLower().StartsWith("movey=")))
            {
                moveY = double.Parse(args.First(a => a.ToLower().StartsWith("movey=")).ToLower().Replace("movey=", string.Empty));
            }

            new Program().PaintFile(new ImageParameters(DateTime.UtcNow, setName, planeWidth, planeHeight, zoom, moveX, moveY, iterationFactor, colourWheelName, colourOffset));
        }

        private Matrix GetMatrixForFormula(string setName, int planeWidth, int planeHeight, double zoom, double moveX, double moveY, int maximumIteration)
        {
            return new FormulaProcessorFactory().Create(setName).Process(planeWidth, planeHeight, zoom, moveX, moveY, maximumIteration);
        }

        public void PaintFile(ImageParameters parameters)
        {
            var colours = this.ColorWheels.Where(cw => cw.ColourWheelName.Equals(parameters.ColourWheelName)).FirstOrDefault().Colours.ToArray();

            Console.WriteLine("{0} - Processing for '{1}' ...", DateTime.UtcNow.ToString("o"), parameters.SetName);

            var processingStopWatch = new Stopwatch();
            processingStopWatch.Start();
            var matrix = this.GetMatrixForFormula(parameters.SetName, parameters.PlaneWidth, parameters.PlaneHeight, parameters.Zoom, parameters.MoveX, parameters.MoveY, (int)(colours.Length * parameters.IterationFactor));
            processingStopWatch.Stop();
            parameters.ProcessingMilliseconds = processingStopWatch.ElapsedMilliseconds;

            Console.WriteLine("{0} Painting ...", DateTime.UtcNow.ToString("o"));

            var paintingStopWatch = new Stopwatch();
            paintingStopWatch.Start();

            using (Image<Rgba32> image = new(parameters.PlaneWidth, parameters.PlaneHeight))
            {
                foreach (var point in matrix)
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

                    image[point.XAxisValue, point.YAxisValue] = colours[colourPoint];
                }
                paintingStopWatch.Stop();
                parameters.PaintMilliseconds = paintingStopWatch.ElapsedMilliseconds;

                var filename = string.Format("{0}-{1}.output", parameters.SetName, DateTime.UtcNow.ToString("o").Replace(":", string.Empty).Replace(".", string.Empty));
                var imagePNGFilename = string.Concat(filename, ".png");
                Console.WriteLine("Saving PNG '{0}' ...", imagePNGFilename);
                image.SaveAsPng(imagePNGFilename);
                parameters.ImageFilenames.Add(imagePNGFilename);

                var imageJPGFilename = string.Concat(filename, ".jpg");
                Console.WriteLine("Saving JPG '{0}' ...", imageJPGFilename);
                image.SaveAsJpeg(imageJPGFilename);
                parameters.ImageFilenames.Add(imageJPGFilename);

                var parametersFilename = string.Concat(filename, ".json");
                Console.WriteLine("Saving Paramaters '{0}' ...", parametersFilename);

                var parametersContent = JsonConvert.SerializeObject(parameters, Formatting.Indented);
                File.WriteAllText(parametersFilename, parametersContent);
            }
        }
    }
}