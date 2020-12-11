using UnityEngine;

public sealed class MovingShpere : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    private float m_MaxSpeed = 10f;
    [SerializeField, Range(0f, 100f)]
    private float m_MaxAcceleration = 10f;
    [SerializeField, Range(0f, 1f)]
    private float m_Bounciness = 0.5f;
    [SerializeField]
    private Rect m_AllowedArea = new Rect(-5f, -5f, 10f, 10f);

    private Vector3 m_Velocity;

    private void Update()
    {
        Vector2 playerInput;

        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * m_MaxSpeed;
        float maxSpeedChange = m_MaxAcceleration * Time.deltaTime;

        m_Velocity.x = Mathf.MoveTowards(m_Velocity.x, desiredVelocity.x, maxSpeedChange);
        m_Velocity.z = Mathf.MoveTowards(m_Velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = m_Velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;

        if (newPosition.x < m_AllowedArea.xMin)
        {
            newPosition.x = m_AllowedArea.xMin;
            //m_Velocity.x = 0f;                            // eliminating velocity
            //m_Velocity.x = -m_Velocity.x;                 // bouncing
            m_Velocity.x = -m_Velocity.x * m_Bounciness;    // bounciness
        }
        else if (newPosition.x > m_AllowedArea.xMax)
        {
            newPosition.x = m_AllowedArea.xMax;
            //m_Velocity.x = 0f;
            //m_Velocity.x = -m_Velocity.x;
            m_Velocity.x = -m_Velocity.x * m_Bounciness;
        }
        if (newPosition.z < m_AllowedArea.yMin)
        {
            newPosition.z = m_AllowedArea.yMin;
            //m_Velocity.z = 0f;
            //m_Velocity.z = -m_Velocity.z;
            m_Velocity.z = -m_Velocity.z * m_Bounciness;
        }
        else if (newPosition.z > m_AllowedArea.yMax)
        {
            newPosition.z = m_AllowedArea.yMax;
            //m_Velocity.z = 0f;
            //m_Velocity.z = -m_Velocity.z;
            m_Velocity.z = -m_Velocity.z * m_Bounciness;
        }

        transform.localPosition = newPosition;
    }
}
