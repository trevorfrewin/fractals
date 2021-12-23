using System;

namespace tfrewin.play.fractal.start.processor
{
    public class FormulaProcessorFactory
    {
        public IFormulaProcesor Create(string formulaName)
        {
            switch(formulaName)
            {
                case "JuliaSet" :
                    return new JuliaSetProcessor();
                case "MandelbrotSet" :
                    return new MandelbrotSetProcessor();
                default:
                    throw new ArgumentException(string.Format("Formula '{0}' is not supported."));
            }
        }
    }
}