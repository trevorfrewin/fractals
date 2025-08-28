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

        private List<Rgba32> GenerateCyanColourBand(int numberOfColours, float startAngle)
        {
            List<Rgba32> colourWheel = [];
            colourWheel.Add(new Rgba32()); // Add [JPG:Black]/[PNG:Transparent] as the first colour (used when the rule breaks in the Fractal)

            for (int i = 0; i < numberOfColours; i++)
            {
                // LightCyan: E0FFFF
                // Cyan: 00FFFF
                // DarkCyan: 008B8B

                // R: E0 -> 00
                // G: FF -> 8B
                // B: FF -> 8B

                // E0 is (dec)224
                // 8B is (dec)139 (or 255-116)
                byte redFloat, greenFloat, blueFloat;
                redFloat = (byte)(224 - (224 * (i / (float)numberOfColours))); //E0 -> 00
                greenFloat = (byte)(255 - (116 * (i / (float)numberOfColours))); // FF -> 8B
                blueFloat = (byte)(255 - (116 * (i / (float)numberOfColours))); // FF -> 8B

                Rgba32 color = new Rgba32(redFloat, greenFloat, blueFloat);
                colourWheel.Add(color);
            }

            return colourWheel;
        }

        private List<Rgba32> GenerateMagentaColourBand(int numberOfColours, float startAngle)
        {
            List<Rgba32> colourWheel = [];
            colourWheel.Add(new Rgba32()); // Add [JPG:Black]/[PNG:Transparent] as the first colour (used when the rule breaks in the Fractal)

            for (int i = 0; i < numberOfColours; i++)
            {
                // LightMagenta: FF80FF
                // Magenta: FF00FF
                // DarkMagenta: 8B008B

                // R: FF -> 8B
                // G: 80 -> 00
                // B: FF -> 8B

                // E0 is (dec)224
                // 8B is (dec)139 (or 255-116)
                byte redFloat, greenFloat, blueFloat;
                redFloat = (byte)(255 - (116 * (i / (float)numberOfColours))); //FF -> 8B
                greenFloat = (byte)(128 - (128 * (i / (float)numberOfColours))); // 80 -> 00
                blueFloat = (byte)(255 - (116 * (i / (float)numberOfColours))); // FF -> 8B

                Rgba32 color = new Rgba32(redFloat, greenFloat, blueFloat);
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
                new("Generated", colourWheelBase),
                new("Random", colourWheelRandom),
                new("Range-cyan-very-narrow", GenerateCyanColourBand(colourGradientVeryNarrow, 0)),
                new("Range-cyan-narrow", GenerateCyanColourBand(colourGradientNarrow, 0)),
                new("Range-cyan", GenerateCyanColourBand(colourGradientNormal, 0)),
                new("Range-cyan-wide", GenerateCyanColourBand(colourGradientWide, 0)),
                new("Range-cyan-very-wide", GenerateCyanColourBand(colourGradientVeryWide, 0)),
                new("Range-magenta-very-narrow", GenerateMagentaColourBand(colourGradientVeryNarrow, 0)),
                new("Range-magenta-narrow", GenerateMagentaColourBand(colourGradientNarrow, 0)),
                new("Range-magenta", GenerateMagentaColourBand(colourGradientNormal, 0)),
                new("Range-magenta-wide", GenerateMagentaColourBand(colourGradientWide, 0)),
                new("Range-magenta-very-wide", GenerateMagentaColourBand(colourGradientVeryWide, 0)),
                new("Gradient-090-very-narrow", GenerateColourRangeGradient(colourGradientVeryNarrow, 90)),
                new("Gradient-090-narrow", GenerateColourRangeGradient(colourGradientNarrow, 90)),
                new("Gradient-090", GenerateColourRangeGradient(colourGradientNormal, 90)),
                new("Gradient-090-wide", GenerateColourRangeGradient(colourGradientWide, 90)),
                new("Gradient-090-very-wide", GenerateColourRangeGradient(colourGradientVeryWide, 90)),
                new("Gradient-180-very-narrow", GenerateColourRangeGradient(colourGradientVeryNarrow, 180)),
                new("Gradient-180-narrow", GenerateColourRangeGradient(colourGradientNarrow, 180)),
                new("Gradient-180", GenerateColourRangeGradient(colourGradientNormal, 180)),
                new("Gradient-180-wide", GenerateColourRangeGradient(colourGradientWide, 180)),
                new("Gradient-180-very-wide", GenerateColourRangeGradient(colourGradientVeryWide, 180)),
                new("Gradient-270-very-narrow", GenerateColourRangeGradient(colourGradientVeryNarrow, 270)),
                new("Gradient-270-narrow", GenerateColourRangeGradient(colourGradientNarrow, 270)),
                new("Gradient-270", GenerateColourRangeGradient(colourGradientNormal, 270)),
                new("Gradient-270-wide", GenerateColourRangeGradient(colourGradientWide, 270)),
                new("Gradient-270-very-wide", GenerateColourRangeGradient(colourGradientVeryWide, 270))
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