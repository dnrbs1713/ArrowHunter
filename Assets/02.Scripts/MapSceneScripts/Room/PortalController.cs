using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;
public class PortalController : MonoBehaviour
{
    [Header("이동 방향")]
    public Direction direction;

    [Header("상태별 오브젝트")]
    public GameObject closedObject;  // 닫힌 포탈 (클리어 전)
    public GameObject openObject;    // 열린 포탈 (클리어 후)

    private Collider _collider;
    private bool _isTriggered = false;
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        SetLocked();
    }

    public void SetLocked()
    {
        _collider.enabled = false;
        if (closedObject != null) closedObject.SetActive(true);
        if (openObject != null) openObject.SetActive(false);
    }

    public void SetOpen()
    {
        _collider.enabled = true;
        if (closedObject != null) closedObject.SetActive(false);
        if (openObject != null) openObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isTriggered) return;

        Debug.Log($"충돌 대상: {other.name}");
        if (!other.CompareTag("PLAYER")) return;

        Debug.Log($"[Portal] {direction} 방향으로 이동");

        _isTriggered = true;
        _collider.enabled = false;

        RoomManager.instance.MoveToRoom(direction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
