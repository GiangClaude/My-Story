using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar
{
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
