using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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