using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHeldItemDisplay : MonoBehaviour
{
    [Header("Tham chiếu")]
    private UIManager uiManager; // Tham chiếu đến UIManager
    private Player player; // Tham chiếu đến Player 
    [SerializeField] private SpriteRenderer heldItemSpriteRenderer; // Tam chiếu hình ảnh

    [Header("Cấu hình")]
    private Sprite currentlyDisplayedSprite = null;// Tên item đang hiển thị để tránh cập nhật không cần thiết

    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>(); 
        if (uiManager == null) Debug.LogError("PlayerHeldItemDisplay: Không tìm thấy UIManager trong Scene!");

        player = GetComponent<Player>(); // Lấy component Player cùng GameObject
        if (player == null) Debug.LogError("PlayerHeldItemDisplay: Script này phải được gắn cùng GameObject với Player script!");

        if (heldItemSpriteRenderer == null) // Nếu chưa gán từ Inspector
        {
            Transform displayPoint = transform.Find("HeldItemDisplayPoint"); // Tìm con tên "HeldItemDisplayPoint"
            if (displayPoint != null)
            {
                heldItemSpriteRenderer = displayPoint.GetComponent<SpriteRenderer>();
            }
            if (heldItemSpriteRenderer == null) Debug.LogError("PlayerHeldItemDisplay: Không tìm thấy SpriteRenderer trên HeldItemDisplayPoint! Đảm bảo đã tạo GameObject con và thêm Component SpriteRenderer.");
        }
        // Đảm bảo sprite ẩn khi bắt đầu
        ClearDisplay();
    }

    public void UpdateDisplay()
    {
        // Kiểm tra các tham chiếu cần thiết
        if (uiManager == null || player == null || player.toolbar == null || heldItemSpriteRenderer == null)
        {
            ClearDisplay(); 
            return;
        }

        Slot_UI selectedSlot = uiManager.SelectedSlot; // Lấy slot đang được chọn từ UIManager
        Sprite spriteToDisplay = null; // Sprite sẽ hiển thị (mặc định là null)

        if (selectedSlot != null && selectedSlot.currentContainer == Slot_UI.ContainerType.Toolbar)
        {
            // Lấy index của slot được chọn
            int index = selectedSlot.slotIndex;
            // Kiểm tra index hợp lệ và lấy dữ liệu slot từ Toolbar data
            if (index >= 0 && index < player.toolbar.slots.Count)
            {
                Slot dataSlot = player.toolbar.slots[index];
                // Kiểm tra xem slot có item không (dựa vào icon)
                if (dataSlot != null && dataSlot.icon != null)
                {
                    spriteToDisplay = dataSlot.icon; // Lấy icon để hiển thị
                }
            }
        }

        // Chỉ cập nhật SpriteRenderer nếu sprite cần hiển thị đã thay đổi
        if (currentlyDisplayedSprite != spriteToDisplay)
        {
            currentlyDisplayedSprite = spriteToDisplay; // Cập nhật sprite đang hiển thị
            if (currentlyDisplayedSprite != null)
            {
                heldItemSpriteRenderer.sprite = currentlyDisplayedSprite;
                heldItemSpriteRenderer.enabled = true; // Hiện SpriteRenderer
            }
            else
            {
                ClearDisplay(); // Nếu không có gì để hiển thị, ẩn đi
            }
        }

    }

    // Hàm để ẩn hiển thị
    private void ClearDisplay()
    {
        if (heldItemSpriteRenderer != null)
        {
            heldItemSpriteRenderer.sprite = null;
            heldItemSpriteRenderer.enabled = false; // Ẩn SpriteRenderer
        }
        currentlyDisplayedSprite = null; 
    }

}
