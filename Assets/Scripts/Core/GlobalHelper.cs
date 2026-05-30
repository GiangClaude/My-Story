using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalHelper
{
       public static string GenerateUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}"; //Chest_3_4
    }


    /*
     * itemDataToSpawn: loai vat pham can tao
     * centerPosition: vi tri trung tam
     * quantity: so luong
     * spawnRadius: ban kinh ngau nhien
     * launchForceMultiplier: luc day nho
     */ 
    public static Item SpawnItemAt(ItemData itemDataToSpawn, Vector3 centerPosition, int quantity = 1, float spawnRadius = 0.5f, float launchForceMultiplier = 0.0001f)
    {
        if (itemDataToSpawn == null)
        {
            Debug.LogError("GlobalHelper.SpawnItemAt: itemDataToSpawn is null.");
            return null;
        }
        if (itemDataToSpawn.itemPrefab == null)
        {
            Debug.LogError($"GlobalHelper.SpawnItemAt: itemPrefab for {itemDataToSpawn.itemName} is null.");
            return null;
        }

        Item itemComponentPrefab = itemDataToSpawn.itemPrefab.GetComponent<Item>();
        if (itemComponentPrefab == null)
        {
            Debug.LogError($"GlobalHelper.SpawnItemAt: itemPrefab for {itemDataToSpawn.itemName} is missing the Item component.");
            return null;
        }

        Item lastSpawnedItem = null;
        for (int i = 0; i < quantity; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = centerPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

            Item droppedItem = Object.Instantiate(itemComponentPrefab, spawnPosition, Quaternion.identity);

            if (droppedItem.rb2d != null && launchForceMultiplier > 0)
            {
                // Lực đẩy nhỏ để item không bị kẹt vào nhau nếu spawn nhiều
                Vector2 launchDirection = randomOffset.normalized;
                if (launchDirection == Vector2.zero) launchDirection = Random.insideUnitCircle.normalized; // Tránh chia cho 0
                if (launchDirection == Vector2.zero) launchDirection = Vector2.up; // Fallback cuối cùng

                droppedItem.rb2d.AddForce(launchDirection * Random.Range(0.5f, 1.5f) * launchForceMultiplier, ForceMode2D.Impulse);
            }
            Debug.Log($"GlobalHelper: Spawned {itemDataToSpawn.itemName} at {spawnPosition}");
            lastSpawnedItem = droppedItem;
        }
        return lastSpawnedItem;
    }
}
