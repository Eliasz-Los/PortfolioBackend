namespace Domain.pathfinder;

public class Point
{
    public Guid Id {get; set;}
    public double XWidth {get; set;}
    public double YHeight {get; set;}
    public Floorplan Floorplan {get; set;}
    
    public Point(){}
    public Point(double xWidth, double yHeight)
    {
        XWidth = xWidth;
        YHeight = yHeight;
    }

    public Point(double xWidth, double yHeight, Floorplan floorplan)
    {
        XWidth = xWidth;
        YHeight = yHeight;
        Floorplan = floorplan;
    }

    public bool Equals(Point? other)
    {
        if (other is null) return false;
        const double tolerance = 1e-10;
            return Math.Abs(XWidth - other.XWidth) < tolerance
                   && Math.Abs(YHeight - other.YHeight) < tolerance;
       
    }

    public override bool Equals(object? obj) => obj is Point other && Equals(other);

    public override int GetHashCode()
    {
        return HashCode.Combine(XWidth, YHeight);
    }
}