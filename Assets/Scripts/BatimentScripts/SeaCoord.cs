using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SeaCoord
{
    public static Vector3 GetFlatCoord(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static Vector3 GetFlatCoord(Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static Vector2 Planify(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Quaternion SetRotation(Quaternion startRotation, float angle)
    {
        return Quaternion.Euler(startRotation.eulerAngles.x, angle, startRotation.eulerAngles.z);
    }

    public static Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}
