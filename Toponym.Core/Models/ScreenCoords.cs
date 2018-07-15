namespace Toponym.Core.Models
{
    public class ScreenCoords
    {
        /// <summary>
        /// To right on the screen, %
        /// </summary>
        public float X { get; }

        /// <summary>
        /// To down on the screen, %
        /// </summary>
        public float Y { get; }

        public ScreenCoords(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
