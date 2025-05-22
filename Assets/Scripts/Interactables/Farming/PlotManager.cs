using System.Collections;
using UnityEngine;

public class PlotManager : InteractableVisuals
{
    public enum PlantPlotState
    {
        Growing, //Dang lon
        NeedsInteraction, // Can tuong tac
        ReadyToHarvest //San sang thu hoach
    }
    public override bool CanInteract()
    {
        return currentPlotState == PlantPlotState.ReadyToHarvest || currentPlotState == PlantPlotState.NeedsInteraction;
    }

    public override void Interact(GameObject interactor)
    {
        if (!CanInteract()) return;

        if (currentPlotState == PlantPlotState.NeedsInteraction)
        {
            HandleNeededInteraction(interactor);
        }
        else if (currentPlotState == PlantPlotState.ReadyToHarvest)
        {
            Harvest(interactor);
        }
    }

    [Header("Runtime State")]
    [SerializeField] private PlantPlotState currentPlotState = PlantPlotState.Growing;

    private PlantObject.InteractionRequirement currentRequire = null;
    private int nextRequireIndex = 0;

    [HideInInspector] public PlantObject selectedPlant;
    [HideInInspector] public Vector3Int plotPosition;

    private bool isPlanted = false;
    private SpriteRenderer plantRenderer;
    private BoxCollider2D plantCollider;

    private int plantStage = 0;
    private float growthTimer;
    private TileManager tileManager;

    /*
    [Header("Visual")]
    [SerializeField] private GameObject highlight;
    public SpriteRenderer highlightRenderer;
    [SerializeField] private GameObject interactionIcon;
    public SpriteRenderer interactionRenderer;
    */

    [Header("State Icon")]
    public Sprite harvestIcon;
    public Sprite deflautIcon;

    /*
    [Header("Feedback Icon")]
    public Sprite wrongItemIcon;
    public Sprite genericIcon;
    */

    //private bool isCurrentlyTargeted = false;

