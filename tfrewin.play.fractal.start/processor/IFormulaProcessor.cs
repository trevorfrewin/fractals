// Fractal explorer
// Copyright (C) 2025 Trevor Frewin
// SPDX-License-Identifier: GPL-3.0-only

using tfrewin.play.fractal.start.processor.output;

namespace tfrewin.play.fractal.start.processor
{
    public interface IFormulaProcesor
    {
        Matrix Process(int planeWidth, int planeHeight, double zoom, double moveX, double moveY, int maximumIteration);
    }
}