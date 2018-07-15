namespace Toponym.Site.Models
{
    public class GeoCoords
    {
        /// <summary>
        /// To top on the globe
        /// </summary>
        public float Latitude { get; }

        /// <summary>
        /// To right on the globe
        /// </summary>
        public float Longitude { get; }

        public GeoCoords(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
