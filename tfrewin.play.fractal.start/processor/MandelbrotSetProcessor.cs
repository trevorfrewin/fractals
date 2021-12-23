using System;

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
        private int CalcMandelbrotSetColor(ComplexNumber c, double maxValueExtent, int maximumIteration)
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
            for (int y = 0; y < planeHeight; y++)
            {
                double yScale = (planeHeight / 2 - y) * scale;
                for (int x = 0; x < planeWidth; x++)
                {
                    double xScale = (x - planeWidth / 2) * scale;

                    var colour = CalcMandelbrotSetColor(new ComplexNumber(xScale + moveX, yScale + moveY), maxValueExtent, maximumIteration);
                    returnThis.Points.Add(new Point(x, y, colour));
                }
            }

            return returnThis;
        }
    }
}