using UnityEngine;

public sealed class CameraTransformation : Transformation
{
    public CameraProjection projection = CameraProjection.Perspective;
    public float focalLength = 1f;

    public override Matrix4x4 Matrix
    {
        get
        {
            if (projection == CameraProjection.Perspective)
            {
                var matrix = new Matrix4x4();
                matrix.SetRow(0, new Vector4(focalLength, 0f, 0f, 0f));
                matrix.SetRow(1, new Vector4(0f, focalLength, 0f, 0f));
                matrix.SetRow(2, new Vector4(0f, 0f, 0f, 0f));
                matrix.SetRow(3, new Vector4(0f, 0f, 1f, 0f));
                return matrix;
            }
            else
            {
                var matrix = new Matrix4x4();
                matrix.SetRow(0, new Vector4(1f, 0f, 0f, 0f));
                matrix.SetRow(1, new Vector4(0f, 1f, 0f, 0f));
                matrix.SetRow(2, new Vector4(0f, 0f, 0f, 0f));
                matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
                return matrix;
            }
        }
    }
}

public enum CameraProjection
{
    Perspective,
    Orthographic
}