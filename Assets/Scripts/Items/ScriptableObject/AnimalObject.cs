using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animal", menuName = "Animal")]
public class AnimalObject : ScriptableObject
{
    public string animalName;
    public int timeHarvest; //Time after feed food to harvest

    [Header("Feeding")]
    public float timeHurry;
    public ItemData requireFeedItem;

    [System.Serializable]
    public class HarvestYield
    {
        public ItemData itemData;
        public int minCount = 1;
        public int maxCount = 2;
        [Range(0f, 1f)]
        public float dropChance = 1f;
    }

    [Header("Harvesting")]
    public List<HarvestYield> potentialYields = new();

    [Header("Prefabs")]
    public GameObject animalPrefabs;
}
