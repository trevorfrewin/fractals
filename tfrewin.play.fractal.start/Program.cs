﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using tfrewin.play.fractal.start.engine;
using tfrewin.play.fractal.start.utilities;

/*
 Some sample commands:
 dotnet tfrewin.play.fractal.start.dll colourOffset=10 zoom=12000 moveX=-1.240128 moveY=0.12258 colourWheelName=First widthmultiplier=2 heightmultiplier=2
 dotnet tfrewin.play.fractal.start.dll colourOffset=10 zoom=12000 moveX=-1.240128 moveY=0.12258 colourWheelName=First widthmultiplier=16 heightmultiplier=16
 dotnet tfrewin.play.fractal.start.dll zoom=12000 moveX=-1.240128 moveY=0.12258 colourWheelName=ninety-narrow widthmultiplier=5 heightmultiplier=5 setname=mandelbrot
 */

namespace tfrewin.play.fractal.start
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            int planeWidth = 600;
            int planeHeight = 400;
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

            var imageParameters = new ImageParameters(DateTime.UtcNow, setName, planeWidth, planeHeight, zoom, moveX, moveY, iterationFactor, colourWheelName, colourOffset);
            var engine = new MatrixEngine();
            var matrix = await engine.PopulateMatrix(imageParameters);
            var results = engine.PopulateImage(imageParameters, matrix);

            Program.OutputFiles(results.Item1, results.Item2);
        }

        public static void OutputFiles(Image image, ImageParameters parameters)
        {
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
            Console.WriteLine("Saving Parameters '{0}' ...", parametersFilename);

            var imageAnnotatedPNGFilename = string.Concat(filename, ".annotated.png");
            parameters.ImageFilenames.Add(imageAnnotatedPNGFilename);

            var parametersContent = JsonConvert.SerializeObject(parameters, Formatting.Indented);
            File.WriteAllText(parametersFilename, parametersContent);

            AnnotateImage(image, parameters);
            Console.WriteLine("Saving Annotated PNG '{0}' ...", imageAnnotatedPNGFilename);
            image.SaveAsPng(imageAnnotatedPNGFilename);
        }

        private static void AnnotateImage(Image original, ImageParameters parameters)
        {
            const float TextPadding = 6f;
            const string TextFont = "Arial";
            const float TextFontSize = 12f;

            if (!SystemFonts.TryGet(TextFont, out FontFamily fontFamily))
                throw new Exception($"Couldn't find font {TextFont}");

            var font = fontFamily.CreateFont(TextFontSize, FontStyle.Regular);

            var options = new TextOptions(font)
            {
                Dpi = 72,
                KerningMode = KerningMode.None
            };

            var penWidth = original.Width < 1001 ? 1 : 3;
            var blackPen = Pens.Solid(Color.Black, penWidth);

            var verticalSpacing = original.Width / 10;
            var horizontalSpacing = original.Height / 10;

            original.Mutate(ctx =>
            {
                for (int x = 0; x < original.Width; x += verticalSpacing)
                {
                    ctx.DrawLine(blackPen, new PointF(x, 0), new PointF(x, original.Height));
                }

                for (int y = 0; y < original.Height; y += horizontalSpacing)
                {
                    ctx.DrawLine(blackPen, new PointF(0, y), new PointF(original.Width, y));
                }
            });

            var annotations = JsonConvert.SerializeObject(parameters, Formatting.Indented);
            var rect = TextMeasurer.MeasureSize(annotations, options);

            original.Mutate(x => x.DrawText(
                annotations,
                font,
                new Color(Rgba32.ParseHex("#06121DFF")),
                new PointF(original.Width - rect.Width - TextPadding,
                        original.Height - rect.Height - TextPadding)));

            if (parameters.MatrixExtents != null)
            {
                var horizontalBlockSize = (parameters.MatrixExtents.TopLeftExtent.Item1 - parameters.MatrixExtents.TopRightExtent.Item1) / 10;
                var verticalBlockSize = (parameters.MatrixExtents.TopLeftExtent.Item2 - parameters.MatrixExtents.BottomLeftExtent.Item1) / 10;

                var sizeSummaryText = string.Format("Per-block offsets.{0}H: {1}{2}V: {3}", Environment.NewLine, horizontalBlockSize, Environment.NewLine, verticalBlockSize);

                rect = TextMeasurer.MeasureSize(sizeSummaryText, options);

                original.Mutate(x => x.DrawText(
                    sizeSummaryText,
                    font,
                    new Color(Rgba32.ParseHex("#06121DFF")),
                    new PointF(TextPadding, TextPadding)));
            }
        }
    }
}