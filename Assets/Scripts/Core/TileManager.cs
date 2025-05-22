using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    [SerializeField] private Grid tilemapGridReference; // Đổi tên biến cho rõ ràng
    public Grid Grid => tilemapGridReference; // Thêm property để truy cập Grid
    [SerializeField] private Tilemap interactableMap;
    [SerializeField] private Tilemap cropMap; // --- Tilemap riêng để quản lý cây trồng ---

    [SerializeField] private Tile hiddenInteractableTile;
    [Header("Interactive Soid States")]
    [Tooltip("Dat da cay")]
    [SerializeField] private Tile tilledDirtTile; // Đã cày

    //[SerializeField] private Tile occupiedTilledDirtTile; //Đã cày và có cây 
    [SerializeField] private Tile wateredTilledDirtTile;

    
    void Start()
    {
        //ktra null
        Debug.Log($"TilemapGridReference: {tilemapGridReference}", this.gameObject);
        if (tilemapGridReference == null)
        {
            Debug.LogError("TileManager: Chưa gán Grid vào trường 'Tilemap Grid Reference' trong Inspector!", this.gameObject);
        }
        if (interactableMap == null) Debug.LogError("TileManager: Chưa gán Interactable Map!", this.gameObject);
        if (cropMap == null) Debug.LogError("TileManager: Chưa gán Crop Map!", this.gameObject);

        //cellBounds: trả về hình hộp chữ nhật trong hệ tọa độ ô TileMa
        //bao quanh tất cả các ô đang có tile => xác định giới hạn nhỏ nhất
        //=>Lặp qua tọa độ ô interactableMap
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            //GetTile => Trả về asset Tile tại position
            TileBase tile = interactableMap.GetTile(position);
            if ( tile != null && tile.name == "Interactable_Visible" )
            {
                //Thay thế tile tại position thành hiddenInteractableTile(ô được set alpha = 0)
                interactableMap.SetTile(position, hiddenInteractableTile);
            }
        }

        if (cropMap != null) cropMap.ClearAllTiles();
    }

    public bool IsInteractable(Vector3Int position)
    {
        TileBase tile = interactableMap.GetTile(position);
        return tile != null && tile == hiddenInteractableTile;
    }

    public bool IsReadyForPlanting(Vector3Int position) {
        TileBase interactableTile = interactableMap.GetTile(position);

        bool isTilled = interactableTile == tilledDirtTile;
        bool isWateredAndTilled = interactableTile == wateredTilledDirtTile;
        TileBase cropTile = cropMap != null ? cropMap.GetTile(position) : null;
        //Xac nhan dat da cay va khong co cay nao => moi co the trong cay

        return interactableTile != null && (isTilled || isWateredAndTilled) && cropTile == null;
        /*if (tile != null)
        {
            if (tile.name == "Interactable_Visible")
            {
                return true;
            }
        }
        return false;*/
    }

    //Thay đổi Interacted thành đã tương tác
    public void TillPlot(Vector3Int position)
    {
        
        if (IsInteractable(position))
        {
            interactableMap.SetTile(position, tilledDirtTile);
            Debug.Log($"Tilled plot at {position}");
        }
    }

    public void SetPlotToTilled(Vector3Int position)
    {
        TileBase current = interactableMap.GetTile(position);
        if (current == tilledDirtTile || current == wateredTilledDirtTile)
        {
            interactableMap.SetTile(position, tilledDirtTile);
            Debug.Log($"Plot {position} reset to TilledDirtTile.");
        }
    }

    public void UpdateSoilTile(Vector3Int position, Tile newSoil)
    {
        if (newSoil == null)
        {
            Debug.LogWarning($"TileManager: Cố gắng cập nhật đất tại {position} với một Tile null. Bỏ qua.");
            return;
        }

        TileBase currentTile = interactableMap.GetTile(position);
        if (currentTile == tilledDirtTile || currentTile == wateredTilledDirtTile)
        {
            interactableMap.SetTile(position, newSoil);
            Debug.Log($"Updated soil at {position} to {newSoil.name}");
        } else
        {
            Debug.LogWarning("TileManager: Smt error! Can not update");
        }
    }

    public Tile GetWateredSoilTile() => wateredTilledDirtTile;
    /*
    public void SetPlotOccupied(Vector3Int position, bool occupied)
    {
        if (cropMap != null)
        {
            if (occupied && interactableMap.GetTile(position) == tilledDirtTile)
            {
                if (occupiedTilledDirtTile != null)
                {
                    interactableMap.SetTile(position, occupiedTilledDirtTile);
                } 
            }
            else if (!occupied && (interactableMap.GetTile(position) == occupiedTilledDirtTile || interactableMap.GetTile(position) == tilledDirtTile))
            {
                interactableMap.SetTile(position, tilledDirtTile);
            }
        }
        Debug.Log($"Set plot {position} occupied state to: {occupied}");
    }

    public bool IsPlotOccupied(Vector3Int position)
    {
        TileBase interactableTile = interactableMap.GetTile(position);
        return interactableTile == occupiedTilledDirtTile;
    }
    */
    // Hàm này để khi thu hoạch xong, trả ô đất về trạng thái đã cày
    public void SetPlotTilled(Vector3Int position)
    {
        interactableMap.SetTile(position, tilledDirtTile);
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        if (Grid != null)
        {
            return Grid.WorldToCell(worldPosition);
        }
        Debug.LogError("TileManager: Grid reference is null. Cannot convert WorldToCell.");
        Debug.LogError("TileManager: Grid reference is null. Cannot convert WorldToCell.");
        return Vector3Int.zero; // Hoặc giá trị mặc định khác
    }

    public Vector3 GetCellCenterWorld(Vector3Int cellPosition)
    {
        if (Grid != null)
        {
            return Grid.GetCellCenterWorld(cellPosition);
        }
        Debug.LogError("TileManager: Grid reference is null. Cannot get CellCenterWorld.");
        return Vector3.zero; // Hoặc giá trị mặc định khác
    }
}
