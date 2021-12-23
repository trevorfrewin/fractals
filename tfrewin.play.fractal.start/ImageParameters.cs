namespace tfrewin.play.fractal.start
{
    public class ImageParameters
    {
        // string setName, int planeWidth, int planeHeight, double zoom, double iterationFactor, int colourOffset

        public string SetName { get; private set; }

        public int PlaneWidth { get; private set; }

        public int PlaneHeight { get; private set; }

        public double Zoom { get; private set; }

        public double IterationFactor { get; private set; }

        public int ColourOffset { get; private set; }

        public string ImageFilename { get; set; }

        public ImageParameters(string setName, int planeWidth, int planeHeight, double zoom, double iterationFactor, int colourOffset)
        {
            this.SetName = setName;
            this.PlaneWidth = planeWidth;
            this.PlaneHeight = planeHeight;
            this.Zoom = zoom;
            this.IterationFactor = iterationFactor;
            this.ColourOffset = colourOffset;
        }
    }
}