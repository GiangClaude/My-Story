using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableVisuals : MonoBehaviour, IInteractable
{
    [Header("Visual Feedback Config")]
    [SerializeField] protected GameObject highlight; // GameObject chứa highlight Sprite
    [SerializeField] protected GameObject interactionIconDisplay;

    [Header("Icons (Assigned in Subclass or Inspector)")]
    [Tooltip("Icon hiển thị khi đối tượng được target")]
    [SerializeField] protected Sprite genericTargetIcon; 
    [Tooltip("Icon hiển thị khi dùng sai item.")]
    [SerializeField] protected Sprite wrongItemFeedbackIcon;

    protected SpriteRenderer highlightRenderer;
    protected SpriteRenderer interactionIconRenderer;

    protected bool isCurrentlyTargeted = false;

    protected virtual void Awake()
    {
        if (highlight != null)
        {
            highlightRenderer = highlight.GetComponent<SpriteRenderer>();
            if (highlightRenderer == null)
            {
                Debug.LogWarning($"InteractableWithVisuals on {gameObject.name}: 'highlight' GameObject is assigned but missing a SpriteRenderer component.");
            }
            highlight.SetActive(false); // Ẩn ban đầu
        }

        if (interactionIconDisplay != null)
        {
            interactionIconRenderer = interactionIconDisplay.GetComponent<SpriteRenderer>();
            if (interactionIconRenderer == null)
            {
                Debug.LogWarning($"InteractableWithVisuals on {gameObject.name}: 'interactionIconDisplay' GameObject is assigned but missing a SpriteRenderer component.");
            }
            interactionIconDisplay.SetActive(false); // Ẩn ban đầu
        }
    }

    public abstract void Interact(GameObject interactor);
    public abstract bool CanInteract();
    public GameObject GetGameObject() => gameObject;

    public virtual void OnTargetedState(bool isTargeted)
    {
        isCurrentlyTargeted = isTargeted;
        UpdateVisuals(); // Cập nhật visuals khi trạng thái targeted thay đổi
    }

    public virtual void UpdateVisuals(Sprite tempIcon = null, float tempDuration = 0f)
    {
        // Nếu có tempIcon, hiển thị nó và thoát sớm
        if (tempIcon != null && tempDuration > 0f)
        {
            StartCoroutine(ShowTemporaryInteractionIcon(tempIcon, tempDuration));
            return;
        }

        // Cập nhật highlight dựa trên trạng thái (ví dụ: sẵn sàng thu hoạch, cần tưới nước)
        UpdateStateVisuals();

        // Cập nhật interaction icon (ví dụ: 'E' để tương tác)
        UpdateTargetingInteractionIcon();
    }

    protected virtual void UpdateStateVisuals()
    {
        // Mặc định: không làm gì với highlight. Subclass sẽ xử lý.
        // Ví dụ: PlotManager sẽ set highlightRenderer.sprite = harvestIcon và highlight.SetActive(true)
        if (highlight != null)
        {
            // highlight.SetActive(false); // Có thể tắt ở đây nếu muốn, hoặc subclass tự quản lý hoàn toàn
        }
    }

    protected virtual void UpdateTargetingInteractionIcon()
    {
        if (interactionIconDisplay == null || interactionIconRenderer == null) return;

        if (isCurrentlyTargeted && CanInteract() && genericTargetIcon != null)
        {
            interactionIconRenderer.sprite = genericTargetIcon;
            interactionIconDisplay.SetActive(true);
        }
        else
        {
            interactionIconDisplay.SetActive(false);
        }
    }

    protected IEnumerator ShowTemporaryInteractionIcon(Sprite iconToShow, float duration)
    {
        if (interactionIconDisplay == null || interactionIconRenderer == null) yield break;

        // Lưu lại sprite và trạng thái active hiện tại của interaction icon (nếu có)
        Sprite originalSprite = interactionIconRenderer.sprite;
        bool WasActive = interactionIconDisplay.activeSelf;

        interactionIconRenderer.sprite = iconToShow;
        interactionIconDisplay.SetActive(true);

        yield return new WaitForSeconds(duration);

        UpdateVisuals(); // Gọi lại UpdateVisuals để nó tự cập nhật đúng trạng thái
    }

    protected void ShowWrongItemFeedback()
    {
        if (wrongItemFeedbackIcon != null)
        {
            UpdateVisuals(wrongItemFeedbackIcon, 1.5f); // Thời gian có thể cấu hình
        }
        else
        {
            Debug.LogWarning($"InteractableWithVisuals on {gameObject.name}: Called ShowWrongItemFeedback but wrongItemFeedbackIcon is not set.");
        }
    }
}
