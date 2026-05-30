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

}
