using UnityEngine;
using TwoStepCollision;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static TwoStepCollision.Func;

public class Player : MonoBehaviour, IMover, IHurtable, ICallbackReciever
{
    //Editor Ref
    public SpriteRenderer weapon; //spriterenderer for melee hitbox

    //Editor Data
    public Box colbox;
    public Vector2 boxOffset;

    //GL
    public Material glMaterial;
    public Color glColor;
    public float timeToWalk;

    //Auto Ref
    RoomController roomController;
    Animator animator;
    CombatSettings combatSettings;
    AudioLibrary audioLibrary;

    //Timers
    float t;

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
    [NonSerialized]
    public bool checkKey = false; //is a key in this room
    [NonSerialized]
    public bool checkDoor = false; //is a key in this room
    bool sliding = false;
    Direction slidingDir;
    int keys = 0; 
    Box attackBox = new Box(0, 0, 1, 1); //dimensions of melee hitbox

    //Movement
    Vector2 movement;
    float speedModifier;
    Vector2 velocity = Vector2.zero;

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

    float Speed { get => speedModifier * combatSettings.player.moveSpeed; }

    bool IHurtable.Hurt(float damage, DamageTypes damageType, Vector2 vector)
    {
        if (damageType == DamageTypes.Shockwave || damageType == DamageTypes.Knife)
        {
            //set knockback
            velocity = vector * combatSettings.armShroom.vMulitplierMelee;
            Health -= damage;
            if(Health < 0)
            {
                Health = 0;

                GameController.Main.Load("start");
                pos = Vector2.zero;
                Health = combatSettings.player.hp;
                Mana = combatSettings.playerMana;
            }
            return true;
        }
        return false;
    }

    Box IHurtable.HitBox => colbox;

    bool IHurtable.Friendly => true;
    
    #region IMover
    Box IMover.box => colbox;

    void IMover.SetPosition(Vector2 position)
    {
        pos = position;
    }
    #endregion

    void Awake()
    {
        roomController = GameController.Main.roomController;
        combatSettings = GameController.Main.combatSettings;

        audioLibrary = GameManager.gMan.audioLibrary;

        colbox.Center = (Vector2)transform.position + boxOffset;
        animator = GetComponent<Animator>();

        Health = maxHealth;
        Mana = maxMana;
        t = timeToWalk;
    }

