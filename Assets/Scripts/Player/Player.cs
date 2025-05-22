using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Tham chiếu túi đồ => Đại diện cho túi đồ của 1 player
    public Inventory inventory;
    public GameObject plantPrefab;
    public Toolbar toolbar;

    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private LayerMask groundLayerForTools;

    private GameManager gameManager;
    public UIManager uiManager;
    private TileManager tileManager;
   
    private void Awake()
    {
        //Túi đồ 18 slots
        inventory = new Inventory(18);
        toolbar = new Toolbar(8);

        uiManager = FindFirstObjectByType<UIManager>();
        tileManager = FindFirstObjectByType<TileManager>();
    }

    private void Start()
    {
        if (uiManager == null) Debug.LogError("Player: Không tìm thấy UIManager!");
        if (tileManager == null) Debug.LogError("Player: Không tìm thấy TileManager!");
        if (plantPrefab == null) Debug.LogError("Player: Chưa gán Plant Prefab trong Inspector!");

        //InitializeFixedToolbarItems(); 


        if (uiManager != null && uiManager.toolbarUI != null)
        {
            uiManager.toolbarUI.Refresh();
        } else
        {
            Toolbar_UI toolbarUI = FindFirstObjectByType<Toolbar_UI>();
            if (toolbarUI != null)
            {
                toolbarUI.Refresh();
            }
            else
            {
                Debug.LogWarning("Player.Start: Không thể tìm thấy Toolbar_UI để Refresh!");
            }
        }
    }

    public void DropItem(Item item)
    {
        //Lấy vị trí của người chơi.
        Vector2 spawnLocation = transform.position;
        //Tạo một vị trí lệch ngẫu nhiên = Random bán kính 1,25 xung quanh gốc 0
        Vector2 spawnOffset = Random.insideUnitCircle*1.25f;

        //Vector3 spawnOffset = new Vector3(randX, randY, 0f).normalized;
        //Sử dụng Instantiate để tạo GameObject từ prefab item tại vị trí như ct, quaternion(góc xoay).identity(Không xoay)
        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        //droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);

    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            HandleLeftClick();
        }
    }

    private bool IsPointerOverUIObject()
    {
        // Tạo đối tượng PointerEventData
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        // Đặt vị trí của sự kiện là vị trí chuột hiện tại
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        // Tạo danh sách để lưu kết quả raycast
        List<RaycastResult> results = new();
        // Thực hiện raycast từ EventSystem vào tất cả các đối tượng UI
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        // Nếu danh sách kết quả có phần tử nào > 0, nghĩa là con trỏ đang trên UI
        return results.Count > 0;
    }

    private void HandleLeftClick()
    {
        if (uiManager == null || tileManager == null) return; // Thoát nếu thiếu manager
        // Lấy ItemData của item đang được chọn trên toolbar
        // Lấy vị trí chuột trên thế giới và chuyển đổi sang tọa độ ô Tilemap
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        ItemData selectedItemData = uiManager.GetSelectedItemData();
        Vector3Int cellPosition = tileManager.WorldToCell(mouseWorldPos);
        Debug.Log($"Mouse Click at World: {mouseWorldPos}, Cell: {cellPosition}");

        if (selectedItemData != null)
        {
            Debug.Log($"Selected Item: {selectedItemData.itemName}, Type: {selectedItemData.itemType}");

            
            if (selectedItemData.itemType == ItemType.Tool && selectedItemData.itemName == "Hoe")
            {
                Vector3 cellCenter = tileManager.GetCellCenterWorld(cellPosition);
                Collider2D obstacleCollider = Physics2D.OverlapPoint(cellCenter, interactableLayerMask);
                if (obstacleCollider != null)
                {
                    Debug.Log("There is something in here!");
                } else TryTillPlot(cellPosition);
            }

            else if (selectedItemData.itemType == ItemType.Seed)
            {
                TryPlantSeed(cellPosition, selectedItemData);
            }
            //Them logic cac cai khac o day

            
        } else
        {
            
            Debug.Log("Clicked with empty hand or non-toolbar item selected.");
        }

    }

    private void TryTillPlot(Vector3Int position)
    {
        if (tileManager.IsInteractable(position))
        {
            tileManager.TillPlot(position);
        } else
        {
            Debug.Log($"Cannot till at {position}. Not interactable dirt.");
        }
    }

    private void TryPlantSeed(Vector3Int position, ItemData seedData)
    {
        if (plantPrefab == null)
        {
            Debug.LogError("Player: Plant Prefab chưa được gán!");
            return;
        }
        if (seedData.correspondingPlant == null)
        {
            Debug.LogError($"Player: Seed '{seedData.itemName}' không có Corresponding Plant được gán!");
            return;
        }

        if (tileManager.IsReadyForPlanting(position))
        {
            Debug.Log($"Planting {seedData.itemName} at {position}");

            // Lấy vị trí trung tâm của ô để đặt cây
            Vector3 plantWorldPosition = tileManager.GetCellCenterWorld(position);

            // Tạo instance của Plant Prefab
            GameObject newPlantGO = Instantiate(plantPrefab, plantWorldPosition, Quaternion.identity);
            //newPlantGO.GetComponent<SpriteRenderer>.sortingOrder = 10;
            PlotManager plotManager = newPlantGO.GetComponent<PlotManager>();

            if (plotManager != null)
            {
                plotManager.InitializePlant(seedData.correspondingPlant, position);
                
                uiManager.ConsumeSelectedItem();
                //Co the them cut sence o day
            } else
            {
                Debug.LogError("Player: Plant Prefab không có component PlotManager!");
                Destroy(newPlantGO); // Hủy instance lỗi
            }
        } else
        {
            Debug.Log($"Cannot plant at {position}. Plot not ready or already occupied.");
        }

    }
}
