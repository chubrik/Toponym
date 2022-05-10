namespace Toponym;

public class ScreenPoint
{
    /// <summary>
    /// To right on the screen, %
    /// </summary>
    public float X { get; }

    /// <summary>
    /// To down on the screen, %
    /// </summary>
    public float Y { get; }

    public ScreenPoint(float x, float y)
    {
        X = x;
        Y = y;
    }
}
