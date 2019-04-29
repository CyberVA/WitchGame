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
    public SpriteRenderer weapon; //spriterenderer for melee hitbox

    //Editor Data
    public Box colbox;
    public Vector2 boxOffset;

    //GL
    public Material glMaterial;
    public Color glColor;

    //Auto Ref
    RoomController roomController;
    Animator animator;
    CombatSettings combatSettings;

    //Stats
    float maxHealth { get => combatSettings.player.hp; }
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
    float maxMana { get => combatSettings.playerMana; }
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
    bool meleeActive;
    float meleeTimer;
    Vector2 meleeVector; //position of melee attack hitbox relative to player and direction of knockback

    //Shroom Attack
    float shroomTimer;

    //Runtime Values
    [NonSerialized]
    public bool checkWin = false; //is the fountain in this room
    Box attackBox = new Box(0, 0, 1, 1); //dimensions of melee hitbox

    //Movement
    Vector2 movement;

    /// <summary>
    /// Shortcut for handling player position
    /// </summary>
    public Vector2 pos
    {
        get => colbox.Center;
        set
        {
            colbox.Center = value;
            transform.position = value - boxOffset;
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
        combatSettings = GameController.Main.combatSettings;

        colbox.Center = (Vector2)transform.position + boxOffset;
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
            movement.y += combatSettings.player.moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y -= combatSettings.player.moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x -= combatSettings.player.moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += combatSettings.player.moveSpeed * Time.deltaTime;
        }
        if (animator != null)
        {
            if (movement.y > 0) //UP
            {
                animator.SetBool("isWalkingUp", true);
            }
            else
            {
                animator.SetBool("isWalkingUp", false);
            }

            if (movement.y < 0) //DOWN
            {
                animator.SetBool("isWalkingDown", true);
            }
            else
            {
                animator.SetBool("isWalkingDown", false);
            }

            if (movement.x < 0) //LEFT
            {
                animator.SetBool("isWalkingLeft", true);
            }
            else
            {
                animator.SetBool("isWalkingLeft", false);
            }
            if (movement.x > 0) //RIGHT
            {
                animator.SetBool("isWalkingRight", true);
            }
            else
            {
                animator.SetBool("isWalkingRight", false);
            }
        }

        //Movement Applied
        SuperTranslate(this, movement, roomController.staticColliders);

        //Post-Movement
        //Check for Win-
        if(checkWin && Intersects(colbox, roomController.winbox))
        {
            Win();
            return;
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
        if (meleeTimer < combatSettings.playerMelee.cooldown)
        {
            //Update Timer
            meleeTimer += Time.deltaTime;
            if(meleeTimer > combatSettings.playerMelee.cooldown)
            {
                meleeTimer = combatSettings.playerMelee.cooldown;
            }
            GameController.Main.statusBars.CoolDowns(0, meleeTimer / combatSettings.playerMelee.cooldown);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Activate Attack
            animator.SetBool("isAttacking", true);
            meleeActive = true;
            weapon.enabled = true;          
            meleeVector = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized; //use this for attack anim
            meleeTimer = 0;
            GameController.Main.statusBars.CoolDowns(0, 0f);
        }
        //Melee Collision-
        if (meleeActive)
        {
            //Manage Melee Hitbox
            if(meleeTimer > combatSettings.playerMeleeLength)
            {
                meleeActive = false;
                weapon.enabled = false;
                animator.SetBool("isAttacking", false);
            }
            else
            {
                attackBox.Center = colbox.Center + meleeVector;
                weapon.transform.position = attackBox.Center;
                foreach (IHurtable h in roomController.enemies)
                {
                    if (!h.Friendly && Intersects(attackBox, h.HitBox))
                    {
                        if(h.Hurt(combatSettings.playerMelee.damage, DamageTypes.Melee, meleeVector))
                        {
                            Mana = Mathf.Min(Mana + 0.5f, maxMana);
                        }
                    }
                }
            }
        }
        //ShroomAncy
        if (shroomTimer < combatSettings.playerShroom.cooldown)
        {
            //Update Timer
            shroomTimer += Time.deltaTime;
            if (shroomTimer > combatSettings.playerShroom.cooldown)
            {
                shroomTimer = combatSettings.playerShroom.cooldown;
            }
            GameController.Main.statusBars.CoolDowns(1, shroomTimer / combatSettings.playerShroom.cooldown);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1) && Mana >= combatSettings.playerShroom.cost)
        {
            //Activate Ability
            Vector2 mouseAim = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized; //direction the projectile will move
            Mana -= combatSettings.playerShroom.cost;
            Spore spore = SporePooler.instance.GetSpore(); //shroomAncy projectiles are pooled. this is used instead of Instantiate 
            spore.Activate(colbox.Center, mouseAim); //enables pooled projectile
            shroomTimer = 0;
            GameController.Main.statusBars.CoolDowns(1, 0f);
        }

    }

    public void Win()
    {
        enabled = false;
        SceneManager.LoadScene("Project/Scenes/WinScene"); //temp win screen
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
