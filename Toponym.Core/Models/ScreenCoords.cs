namespace Toponym.Core.Models {
    public class ScreenCoords {

        /// <summary>
        /// На экране вправо, %
        /// </summary>
        public float X { get; private set; }

        /// <summary>
        /// На экране вниз, %
        /// </summary>
        public float Y { get; private set; }

        public ScreenCoords(float x, float y) {
            X = x;
            Y = y;
        }
    }
}
