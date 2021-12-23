using System;
using System.Collections.Generic;

namespace tfrewin.play.fractal.start.processor
{
    public interface IFormulaProcesor
    {
        Matrix Process(int planeWidth, int planeHeight, double zoom, int maximumIteration);
    }
}