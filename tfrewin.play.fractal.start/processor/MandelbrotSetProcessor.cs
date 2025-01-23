using System;
using System.Threading.Tasks;
using tfrewin.play.fractal.start.processor.output;

namespace tfrewin.play.fractal.start.processor
{
    struct ComplexNumber
    {
        public double Re;
        public double Im;

        public ComplexNumber(double re, double im)
        {
            this.Re = re;
            this.Im = im;
        }

        public static ComplexNumber operator +(ComplexNumber x, ComplexNumber y)
        {
            return new ComplexNumber(x.Re + y.Re, x.Im + y.Im);
        }

        public static ComplexNumber operator *(ComplexNumber x, ComplexNumber y)
        {
            return new ComplexNumber(x.Re * y.Re - x.Im * y.Im,
                x.Re * y.Im + x.Im * y.Re);
        }

        public double Norm()
        {
            return Re * Re + Im * Im;
        }
    }

    public class MandelbrotSetProcessor : IFormulaProcesor
    {
        private static int CalcMandelbrotSetColor(ComplexNumber c, double maxValueExtent, int maximumIteration)
        {
            // from http://en.wikipedia.org/w/index.php?title=Mandelbrot_set
            double MaxNorm = maxValueExtent * maxValueExtent;

            int iteration = 0;
            ComplexNumber z = new ComplexNumber();
            do
            {
                z = z * z + c;
                iteration++;
            } while (z.Norm() < MaxNorm && iteration < maximumIteration);
            if (iteration < maximumIteration)
                return iteration;
            else
                return 0; // black
        }

        public Matrix Process(int planeWidth, int planeHeight, double zoom, double moveX, double moveY, int maximumIteration)
        {
            var returnThis = new Matrix(maximumIteration);

            var maxValueExtent = 2.0;

            double scale = 2 * maxValueExtent / Math.Min(planeWidth, planeHeight);
            scale /= zoom;

            Parallel.For(0, planeHeight, (y) => {
                double yScale = (planeHeight / 2 - y) * scale;

                Parallel.For(0, planeWidth, (x) =>
                                {
                                    double xScale = (x - planeWidth / 2) * scale;

                                    if (y == 0 && x == 0)
                                    {
                                        returnThis.MatrixExtents.TopLeftExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    if (y == 0 && x == planeWidth / 2)
                                    {
                                        returnThis.MatrixExtents.TopMiddleExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    if (y == 0 && x == planeWidth - 1)
                                    {
                                        returnThis.MatrixExtents.TopRightExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    if (y == planeHeight / 2 && x == 0)
                                    {
                                        returnThis.MatrixExtents.MiddleLeftExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    if (y == planeHeight / 2 && x == planeWidth / 2)
                                    {
                                        returnThis.MatrixExtents.MiddleMiddleExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    if (y == planeHeight / 2 && x == planeWidth - 1)
                                    {
                                        returnThis.MatrixExtents.MiddleRightExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    if (y == planeHeight - 1 && x == 0)
                                    {
                                        returnThis.MatrixExtents.BottomLeftExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    if (y == planeHeight - 1 && x == planeWidth / 2)
                                    {
                                        returnThis.MatrixExtents.BottomMiddleExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    if (y == planeHeight - 1 && x == planeWidth - 1)
                                    {
                                        returnThis.MatrixExtents.BottomRightExtent = new Tuple<double, double>(xScale + moveX, yScale + moveY);
                                    }

                                    var colour = CalcMandelbrotSetColor(new ComplexNumber(xScale + moveX, yScale + moveY), maxValueExtent, maximumIteration);

                                    returnThis.Add(new Point(x, y, colour));
                                });
            });

            return returnThis;
        }
    }
}