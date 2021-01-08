using UnityEngine;

public sealed class Controller : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;

    private Camera _viewCamera;
    private Rigidbody _rigidbody;
    private Vector3 _velocity;

    private void Start()
    {
        _viewCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _viewCamera.transform.position.y);
        var mouseWorldPos = _viewCamera.ScreenToWorldPoint(mouseScreenPos);

        transform.LookAt(mouseWorldPos + Vector3.up * transform.position.y);

        _velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized * _moveSpeed;
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }
}
