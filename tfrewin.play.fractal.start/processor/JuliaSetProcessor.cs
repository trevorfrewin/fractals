using System;
using System.Collections.Generic;

namespace tfrewin.play.fractal.start.processor
{
    public class JuliaSetProcessor : IFormulaProcesor
    {
        public Matrix Process(int planeWidth, int planeHeight, int zoom)
        {
            const int maximumIteration = 250;
            const int moveX = 0;
            const int moveY = 0;
            const double cX = -0.7;
            const double cY = 0.27015;
            double zx, zy, tmp;
            int i;

            var returnThis = new Matrix(maximumIteration);

            for (int x = 0; x < planeWidth; x++)
            {
                for (int y = 0; y < planeHeight; y++)
                {
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

                    returnThis.Points.Add(new Point(x, y, i));
                }
            }

            return returnThis;
        }
    }
}