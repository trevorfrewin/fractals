using tfrewin.play.fractal.start.processor.output;

namespace tfrewin.play.fractal.start.processor
{
    public interface IFormulaProcesor
    {
        Matrix Process(int planeWidth, int planeHeight, double zoom, int maximumIteration);
    }
}