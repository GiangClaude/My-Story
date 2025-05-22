using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    //Tham chiếu đến GameObject panel chính của túi đồ
    public GameObject inventoryPanel;
    public Player player;
    //List ô trên giao diện
    public List<Slot_UI> slots = new();
    /*
    public Slot_UI selectedSlot;
    int selectedSlotIndex;
    [SerializeField] private Canvas canvas;
    //Lưu trữ Slot đang được kéo
    //Note, hiện tại chưa làm xong về vấn đề kéo thả này
    private Slot_UI draggedSlot;

    private Image draggedIcon;
    // Update is called once per frame

    private void Awake()
    {
        //Tìm và tham chiếu đến bất cứ Object nào có type Canvas
        canvas = FindAnyObjectByType<Canvas>();
    }
    */
    void Update()
    {
        //Kiểm tra ấn nút Tab => bật/tắt Inventory
        if (Input.GetKeyDown(KeyCode.Tab)) {
            ToggleInventory();
        }
    }

    public void InitializeSlots(UIManager uiManager)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null)
            {
                slots[i].Init(Slot_UI.ContainerType.Inventory, i, uiManager);
            }
            else
            {
                Debug.LogWarning($"Inventory_UI: Slot at index {i} is null!");
            }
        }
    }

    public void ToggleInventory()
    {
        //Nếu panel đang set unactive => Set bật lên, refresh cập nhật các ô
        if(!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true); //Bat inventoryPanel
            Refresh();
        }
        else
        {
            //Them logic khi tat Inventory thi xoa o da chon
            inventoryPanel.SetActive(false);
        }
    }
    //Cập nhật giao diện ô dựa trên dữ liệu mới
    public void Refresh()
    {
        if (player == null || player.inventory == null) return;

        //Kiểm tra số lượng slot có bằng số lượng slot trong túi đồ k
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < player.inventory.slots.Count)
            {
                if (slots[i] != null)
                {
                    slots[i].SetItem(player.inventory.slots[i]);
                    slots[i].SetHighLight(false);
                }
            }
            else
            {
                if (slots[i] != null) slots[i].SetEmpty();
            }
           
        }
          /*  {
                //nếu trong túi đồ có vật phẩm
                if (player.inventory.slots[i].itemName != "")
                {
                    //Set Item để hiển thị
                    slots[i].SetItem(player.inventory.slots[i]);
                }
                else
                {
                    slots[i].SetEmpty();
                }
            }
          */
    }

    public void Remove(int slotID)
    {
        //Lấy itemName từ itemManager có ID = slotID trong túi đồ của player
        Item itemToDrop = GameManager.instance.itemManager.GetItemByName(
            player.inventory.slots[slotID].itemName);

        if (itemToDrop != null)
        {
            player.DropItem(itemToDrop);
            player.inventory.Remove(slotID);
            Refresh();
        }
    }
    /*
    public void SelectSlot(int index)
    {
        selectedSlot = slots[index];
        Debug.Log("Select slot " + index);
        selectedSlotIndex = index;
        changeToToolbar();
    }

    

    public void changeToToolbar()
    {
        //Xoa item slot trong inventory
        //Add item slot trong inventory
        player.inventory.RemoveAll(selectedSlotIndex);

        Refresh();
    }
    

    public void SlotBeginDrag(Slot_UI slot)
    {
        //lưu bản sao
        draggedIcon = Instantiate(draggedSlot.itemIcon);
        draggedSlot = slot;
        //Đặt draggerIcon làm con của Canvas => Hiển thị bên trên Inventory
        draggedIcon.transform.SetParent(canvas.transform);
        //Tắt raycastTarget để nhận các event
        draggedIcon.raycastTarget = false;
        draggedIcon.rectTransform.sizeDelta = new Vector2(50, 50);
        //Di chuyển icon đến vị trí chuột
        MoveToMousePosition(draggedIcon.gameObject);
        Debug.Log("Start Drag: " + draggedSlot.name);
        
    }
    

    public void SlotDrag()
    {
        Debug.Log("Dragging: " + draggedSlot.name);
    }

    public void SlotEndDrag()
    {
        Debug.Log("Done Dragging: " + draggedSlot.name);
    }

    public void SlotDrop(Slot_UI slot)
    {
        Debug.Log("Dropped " + draggedSlot.name + "on " + slot.name);
    }

    private void MoveToMousePosition(GameObject toMove)
    {
        if (canvas != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, Input.mousePosition, null, out position);
            //RecTransformUtility...: Chuyển đổi tọa độ điểm trên full màn hình 
            // => thành Tọa độ điểm trong RectTransform(khung màn hình canvas quy định)
            //Input.mousePosition: điểm nguồn cần chuyển đổi sang tọa độ cục bộ
            //out position: lưu tọa độ đã chuyển

            //TransformPoint: chuyển ngược từ tọa độ cục bộ => tọa độ chung
            toMove.transform.position = canvas.transform.TransformPoint(position);
        }
    }
    */

}
