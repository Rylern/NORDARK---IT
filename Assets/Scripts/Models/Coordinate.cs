public class Coordinate
{
    public double latitude { get; set; }
    public double longitude { get; set; }

    public Coordinate(double latitude, double longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public Coordinate(Mapbox.Utils.Vector2d latLong)
    {
        this.latitude = latLong.x;
        this.longitude = latLong.y;
    }

    override public string ToString()
    {
        return "{latitude:" + latitude.ToString() + "; longitude:" + longitude.ToString() + ";}";
    }
}