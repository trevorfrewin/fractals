using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SixLabors.ImageSharp.PixelFormats;

namespace tfrewin.play.fractal.start.utilities
{
    public class ColourWheelGenerator
    {
        private List<List<Rgba32>> GenerateColourSpokes()
        {
            var returnThis = new List<List<Rgba32>>();
            
            // The range of RED
            var currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(i, 0, 0, 127));
            }
            returnThis.Add(currentSet);

            // RED and the range of GREEN
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(255, i, 0, 127));
            }
            returnThis.Add(currentSet);

            // GREEN and the reverse range of RED
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(255-i, 255, 0, 127));
            }
            returnThis.Add(currentSet);

            // GREEN and the range of BLUE
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(0, 255, i, 127));
            }
            returnThis.Add(currentSet);

            // BLUE and the reverse range of GREEN
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(0, 255-i, 255, 127));
            }
            returnThis.Add(currentSet);

            // BLUE and the range of RED
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(i, 0, 255, 127));
            }
            returnThis.Add(currentSet);

            // All GREEN AND All BLUE AND a range of RED
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(i, 255, 255, 127));
            }
            returnThis.Add(currentSet);

            // All RED AND All BLUE AND a range of GREEN
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(255, i, 255, 127));
            }
            returnThis.Add(currentSet);

            // All GREEN AND All RED AND a range of BLUE
            currentSet = new List<Rgba32>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(new Rgba32(255, 255, i, 127));
            }
            returnThis.Add(currentSet);

            return returnThis;
        }

        public List<ColourWheel> GenerateColourWheels()
        {
            var colourSpokes = GenerateColourSpokes();

            var colourWheelFirst = new List<Rgba32>();
            foreach (var colourSpoke in colourSpokes)
            {
                colourWheelFirst.AddRange(colourSpoke);
            }

            var colourWheelRandom = new List<Rgba32>();
            foreach (var colourSpoke in colourSpokes.OrderBy(cs => Guid.NewGuid()).ToList())
            {
                colourWheelRandom.AddRange(colourSpoke);
            }

            var returnThis = new List<ColourWheel>
            {
                new("first", colourWheelFirst),
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