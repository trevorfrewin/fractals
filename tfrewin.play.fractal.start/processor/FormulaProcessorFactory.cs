using System;

namespace tfrewin.play.fractal.start.processor
{
    public class FormulaProcessorFactory
    {
        public IFormulaProcesor Create(string formulaName)
        {
            return formulaName switch
            {
                "Julia" => new JuliaSetProcessor(),
                "Mandelbrot" => new MandelbrotSetProcessor(),
                _ => throw new ArgumentException(string.Format("Formula '{0}' is not supported.", formulaName)),
            };
        }
    }
}