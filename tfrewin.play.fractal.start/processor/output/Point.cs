namespace tfrewin.play.fractal.start.processor.output
{
    public class Point
    {
        public int XAxisValue { get; private set; }

        public int YAxisValue { get; private set; }

        public int IterationCount { get; private set; }

        public Point(int xAxisValue, int yAxisValue, int iterationCount)
        {
            this.XAxisValue = xAxisValue;
            this.YAxisValue = yAxisValue;
            this.IterationCount = iterationCount;
        }
    }
}