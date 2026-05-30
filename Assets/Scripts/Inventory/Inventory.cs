using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cho phép các biến trong class được phép hiển thị và
//chỉnh sửa(Serialize) trên Unity khi class là một biến public.
[System.Serializable]
public class Inventory 
{

    // List vật phẩm trong inventory
    public List<Slot> slots = new();
    //Khởi tạo inventory => Tạo các slot trống và add vào list
    //Số lượng quy định = numSlots
    public Inventory(int numSlots) {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new ();
            slots.Add(slot);
        }
    }

    //Thêm item vào slot
    public bool Add(Item item)
    {
        foreach(Slot slot in slots)
        {
            //Duyệt qua toàn bộ slot trong List
            //Tìm slot đầu tiên có cùng loại vật phẩm và còn có thể thêm
            if (slot.itemName == item.data.itemName && slot.CanAddItem())
            {
                //Nếu tìm thấy, gọi hàm thêm slot và kết thúc hàm Add.
                slot.AddItem(item);
                return true;
            }
        }

        foreach(Slot slot in slots)
        {
            //Duyệt lại slots => TIm ô đầu tiên còn trống
            if (slot.itemName == "")
            {
                //Nếu tìm thấy thì gọi hàm thêm slot và kết thúc hàm Add
                slot.AddItem(item);
                return true;
            }
        }

        

        return false;
        //Note đây để code thêm thông báo UI hiện ra khi full slot.
    }


    //Loại bỏ vật phẩm có chỉ số index chỉ định.
    public void Remove(int index)
    {
        //Collectable itemToDrop = GameManager.instance.itemManager.GetItemByType(player.inventory.slots[slotID].type);
        slots[index].RemoveItem();
    }

    public void RemoveAll(int index)
    {
        slots[index].SetEmpty();
    }
}
