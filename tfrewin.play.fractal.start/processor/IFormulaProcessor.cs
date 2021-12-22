using System;
using System.Collections.Generic;

namespace tfrewin.play.fractal.start.processor
{
    public interface IFormulaProcesor
    {
        Dictionary<int, List<Tuple<int,int>>> Process(int planeWidth, int planeHeight, int zoom, int maximumIteration);
    }
}