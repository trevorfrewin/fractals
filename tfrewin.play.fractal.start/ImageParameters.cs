using System;

namespace tfrewin.play.fractal.start
{
    public class ImageParameters
    {
        public DateTime StartTime { get; private set; }

        public long ProcessingMilliseconds { get; set; }

        public long PaintMilliseconds { get; set; }

        public string SetName { get; private set; }

        public int PlaneWidth { get; private set; }

        public int PlaneHeight { get; private set; }

        public double Zoom { get; private set; }

        public double MoveX { get; private set; }

        public double MoveY { get; private set; }

        public double IterationFactor { get; private set; }

        public string ColourWheelName { get; private set; }

        public int ColourOffset { get; private set; }

        public string ImageFilename { get; set; }

        public ImageParameters(DateTime startTime, string setName, int planeWidth, int planeHeight, double zoom, double moveX, double moveY, double iterationFactor, string colourWheelName, int colourOffset)
        {
            this.StartTime = startTime;
            this.SetName = setName;
            this.PlaneWidth = planeWidth;
            this.PlaneHeight = planeHeight;
            this.Zoom = zoom;
            this.MoveX = moveX;
            this.MoveY = moveY;
            this.IterationFactor = iterationFactor;
            this.ColourWheelName = colourWheelName;
            this.ColourOffset = colourOffset;
        }
    }
}