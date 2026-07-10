using UnityEngine;

public enum ColorType
{
    Red,
    Blue,
    Green
}

public enum DirectionType
{
    Up,
    Down,
    Left,
    Right,
    All
}

public static class DirectionUtility
{
    public static Vector2Int ToOffset(this DirectionType direction)
    {
        switch (direction)
        {
            case DirectionType.Up: return new Vector2Int(0, 1);
            case DirectionType.Down: return new Vector2Int(0, -1);
            case DirectionType.Left: return new Vector2Int(-1, 0);
            case DirectionType.Right: return new Vector2Int(1, 0);
            default: return Vector2Int.zero;
        }
    }

    // 홀이 이 방향으로 들어왔을 때, 오브젝트가 이 방향을 바라보고 있어야 서로 마주본다(매칭).
    public static DirectionType Opposite(this DirectionType direction)
    {
        switch (direction)
        {
            case DirectionType.Up: return DirectionType.Down;
            case DirectionType.Down: return DirectionType.Up;
            case DirectionType.Left: return DirectionType.Right;
            case DirectionType.Right: return DirectionType.Left;
            default: return DirectionType.All;
        }
    }

    public static Color ToColor(this ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.Red: return Color.red;
            case ColorType.Blue: return Color.blue;
            case ColorType.Green: return Color.green;
            default: return Color.white;
        }
    }
}
