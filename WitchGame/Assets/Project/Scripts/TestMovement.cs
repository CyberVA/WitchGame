using UnityEngine;
using TwoStepCollision;
using System;
using System.Collections;
using System.Collections.Generic;
using static TwoStepCollision.Func;

public class TestMovement : MonoBehaviour, IMover
{
    //Editor Ref
    public RoomController roomController;
    public LoadOnAwake roomLoader;
    public StatusBars statusBars;
    public SpriteRenderer weapon;

    //Editor Data
    public Box colbox;
    public float speed = 3f;

    public Material glMaterial;
    public Color glColor;

    //Auto Ref
    Animator animator;

    //Melee Attack
    public float meleeLength = 1f;
    public float meleeCooldown = 1.3f;
    bool meleeActive;
    float meleeTimer;
    Vector2 meleeVector;

    //Shroom Attack
    public float shroomCooldown = 1.3f;
    float shroomTimer;

    //Data
    Box attackBox = new Box(0, 0, 1, 1);

    //temp var used every frame
    Vector2 movement;

    Vector2 pos
    {
        get => colbox.Center;
        set
        {
            colbox.Center = value;
            transform.position = value;
        }
    }


    #region IMover
    Box IMover.box => colbox;
    void IMover.SetPosition(Vector2 position)
    {
        transform.position = position;
    }
    #endregion

    void Awake()
    {
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
        SuperTranslate(this, movement, roomController.staticColliders);

        //Post-Movement

        //Room travel
        if (pos.x > roomController.roomBounds.Right)
        {
            roomLoader.LoadEast();
            pos -= new Vector2(roomController.roomBounds.width, 0f);
            return;
        }
        else if (pos.x < roomController.roomBounds.Left)
        {
            roomLoader.LoadWest();
            pos += new Vector2(roomController.roomBounds.width, 0f);
            return;
        }
        if (pos.y > roomController.roomBounds.Top)
        {
            roomLoader.LoadNorth();
            pos -= new Vector2(0f, roomController.roomBounds.height);
            return;
        }
        else if (pos.y < roomController.roomBounds.Bottom)
        {
            roomLoader.LoadSouth();
            pos += new Vector2(0f, roomController.roomBounds.height);
            return;
        }

        //Melee Attack
        if (meleeTimer < meleeCooldown)
        {
            meleeTimer += Time.deltaTime;
            if(meleeTimer > meleeCooldown)
            {
                meleeTimer = meleeCooldown;
            }
            //set status bar here
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            meleeActive = true;
            weapon.enabled = true;
            meleeVector = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized;
            meleeTimer = 0;
            //set status bar here
        }
        if (meleeActive)
        {
            if(meleeTimer > meleeLength)
            {
                meleeActive = false;
                weapon.enabled = false;
            }
            else
            {
                attackBox.Center = colbox.Center + meleeVector;
                weapon.transform.position = attackBox.Center;
                foreach (IHurtable h in GlobalData.instance.combatTriggers)
                {
                    if (!h.Friendly && Intersects(attackBox, h.HitBox))
                    {
                        h.Hurt(1, DamageTypes.Melee, meleeVector);
                    }
                }
            }
        }
        //Shroom
        if (shroomTimer < shroomCooldown)
        {
            shroomTimer += Time.deltaTime;
            if (shroomTimer > shroomCooldown)
            {
                shroomTimer = shroomCooldown;
            }
            //set status bar here
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 mouseAim = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized;
            Spore spore = SporePooler.instance.GetSpore();
            spore.Activate(colbox.Center, mouseAim, 5f);
            shroomTimer = 0;
            //set status bar here
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BeginBoxesGL(glMaterial, glColor);
        DrawBoxGL(colbox);
        if(meleeActive)
        {
            GL.Color(Color.red);
            DrawBoxGL(attackBox);
        }
        EndBoxesGL();
    }
#endif
}
