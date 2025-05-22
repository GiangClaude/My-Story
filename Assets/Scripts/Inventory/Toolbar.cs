using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar
{
    /*
    [System.Serializable]
    public class Slot
    {
        public string itemName;
        public int count;
        public int maxAllowed;
        public Sprite icon;

        public Slot()
        {
            itemName = "";
            count = 0;
            maxAllowed = 10;
        }

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
    public Toolbar(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new();
            slots.Add(slot);
        }
    }
}
