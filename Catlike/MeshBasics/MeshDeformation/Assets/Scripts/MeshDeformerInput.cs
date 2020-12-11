using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10f;
    public float forceOffset = 0.1f;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;

                // The experience we're trying to evoke is of the mesh being poked and dented by the user.
                // This requires that the vertices near the contact point are pushed into the surface.
                // However, the deforming force doesn't have an inherent direction. It will be applied in all directions equally.
                // This will cause vertices on a flat surface to be pushed apart, not pushed inward.
                // We can add a direction by pulling the force point away from the surface.
                // A slight offset already guarantees that vertices are always pushed into the surface.
                // The normal at the contact point can be used as the offset direction.
                point += hit.normal * forceOffset;

                deformer.AddDeformingForce(point, force);
            }
        }
    }
}
