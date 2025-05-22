using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupThing : InteractableVisuals
{
    public Player player;
    public string AssetID { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AssetID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }
    public override bool CanInteract()
    {
        return true; 
    }

    public override void Interact(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("pickupThing: Interactor is not a Player!");
            return;
        }
        Item item = GetComponent<Item>();
        if (item != null && item.data != null)
        {
            if (player.inventory.Add(item))
            {
                Destroy(this.gameObject);
            } else { };
        }
    }

    protected override void UpdateStateVisuals()
    {
        if (highlight != null)
        {
            highlight.SetActive(false);
        }
    }


}
