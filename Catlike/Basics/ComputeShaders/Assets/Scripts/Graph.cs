using UnityEngine;

public sealed class Graph : MonoBehaviour
{
    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    private Transform m_PointPrefab;

    [SerializeField, Range(10, 200)]
    private int m_Resolution = 10;

    [SerializeField]
    private FunctionLibrary.FunctionName m_Function = default;

    [SerializeField, Min(0f)]
    private float m_FunctionDuration = 1f, m_TransitionDuration = 1f;

    [SerializeField]
    private TransitionMode m_TransitionMode = TransitionMode.Cycle;

    private Transform[] m_Points;

    private float m_Duration;

    private bool m_Transitioning;
    private FunctionLibrary.FunctionName m_TransitionFunction;

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
        m_Duration += Time.deltaTime;

        if (m_Transitioning)
        {
            if (m_Duration >= m_FunctionDuration)
            {
                m_Duration -= m_FunctionDuration;
                m_Transitioning = false;
            }
        }
        else if (m_Duration >= m_FunctionDuration)
        {
            m_Duration -= m_FunctionDuration;
            m_Transitioning = true;
            m_TransitionFunction = m_Function;

            PickNextFunction();
        }

        if (m_Transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }

    private void UpdateFunction()
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

    private void UpdateFunctionTransition()
    {
        var from = FunctionLibrary.GetFunction(m_TransitionFunction);
        var to = FunctionLibrary.GetFunction(m_Function);

        float progress = m_Duration / m_TransitionDuration;
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

            m_Points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
        }
    }

    private void PickNextFunction()
    {
        m_Function = m_TransitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(m_Function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(m_Function);
    }
}