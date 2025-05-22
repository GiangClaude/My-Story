using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Trung tâm quản lý => quản lý các hệ thống quản lý trong game
//Chỉ tồn tại 1 cái
public class GameManager : MonoBehaviour
{
    //Biến static duy nhất để truy cập GameManager
    public static GameManager instance;
    //Tham chiếu đến itemManager và tileManager
    public ItemManager itemManager;
    public TileManager tileManager;

    private void Awake()
    {
        if (instance == null)
        {
            //Nếu instance chưa tồn tại thì gán this (script hiện tại cho instance)
            //Đánh dấu không hủy khi chuyển scene.
            instance = this;
            DontDestroyOnLoad(this.gameObject); // Chỉ giữ lại instance đầu tiên

            // Chỉ lấy component nếu đây là instance duy nhất được giữ lại
            itemManager = GetComponent<ItemManager>();
            tileManager = GetComponent<TileManager>();

            // Kiểm tra sau khi GetComponent (Tùy chọn nhưng nên làm)
            if (itemManager == null)
            {
                Debug.LogError("GameManager: ItemManager component not found on the same GameObject!");
            }
            if (tileManager == null)
            {
                Debug.LogError("GameManager: TileManager component not found on the same GameObject!");
            }

        }
        else if (instance != this)
        {
            Destroy(this.gameObject); // Hủy các instance trùng lặp
            return; // Không chạy code còn lại cho instance thừa
        }
    }

}
