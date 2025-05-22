using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalHelper
{
       public static string GenerateUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}"; //Chest_3_4
    }

    /// <summary>
    /// Spawns a specified quantity of an item at a given position with a slight random offset and launch force.
    /// </summary>
    /// <param name="itemDataToSpawn">The ItemData of the item to spawn.</param>
    /// <param name="centerPosition">The central position around which the item will spawn.</param>
    /// <param name="quantity">Number of items to spawn.</param>
    /// <param name="spawnRadius">The radius within which the item will be randomly offset from the centerPosition.</param>
    /// <param name="launchForceMultiplier">A multiplier for the small launch force applied to the item.</param>
    /// <returns>The last spawned Item component, or null if spawning failed.</returns>
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
            // droppedItem.data = itemDataToSpawn; // Thường thì prefab đã có ItemData rồi, nhưng có thể gán lại nếu cần

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
