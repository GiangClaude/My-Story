using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;

//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   public Inventory_UI inventoryUI;
    public Toolbar_UI toolbarUI;
    public Player player;

    private Slot_UI _selectedSlot = null;
    public Slot_UI SelectedSlot => _selectedSlot;
    //private readonly int nonSwappableToolbarSlots = 3;

    private void Start()
    {
        if (inventoryUI == null || toolbarUI == null || player == null)
        {
            Debug.LogError("UIManager is missing ref!");
            return;
        }

        inventoryUI.InitializeSlots(this);
        toolbarUI.InitializeSlots(this);
    }

    public void HandleSlotClick(Slot_UI clickedSlot)
    {
        if (_selectedSlot == null)
        {
            _selectedSlot = clickedSlot;
            _selectedSlot.SetHighLight(true);
            UpdateHeldItemDisplay();
        } else
        {
            if (_selectedSlot == clickedSlot)
            {
                //Neu click vao slot dang chon => bo chon => update
                _selectedSlot.SetHighLight(false);
                _selectedSlot = null;
                UpdateHeldItemDisplay();
            }
            else
            {
                if ((_selectedSlot.currentContainer == Slot_UI.ContainerType.Toolbar && clickedSlot.currentContainer == Slot_UI.ContainerType.Toolbar))
                {
                    _selectedSlot.SetHighLight(false);
                    _selectedSlot = clickedSlot;
                    _selectedSlot.SetHighLight(true);
                    UpdateHeldItemDisplay();
                }
                else
                {
                    AttemptSwap(_selectedSlot, clickedSlot);
                    _selectedSlot.SetHighLight(false);
                    _selectedSlot = null;
                    UpdateHeldItemDisplay();
                }
            }
        } 
    }

    private void AttemptSwap(Slot_UI slot1, Slot_UI slot2)
    {
        var dataSlot1 = GetDataSlot(slot1.currentContainer, slot1.slotIndex);
        var dataSlot2 = GetDataSlot(slot2.currentContainer, slot2.slotIndex);

        if ((slot1.currentContainer == Slot_UI.ContainerType.Toolbar && slot2.currentContainer == Slot_UI.ContainerType.Toolbar))
        {
            //Debug.LogWarning("Cant swap two Toolbar slot");
            return;
        }

        bool slot1IsEmpty = string.IsNullOrEmpty(dataSlot1.itemName);
        bool slot2IsEmpty = string.IsNullOrEmpty(dataSlot2.itemName);

        if (slot1IsEmpty && slot2IsEmpty)
        {
            Debug.LogWarning("Swap cancelled! Can not swap two empty slot!");
            return;
        }

        string tempName = dataSlot1.itemName;
        int tempCount = dataSlot1.count;
        int tempMax = dataSlot1.maxAllowed;
        Sprite tempIcon = dataSlot1.icon;


        AssignDataSlot(slot1.currentContainer, slot1.slotIndex, dataSlot2.itemName, dataSlot2.count, dataSlot2.maxAllowed, dataSlot2.icon);
        
        AssignDataSlot(slot2.currentContainer, slot2.slotIndex, tempName, tempCount, tempMax, tempIcon);

        Debug.Log($"Swapped: {slot1.currentContainer} Slot {slot1.slotIndex} <-> {slot2.currentContainer} Slot {slot2.slotIndex}");

        toolbarUI.Refresh();

        inventoryUI.Refresh();
    }

    private void UpdateHeldItemDisplay()
    {
        PlayerHeldItemDisplay heldItemDisplay = player.GetComponent<PlayerHeldItemDisplay>();
        if (heldItemDisplay != null)
        {
            heldItemDisplay.UpdateDisplay(); // Gọi hàm cập nhật của script hiển thị
        }
        else
        {
            Debug.LogWarning("UIManager: Player does not have PlayerHeldItemDisplay component!");
        }
    }

    private Slot GetToolbarDataSlot(int index)
    {
        if (player != null && player.toolbar != null && index >= 0 && index < player.toolbar.slots.Count)
        {
            return player.toolbar.slots[index];
        }
        return null;
    }
    private Slot GetInventoryDataSlot(int index)
    {
        if (player != null && player.inventory != null && index >= 0 && index < player.inventory.slots.Count)
        {
            return player.inventory.slots[index];
        }
        return null; // Trả về null nếu không hợp lệ
    }


    /*private Inventory.Slot GetDataSlot(Slot_UI.ContainerType container, int index)
    {
        if (container == Slot_UI.ContainerType.Inventory)
        {
            //inventory slot
            return player.inventory.slots[index];
        }
        else
        {
            //toolbar slot
            var toolbarSlot = player.toolbar.slots[index];
            return new Inventory.Slot
            {
                itemName = toolbarSlot.itemName,
                count = toolbarSlot.count,
                maxAllowed = toolbarSlot.maxAllowed,
                icon = toolbarSlot.icon
            };
        }
    }*/

    private Slot GetDataSlot(Slot_UI.ContainerType container, int index) // Sử dụng dynamic hoặc tạo một interface/base class chung
    {
        if (container == Slot_UI.ContainerType.Inventory)
        {
            return GetInventoryDataSlot(index);
        }
        else // Toolbar
        {
            return GetToolbarDataSlot(index);
        }
    }

    private void AssignDataSlot(Slot_UI.ContainerType container, int index, string name, int count, int max, Sprite icon)
    {
        Slot targetSlot = GetDataSlot(container, index);
        if (targetSlot != null)
        {
            targetSlot.itemName = name;
            targetSlot.count = count;
            targetSlot.maxAllowed = max; // Giữ lại maxAllowed vì nó là thuộc tính của slot
            targetSlot.icon = icon;
        }

        /*
        if (container == Slot_UI.ContainerType.Inventory)
        {
            var invSlot = GetInventoryDataSlot(index);
            if (invSlot != null)
            {
                invSlot.itemName = name;
                invSlot.count = count;
                invSlot.maxAllowed = max;
                invSlot.icon = icon;
            }
        }
        else
        {
            var toolSlot = GetToolbarDataSlot(index);
            if (toolSlot != null)
            {
                toolSlot.itemName = name;
                toolSlot.count = count;
                // toolSlot.maxAllowed = max; // Toolbar slot có thể có maxAllowed riêng
                toolSlot.icon = icon;
            }
        }*/

    }

    public ItemData GetSelectedItemData()
    {
        if (_selectedSlot != null && _selectedSlot.currentContainer == Slot_UI.ContainerType.Toolbar)
        {
            Slot toolbarSlotData = GetToolbarDataSlot(_selectedSlot.slotIndex);
            if (toolbarSlotData != null && !string.IsNullOrEmpty(toolbarSlotData.itemName)) {
                if (GameManager.instance != null && GameManager.instance.itemManager != null)
                {
                    Item item = GameManager.instance.itemManager.GetItemByName(toolbarSlotData.itemName);
                    if (item != null)
                    {
                        return item.data;
                    } else
                    {
                        Debug.LogWarning($"UIManager: Không tìm thấy ItemData cho '{toolbarSlotData.itemName}' trong ItemManager.");
                    }
                } else
                {
                    Debug.LogError("UIManager: GameManager hoặc ItemManager chưa sẵn sàng.");
                }
            }
        }
        return null;
    }

    public void ConsumeSelectedItem()
    {
        if (_selectedSlot != null && _selectedSlot.currentContainer == Slot_UI.ContainerType.Toolbar)
        {
            Slot toolbarSlotData = GetToolbarDataSlot(_selectedSlot.slotIndex);
            if (toolbarSlotData != null && !string.IsNullOrEmpty(toolbarSlotData.itemName))
            {
                toolbarSlotData.RemoveItem();
                toolbarUI.Refresh();
                UpdateHeldItemDisplay();
                Debug.Log($"Consumed 1 {toolbarSlotData.itemName} from Toolbar slot {_selectedSlot.slotIndex}. Remaining: {toolbarSlotData.count}");
                
                /*
                // Nếu hết item, bỏ chọn slot luôn
                if (toolbarSlotData.count == 0)
                {
                    _selectedSlot.SetHighLight(false);
                    _selectedSlot = null;
                    UpdateHeldItemDisplay();
                }*/
                
                //if (_selectedSlot.slotIndex >= nonSwappableToolbarSlots){Code ben tren vao day}
            }
        }
    }

}
