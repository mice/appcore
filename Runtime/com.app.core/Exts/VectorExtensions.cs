using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToVector2(this float val)
    {
        return new Vector2(val, val);
    }

    public static Vector3 ToVector3(this float val)
    {
        return new Vector3(val, val, val);
    }

    public static Vector4 ToVector4(this float val)
    {
        return new Vector4(val, val, val, val);
    }

    public static Vector2 ToVector2XY(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    public static Vector2 ToVector2XZ(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static Vector3 ToVector3XY(this Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y);
    }

    public static Vector3 ToVector3XY(this Vector2 vector2, float z)
    {
        return new Vector3(vector2.x, vector2.y, z);
    }

    public static Vector3 ToVector3XZ(this Vector2 vector2, float y = 0.0f)
    {
        return new Vector3(vector2.x, y, vector2.y);
    }

    public static Color ToColor(this Vector3 vector3)
    {
        return new Color(vector3.x, vector3.y, vector3.z);
    }

    public static Vector3 ToVector3(this Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }

    public static Vector4 ToVector4(this Color color)
    {
        return new Vector4(color.r, color.g, color.b,1.0f);
    }

    public static Vector4 ToVector4(this Vector3 vec3)
    {
        return new Vector4(vec3.x, vec3.y, vec3.z, 1f);
    }

    public static Vector4 ToVector4(this Vector3 vec3, float w)
    {
        return new Vector4(vec3.x, vec3.y, vec3.z, w);
    }

    public static Vector2 ToVector2XY(this Vector4 vec4)
    {
        return new Vector2(vec4.x, vec4.y);
    }

    public static Vector2 ToVector2ZW(this Vector4 vec4)
    {
        return new Vector2(vec4.z, vec4.w);
    }

    public static Vector3 ToVector3(this Vector4 vec4)
    {
        return new Vector3(vec4.x, vec4.y, vec4.z);
    }

    public static Vector3 ToVector3XY(this Vector4 vec4)
    {
        return new Vector3(vec4.x, vec4.y, 0f);
    }

    public static Vector3 Mul(this Vector3 this_vector3, Vector3 mul_vector3)
    {
        this_vector3.x *= mul_vector3.x;
        this_vector3.y *= mul_vector3.y;
        this_vector3.z *= mul_vector3.z;
        return this_vector3;
    }

    public static Vector3 Div(this Vector3 this_vector3, Vector3 div_vector3)
    {
        this_vector3.x /= div_vector3.x;
        this_vector3.y /= div_vector3.y;
        this_vector3.z /= div_vector3.z;
        return this_vector3;
    }

    public static float DistanceXZ(this Vector3 this_vector3, Vector3 to_vector3)
    {
        var diffX = this_vector3.x - to_vector3.x;
        var diffZ = this_vector3.z - to_vector3.z;
        return Mathf.Sqrt(diffX * diffX + diffZ * diffZ);
    }

    public static float DistanceXY(this Vector3 this_vector3, Vector3 to_vector3)
    {
        var diffX = this_vector3.x - to_vector3.x;
        var diffY = this_vector3.y - to_vector3.y;
        return Mathf.Sqrt(diffX * diffX + diffY * diffY);
    }


    public static float SqrDistanceXZ(this Vector3 this_vector3, Vector3 to_vector3)
    {
        var diffX = this_vector3.x - to_vector3.x;
        var diffZ = this_vector3.z - to_vector3.z;
        return diffX * diffX + diffZ * diffZ;
    }

    public static float SqrDistanceXY(this Vector3 this_vector3, Vector3 to_vector3)
    {
        var diffX = this_vector3.x - to_vector3.x;
        var diffY = this_vector3.y - to_vector3.y;
        return diffX * diffX + diffY * diffY;
    }

    public static Quaternion XZLookAt(this Vector3 from, Vector3 to, Quaternion defaultRotation, float offset)
    {
        var new_from = new Vector3(from.x, 0, from.z);
        var new_to = new Vector3(to.x, 0, to.z);
        var targetDir = new Vector3(to.x - from.x, 0, to.z - from.z);
        if (targetDir.magnitude > 0.001f)
        {
            return Quaternion.Euler(0, Quaternion.FromToRotation(new_from, new_to).eulerAngles.y + offset, 0);
        }
        return defaultRotation;
    }
}