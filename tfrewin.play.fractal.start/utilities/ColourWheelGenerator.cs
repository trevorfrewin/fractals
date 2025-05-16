using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp.PixelFormats;

namespace tfrewin.play.fractal.start.utilities
{
    public class ColourWheelGenerator
    {
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

        private List<Rgba32> GenerateColourRangeGradient(int numberOfColours, float startAngle)
        {
            List<Rgba32> colourWheel = [];
            colourWheel.Add(new Rgba32()); // Add [JPG:Black]/[PNG:Transparent] as the first colour (used when the rule breaks in the Fractal)

            for (int i = 0; i < numberOfColours; i++)
            {
                float hue = ((i / (float)numberOfColours) * 360.0f + startAngle) % 360.0f;
                Rgba32 color = HsvToRgba(hue, 1.0f, 1.0f, 1.0f); // Full saturation, value, and alpha
                colourWheel.Add(color);
            }

            return colourWheel;
        }

        public List<ColourWheel> GenerateColourWheels()
        {
            var colourGradientVeryNarrow = 150;
            var colourGradientNarrow = 300;
            var colourGradientNormal = 1500;
            var colourGradientWide = 3000;
            var colourGradientVeryWide = 15000;

            var colourWheelBase = GenerateColourRangeGradient(colourGradientNormal, 0);

            var colourWheelRandom = new List<Rgba32>();
            foreach (var colourSpoke in colourWheelBase.OrderBy(cs => Guid.NewGuid()).ToList())
            {
                colourWheelRandom.Add(colourSpoke);
            }

            var returnThis = new List<ColourWheel>
            {
                new("generated", colourWheelBase),
                new("random", colourWheelRandom),
                new("ninety-very-narrow", GenerateColourRangeGradient(colourGradientVeryNarrow, 90)),
                new("ninety-narrow", GenerateColourRangeGradient(colourGradientNarrow, 90)),
                new("ninety", GenerateColourRangeGradient(colourGradientNormal, 90)),
                new("ninety-wide", GenerateColourRangeGradient(colourGradientWide, 90)),
                new("ninety-very-wide", GenerateColourRangeGradient(colourGradientVeryWide, 90)),
                new("oneeighty-very-narrow", GenerateColourRangeGradient(colourGradientVeryNarrow, 180)),
                new("oneeighty-narrow", GenerateColourRangeGradient(colourGradientNarrow, 180)),
                new("oneeighty", GenerateColourRangeGradient(colourGradientNormal, 180)),
                new("oneeighty-wide", GenerateColourRangeGradient(colourGradientWide, 180)),
                new("oneeighty-very-wide", GenerateColourRangeGradient(colourGradientVeryWide, 180))
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