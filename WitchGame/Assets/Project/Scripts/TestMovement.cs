using UnityEngine;
using TwoStepCollision;
using System;
using System.Collections;
using System.Collections.Generic;
using static TwoStepCollision.Func;

public class TestMovement : MonoBehaviour
{
    //Editor Ref
    public RoomController roomController;
    public Box colbox;
    public float speed = 5f;

    public Material glMaterial;
    public Color glColor;

    //Auto Ref
    IEnumerable<Box> boxes;
    Animator animator;

    //temp var used every frame
    Vector2 movement;
    float f;
    
    void Awake()
    {
        boxes = roomController.staticColliders;
        colbox.Center = transform.position;
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        //Pre-movement

        //Movement Calculation
        movement = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            movement.y += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y -= speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x -= speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += speed * Time.deltaTime;
        }
        if (animator != null)
        {
            if (movement != Vector2.zero)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        //Movement Applied
        SuperTranslate(colbox, movement, boxes);
        //colbox.Center += movement;
        transform.position = colbox.Center;

        //Post-Movement

    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BeginBoxesGL(glMaterial, glColor);
        DrawBoxGL(colbox);
        EndBoxesGL();
    }
#endif
}
