using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Yêu cầu Rigibody2D => Tác động vật lý
[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public ItemData data;

    [HideInInspector] public Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
}
