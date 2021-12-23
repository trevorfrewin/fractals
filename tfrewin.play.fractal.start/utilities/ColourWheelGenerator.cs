using System.Collections.Generic;
using System.Drawing;

namespace tfrewin.play.fractal.start.utilities
{
    public class ColourWheelGenerator
    {
        public List<List<Color>> GenerateColourWheels()
        {
            var colourWheelFirst = new List<Color>();

            // The range of RED
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(i, 0, 0));
            }

            // All RED AND The range of GREEN
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(255, i, 0));
            }

            // All GREEN and the reverse range of RED
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(255-i, 255, 0));
            }

            // All GREEN AND the range of BLUE
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(0, 255, i));
            }

            // All BLUE AND the reverse range of GREEN
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(0, 255-i, 255));
            }

            // All BLUE AND the range of RED
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(i, 0, 255));
            }

            // All GREEN AND All BLUE AND a range of RED
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(i, 255, 255));
            }

            // All RED AND All BLUE AND a range of GREEN
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(255, i, 255));
            }

            // All GREEN AND All RED AND a range of BLUE
            for (int i = 0; i < 256; i++)
            {
                colourWheelFirst.Add(Color.FromArgb(255, 255, i));
            }

            var returnThis = new List<List<Color>>();
            returnThis.Add(colourWheelFirst);
            return returnThis;
        }
    }
}