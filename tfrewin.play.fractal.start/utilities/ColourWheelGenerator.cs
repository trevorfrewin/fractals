using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp.PixelFormats;

namespace tfrewin.play.fractal.start.utilities
{
    public class ColourWheelGenerator
    {
        private List<List<Rgba32>> GenerateColourSpokes(Int32 intensity)
        {
            var returnThis = new List<List<Rgba32>>();

            // The range of RED
            var currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(i, 0, 0, intensity));
            }
            returnThis.Add(currentSet);

            // RED and the range of GREEN
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(255, i, 0, intensity));
            }
            returnThis.Add(currentSet);

            // GREEN and the reverse range of RED
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(255 - i, 255, 0, intensity));
            }
            returnThis.Add(currentSet);

            // GREEN and the range of BLUE
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(0, 255, i, intensity));
            }
            returnThis.Add(currentSet);

            // BLUE and the reverse range of GREEN
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(0, 255 - i, 255, intensity));
            }
            returnThis.Add(currentSet);

            // BLUE and the range of RED
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(i, 0, 255, intensity));
            }
            returnThis.Add(currentSet);

            // All GREEN AND All BLUE AND a range of RED
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(i, 255, 255, intensity));
            }
            returnThis.Add(currentSet);

            // All RED AND All BLUE AND a range of GREEN
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(255, i, 255, intensity));
            }
            returnThis.Add(currentSet);

            // All GREEN AND All RED AND a range of BLUE
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(255, 255, i, intensity));
            }
            returnThis.Add(currentSet);

            return returnThis;
        }

        private static Rgba32 HsvToRgba(float h, float s, float v, float a)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            float m = v - c;

            float r, g, b;
            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return new Rgba32(r + m, g + m, b + m, a);
        }

        private List<Rgba32> GenerateColourRangeGradient()
        {
            int numColours = 2400;
            List<Rgba32> colourWheel = [];
            colourWheel.Add(new Rgba32()); // Add [JPG:Black]/[PNG:Transparent] as the first colour (used when the rule breaks in the Fractal)

            for (int i = 0; i < numColours; i++)
            {
                float hue = (i / (float)numColours) * 360.0f; // Scale to 360-degree hue range
                Rgba32 color = HsvToRgba(hue, 1.0f, 1.0f, 1.0f); // Full saturation, value, and alpha
                colourWheel.Add(color);
            }

            // Output first 10 colors for validation
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Color {i}: {colourWheel[i]}");
            }

            return colourWheel;
        }

        public List<ColourWheel> GenerateColourWheels()
        {
            var colourSpokesNormal = GenerateColourSpokes(255);
            var colourWheelFirst = new List<Rgba32>();
            foreach (var colourSpoke in colourSpokesNormal)
            {
                colourWheelFirst.AddRange(colourSpoke);
            }

            var colourSpokesDark = GenerateColourSpokes(50);
            var colourWheelDark = new List<Rgba32>();
            foreach (var colourSpoke in colourSpokesDark)
            {
                colourWheelDark.AddRange(colourSpoke);
            }

            var colourWheelGenerated = GenerateColourRangeGradient();
            var colourWheelRandom = new List<Rgba32>();
            foreach (var colourSpoke in colourSpokesNormal.OrderBy(cs => Guid.NewGuid()).ToList())
            {
                colourWheelRandom.AddRange(colourSpoke);
            }

            var returnThis = new List<ColourWheel>
            {
                new("first", colourWheelFirst),
                new("dark", colourWheelDark),
                new("generated", colourWheelGenerated),
                new("random", colourWheelRandom)
            };
            return returnThis;
        }
    }

    public class ColourWheel
    {
        public string ColourWheelName { get; private set; }

        public List<Rgba32> Colours { get; private set; }

        public ColourWheel(string colourWheelName, List<Rgba32> colours)
        {
            this.ColourWheelName = colourWheelName;
            this.Colours = colours;
        }
    }
}