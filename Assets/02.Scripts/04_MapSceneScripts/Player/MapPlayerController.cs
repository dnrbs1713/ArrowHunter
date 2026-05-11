using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
public class MapPlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;

    private Rigidbody _rb;
    private Transform _cameraTransform;

    private bool _isMovementLocked;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (_isMovementLocked)
        {
            StopMovement();
            return;
        }

        Move();
    }

    public void LockMovement(float seconds)
    {
        StartCoroutine(LockMovementRoutine(seconds));
    }

    private IEnumerator LockMovementRoutine(float seconds)
    {
        _isMovementLocked = true;
        StopMovement();

        yield return new WaitForSecondsRealtime(seconds);

        _isMovementLocked = false;
    }

    private void StopMovement()
    {
        _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0 && v == 0)
        {
            StopMovement();
            return;
        }

        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        _rb.linearVelocity = new Vector3(
            moveDir.x * moveSpeed,
            _rb.linearVelocity.y,
            moveDir.z * moveSpeed);

        transform.rotation = Quaternion.LookRotation(moveDir);
    }
}
