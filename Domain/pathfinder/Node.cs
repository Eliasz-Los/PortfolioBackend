namespace Domain.pathfinder;

public class Node : IComparable<Node>
{
    public Point Point { get; set; }
    public double GCost { get; set; }  = Double.MaxValue; // Cost from start to current node
    public double HCost { get; set; } // Cost from current node to end
    public double FCost => GCost + HCost; // Total cost
    public Node? Parent { get; set; } // Parent node in this path

    public Node(Point point)
    {
        Point = point;
    }
    
    public  bool Equals(Node? other)
    {
        if (other is null) return false;
        return Point.Equals(other.Point);
    }

    public override bool Equals(object? obj) => obj is Node other && Equals(other);
    
    public override int GetHashCode() => Point.GetHashCode();
    
    public int CompareTo(Node? other)
    {
        if (other is null) return 1;
        return FCost.CompareTo(other.FCost); // zien dat andere nodes cost niet slechter is in A* algoritme
    }
}