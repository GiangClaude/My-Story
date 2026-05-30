using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Toolbar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> toolbarSlots = new();
    //Tham chiếu đến GameObject panel chính của túi đồ
    public GameObject toolbarPanel;
    public Player player;
    public UIManager uiManager;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null )
        {
            Debug.Log("Can not find UIManager!");
        }
    }
    private void Update()
    {
       CheckAlphaNumericKeysForSelection();
       
    }

    

    public void InitializeSlots(UIManager uiManager)
    {
        for (int i = 0; i < toolbarSlots.Count; i++)
        {
            if (toolbarSlots[i] != null)
            {
                toolbarSlots[i].Init(Slot_UI.ContainerType.Toolbar, i, uiManager);
            } else
            {
                Debug.LogWarning($"Toolbar_UI: Slot at index {i} is null!");
            }
        }
        Refresh();
    }

    public void Refresh()
    {

        if (player == null || player.toolbar == null) return;

        for (int i = 0; i < toolbarSlots.Count; i++)
        {
            
            if (i < player.toolbar.slots.Count)
            {
                if (toolbarSlots[i] != null)
                {
                    // --- THÊM DEBUG LOG ---
                    var dataSlot = player.toolbar.slots[i];
                    
                    //------------
                    toolbarSlots[i].SetItem(player.toolbar.slots[i]);
                    toolbarSlots[i].SetHighLight(false);
                }
            } else
            {
                if (toolbarSlots[i] != null) toolbarSlots[i].SetEmpty();
            }
           
        }
    }

   private void CheckAlphaNumericKeysForSelection()
    {
        
 //       UIManager uiManager = FindFirstObjectByType<UIManager>();
   //     if (uiManager != null) return;

 

        for (int i = 0; i < toolbarSlots.Count;i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                Debug.Log("Selected slot: " + i);
                uiManager.HandleSlotClick(toolbarSlots[i]);
                break;
            }
        }
    }
}
