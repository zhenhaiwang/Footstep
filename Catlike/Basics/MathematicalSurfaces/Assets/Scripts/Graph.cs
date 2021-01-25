using UnityEngine;

public sealed class Graph : MonoBehaviour
{
    [SerializeField]
    private Transform m_PointPrefab;

    [SerializeField, Range(10, 100)]
    private int m_Resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName m_Function = default;

    private Transform[] m_Points;

    private void Awake()
    {
        m_Points = new Transform[m_Resolution * m_Resolution];

        float step = 2f / m_Resolution;
        var scale = Vector3.one * step;

        for (int i = 0; i < m_Points.Length; i++)
        {
            var point = Instantiate(m_PointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);

            m_Points[i] = point;
        }
    }

    private void Update()
    {
        var function = FunctionLibrary.GetFunction(m_Function);

        float time = Time.time;
        float step = 2f / m_Resolution;
        float v = 0.5f * step - 1f;

        for (int i = 0, x = 0, z = 0; i < m_Points.Length; i++, x++)
        {
            if (x == m_Resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }

            float u = (x + 0.5f) * step - 1f;

            m_Points[i].localPosition = function(u, v, time);
        }
    }
}