using System.Collections.Concurrent;
using System.Collections.Generic;

namespace tfrewin.play.fractal.start.processor.output
{
    public class Matrix : ConcurrentBag<Point>
    {
        public int MaximumIterations { get; private set; }

        public Matrix(int maximumIterations)
        {
            this.MaximumIterations = maximumIterations;
        }
    }
}