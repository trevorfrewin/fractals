using System;
using System.Collections.Generic;

namespace tfrewin.play.fractal.start.processor
{
    public class JuliaSetProcessor : IFormulaProcesor
    {
        public Dictionary<int, List<Tuple<int,int>>> Process(int planeWidth, int planeHeight, int zoom, int maximumIteration)
        {
            Dictionary<int, List<Tuple<int,int>>> returnThis = new Dictionary<int, List<Tuple<int,int>>>();

            const int moveX = 0;
            const int moveY = 0;
            const double cX = -0.7;
            const double cY = 0.27015;
            double zx, zy, tmp;
            int i;
 
            for (int x = 0; x < planeWidth; x++)
            {
                for (int y = 0; y < planeHeight; y++)
                {
                    if (!returnThis.ContainsKey(y))
                    {
                        returnThis.Add(y, new List<Tuple<int,int>>());
                    }

                    zx = 1.5 * (x - planeWidth / 2) / (0.5 * zoom * planeWidth) + moveX;
                    zy = 1.0 * (y - planeHeight / 2) / (0.5 * zoom * planeHeight) + moveY;
                    i = maximumIteration;
                    while (zx * zx + zy * zy < 4 && i > 1)
                    {
                        tmp = zx * zx - zy * zy + cX;
                        zy = 2.0 * zx * zy + cY;
                        zx = tmp;
                        i -= 1;
                    }

                    returnThis[y].Add(new Tuple<int,int>(x, i));
                }
            }

            return returnThis;
        }
    }
}