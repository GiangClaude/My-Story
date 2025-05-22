using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cho phép các biến trong class được phép hiển thị và
//chỉnh sửa(Serialize) trên Unity khi class là một biến public.
[System.Serializable]
public class Inventory 
{
    /*
    [System.Serializable]
    public class Slot
    {
        //Bao nhieu item/1 slot
        //Bao nhieu item do da co trong 1 slot
        //Item do co trong slot chua?
        public string itemName;
        public int count;
        public int maxAllowed;
        public Sprite icon;
        
        // Hàm khởi tạo với giá trị max/1 slot là 10.
        public Slot()
        {
            itemName = "";
            count = 0;
            maxAllowed = 10;
        }

        //Kiểm tra có thể thêm vật phẩm vào ô không
        //Số lượng đã max chưa.
        public bool CanAddItem()
        {
            if (count < maxAllowed)
            {
                return true;
            }
            return false;
        }

        //Thêm item vào giỏ. 
        public void AddItem(Item item)
        {
            //Lấy item từ gameObject item và điền các dữ liệu vào slot
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            count++;
        }


        public void RemoveItem()
        {
            
            if (count > 0)
            {
                count--;
                if (count == 0)
                {
                    icon = null;
                    itemName = "";

                }
            }
        }

        public void SetEmpty()
        {
            itemName = "";
            count = 0;
            maxAllowed = 10;
        }
    }
    */

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
