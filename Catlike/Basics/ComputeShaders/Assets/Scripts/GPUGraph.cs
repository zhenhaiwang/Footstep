using UnityEngine;

public sealed class GPUGraph : MonoBehaviour
{
    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    private ComputeShader m_ComputeShader = default;

    [SerializeField]
    private Material m_Material = default;

    [SerializeField]
    private Mesh m_Mesh = default;

    [SerializeField, Range(10, MAX_RESOLUTION)]
    private int m_Resolution = 10;

    [SerializeField]
    private FunctionLibrary.FunctionName m_Function = default;

    [SerializeField, Min(0f)]
    private float m_FunctionDuration = 1f, m_TransitionDuration = 1f;

    [SerializeField]
    private TransitionMode m_TransitionMode = TransitionMode.Cycle;

    private const int MAX_RESOLUTION = 1000;

    private static readonly int
        m_PositionsId = Shader.PropertyToID("_Positions"),
        m_ResolutionId = Shader.PropertyToID("_Resolution"),
        m_StepId = Shader.PropertyToID("_Step"),
        m_TimeId = Shader.PropertyToID("_Time"),
        m_TransitionProgressId = Shader.PropertyToID("_TransitionProgress");

    private float m_Duration;

    private bool m_Transitioning;
    private FunctionLibrary.FunctionName m_TransitionFunction;

    private ComputeBuffer m_PositionsBuffer;

    private void OnEnable()
    {
        m_PositionsBuffer = new ComputeBuffer(MAX_RESOLUTION * MAX_RESOLUTION, 3 * 4);
    }

    private void OnDisable()
    {
        m_PositionsBuffer.Release();
        m_PositionsBuffer = null;
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

        UpdateFunctionOnGPU();
    }

    private void UpdateFunctionOnGPU()
    {
        float step = 2f / m_Resolution;
        m_ComputeShader.SetInt(m_ResolutionId, m_Resolution);
        m_ComputeShader.SetFloat(m_StepId, step);
        m_ComputeShader.SetFloat(m_TimeId, Time.time);

        if (m_Transitioning)
        {
            m_ComputeShader.SetFloat(m_TransitionProgressId, Mathf.SmoothStep(0f, 1f, m_Duration / m_TransitionDuration));
        }

        int kernelIndex = (int)m_Function + (int)(m_Transitioning ? m_TransitionFunction : m_Function) * FunctionLibrary.FunctionCount;
        m_ComputeShader.SetBuffer(kernelIndex, m_PositionsId, m_PositionsBuffer);
        int groups = Mathf.CeilToInt(m_Resolution / 8f);
        m_ComputeShader.Dispatch(kernelIndex, groups, groups, 1);

        m_Material.SetBuffer(m_PositionsId, m_PositionsBuffer);
        m_Material.SetFloat(m_StepId, step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / m_Resolution));
        Graphics.DrawMeshInstancedProcedural(m_Mesh, 0, m_Material, bounds, m_Resolution * m_Resolution);
    }

    private void PickNextFunction()
    {
        m_Function = m_TransitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(m_Function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(m_Function);
    }
}