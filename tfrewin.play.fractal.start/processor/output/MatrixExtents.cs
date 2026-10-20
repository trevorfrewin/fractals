// Fractal explorer
// Copyright (C) 2025 Trevor Frewin
// SPDX-License-Identifier: GPL-3.0-only

using System;

namespace tfrewin.play.fractal.start.processor.output
{
    public class MatrixExtents
    {
        public Tuple<double, double> TopLeftExtent { get; set; }
        public Tuple<double, double> TopMiddleExtent { get; set; }
        public Tuple<double, double> TopRightExtent { get; set; }
        public Tuple<double, double> MiddleLeftExtent { get; set; }
        public Tuple<double, double> MiddleMiddleExtent { get; set; }
        public Tuple<double, double> MiddleRightExtent { get; set; }
        public Tuple<double, double> BottomLeftExtent { get; set; }
        public Tuple<double, double> BottomMiddleExtent { get; set; }
        public Tuple<double, double> BottomRightExtent { get; set; }
    }
}