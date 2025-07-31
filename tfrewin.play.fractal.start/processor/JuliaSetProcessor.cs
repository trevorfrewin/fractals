using System;
using System.Threading.Tasks;
using tfrewin.play.fractal.start.processor.output;

namespace tfrewin.play.fractal.start.processor
{
    public class JuliaSetProcessor : IFormulaProcesor
    {
        public Matrix Process(int planeWidth, int planeHeight, double zoom, double moveX, double moveY, int maximumIteration)
        {
            const double cX = -0.7;
            const double cY = 0.27015;

            var returnThis = new Matrix(maximumIteration);

            Parallel.For(0, planeWidth, (x) =>
            {
                Parallel.For(0, planeHeight, (y) =>
                {
                    double zx, zy, tmp;
                    int i;

                    zx = 1.5 * (x - planeWidth / 2) / (0.5 * zoom * planeWidth) + moveX;
                    zy = 1.0 * (y - planeHeight / 2) / (0.5 * zoom * planeHeight) + moveY;

                    if (y == 0 && x == 0)
                    {
                        returnThis.MatrixExtents.TopLeftExtent = new Tuple<double, double>(zx, zy);
                    }

                    if (y == 0 && x == planeWidth / 2)
                    {
                        returnThis.MatrixExtents.TopMiddleExtent = new Tuple<double, double>(zx, zy);
                    }

                    if (y == 0 && x == planeWidth - 1)
                    {
                        returnThis.MatrixExtents.TopRightExtent = new Tuple<double, double>(zx, zy);
                    }

                    if (y == planeHeight / 2 && x == 0)
                    {
                        returnThis.MatrixExtents.MiddleLeftExtent = new Tuple<double, double>(zx, zy);
                    }

                    if (y == planeHeight / 2 && x == planeWidth / 2)
                    {
                        returnThis.MatrixExtents.MiddleMiddleExtent = new Tuple<double, double>(zx, zy);
                    }

                    if (y == planeHeight / 2 && x == planeWidth - 1)
                    {
                        returnThis.MatrixExtents.MiddleRightExtent = new Tuple<double, double>(zx, zy);
                    }

                    if (y == planeHeight - 1 && x == 0)
                    {
                        returnThis.MatrixExtents.BottomLeftExtent = new Tuple<double, double>(zx, zy);
                    }

                    if (y == planeHeight - 1 && x == planeWidth / 2)
                    {
                        returnThis.MatrixExtents.BottomMiddleExtent = new Tuple<double, double>(zx, zy);
                    }

                    if (y == planeHeight - 1 && x == planeWidth - 1)
                    {
                        returnThis.MatrixExtents.BottomRightExtent = new Tuple<double, double>(zx, zy);
                    }

                    i = maximumIteration;
                    while (zx * zx + zy * zy < 4 && i > 1)
                    {
                        tmp = zx * zx - zy * zy + cX;
                        zy = 2.0 * zx * zy + cY;
                        zx = tmp;
                        i -= 1;
                    }

                    returnThis.Add(new Point(x, y, i));
                });
            });

            return returnThis;
        }
    }
}