using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;

    //Tham chiếu component animator
    public Animator animator;
    //Nhan gia tri dau vao
    //movement
    private Vector3 direction;
    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); //Lay vi tri ngang
        float vertical = Input.GetAxisRaw("Vertical");// Lay vi tri doc
        
        direction = new Vector3(horizontal, vertical);//Tao vecto

        //Vị trí mới = vị trí cũ + vecto chuyển đổi * tốc độ * thời gian chênh lệch;

        AnimateMovement(direction);

    }

    private void FixedUpdate()
    {
        this.transform.position += direction * speed * Time.deltaTime;
    }

    void AnimateMovement(Vector3 direction)
    {
        //kiểm tra animator có tồn tại không
        if (animator != null)
        {
            //Nếu giá trị Diraction >0 => magnitude trả về length của vector = x^2+y^2+z^2
            if (direction.magnitude > 0)
            {
                //Set có di chuyển
                animator.SetBool("isMoving", true);
                animator.SetFloat("Horizontal", direction.x);
                animator.SetFloat("Vertical", direction.y);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }
}
