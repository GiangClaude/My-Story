using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Yêu cầu Object được gắn Collectable có thành phần Item. 
//Tự động thêm Component Item vào nếu Object chưa được gắn
[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    //OnTriggerEnter2D: hàm Unity tự động gọi khi 1 Collider2D 
    // va chạm với 1 Trigger Collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Lấy Component Player từ đối tượng đã va chạm(collision)
        //Còn đối tương Trigger là this, đối tượng chứa script Collectable
        Player player = collision.GetComponent<Player>();
        //Nếu là Player va chạm
        if (player)
        {
            //Lấy component Item từ GameObject chứa script Collectable
            Item item = GetComponent<Item>();
            if (item != null) {
                //Nếu lấy được component 'Item' => đúng là vật phẩm có thể lấy
                //Hàm add để thêm vào túi đồ của player.
                //Xóa GameObject khỏi màn chơi
                if (player.inventory.Add(item))
                {
                    Destroy(this.gameObject);
                }
            }
        }

    }

}


