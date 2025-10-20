using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 WithZ(this Vector3 vector)
    {
        return Vector3.forward * vector.z;
    }

    public static Vector3 WithoutZ(this Vector3 vector)
    {
        return Vector3.Scale(vector, Vector3.up + Vector3.right);
    }
    public static Vector3 WithY(this Vector3 vector)
    {
        return Vector3.up * vector.y;
    }

    public static Vector3 WithoutY(this Vector3 vector)
    {
        return Vector3.Scale(vector, Vector3.forward + Vector3.right);
    }

    
    public static Vector3 WithX(this Vector3 vector)
    {
        return Vector3.right * vector.x;
    }

    public static Vector3 WithoutX(this Vector3 vector)
    {
        return Vector3.Scale(vector, Vector3.forward + Vector3.up);
    }
}
