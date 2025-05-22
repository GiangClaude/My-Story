using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot_UI : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    [SerializeField] private GameObject highLight;

    public enum ContainerType { Inventory, Toolbar }
    public ContainerType currentContainer;
    public int slotIndex;
    private UIManager uiManager;

    public void Init(ContainerType container, int index, UIManager manager)
    {
        currentContainer = container;
        slotIndex = index;
        uiManager = manager;
    }


    public void SetItem(Slot slot)
    {
        if (slot != null && !string.IsNullOrEmpty(slot.itemName))
        {
            itemIcon.sprite = slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            quantityText.text = slot.count > 1 ? slot.count.ToString() : "";
        }
        else
        {
            SetEmpty();
        }
    }
    /*
    public void SetItem(Slot slot)
    {
        if (slot != null && !string.IsNullOrEmpty(slot.itemName))
        {

            itemIcon.sprite = slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            quantityText.text = slot.count > 1 ? slot.count.ToString() : "";
            Debug.Log("Already copy to Toolbar!");
        } else
        {
            SetEmpty();
        }
    }*/
    public void SetEmpty()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
    }

    public void SetHighLight(bool isOn)
    {
        if (highLight != null)
        {
            highLight.SetActive(isOn);
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiManager != null)
        {
            uiManager.HandleSlotClick(this);
        }
        else
        {
            Debug.LogError("UIManager ref is missing in Slot_UI");
        }
    }
}
