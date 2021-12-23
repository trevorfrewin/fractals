using System.Collections.Generic;

namespace tfrewin.play.fractal.start
{
    public class Matrix
    {
        public int MaximumIterations { get; private set; }

        public List<Point> Points { get; private set; }

        public Matrix(int maximumIterations)
        {
            this.MaximumIterations = maximumIterations;
            this.Points = new List<Point>();
        }
    }
}