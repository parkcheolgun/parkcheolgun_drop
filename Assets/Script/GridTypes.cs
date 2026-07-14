using UnityEngine;

public enum ColorType
{
    Red,
    Blue,
    Green
}

// 홀이 드래그로 이동하는 방향(v2 §9.14.1: 오브젝트의 "바라보는 방향" 속성은 폐지되어
// 이동 방향으로만 쓰이고, 매칭 판정에는 더 이상 사용되지 않음).
public enum DirectionType
{
    Up,
    Down,
    Left,
    Right
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