    //Lấy dữ liệu đã add từ PlantObject (Ví dụ tên cây, tg trồng,...)
    //public PlantObject selectedPlant;
    protected override void Awake()
    {
        base.Awake();
        Transform childSprite = transform.GetChild(0); //Lay transform cua con
        if (childSprite != null)
        {
            //Lấy thông tin sprite Renderer từ con đầu tiên của GameObject được gắn script
            plantRenderer = childSprite.GetComponent<SpriteRenderer>();
            plantCollider = childSprite.GetComponent<BoxCollider2D>();

            if (plantRenderer == null) Debug.LogError("PlotManager: Không tìm thấy SpriteRenderer trên con!");
            if (plantCollider == null) Debug.LogError("PlotManager: Không tìm thấy BoxCollider2D trên con!");
            else plantCollider.enabled = false; // Tắt collider ban đầu, chỉ bật khi có cây
        }
        else
        {
            Debug.LogError("PlotManager: GameObject không có đối tượng con để lấy SpriteRenderer/Collider!");
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
        else
        {
            Debug.LogError("Thieu highlight!");
        }
        // Tìm TileManager trong Scene
        tileManager = FindFirstObjectByType<TileManager>();
        if (tileManager == null) Debug.LogError("PlotManager: Không tìm thấy TileManager trong Scene!");

        if (genericTargetIcon == null) genericTargetIcon = Resources.Load<Sprite>("DefaultGenericTargetIconPath"); // Ví dụ
        if (wrongItemFeedbackIcon == null) wrongItemFeedbackIcon = Resources.Load<Sprite>("DefaultWrongItemIconPath");
        /*
        if (interactionIcon != null)
        {
            interactionRenderer = interactionIcon.GetComponent<SpriteRenderer>();  
            if (interactionRenderer == null)
            {
                Debug.Log("Chua gan sprite cho interactionIcon");
            }
            interactionIcon.SetActive(false);
        }*/

    }

    public void InitializePlant(PlantObject plantData, Vector3Int position)
    {
        selectedPlant = plantData;
        plotPosition = position;
        isPlanted = true;
        plantStage = 0;

        if (selectedPlant == null || selectedPlant.plantStages == null || selectedPlant.plantStages.Length == 0)
        {
            Debug.LogError($"PlotManager: Dữ liệu cây trồng không hợp lệ cho vị trí {plotPosition}");
            Destroy(gameObject); // Hủy nếu dữ liệu không đúng
            return;
        }


        // Đặt sprite và collider ban đầu
        UpdatePlantVisuals();
        SetNewGrowthTimer();

        if (selectedPlant.timeBtwStages != null && selectedPlant.timeBtwStages.Length > plantStage)
        {
            growthTimer = selectedPlant.timeBtwStages[plantStage];
            Debug.Log($"PlotManager at {plotPosition}: growthTimer for Stage {plantStage + 1} is: {growthTimer}");
        }
        else
        {
            Debug.LogWarning($"PlotManager at {plotPosition}: Thiếu cấu hình timeBtwStages cho stage {plantStage}. Cây sẽ không lớn.");
            if (selectedPlant.plantStages.Length == 1)
            {
                plantStage = 0;
                Debug.Log($"PlotManager at {plotPosition}: Cây chỉ có 1 giai đoạn.");
            }
        }

        if (plantRenderer != null)
        {
            plantRenderer.sortingOrder = 10;
            plantRenderer.gameObject.SetActive(true);
        }
        if (plantCollider != null)
        {
            plantCollider.enabled = true;
            plantCollider.isTrigger = true;
        }

        if (highlight != null) highlight.SetActive(false);

        CheckForNextInteraction();
        base.UpdateVisuals();
    }


    // Update is called once per frame
    void Update()
    {
        if (!isPlanted || selectedPlant == null) return;
        //Nếu ô trống đã trồng => giảm growthTimer mỗi lần update frame
        if (currentPlotState == PlantPlotState.Growing)
        {
            growthTimer -= Time.deltaTime;
            if (growthTimer < 0)
            {
                //Nếu growthTimer <0  và plantStage chưa đến trạng thái trưởng thành aka trạng thái cuối
                if (plantStage < selectedPlant.plantStages.Length - 1)
                {
                    plantStage++;
                    UpdatePlantVisuals();
                    SetNewGrowthTimer();
                    CheckForNextInteraction();
                }
                else
                {
                    growthTimer = float.MaxValue;
                    if (currentRequire == null && nextRequireIndex >= selectedPlant.interactionRequirements.Count)
                    {
                        TransitionToPlotState(PlantPlotState.ReadyToHarvest);
                    }
                }
            }

        }

    }

    private void TransitionToPlotState(PlantPlotState newState)
    {
        if (currentPlotState == newState && highlight != null && highlight.activeSelf && currentPlotState == PlantPlotState.NeedsInteraction)
        {

        }
        else if (currentPlotState == newState)
        {
            return;
        }

        currentPlotState = newState;
       base.UpdateVisuals();
    }
    /*
    private void UpdateVisuals(Sprite tempIcon = null, float tempDuration = 0f)
    {
        UpdateStateVisuals(); //Cap nhat icon trang thai
        UpdateInteractionVisuals(tempIcon, tempDuration);
    }*/

    protected override void UpdateStateVisuals()
    {
        if (highlight == null || highlightRenderer == null) return;

        bool showHighlight = false;
        Sprite icon = null;

        switch (currentPlotState)
        {
            case PlantPlotState.Growing:
                showHighlight = false;
                break;
            case PlantPlotState.NeedsInteraction:
                if (currentRequire != null)
                {
                    icon = currentRequire.interactionIconSprite != null ? currentRequire.interactionIconSprite : deflautIcon;
                    showHighlight = true;
                }
                break;
            case PlantPlotState.ReadyToHarvest:
                icon = harvestIcon;
                showHighlight = true;
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
    /*
    private void UpdateInteractionVisuals(Sprite tempIcon = null, float tempDuration = 0f)
    { 
        if (interactionIcon == null || interactionRenderer == null) return;

        if (tempIcon != null && tempDuration > 0)
        {
            StartCoroutine(ShowTempHighlightIcon(tempIcon, tempDuration));
            return;
        }

        if (isCurrentlyTargeted)
        {
            interactionRenderer.sprite = genericIcon;
            interactionIcon.SetActive(true);
            return;
        }
        else interactionIcon.SetActive(false);


    }

    private IEnumerator ShowTempHighlightIcon(Sprite icon, float duration)
    {
        if (interactionIcon == null || interactionRenderer == null) yield break;

        interactionRenderer.sprite = icon;
        interactionIcon.SetActive(true);

        yield return new WaitForSeconds(duration);

        UpdateVisuals();
    }*/

    private void SetNewGrowthTimer()
    {
        if (plantStage < selectedPlant.plantStages.Length - 1)
        {
            if (selectedPlant.timeBtwStages != null && plantStage < selectedPlant.timeBtwStages.Length)
            {
                growthTimer = selectedPlant.timeBtwStages[plantStage];
            }
            else
            {
                growthTimer = float.MaxValue;
            }
        }
        else
        {
            growthTimer = float.MaxValue;
        }
    }

    private void CheckForNextInteraction()
    {
        if (currentPlotState == PlantPlotState.ReadyToHarvest || selectedPlant.interactionRequirements == null) return;
        if (currentRequire != null) return;

        if (nextRequireIndex < selectedPlant.interactionRequirements.Count)
        {
            PlantObject.InteractionRequirement nextReq = selectedPlant.interactionRequirements[nextRequireIndex];
            if (plantStage >= nextReq.triggerAtPlantStage)
            {
                currentRequire = nextReq;
                //nextRequireIndex++;
                TransitionToPlotState(PlantPlotState.NeedsInteraction);
                growthTimer = float.MaxValue;
            }
        }
        else
        {

            if (plantStage >= selectedPlant.plantStages.Length - 1)
            {
                TransitionToPlotState(PlantPlotState.ReadyToHarvest);
            }
            else if (currentPlotState != PlantPlotState.Growing)
            {
                TransitionToPlotState(PlantPlotState.Growing);
                SetNewGrowthTimer();
            }
        }
    }

    private void HandleNeededInteraction(GameObject interactor)
    {
        if (currentRequire == null || currentPlotState != PlantPlotState.NeedsInteraction) return;

        Player player = interactor.GetComponent<Player>();

        if (player == null || player.uiManager == null) return;
        if (currentRequire == null)
        {
            CompleteCurrentInteraction();
            return;
        }

        ItemData selectedItem = player.uiManager.GetSelectedItemData();

        if (selectedItem != null && selectedItem == currentRequire.requiredItem)
        {
            if (currentRequire.requiredItem.itemType != ItemType.Tool) player.uiManager.ConsumeSelectedItem();
            CompleteCurrentInteraction();
        }
        else
        {
            Debug.LogWarning($"PlotManager at {plotPosition}: Sai item! Yêu cầu '{currentRequire.interactionPrompt}' ...");
            ShowWrongItemFeedback();
            /*if (wrongItemIcon != null) // Hiển thị icon sai item
            {
                UpdateVisuals(wrongItemIcon, 1.5f); // Hiện icon "sai" trong 1.5 giây
            }*/
        }
    }

    private void CompleteCurrentInteraction()
    {
        if (currentRequire == null) return;

        if (tileManager != null && currentRequire.soilTileAfterInteraction != null)
        {
            tileManager.UpdateSoilTile(plotPosition, currentRequire.soilTileAfterInteraction);
        }

        currentRequire = null;
        nextRequireIndex++;

        CheckForNextInteraction();

        if (currentRequire == null && currentPlotState != PlantPlotState.ReadyToHarvest)
        {
            TransitionToPlotState(PlantPlotState.Growing);
            SetNewGrowthTimer();
        }

        base.UpdateVisuals();
    }

    public void Harvest(GameObject interactor)
    {
        if (currentPlotState != PlantPlotState.ReadyToHarvest) return;
        Debug.Log($"Harvesting plant at {plotPosition}");
        isPlanted = false;

        Player player = interactor.GetComponent<Player>();

        //item roi ra
        if (selectedPlant.potentialYields != null && selectedPlant.potentialYields.Count > 0)
        {
            foreach (PlantObject.HarvestYield yieldInfo in selectedPlant.potentialYields)
            {
                if (yieldInfo == null || yieldInfo.itemData == null)
                {
                    Debug.LogWarning($"PlotManager: Bỏ qua một yield entry bị null hoặc thiếu ItemData trong PlantObject '{selectedPlant.plantName}'.");
                    continue; // Bỏ qua entry này, xử lý entry tiếp theo
                }

                if (Random.value <= yieldInfo.dropChance)
                {

                    int quantityToDrop = Random.Range(yieldInfo.minCount, yieldInfo.maxCount + 1);

                    if (yieldInfo.itemData.itemPrefab != null)
                    {
                        GlobalHelper.SpawnItemAt(yieldInfo.itemData, transform.position, quantityToDrop, 0.5f, 0.0001f);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"PlotManager: Không có thông tin yield hoặc prefab cho cây {selectedPlant.plantName}");
        }

        if (tileManager != null)
        {
            //tileManager.SetPlotToTilled(plotPosition);
            tileManager.SetPlotToTilled(plotPosition); // Đảm bảo ô đất vẫn ở trạng thái đã cày
        }

        if (highlight != null) highlight.SetActive(false);
        if (interactionIconDisplay != null) interactionIconDisplay.SetActive(false);
        isCurrentlyTargeted = false;

        Destroy(gameObject);
    }


    void UpdatePlantVisuals()
    {
        if (plantRenderer == null || plantCollider == null || selectedPlant == null || selectedPlant.plantStages == null || plantStage >= selectedPlant.plantStages.Length)
        {
            Debug.LogError($"PlotManager at {plotPosition}: Không thể cập nhật hình ảnh/collider do thiếu dữ liệu hoặc component.");
            return; // Thoát nếu thiếu component hoặc dữ liệu không hợp lệ
        }

        plantRenderer.sprite = selectedPlant.plantStages[plantStage];
        //Điều chỉnh Collider cho hợp với hình ảnh mới
        if (plantRenderer.sprite != null)
        {
            plantCollider.size = new Vector2(1f, 1f);
            //plantCollider.size = plantRenderer.sprite.bounds.size;
            plantCollider.offset = Vector2.zero;
            //plantCollider.offset = new Vector2(0, plantRenderer.bounds.center.y);
            plantCollider.enabled = true;

        }
        else
        {
            plantCollider.enabled = false;
            Debug.LogWarning($"PlotManager at {plotPosition}: Sprite cho stage {plantStage} bị null.");
        }
    }
}
