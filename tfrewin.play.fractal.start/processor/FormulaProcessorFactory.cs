using System;

namespace tfrewin.play.fractal.start.processor
{
    public class FormulaProcessorFactory
    {
        public IFormulaProcesor Create(string formulaName)
        {
            switch(formulaName)
            {
                case "julia" :
                    return new JuliaSetProcessor();
                case "mandelbrot" :
                    return new MandelbrotSetProcessor();
                default:
                    throw new ArgumentException(string.Format("Formula '{0}' is not supported."));
            }
        }
    }
}