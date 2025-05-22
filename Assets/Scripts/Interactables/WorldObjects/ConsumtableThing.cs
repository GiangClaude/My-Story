using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.AccessControl;
using UnityEngine;

public class ConsumtableThing : InteractableVisuals
{
    public bool IsOpened { get; private set; }
    public string AssetID { get; private set; }
    public List<itemReceives> items;
    public Sprite openedSprite;
    public float delay = 0f;
    //private UIManager uiManager;

    [SerializeField] public ItemData itemNeedToInteract;
    /*
    [Tooltip("(Optional) GameObject chứa SpriteRenderer để hiển thị Wrong Item Icon.")]
    [SerializeField] public GameObject interactionIcon;
    private SpriteRenderer interactionRenderer;

    [Header("Feedback Icons")]
    public Sprite wrongItemIcon; // Icon khi dùng sai item
    public Sprite genericIcon;

    private bool isCurrentlyTargeted = false;
    */

    [System.Serializable]
    public class itemReceives
    {
        public ItemData itemData;
        public int minCount = 1;
        public int maxCount = 2;
        [Range(0f, 1f)]
        public float dropChance = 1f;
    }

    protected override void Awake()
    {
        base.Awake();
        AssetID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }


    public override bool CanInteract()
    {
        return !IsOpened;
    }

    public override void Interact(GameObject interactor)
    {
        if (!CanInteract()) return;
        Player player = interactor.GetComponent<Player>();
        if (player == null || player.uiManager == null)
        {
            Debug.LogError("UIManager is null! Ensure a UIManager component exists in the scene.");
            return;
        }

        bool canOpen = false;
        if (itemNeedToInteract == null)
        {
            canOpen = true;
        } else
        {
            ItemData selectedItem = player.uiManager.GetSelectedItemData();
            if (selectedItem != null && selectedItem == itemNeedToInteract)
            {
                canOpen = true;
                if (selectedItem.itemType != ItemType.Tool)
                {
                    player.uiManager.ConsumeSelectedItem();
                }
            } else
            {
                Debug.Log($"ConsumableThing: Wrong item! Need '{itemNeedToInteract.itemName}'.");
                ShowWrongItemFeedback();
            }
        } 

        if (canOpen)
        {
            OpenConsumable();
        }
    }

    private void OpenConsumable()
    {
        if (IsOpened) return;
        IsOpened = true;
        //Set Opened
        SetOpened(true);

        if (DeleteGameObject())
        {
            SpawnContainedItem();
        }
        base.UpdateVisuals();
    }

    bool DeleteGameObject()
    {
        if (!IsOpened) return false;
        Destroy(gameObject, delay);
        return true;
    }

    private void SpawnContainedItem()
    {
        if (items == null || items.Count == 0) return;
        /*
        if (GameManager.instance == null || GameManager.instance.itemManager == null)
        {
            Debug.LogError("ConsumableThing: Cannot spawn items, GameManager or ItemManager is missing!", this);
            return;
        }*/

        foreach (itemReceives item in items)
        {
            if (item.itemData == null) continue;

            if (Random.value <= item.dropChance)
            {
                int count = Random.Range(item.minCount, item.maxCount + 1);

                if (count > 0 && item.itemData != null)
                {
                    GlobalHelper.SpawnItemAt(item.itemData, transform.position, count, 1f, 0.0002f);
                    /*
                    Item itemPrefab = GameManager.instance.itemManager.GetItemByName(item.itemData.itemName);

                    if (itemPrefab != null)
                    {
                        Debug.Log("Rot vat pham!");
                        for (int i = 0; i < count; i++)
                        {
                            Debug.Log("Rot 1 vat pham!");
                            SpawnYieldItem(itemPrefab, transform.position);
                        }
                    }*/
                }
            }
        }
    }

    public void SetOpened(bool opened)
        {
            if (IsOpened == opened && openedSprite != null)
            {
                GetComponent<SpriteRenderer>().sprite = openedSprite;
            }
            Debug.Log("Already change Chest sprite!");

    }

}
