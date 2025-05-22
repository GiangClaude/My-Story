using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Tool, 
    Seed,
    Crop,
    Consumable,
    Generic
}


[CreateAssetMenu(fileName = "Item Data", menuName = "Item Data", order = 50)]
public class ItemData : ScriptableObject
{
    public string itemName = "Item Name";
    public Sprite icon;
    public ItemType itemType = ItemType.Generic;
    public GameObject itemPrefab;

    [Header("Nếu là Seed")]
    public PlantObject correspondingPlant;
}
