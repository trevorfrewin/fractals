using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using tfrewin.play.fractal.start.processor;
using tfrewin.play.fractal.start.processor.output;
using tfrewin.play.fractal.start.utilities;

namespace tfrewin.play.fractal.start.engine
{
    public class MatrixEngine
    {
        private List<ColourWheel> ColorWheels;

        public MatrixEngine()
        {
            this.ColorWheels = new ColourWheelGenerator().GenerateColourWheels();
        }

        public Matrix PopulateMatrix(ImageParameters parameters)
        {
            var colours = this.ColorWheels.Where(cw => cw.ColourWheelName.Equals(parameters.ColourWheelName)).FirstOrDefault().Colours.ToArray();

            Console.WriteLine("{0} - Processing for '{1}' ...", DateTime.UtcNow.ToString("o"), parameters.SetName);

            var processingStopWatch = new Stopwatch();
            processingStopWatch.Start();

            var matrix = new FormulaProcessorFactory().Create(parameters.SetName).Process(parameters.PlaneWidth, parameters.PlaneHeight, parameters.Zoom, parameters.MoveX, parameters.MoveY, (int)(colours.Length * parameters.IterationFactor));

            processingStopWatch.Stop();
            parameters.ProcessingMilliseconds = processingStopWatch.ElapsedMilliseconds;
            parameters.MatrixExtents = matrix.MatrixExtents;

            return matrix;
        }

        public Tuple<Image, ImageParameters> PopulateImage(ImageParameters parameters, Matrix matrix)
        {
            var colours = this.ColorWheels.Where(cw => cw.ColourWheelName.Equals(parameters.ColourWheelName)).FirstOrDefault().Colours.ToArray();

            Console.WriteLine("{0} Painting ...", DateTime.UtcNow.ToString("o"));

            var paintingStopWatch = new Stopwatch();
            paintingStopWatch.Start();

            Image<Rgba32> image = new(parameters.PlaneWidth, parameters.PlaneHeight);
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

            return new Tuple<Image, ImageParameters>(image, parameters);
        }
    }
}