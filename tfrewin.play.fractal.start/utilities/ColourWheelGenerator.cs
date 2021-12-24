using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace tfrewin.play.fractal.start.utilities
{
    public class ColourWheelGenerator
    {
        private List<List<Color>> GenerateColourSpokes()
        {
            var returnThis = new List<List<Color>>();
            
            // The range of RED
            var currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(i, 0, 0));
            }
            returnThis.Add(currentSet);

            // RED and the range of GREEN
            currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(255, i, 0));
            }
            returnThis.Add(currentSet);

            // GREEN and the reverse range of RED
            currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(255-i, 255, 0));
            }
            returnThis.Add(currentSet);

            // GREEN and the range of BLUE
            currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(0, 255, i));
            }
            returnThis.Add(currentSet);

            // BLUE and the reverse range of GREEN
            currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(0, 255-i, 255));
            }
            returnThis.Add(currentSet);

            // BLUE and the range of RED
            currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(i, 0, 255));
            }
            returnThis.Add(currentSet);

            // All GREEN AND All BLUE AND a range of RED
            currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(i, 255, 255));
            }
            returnThis.Add(currentSet);

            // All RED AND All BLUE AND a range of GREEN
            currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(255, i, 255));
            }
            returnThis.Add(currentSet);

            // All GREEN AND All RED AND a range of BLUE
            currentSet = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                currentSet.Add(Color.FromArgb(255, 255, i));
            }
            returnThis.Add(currentSet);

            return returnThis;
        }

        public List<ColourWheel> GenerateColourWheels()
        {
            var colourSpokes = GenerateColourSpokes();

            var colourWheelFirst = new List<Color>();
            foreach (var colourSpoke in colourSpokes)
            {
                colourWheelFirst.AddRange(colourSpoke);
            }

            var colourWheelRandom = new List<Color>();
            foreach (var colourSpoke in colourSpokes.OrderBy(cs => Guid.NewGuid()).ToList())
            {
                colourWheelRandom.AddRange(colourSpoke);
            }

            var returnThis = new List<ColourWheel>();
            returnThis.Add(new ColourWheel("first", colourWheelFirst));
            returnThis.Add(new ColourWheel("random", colourWheelRandom));
            return returnThis;
        }
    }

    public class ColourWheel
    {
        public string ColourWheelName { get; private set; }

        public List<Color> Colours { get; private set; }

        public ColourWheel(string colourWheelName, List<Color> colours)
        {
            this.ColourWheelName = colourWheelName;
            this.Colours = colours;
        }
    }
}