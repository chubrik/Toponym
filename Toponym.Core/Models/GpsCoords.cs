namespace Toponym.Core.Models {
    public class GpsCoords {

        /// <summary>
        /// To top on the globe
        /// </summary>
        public float Latitude { get; private set; }

        /// <summary>
        /// To right on the globe
        /// </summary>
        public float Longitude { get; private set; }

        public GpsCoords(float latitude, float longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
