using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    public float springForce = 20f;
    public float damping = 5f;

    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;

    float uniformScale = 1f;

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    /// <summary>
    /// Each update all vertices are displaced, assigned to the mesh, and normals are recalculated.
    /// </summary>
    void Update()
    {
        uniformScale = transform.localScale.x;

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    /// <summary>
    /// Updating a vertex is a matter of adjusting its position, via Δp=vΔt.
    /// </summary>
    void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];

        // Whenever the deformed vertex is moved away from the original, the spring will pull it back.
        // The further away the deformed vertex is, the stronger the pull of the spring becomes.
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        displacement *= uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        // This dampening effect is a substitute for resistances, drag, inertia, and so on. It is a simple factor that decreases velocity over time.
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;

        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }

    /// <summary>
    /// Loop through all currently displaced vertices and apply the deforming force to each vertex individually.
    /// </summary>
    public void AddDeformingForce(Vector3 point, float force)
    {
        Debug.DrawLine(Camera.main.transform.position, point);

        // World to local space
        point = transform.InverseTransformPoint(point);

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force)
    {
        // We need to know both the direction and the distance of the deforming force per vertex.
        // Both can be derived from a vector that points from the force point to the vertex position.
        Vector3 pointToVertex = displacedVertices[i] - point;

        // Transform uniform scale
        pointToVertex *= uniformScale;

        // The attenuated force can now be found using the inverse-square law. Just divide the original force by the distance squared.
        // Actually, I divide by one plus the distance squared. 
        // This guarantees that the force is at full strength when the distance is zero.
        // Otherwise the force would be at full strength at a distance of one and it shoots toward infinity the closer you get to the point.
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);

        // Actually, the force is first converted into an acceleration via a=F/m. Then the change in velocity is found via Δv=aΔt.
        // To keep things simple, we'll ignore the mass as if it were one for each vertex. So we end up with Δv=FΔt.
        float velocity = attenuatedForce * Time.deltaTime;

        // At this point we have a velocity delta, but not yet a direction.
        // We find that by normalizing the vector that we started with. Then we can add the result to the vertex velocity.
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }
}
