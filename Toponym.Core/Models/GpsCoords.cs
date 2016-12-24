namespace Toponym.Core.Models {
    public class GpsCoords {

        /// <summary>
        /// Широта (на глобусе вверх)
        /// </summary>
        public float Latitude { get; private set; }

        /// <summary>
        /// Долгота (на глобусе вправо)
        /// </summary>
        public float Longitude { get; private set; }

        public GpsCoords(float latitude, float longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
