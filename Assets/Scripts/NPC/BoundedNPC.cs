using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundedNPC : MonoBehaviour
{
    private Vector3 directionVector;
    private Transform myTransform;
    public float speed;
    private Rigidbody2D myRigidbody;
    private Animator anim;
    public Collider2D bounds;

    private bool canMove = true;
    private bool forcedStay = false;
    private float originalSpeed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        myTransform = transform;
        myRigidbody = GetComponent<Rigidbody2D>();
        originalSpeed = speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !forcedStay)
        {
            Move();
        }

    }

    private void Move()
    {
        Vector3 temp = myTransform.position + directionVector * speed * Time.deltaTime;
        //myRigidbody.MovePosition(temp);
        if (bounds == null || bounds.bounds.Contains(temp))
        {
            myRigidbody.MovePosition(temp);

        } else
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        if (!canMove || forcedStay)
        {
            if (forcedStay)
            {
                directionVector = Vector3.zero;
                if (anim != null) anim.SetFloat("MoveLeft", 2);
            }
            return;
        }
        int direction = UnityEngine.Random.Range(0, 5);
        //Debug.Log("The direction of Chicken: " + direction);
        switch(direction)
        {
            case 0:
                //Walk right
                anim.SetFloat("MoveLeft", 0);
                directionVector = Vector3.right;
                break;
            case 1:
                //Walk Left
                anim.SetFloat("MoveLeft", 1);
                directionVector = Vector3.left;
                break;
            case 2:
                //Stay
                anim.SetFloat("MoveLeft", 2);
                directionVector = Vector3.zero;
                break;
            case 3:
                //Walk Left
                anim.SetFloat("MoveLeft", 1);
                directionVector = Vector3.left;
                break;
            case 4:
                //Walk Left
                anim.SetFloat("MoveLeft", 0);
                directionVector = Vector3.right;
                break;
            default:
                break;
        }
        //UpdateAnimation();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!canMove || forcedStay) return;
        Vector3 temp = directionVector;
        ChangeDirection();
        int loops = 0;
        while (temp == directionVector && loops < 100)
        {
            loops++;
            ChangeDirection();
        }
    }

    public void StopMovementAndStay()
    {
        forcedStay = true;
        speed = 0;
        directionVector = Vector3.zero;
        if (anim != null)
        {
            anim.SetFloat("MoveLeft", 2);
            anim.speed = 1;
        }
    }

    public void ResumeMovement()
    {
        forcedStay = false;
        speed = originalSpeed;
        ChangeDirection();
        Debug.Log($"{gameObject.name} movement resumed.");
    }
}
