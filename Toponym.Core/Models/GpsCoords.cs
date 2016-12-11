namespace Toponym.Core.Models {
    public class GpsCoords {

        /// <summary>
        /// Широта (на глобусе вверх)
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Долгота (на глобусе вправо)
        /// </summary>
        public float Longitude { get; set; }

        public GpsCoords(float latitude, float longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
