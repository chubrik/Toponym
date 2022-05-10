namespace Toponym.Web;

public class GeoPoint
{
    /// <summary>
    /// To top on the globe
    /// </summary>
    public float Latitude { get; }

    /// <summary>
    /// To right on the globe
    /// </summary>
    public float Longitude { get; }

    public GeoPoint(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}
