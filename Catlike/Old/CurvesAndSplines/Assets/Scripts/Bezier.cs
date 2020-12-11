using UnityEngine;

public static class Bezier
{
    /// <summary>
    /// This kind of curve is known as a quadratic Beziér curve, because of the polynomial math involved.
    /// The linear curve can be written as B(t) = (1 - t) P0 + t P1.
    /// One step deeper you get B(t) = (1 - t) ((1 - t) P0 + t P1) + t((1 - t) P1 + t P2). This is really just the linear curve with P0 and P1 replaced by two new linear curves. It can also be rewritten into the more compact form B(t) = (1 - t)^2 P0 + 2 (1 - t) t P1 + t^2 P2.
    /// So we could use the quadratic formula instead of three calls to Vector3.Lerp.
    /// </summary>
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);

        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    /// <summary>
    /// The first derivative of our quadratic Beziér curve is B'(t) = 2 (1 - t) (P1 - P0) + 2 t (P2 - P1).
    /// This function produces lines tangent to the curve, which can be interpreted as the speed with which we move along the curve.
    /// </summary>
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }

    /// <summary>
    /// B(t) = (1 - t)^3 P0 + 3 (1 - t)^2 t P1 + 3 (1 - t) t^2 P2 + t^3 P3 
    /// </summary>
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    /// <summary>
    /// B'(t) = 3 (1 - t)^2 (P1 - P0) + 6 (1 - t) t (P2 - P1) + 3 t^2 (P3 - P2).
    /// </summary>
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }
}
