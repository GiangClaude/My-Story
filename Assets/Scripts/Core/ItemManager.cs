using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    //List item
    public Item[] items;
    // Lưu trữ Item dựa theo key là tên item.
    private Dictionary<string, Item> nameToItemsDict = new Dictionary<string, Item>();

    private void Awake()
    {
        foreach(Item item in items)
        {
            //Với mỗi item => duyệt thêm vào từ điển item
            AddItem(item);
        }
    }

    //Nếu trong Dictionary chưa có item => Thêm vào.
    private void AddItem (Item item)
    {
        if (!nameToItemsDict.ContainsKey(item.data.itemName))
            {
            nameToItemsDict.Add(item.data.itemName, item);
            }
    }

    //Tìm và trả về Item trong Dictionary dựa vào key.
    public Item GetItemByName(string key)
    {
        if (nameToItemsDict.ContainsKey(key))
        {
            return nameToItemsDict[key];
        }
        return null;
    }

}
