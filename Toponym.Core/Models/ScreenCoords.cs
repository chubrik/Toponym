namespace Toponym.Core.Models {
    public class ScreenCoords {

        /// <summary>
        /// To right on the screen, %
        /// </summary>
        public float X { get; private set; }

        /// <summary>
        /// To down on the screen, %
        /// </summary>
        public float Y { get; private set; }

        public ScreenCoords(float x, float y) {
            X = x;
            Y = y;
        }
    }
}
