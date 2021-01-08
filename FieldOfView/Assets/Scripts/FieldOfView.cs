using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FieldOfView : MonoBehaviour
{
    [SerializeField]
    [Range(0, 360f)]
    private float _viewAngle = 90f;
    [SerializeField]
    private float _viewRadius = 5f;
    [SerializeField]
    private float _detectInterval = 0.1f;
    [SerializeField]
    private float _meshResolution = 1f;
    [SerializeField]
    private float _edgeResolveIterations = 1f;
    [SerializeField]
    private float _edgeDstThreshold = 1f;
    [SerializeField]
    private float _maskCutawayDst = 0.15f;
    [SerializeField]
    private MeshFilter _viewMeshFilter;
    [SerializeField]
    private LayerMask _targetMask;
    [SerializeField]
    private LayerMask _obstacleMask;

    private List<Transform> _visibleTargets;

    private Mesh _viewMesh;

    public float viewRadius
    {
        get { return _viewRadius; }
    }

    public float viewAngle
    {
        get { return _viewAngle; }
    }

    public float eulerAngleY
    {
        get { return transform.eulerAngles.y; }
    }

    public List<Transform> visibleTargets
    {
        get { return _visibleTargets; }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool global = false)
    {
        if (!global)
        {
            angleInDegrees += eulerAngleY;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void Start()
    {
        CreateViewMesh();
        StartCoroutine(DetectVisibleTargets());
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private void CreateViewMesh()
    {
        _viewMesh = new Mesh();
        _viewMesh.name = "ViewMesh";
        _viewMeshFilter.mesh = _viewMesh;
    }

    private IEnumerator DetectVisibleTargets()
    {
        _visibleTargets = new List<Transform>();

        var waitForSeconds = new WaitForSeconds(Mathf.Max(_detectInterval, 0f));

        while (true)
        {
            yield return waitForSeconds;

            _visibleTargets.Clear();

            var targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);
            int targetCount = targetsInViewRadius != null ? targetsInViewRadius.Length : 0;

            for (int i = 0; i < targetCount; i++)
            {
                var target = targetsInViewRadius[i].transform;
                var dirToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2f)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
                    {
                        _visibleTargets.Add(target);
                    }
                }
            }
        }
    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        var viewPoints = new List<Vector3>();

        var oldViewCastInfo = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = eulerAngleY - viewAngle / 2 + stepAngleSize * i;
            var newViewCastInfo = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCastInfo.dst - newViewCastInfo.dst) > _edgeDstThreshold;

                if (oldViewCastInfo.hit != newViewCastInfo.hit || (oldViewCastInfo.hit && newViewCastInfo.hit && edgeDstThresholdExceeded))
                {
                    var edge = FindEdge(oldViewCastInfo, newViewCastInfo);

                    if (edge.minPoint != Vector3.zero)
                    {
                        viewPoints.Add(edge.minPoint);
                    }

                    if (edge.maxPoint != Vector3.zero)
                    {
                        viewPoints.Add(edge.maxPoint);
                    }
                }
            }

            viewPoints.Add(newViewCastInfo.point);
            oldViewCastInfo = newViewCastInfo;
        }

        int vertexCount = viewPoints.Count + 1; // plus one for origin vertex
        var vertices = new Vector3[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * _maskCutawayDst;

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        var dir = DirFromAngle(globalAngle, true);

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, viewRadius, _obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCastInfo, ViewCastInfo maxViewCastInfo)
    {
        float minAngle = minViewCastInfo.angle;
        float maxAngle = maxViewCastInfo.angle;

        var minPoint = Vector3.zero;
        var maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            var viewCastInfo = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCastInfo.dst - viewCastInfo.dst) > _edgeDstThreshold;

            if (viewCastInfo.hit == minViewCastInfo.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = viewCastInfo.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = viewCastInfo.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float dst, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.dst = dst;
            this.angle = angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 minPoint;
        public Vector3 maxPoint;

        public EdgeInfo(Vector3 minPoint, Vector3 maxPoint)
        {
            // one is the closest point to the edge on the obstacle
            // the other is the closest point to the edge off the obstacle

            this.minPoint = minPoint;
            this.maxPoint = maxPoint;
        }
    }
}
