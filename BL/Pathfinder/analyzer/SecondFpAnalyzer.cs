using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Point = Domain.pathfinder.Point;

namespace BL.Pathfinder.analyzer;

public class SecondFpAnalyzer : ISecondAnalyzer
{
    private readonly ILogger<SecondFpAnalyzer> _logger;

    public SecondFpAnalyzer(ILogger<SecondFpAnalyzer> logger) => _logger = logger;

    public (Point start, Point end, bool[,] walkableGrid) GetWalkableGrid(
        string imagePath,
        Point startCoords,
        Point endCoords,
        int threshold = 75)
    {
        using var image = Image.Load<Rgba32>(imagePath);
        int margin = 1000;
        
        int width = image.Width;
        int height = image.Height;
        
        var walkable = new bool[width, height];

        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (int x = 0; x < width; x++)
                {
                    var px = row[x];
                    walkable[x, y] = !IsBlackWall(px, threshold);
                }
            }
        });

        int sx = startCoords.XWidth;
        int sy = startCoords.YHeight;
        int ex = endCoords.XWidth;
        int ey = endCoords.YHeight;

        if ((uint)sx >= (uint)width || (uint)sy >= (uint)height ||
            (uint)ex >= (uint)width || (uint)ey >= (uint)height)
            throw new Exception("Start or end point is out of bounds.");

        if (!walkable[sx, sy] || !walkable[ex, ey])
            throw new Exception("Start or end point is not walkable.");

        _logger.LogInformation("Walkable grid created. Size: {W}x{H}", width, height);

        return (new Point(sx, sy), new Point(ex, ey), walkable);
    }

    private static bool IsBlackWall(Rgba32 pixel, int threshold)
        => pixel.R < threshold && pixel.G < threshold && pixel.B < threshold;
}