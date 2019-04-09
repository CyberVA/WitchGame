﻿using UnityEngine;
using TwoStepCollision;
using System;
using System.Collections;
using System.Collections.Generic;
using static TwoStepCollision.Func;

public class TestMovement : MonoBehaviour
{
    //Editor Ref
    public RoomController roomController;
    public SpriteRenderer weapon;

    //Editor Data
    public Box colbox;
    public float speed = 3f;
    public float attackLength = 1f;

    public Material glMaterial;
    public Color glColor;

    //Auto Ref
    IEnumerable<Box> boxes;
    Animator animator;

    //Data
    bool attackActive;
    float attackTimer;
    Box attackBox = new Box(0, 0, 1, 1);
    Vector2 attackVector;

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
        if (!attackActive && Input.GetKeyDown(KeyCode.Space))
        {
            attackActive = true;
            weapon.enabled = true;
            attackVector = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized;
            attackTimer += attackLength;
        }
        if (attackActive)
        {
            attackTimer -= Time.deltaTime;
            if(attackTimer < 0)
            {
                attackActive = false;
                weapon.enabled = false;
            }
            else
            {
                attackBox.Center = colbox.Center + attackVector;
                weapon.transform.position = attackBox.Center;
                foreach (IHurtable h in GlobalData.instance.combatTriggers)
                {
                    if (!h.Friendly && Intersects(attackBox, h.HurtBox))
                    {
                        h.Hurt(1, DamageTypes.Melee);
                    }
                }
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BeginBoxesGL(glMaterial, glColor);
        DrawBoxGL(colbox);
        if(attackActive)
        {
            GL.Color(Color.red);
            DrawBoxGL(attackBox);
        }
        EndBoxesGL();
    }
#endif
}