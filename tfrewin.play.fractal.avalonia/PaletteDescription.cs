// Fractal explorer
// Copyright (C) 2025 Trevor Frewin
// SPDX-License-Identifier: GPL-3.0-only

using System.Collections.Generic;

namespace tfrewin.play.fractal.avalonia
{
    public class PaletteDescription
    {
        public List<string> ColourPalette { get; set; } = new();
        public List<WeightedColour> WeightedColourPalette { get; set; } = new();
    }

    public class WeightedColour
    {
        public string Colour { get; set; }
        public int Count { get; set; }
    }
}