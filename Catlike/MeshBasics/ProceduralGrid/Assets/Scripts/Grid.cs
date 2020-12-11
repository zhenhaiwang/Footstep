using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize, ySize;

    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
        //StartCoroutine(Generate());
        Generate();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }

    private void Generate()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        // The amount of vertices depends on the size of the grid.
        // We need a vertex at the corners of every quad, but adjacent quads can share the same vertex.
        // So we need one more vertex than we have tiles in each dimension.
        // (#x + 1)(#y + 1)
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];

        // We need to know how to project this texture onto the triangles of the mesh. This is done by adding 2D texture coordinates to the vertices.
        // The two dimensions of texture space are referred to as U and V, which is why they're know as UV coordinates.
        // These coordinates typically lie between (0, 0) and (1, 1), which covers the entire texture.
        // Coordinates outside that range are either clamped or cause tiling, depending on the texture settings.
        Vector2[] uv = new Vector2[vertices.Length];

        // Normal maps are defined in tangent space. This is a 3D space that flows around the surface of an object.
        // This approach allows us to apply the same normal map in different places and orientations.
        // So a tangent is a 3D vector, but Unity actually uses a 4D vector. Its fourth component is always either −1 or 1, which is used to control the direction of the third tangent space dimension – either forward or backward. 
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
                //yield return wait;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        // Which side a triangle is visible from is determined by the orientation of its vertex indices.
        // By default, if they are arranged in a clockwise direction the triangle is considered to be forward-facing and visible.
        // Counter -clockwise triangles are discarded.

        //int[] triangles = new int[6];
        //triangles[0] = 0;
        //triangles[1] = xSize + 1;
        //triangles[2] = 1;
        //triangles[3] = 1;
        //triangles[4] = xSize + 1;
        //triangles[5] = xSize + 2;

        // As these triangles share two vertices, we could reduce this to four lines of code, explicitly mentioning each vertex index only once.
        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;

                //mesh.triangles = triangles;
                //yield return wait;
            }
        }

        // Triangles are defined via an array of vertex indices.
        // As each triangle has three points, three consecutive indices describe one triangle.
        mesh.triangles = triangles;

        // The Mesh.RecalculateNormals method computes the normal of each vertex by figuring out which triangles connect with that vertex,
        // determining the normals of those flat triangles, averaging them, and normalizing the result.
        mesh.RecalculateNormals();
    }
}
