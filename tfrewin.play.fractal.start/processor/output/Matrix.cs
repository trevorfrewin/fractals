using System.Collections.Concurrent;

namespace tfrewin.play.fractal.start.processor.output
{
    public class Matrix : ConcurrentBag<Point>
    {
        public int MaximumIterations { get; private set; }

        public MatrixExtents MatrixExtents { get; private set; }

        public Matrix(int maximumIterations)
        {
            this.MaximumIterations = maximumIterations;
            this.MatrixExtents = new MatrixExtents();
        }
    }
}