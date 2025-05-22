using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    [Tooltip("Layer của các đối tượng có thể tương tác.")]
    [SerializeField] private LayerMask interactableLayerMask;

    [Tooltip("Collider dùng để phát hiện các đối tượng tương tác trong phạm vi.")]
    [SerializeField] private Collider2D detectionCollider;

    [Tooltip("Icon hiển thị gợi ý tương tác chung (ví dụ: phím 'E'). Sẽ được đặt tại vị trí của mục tiêu.")]
    [SerializeField] private GameObject interactionPromptPrefab;

    private GameObject currentPromptInstance;

    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private IInteractable currentTargetInteractable = null;
    public IInteractable CurrentTargetInteractable => currentTargetInteractable;
    public GameObject interactionIcon;

    private Player ownerPlayer;

    public Transform referenceTransformForDistance;

    // Start is called before the first frame update
    void Start()
    {
        ownerPlayer = GetComponentInParent<Player>();
        if (ownerPlayer == null)
        {
            Debug.LogError("InteractionDetector: Không tìm thấy Player script ở GameObject cha! " +
                           "Hãy đảm bảo InteractionDetector là con của một GameObject có gắn Player.cs.", this.gameObject);
        }
        if (detectionCollider == null)
        {
            detectionCollider = GetComponent<Collider2D>();
            if (detectionCollider == null)
            {
                Debug.LogError("InteractionDetector: Thiếu Collider2D để phát hiện. Vui lòng gán hoặc thêm một Collider2D (IsTrigger=true).", this.gameObject);
                enabled = false; // Vô hiệu hóa script nếu không có collider
                return;
            }
        }
        if (interactionIcon != null) interactionIcon.SetActive(false);
        if (referenceTransformForDistance == null)
        {
            referenceTransformForDistance = transform;
            Debug.LogWarning("InteractionDetector: 'referenceTransformForDistance' chưa được gán trong Inspector. Đang sử dụng transform của GameObject này làm tham chiếu.");
        }
        if (interactionPromptPrefab == null)
        {
            Debug.LogWarning("InteractionDetector: 'interactionPromptPrefab' chưa được gán. Sẽ không có gợi ý tương tác chung.", this.gameObject);
        }
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.performed && currentTargetInteractable != null && currentTargetInteractable.CanInteract() )
        {
            if (ownerPlayer != null)
            {
                currentTargetInteractable.Interact(ownerPlayer.gameObject);
            } else
            {
                Debug.LogError("InteractionDetector: Owner Player is null, can not interact!", this.gameObject);
            }
        }
    }

    void Update()
    {
        // Liên tục cập nhật đối tượng tương tác mục tiêu trong mỗi frame
        UpdateTargetInteractableLogic();
    }

    /*
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentTargetInteractable != null)
            {
                if (ownerPlayer != null)
                {
                    currentTargetInteractable.Interact(ownerPlayer.gameObject);
                    //interactableInRange?.Interact();

                } else
                {
                    Debug.Log("InteractionDetector: ownerPlayer is null");
                }
                UpdateTargetInteractableLogic();

            }

        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((interactableLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;


       if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            
            if (!interactablesInRange.Contains(interactable))
            {
                interactablesInRange.Add(interactable);
                // Debug.Log($"[InteractionDetector] Added to range: {collision.gameObject.name}");
            }
            interactionIcon.SetActive(true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((interactableLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactablesInRange.Contains(interactable))
            {
                interactablesInRange.Remove(interactable);
                if (currentTargetInteractable == interactable)
                {
                    ChangeTarget(null);
                }
                // Debug.Log($"[InteractionDetector] Removed from range: {collision.gameObject.name}");
            }
            //interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }

 

    private void UpdateTargetInteractableLogic()
    {
        // Loại bỏ các đối tượng null (đã bị Destroy) hoặc không hợp lệ khỏi danh sách
        interactablesInRange.RemoveAll(item => item == null || (item is MonoBehaviour mono && mono == null));

        IInteractable newTarget = null;

        if (interactablesInRange.Count > 0)
        {
            newTarget = interactablesInRange.Where(i => i.CanInteract())
                .OrderBy(i => Vector3.Distance(referenceTransformForDistance.position, i.GetGameObject().transform.position)).FirstOrDefault();
        }

        if (currentTargetInteractable != newTarget)
        {
            ChangeTarget(newTarget);
        }

        if (interactionPromptPrefab != null)
        {
            bool showPrompt = currentTargetInteractable != null && currentTargetInteractable.CanInteract();
            if (showPrompt && currentTargetInteractable == null)
            {
                currentPromptInstance = Instantiate(interactionPromptPrefab);
            } else if (!showPrompt && currentTargetInteractable != null)
            {
                Destroy(currentPromptInstance);
                currentPromptInstance = null;
            }
        }
        /*
        IInteractable newClosestTarget = null;
        float shortestDistance = float.MaxValue;

        if (referenceTransformForDistance == null)
        {
            // Debug.LogError("InteractionDetector: referenceTransformForDistance is null. Không thể tính khoảng cách.");
            if (interactionIcon != null) interactionIcon.SetActive(false); // Tắt icon nếu không có tham chiếu
            currentTargetInteractable = null;
            return;
        }

        foreach (var interactable in interactablesInRange)
        {
            // Chỉ xét những đối tượng có thể tương tác (CanInteract() == true)
            if (interactable.CanInteract())
            {
                MonoBehaviour interactableMono = interactable as MonoBehaviour;
                if (interactableMono != null) // Đảm bảo đối tượng là một MonoBehaviour để lấy transform
                {
                    float distanceToPlayer = Vector3.Distance(referenceTransformForDistance.position, interactableMono.transform.position);
                    if (distanceToPlayer < shortestDistance)
                    {
                        shortestDistance = distanceToPlayer;
                        newClosestTarget = interactable;
                    }
                }
            }
        }

        // Cập nhật currentTargetInteractable nếu nó thay đổi
        if (currentTargetInteractable != newClosestTarget)
        {
            currentTargetInteractable = newClosestTarget;
        }
    */

        // Cập nhật trạng thái của icon tương tác
        if (interactionIcon != null)
        {
            if (currentTargetInteractable != null)
            {
                interactionIcon.SetActive(true);
            }
            else
            {
                interactionIcon.SetActive(false);
            }
        }
    }

    private void ChangeTarget(IInteractable newTarget)
    {
        if (currentTargetInteractable != null)
        {
            currentTargetInteractable.OnTargetedState(false);
        }
        currentTargetInteractable = newTarget;

        if (currentTargetInteractable != null)
        {
            currentTargetInteractable.OnTargetedState(true);
        } else { }
    }

    private void UpdateInteractionPromptPosition()
    {
        if (currentPromptInstance != null && currentTargetInteractable != null)
        {
            Transform targetTransform = currentTargetInteractable.GetGameObject().transform;

            SpriteRenderer targetSprite = currentTargetInteractable.GetGameObject().GetComponentInChildren<SpriteRenderer>();
            float yOffset = 0.5f;
            if (targetSprite != null)
            {
                yOffset = targetSprite.bounds.extents.y + 0.2f;
            }
            currentPromptInstance.transform.position = targetTransform.position + new Vector3(0, yOffset, 0);
        }
    }
}
