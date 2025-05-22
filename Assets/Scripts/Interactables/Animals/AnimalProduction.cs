using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundedNPC))]
public class AnimalProduction : InteractableVisuals
{
    public enum AnimalState
    {
        Idle,           // Trạng thái chờ, sẽ chuyển sang NeedsFeeding sau timeHurry
        NeedsFeeding,   // Đang cần cho ăn
        Producing,      // Đã được cho ăn, đang trong quá trình tạo sản phẩm
        ReadyToHarvest  // Sẵn sàng để thu hoạch
    }


    [Header("State")]
    [SerializeField] private AnimalState currentState = AnimalState.Idle;
    private float currentTimer = 0f;

    [Header("Data")]
    public AnimalObject animalData;
    /*
    [Header("Components")]
    [SerializeField] private GameObject highlight;
    private SpriteRenderer highlightRenderer;
    [SerializeField] private GameObject interactionIcon;
    private SpriteRenderer interactionRenderer;
    */
    [Header("State Icon")]
    public Sprite harvestIcon;
    public Sprite hurryIcon;

    [Header("Interaction Icon")]
    public Sprite wrongIcon;
    //public Sprite genericIcon;

    private BoundedNPC boundedNPC;
    //private bool isCurrentlyTargeted = false;
    //private Animator anim;

    protected override void Awake()
    {
        base.Awake();
        boundedNPC = GetComponent<BoundedNPC>();
        //anim = GetComponent<Animator>();

        if (animalData == null)
        {
            Debug.LogError($"AnimalProduction trên {gameObject.name} thiếu AnimalObject data!", this);
        }
        if (highlight != null)
        {
            highlightRenderer = highlight.GetComponent<SpriteRenderer>();
            if (highlightRenderer == null)
            {
                Debug.Log("Chua gan sprite cho highlight");

            }
            highlight.SetActive(false);
        }
        if (boundedNPC == null)
        {
            Debug.LogError($"AnimalProduction trên {gameObject.name} không tìm thấy BoundedNPC component!", this);
        }
    }

    private void Start()
    {
        TransitionToState(AnimalState.Idle);
        //ResetTimer();
    }

    private void Update()
    {
        if (animalData == null) return;

        
        switch(currentState)
        {
            case AnimalState.Idle:
                if (animalData.timeHurry > 0)
                {
                    currentTimer += Time.deltaTime;
                    if (currentTimer >= animalData.timeHurry)
                    TransitionToState(AnimalState.NeedsFeeding);
                }
                break;
            case AnimalState.Producing:
                if (animalData.timeHarvest > 0)
                {
                    currentTimer += Time.deltaTime;
                    if (currentTimer >= animalData.timeHarvest)
                    TransitionToState(AnimalState.ReadyToHarvest);
                }
                break;
            case AnimalState.NeedsFeeding:
            case AnimalState.ReadyToHarvest:
                break;
        }
    }


    public override bool CanInteract()
    {
        return currentState == AnimalState.NeedsFeeding || currentState == AnimalState.ReadyToHarvest;
    }

    public override void Interact(GameObject interactor)
    {
        if (!CanInteract()) return;
        //Harvest
        Player player = interactor.GetComponent<Player>();

        if (currentState == AnimalState.NeedsFeeding)
        {
            HandleFeeding(player);
        }
        else if (currentState == AnimalState.ReadyToHarvest)
        {
            Harvest();
        }

    }

    private void TransitionToState(AnimalState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        currentTimer = 0f;

        //if (highlight != null) highlight.SetActive(false);

        switch(currentState )
        {
            case AnimalState.Idle:
            case AnimalState.Producing:
                boundedNPC?.ResumeMovement();
                break;
            case AnimalState.NeedsFeeding:
            case AnimalState.ReadyToHarvest:
                boundedNPC?.StopMovementAndStay();
                break;
        }

        base.UpdateVisuals();
    }
    protected override void UpdateStateVisuals()
    {
        if (highlight == null || highlightRenderer == null) return;

        bool showHighlight = false;
        Sprite icon = null;

        switch (currentState)
        {
            case AnimalState.NeedsFeeding:
                icon = hurryIcon;
                showHighlight = CanInteract();
                break;
            case AnimalState.ReadyToHarvest:
                icon = harvestIcon;
                showHighlight = CanInteract();
                break;
            default:
                showHighlight = false;
                break;
        }

        if (showHighlight && icon != null)
        {
            highlightRenderer.sprite = icon;
            highlight.SetActive(true);
        }
        else
        {
            highlight.SetActive(false);
        }
    }

    private void HandleFeeding(Player player)
    {
        if (player.uiManager == null) return;

        if (animalData.requireFeedItem == null)
        {
            Debug.Log("Animal khong can an");
            TransitionToState(AnimalState.Producing);
            return;
        }

        ItemData selectedItem = player.uiManager.GetSelectedItemData();
        if (selectedItem != null && selectedItem == animalData.requireFeedItem)
        {
            Debug.Log($"AnimalProduction: Feeding {animalData.animalName} with {selectedItem.itemName}");
            player.uiManager.ConsumeSelectedItem();
            TransitionToState(AnimalState.Producing);
        } else
        {
            Debug.LogWarning("AnimalProduction: Ban dang cho an cai gi the?");
            if (wrongIcon != null)
            {
                UpdateVisuals(wrongIcon, 1.5f);
            } else
            {
                ShowWrongItemFeedback();
            }
        }
    }

    private void EnterProducingState()
    {
        TransitionToState(AnimalState.Producing);
    }
    
    public void Harvest()
    {
        if (currentState != AnimalState.ReadyToHarvest || animalData == null || animalData.potentialYields == null) return;


        foreach (AnimalObject.HarvestYield yield in animalData.potentialYields)
        {
            if (yield.itemData == null) continue;
            if (Random.value <= yield.dropChance)
            {
                //Random min -> max
                int count = Random.Range(yield.minCount, yield.maxCount + 1);

                if (count > 0)
                {
                    GlobalHelper.SpawnItemAt(yield.itemData, transform.position, count, 1f, 0.0005f);
                }
            }
        }
        TransitionToState(AnimalState.Idle);
    }
}