    private void Update()
    {
        //Pre-movement

        //Movement Calculation
        movement = Vector2.zero;

        //Knockback velocity-
        if (velocity != Vector2.zero)
        {
            //Apply velocity to movement
            movement += velocity;

            //reduce velocity
            velocity *= 1f - (Time.deltaTime * combatSettings.player.inertia);

            //set velocity to zero if below minimum
            if (velocity.magnitude < combatSettings.minVelocity)
            {
                velocity = Vector2.zero;
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            movement.y += Speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y -= Speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x -= Speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += Speed;
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            speedModifier = 3f;
        }
        else
        {
            speedModifier = 1f;
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

        //Movement Mod
        /*
        if (sliding)
        {
            switch (slidingDir)
            {
                case Direction.Up:
                    movement.y = Mathf.Max(combatSettings.slideSpeed, movement.y);
                    break;
                case Direction.Down:
                    movement.y = Mathf.Min(-combatSettings.slideSpeed, movement.y);
                    break;
                case Direction.Left:
                    movement.x = Mathf.Min(-combatSettings.slideSpeed, movement.x);
                    break;
                case Direction.Right:
                    movement.x = Mathf.Max(combatSettings.slideSpeed, movement.x);
                    break;
            }
        }*/

        //Movement Applied
        SuperTranslate(this, movement * Time.deltaTime, roomController.GetStaticBoxes(keys == 0));

        if (sliding)
        {
            sliding = false;
            foreach (NoMove noMove in roomController.noMoves)
            {
                if (Intersects(colbox, noMove.box))
                {
                    sliding = true;
                    break;
                }
            }
        }
        else
        {
            foreach (NoMove noMove in roomController.noMoves)
            {
                if (Intersects(colbox, noMove.box))
                {
                    switch (noMove.direction)
                    {
                        case Direction.Up:
                            if(movement.y < 0f)
                            {
                                pos = new Vector2(pos.x, noMove.box.y + noMove.box.height * 0.5f + colbox.height * 0.5f);
                            }
                            else
                            {
                                sliding = true;
                                slidingDir = noMove.direction;
                            }
                            break;
                        case Direction.Down:
                            if (movement.y > 0f)
                            {
                                pos = new Vector2(pos.x, noMove.box.y - noMove.box.height * 0.5f - colbox.height * 0.5f);
                            }
                            else
                            {
                                sliding = true;
                                slidingDir = noMove.direction;
                            }
                            break;
                        case Direction.Left:
                            if (movement.x > 0f)
                            {
                                pos = new Vector2(noMove.box.x - noMove.box.width * 0.5f - colbox.width * 0.5f, pos.y);
                            }
                            else
                            {
                                sliding = true;
                                slidingDir = noMove.direction;
                            }
                            break;
                        case Direction.Right:
                            if (movement.x < 0f)
                            {
                                pos = new Vector2(noMove.box.x + noMove.box.width * 0.5f + colbox.width * 0.5f, pos.y);
                            }
                            else
                            {
                                sliding = true;
                                slidingDir = noMove.direction;
                            }
                            break;
                    }
                }
            }
        }

        //Post-Movement

        //Check for Win-
        if(checkWin && Intersects(colbox, roomController.winbox))
        {
            Win();
            return;
        }
        //Check for Key-
        if (checkKey && Intersects(colbox, roomController.keyBox))
        {
            roomController.CollectKey();
            keys++; //ADD UI CODE LATER
            checkKey = false;
        }

        //Check for Door
        if(checkDoor && keys > 0)
        {
            foreach(Box doorBox in roomController.doorBoxes)
            {
                if(Intersects(colbox, doorBox))
                {
                    roomController.UnlockDoor();
                    keys--;
                    break;
                }
            }
        }

        //Room travel-
        if (pos.x > roomController.roomBounds.Right)
        {
            pos -= new Vector2(roomController.roomBounds.width, 0f);
            GameController.Main.LoadEast();
            return;
        }
        else if (pos.x < roomController.roomBounds.Left)
        {
            pos += new Vector2(roomController.roomBounds.width, 0f);
            GameController.Main.LoadWest();
            return;
        }
        if (pos.y > roomController.roomBounds.Top)
        {
            pos -= new Vector2(0f, roomController.roomBounds.height);
            GameController.Main.LoadNorth();
            return;
        }
        else if (pos.y < roomController.roomBounds.Bottom)
        {
            pos += new Vector2(0f, roomController.roomBounds.height);
            GameController.Main.LoadSouth();
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
            meleeActive = true;
            weapon.enabled = true;          
            meleeVector = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized; //use this for attack anim
            switch (Utils.GetDirection(meleeVector))
            {
                case Direction.Up:
                    animator.SetTrigger("attackUp");
                    break;
                case Direction.Down:
                    animator.SetTrigger("attackDown");
                    break;
                case Direction.Left:
                    animator.SetTrigger("attackLeft");
                    break;
                case Direction.Right:
                    animator.SetTrigger("attackRight");
                    break;
            }

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
            }
            else
            {
                audioLibrary.PlayerSounds(playerEffects.Attack);
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
            audioLibrary.PlayerSounds(playerEffects.MushMancy);
        }
        //Audio
        if (movement != Vector2.zero)
        {
            t -= Time.deltaTime;
            if (t < 0)
            {
                t = timeToWalk;
                audioLibrary.WalkingSounds(walk.WalkLight);
            }
        }

    }

    public void Win()
    {
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);
        animator.SetBool("isWalkingLeft", false);
        animator.SetBool("isWalkingRight", false);
        enabled = false;
        SceneManager.LoadScene("Project/Scenes/WinScene"); //temp win screen
    }

    void ICallbackReciever.Callback(uint callBackCode)
    {
        
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
