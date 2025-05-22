using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.Tilemaps;

//Định nghĩa lưu asset  thông tin cho các loại cây trông mà không viết code mới
[CreateAssetMenu(fileName = "New Plant", menuName = "Plant")]
public class PlantObject : ScriptableObject
{
    public string plantName;
    //Các giai đoạn phát triển cảu plant
    public Sprite[] plantStages;
    //Tg cần thiết giữa trạng thái trưởng thành
    public float[] timeBtwStages;

    [System.Serializable]
    public class InteractionRequirement
    {
        [Tooltip("Index của plantStage (trong mảng plantStages) mà tại đó yêu cầu tương tác này sẽ kích hoạt. Ví dụ: 0 cho stage đầu tiên, 1 cho stage thứ hai,...")]
        public int triggerAtPlantStage;

        [Tooltip("Item người chơi cần sử dụng để hoàn thành tương tác này.")]
        public ItemData requiredItem;

        [Tooltip("Sprite hiển thị trên cây khi đang yêu cầu tương tác này (ví dụ: biểu tượng bình tưới nước, phân bón). Để trống nếu không có.")]
        public Sprite interactionIconSprite;

        [Tooltip("Mô tả ngắn về hành động cần làm, có thể dùng cho UI.")]
        public string interactionPrompt = "Cần chăm sóc";

        //[Tooltip("Tile sẽ được áp dụng cho ô đất sau khi tương tác này hoàn thành. Để trống nếu không thay đổi đất.")]
        public Tile soilTileAfterInteraction;

    }
    [Header("Interaction Requirements")]
    [Tooltip("Danh sách các yêu cầu tương tác trong suốt quá trình phát triển của cây. Sắp xếp theo thứ tự mong muốn.")]
    public List<InteractionRequirement> interactionRequirements = new List<InteractionRequirement>();

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
    /*
    public ItemData yieldItemData; // Item sẽ nhận được khi thu hoạch
    public int yieldCount = 2;    // Số lượng item nhận được
    */
}
