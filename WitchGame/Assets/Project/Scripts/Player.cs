using UnityEngine;
using TwoStepCollision;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static TwoStepCollision.Func;

public class Player : MonoBehaviour, IMover
{
    //Editor Ref
    public SpriteRenderer weapon;

    //Editor Data
    public Box colbox;
    public float speed = 3f;

    public Material glMaterial;
    public Color glColor;

    //Auto Ref
    RoomController roomController;
    Animator animator;

    //Stats
    public float maxHealth = 1f;
    float _health;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            GameController.Main.statusBars.Health(_health / maxHealth);
        }
    }
    public float maxMana = 1f;
    float _mana;
    public float Mana
    {
        get
        {
            return _mana;
        }
        set
        {
            _mana = value;
            GameController.Main.statusBars.Mana(_mana / maxMana);
        }
    }


    //Melee Attack
    public float meleeLength = 1f;
    public float meleeCooldown = 1.3f;
    public float meleeDamage = 1f;
    bool meleeActive;
    float meleeTimer;
    Vector2 meleeVector;

    //Shroom Attack
    public float shroomCooldown = 1.3f;
    public float shroomDamage = 1f;
    public float shroomCost = 1f;
    public float shroomSpeed = 5f;
    float shroomTimer;

    //Data
    [NonSerialized]
    public bool checkWin = false;
    Box attackBox = new Box(0, 0, 1, 1);

    //temp var used every frame
    Vector2 movement;

    /// <summary>
    /// Shortcut for handling player position
    /// </summary>
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
        roomController = GameController.Main.roomController;

        colbox.Center = transform.position;
        animator = GetComponent<Animator>();

        Health = maxHealth;
        Mana = maxMana;
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
        //Check for Win-
        if(checkWin && Intersects(colbox, roomController.win))
        {
            Win();
        }

        //Room travel-
        if (pos.x > roomController.roomBounds.Right)
        {
            GameController.Main.LoadEast();
            pos -= new Vector2(roomController.roomBounds.width, 0f);
            return;
        }
        else if (pos.x < roomController.roomBounds.Left)
        {
            GameController.Main.LoadWest();
            pos += new Vector2(roomController.roomBounds.width, 0f);
            return;
        }
        if (pos.y > roomController.roomBounds.Top)
        {
            GameController.Main.LoadNorth();
            pos -= new Vector2(0f, roomController.roomBounds.height);
            return;
        }
        else if (pos.y < roomController.roomBounds.Bottom)
        {
            GameController.Main.LoadSouth();
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
            GameController.Main.statusBars.CoolDowns(0, meleeTimer / meleeCooldown);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            meleeActive = true;
            weapon.enabled = true;
            meleeVector = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized;
            meleeTimer = 0;
            GameController.Main.statusBars.CoolDowns(0, 0f);
        }
        //Melee Collision-
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
                foreach (IHurtable h in roomController.enemies)
                {
                    if (!h.Friendly && Intersects(attackBox, h.HitBox))
                    {
                        if(h.Hurt(meleeDamage, DamageTypes.Melee, meleeVector))
                        {
                            Mana = Mathf.Min(Mana + 0.5f, maxMana);
                        }
                    }
                }
            }
        }
        //Shroomancy
        if (shroomTimer < shroomCooldown)
        {
            shroomTimer += Time.deltaTime;
            if (shroomTimer > shroomCooldown)
            {
                shroomTimer = shroomCooldown;
            }
            GameController.Main.statusBars.CoolDowns(1, shroomTimer / shroomCooldown);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1) && Mana >= shroomCost)
        {
            Vector2 mouseAim = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized;
            Mana -= shroomCost;
            Spore spore = SporePooler.instance.GetSpore();
            spore.Activate(colbox.Center, mouseAim, shroomSpeed, shroomDamage);
            shroomTimer = 0;
            GameController.Main.statusBars.CoolDowns(1, 0f);
        }

    }

    public void Win()
    {
        //fill out later
        animator.SetBool("isWalking", false);
        enabled = false;
        SceneManager.LoadScene("Project/Scenes/WinScene");
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
