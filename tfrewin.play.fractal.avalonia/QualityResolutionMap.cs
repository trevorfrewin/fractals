using System.Collections.Generic;
using System.Linq;

namespace tfrewin.play.fractal.avalonia
{
    public class QualityResolutionMap
    {
        private static readonly List<(string Name, int Width, int Height)> _map = new()
        {
            ("Fast", 300, 200),
            ("Icon", 256, 256),
            ("Medium", 900, 600),
            ("FHD", 1920, 1080),
            ("Portrait-FHD", 1080, 1920),
            ("Portrait-Irregular", 973, 1915),
            ("QHD", 2560, 1440),
            ("4K", 3840, 2160),
            ("8K", 7680, 4320),
            ("High Quality", 2400, 1600),
            ("Superb", 3000, 2000),
            ("Ridiculous", 6000, 4000),
            ("Ludicrous", 12000, 8000),
            ("Plaid", 24000, 16000)
        };

        public static (int Width, int Height)? GetResolution(string qualityName)
        {
            var entry = _map.FirstOrDefault(q => q.Name == qualityName);
            return entry != default ? (entry.Width, entry.Height) : null;
        }

        public static string? GetQualityName(int width, int height)
        {
            var entry = _map.FirstOrDefault(q => q.Width == width && q.Height == height);
            return entry != default ? entry.Name : null;
        }

        public static IReadOnlyList<string> GetAllQualityNames()
        {
            return [.. _map.Select(q => q.Name)];
        }
    }
}