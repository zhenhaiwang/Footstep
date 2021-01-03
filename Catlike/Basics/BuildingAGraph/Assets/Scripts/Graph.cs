using UnityEngine;

public sealed class Graph : MonoBehaviour
{
    [SerializeField]
    private Transform m_PointPrefab;

    [SerializeField, Range(10, 100)]
    private int m_Resolution = 10;

    private Transform[] m_Points;

    private void Awake()
    {
        m_Points = new Transform[m_Resolution];

        float step = 2f / m_Resolution;

        var position = Vector3.zero;
        var scale = Vector3.one * step;

        for (int i = 0; i < m_Points.Length; i++)
        {
            var point = Instantiate(m_PointPrefab);
            position.x = (i + 0.5f) * step - 1f;
            //position.y = position.x * position.x * position.x;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform, false);

            m_Points[i] = point;
        }
    }

    private void Update()
    {
        float time = Time.time;

        for (int i = 0; i < m_Points.Length; i++)
        {
            var point = m_Points[i];
            var position = point.localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.x + time));
            point.localPosition = position;
        }
    }
}