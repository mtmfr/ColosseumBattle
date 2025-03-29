using UnityEngine;

public static class Extension_Vector2
{
    public static bool Approximately(this Vector2 vector2, Vector2 toCompare)
    {
        bool xEqual = Mathf.Sqrt(vector2.x * vector2.x) - Mathf.Sqrt(toCompare.x * toCompare.x) < 0.01f;
        bool yEqual = Mathf.Sqrt(vector2.y * vector2.y) - Mathf.Sqrt(toCompare.y * toCompare.y) < 0.01f;

        return xEqual && yEqual;
    }
}
