using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
public interface IInteractable
{
    void Interact();
    string GetPrompt();  // "E - 상점 입장" 같은 안내 문구
}

public class InteractionSystem : MonoBehaviour
{
    [Header("상호작용 범위")]
    public float interactRadius = 2f;

    [Header("상호작용 레이어")]
    public LayerMask interactLayer;

    private IInteractable _nearestInteractable;

    private void Update()
    {
        DetectInteractable();

        if (_nearestInteractable != null && Input.GetKeyDown(KeyCode.E))
            _nearestInteractable.Interact();
    }

    private void DetectInteractable()
    {
        // [설계 이유] OverlapSphere로 범위 내 오브젝트 감지
        Collider[] hits = Physics.OverlapSphere(
            transform.position, interactRadius, interactLayer);

        _nearestInteractable = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IInteractable>(out var interactable))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    _nearestInteractable = interactable;
                }
            }
        }

        // TODO: UI에 상호작용 프롬프트 표시
        // if (_nearestInteractable != null)
        //     MapUIManager.instance.ShowPrompt(_nearestInteractable.GetPrompt());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
