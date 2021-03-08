using UnityEngine;
using TMPro;

public sealed class FrameRateCounter : MonoBehaviour
{
    public enum DisplayMode { FPS, MS }

    [SerializeField]
    private TextMeshProUGUI m_DisplayText = default;

    [SerializeField]
    private DisplayMode m_DisplayMode = DisplayMode.FPS;

    [SerializeField, Range(0.1f, 2f)]
    private float m_SampleDuration = 1f;

    private int m_Frames;
    private float m_Duration;
    private float m_BestDuration = float.MaxValue;
    private float m_WorstDuration;

    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;

        m_Frames += 1;
        m_Duration += frameDuration;

        if (frameDuration < m_BestDuration)
        {
            m_BestDuration = frameDuration;
        }
        if (frameDuration > m_WorstDuration)
        {
            m_WorstDuration = frameDuration;
        }

        if (m_Duration >= m_SampleDuration)
        {
            if (m_DisplayMode == DisplayMode.FPS)
            {
                m_DisplayText.SetText
                (
                    "FPS\n{0:0}\n{1:0}\n{2:0}",
                    1f / m_BestDuration,
                    m_Frames / m_Duration,
                    1f / m_WorstDuration
                );
            }
            else
            {
                m_DisplayText.SetText
                (
                    "MS\n{0:1}\n{1:1}\n{2:1}",
                    1000f * m_BestDuration,
                    1000f * m_Duration / m_Frames,
                    1000f * m_WorstDuration
                );
            }

            m_Frames = 0;
            m_Duration = 0f;
            m_BestDuration = float.MaxValue;
            m_WorstDuration = 0f;
        }
    }
}
