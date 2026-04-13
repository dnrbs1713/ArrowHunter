using UnityEngine;

public class MapPlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;

    private Rigidbody _rb;
    private Transform _cameraTransform;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        // [설계 이유] Camera.main 자동 참조
        // → 시네머신 사용 시 Inspector 연결 불필요
        _cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0 && v == 0)
        {
            // 입력 없으면 수평 속도만 0 — 중력(Y)은 유지
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }

        // [설계 이유] 쿼터뷰 방향 보정
        // 카메라 Y축 기준으로 이동 방향 변환
        // → W키가 항상 화면 위쪽으로 이동
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        _rb.linearVelocity = new Vector3(
            moveDir.x * moveSpeed,
            _rb.linearVelocity.y,  // 중력(Y)은 물리엔진에 맡김
            moveDir.z * moveSpeed);

        // 이동 방향으로 캐릭터 회전
        transform.rotation = Quaternion.LookRotation(moveDir);
    }
}