namespace Domain.pathfinder;

public class Floorplan
{
    
    public Guid Id {get; set;}
    public string Name {get; set;}
    
    public int FloorNumber { get; set; }
    
    public string Image {get; set;}
    public string Scale {get; set;}
    
   public HashSet<Point> Points {get; set;} 
    
    public Floorplan(string name, int floorNumber, string scale, string image)
    {
        Name = name;
        FloorNumber = floorNumber;
        Scale = scale;
        Image = image;
    }
}