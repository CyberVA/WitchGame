using UnityEngine;
using TwoStepCollision;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static TwoStepCollision.Func;

public class Player : MonoBehaviour, ICollisionAgent, IHurtable, ICallbackReciever
{
    //Editor Ref
    public SpriteRenderer weapon; //spriterenderer for melee hitbox
    public SpriteRenderer flash;
    public SpriteMask spriteMask;
    public GameObject keyUI;

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
    SpriteRenderer spriteRenderer;
    CombatSettings combatSettings;
    AudioLibrary audioLibrary;
    TextMesh keyText;

    //Timers
    float t;
    float flashTimer;
    float shroomTimer;
    float healTimer;
    float cloudTimer;

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

    //Runtime Values
    [NonSerialized]
    public bool checkWin = false; //is the fountain in this room
    [NonSerialized]
    public bool checkKey = false; //is a key in this room
    [NonSerialized]
    public bool checkDoor = false; //is a key in this room
    bool sliding = false;
    bool devSuperSpeed = false;
    Direction slidingDir;
    int keys = 0; 
    Box attackBox = new Box(0, 0, 1, 1); //dimensions of melee hitbox

    //Movement
    Vector2 movement;
    float speedModifier = 3f;
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

    float Speed
    {
        get
        {
            if(!devSuperSpeed)
            {
                return combatSettings.player.moveSpeed;
            }
            else
            {
                return speedModifier * combatSettings.player.moveSpeed;
            }
        }
    }

    bool IHurtable.Hurt(float damage, DamageTypes damageType, Vector2 vector)
    {
        if (damageType == DamageTypes.Shockwave || damageType == DamageTypes.Knife)
        {
            TriggerFlash();
            //set knockback
            velocity = vector * combatSettings.armShroom.vMulitplierMelee;
            Health -= damage;
            if(Health < 0)
            {
                Health = 0;

                flash.gameObject.SetActive(false);
                Time.timeScale = 0f;
                enabled = false;
                GameController.Main.deathSceneTransition.OnClick();

                /*
                GameController.Main.Load("start");
                pos = Vector2.zero;
                velocity = Vector2.zero;
                Health = combatSettings.player.hp;
                Mana = combatSettings.playerMana;*/
            }
            return true;
        }
        return false;
    }

    Box IHurtable.HitBox => colbox;

    bool IHurtable.Friendly => true;
    
    #region IMover
    Box ICollisionAgent.box => colbox;

    void ICollisionAgent.SetPosition(Vector2 position)
    {
        pos = position;
    }
    #endregion

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        keyText = keyUI.GetComponentInChildren<TextMesh>();

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
        if(GameController.Main.gamePaused)
        {
            return;
        }

        //Pre-movement
        UpdateFlash();

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
        //DEV CHEATS
        if(Input.GetKeyDown(KeyCode.Keypad8))
        {
            Mana = maxMana;
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            devSuperSpeed = !devSuperSpeed;
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
        SuperTranslate(this, movement * Time.deltaTime, roomController.GetStaticBoxes(keys == 0));

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

        if (sliding)
        {
            sliding = false;
            foreach (NoMove noMove in roomController.noMoves)
            {
                switch (noMove.direction)
                {
                    case Direction.Up:
                        if (Intersects(noMove.box, colbox))
                        {
                            sliding = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    case Direction.Down:
                    case Direction.Left:
                    case Direction.Right:
                        if (Contains(noMove.box, pos))
                        {
                            sliding = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                }
                //only reached if continue isnt run
                break;
            }
        }
        else
        {
            foreach (NoMove noMove in roomController.noMoves)
            {
                switch (noMove.direction)
                {
                    case Direction.Up:
                        if (Intersects(noMove.box, colbox))
                        {
                            if (movement.y < 0f)
                            {
                                pos = new Vector2(pos.x, noMove.box.y + noMove.box.height * 0.5f + colbox.height * 0.5f);
                            }
                            else
                            {
                                sliding = true;
                                slidingDir = noMove.direction;
                            }
                        }
                        break;
                    case Direction.Down:
                        if (Contains(noMove.box, pos))
                        {
                            if (movement.y > 0f)
                            {
                                pos = new Vector2(pos.x, noMove.box.y - noMove.box.height * 0.5f);
                            }
                            else
                            {
                                sliding = true;
                                slidingDir = noMove.direction;
                            }
                        }
                        break;
                    case Direction.Left:
                        if (Contains(noMove.box, pos))
                        {
                            if (movement.x > 0f)
                            {
                                pos = new Vector2(noMove.box.x - noMove.box.width * 0.5f, pos.y);
                            }
                            else
                            {
                                sliding = true;
                                slidingDir = noMove.direction;
                            }
                        }
                        break;
                    case Direction.Right:
                        if (Contains(noMove.box, pos))
                        {
                            if (movement.x < 0f)
                            {
                                pos = new Vector2(noMove.box.x + noMove.box.width * 0.5f, pos.y);
                            }
                            else
                            {
                                sliding = true;
                                slidingDir = noMove.direction;
                            }
                        }
                        break;
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
            if(keys == 1)
            {
                keyUI.SetActive(true);
            }
            keyText.text = keys.ToString();
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
                    if (keys == 0)
                    {
                        keyUI.SetActive(false);
                    }
                    keyText.text = keys.ToString();
                    break;
                }
            }
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
            meleeVector = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized;
            switch (Utils.GetDirection(meleeVector))
            {
                case Direction.Up:
                    animator.SetTrigger("attackUp");
                    weapon.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case Direction.Down:
                    animator.SetTrigger("attackDown");
                    weapon.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                    break;
                case Direction.Left:
                    animator.SetTrigger("attackLeft");
                    weapon.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
                case Direction.Right:
                    animator.SetTrigger("attackRight");
                    weapon.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                    break;
            }

            meleeTimer = 0;
            GameController.Main.statusBars.CoolDowns(0, 0f);
            audioLibrary.PlayerSounds(playerEffects.Attack);
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
                attackBox.Center = colbox.Center + meleeVector;
                weapon.transform.position = attackBox.Center;
                foreach (IHurtable h in roomController.enemies)
                {
                    if (!h.Friendly && Intersects(attackBox, h.HitBox))
                    {
                        if(h.Hurt(combatSettings.playerMelee.damage, DamageTypes.Melee, meleeVector))
                        {
                            Mana = Mathf.Min(Mana - combatSettings.playerMelee.cost, maxMana);
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
            switch (Utils.GetDirection(mouseAim))
            {
                case Direction.Up:
                    animator.SetTrigger("magicUp");
                    break;
                case Direction.Down:
                    animator.SetTrigger("magicDown");
                    break;
                case Direction.Left:
                    animator.SetTrigger("magicLeft");
                    break;
                case Direction.Right:
                    animator.SetTrigger("magicRight");
                    break;
            }
            Mana -= combatSettings.playerShroom.cost;
            Spore spore = SporePooler.instance.GetSpore(); //shroomAncy projectiles are pooled. this is used instead of Instantiate 
            spore.Activate(colbox.Center, mouseAim); //enables pooled projectile
            shroomTimer = 0;
            GameController.Main.statusBars.CoolDowns(1, 0f);
            audioLibrary.PlayerSounds(playerEffects.MushMancy, 0.1f);
        }
        //cloud
        if (cloudTimer < combatSettings.playerCloud.cooldown)
        {
            //Update Timer
            cloudTimer += Time.deltaTime;
            if (cloudTimer > combatSettings.playerCloud.cooldown)
            {
                cloudTimer = combatSettings.playerCloud.cooldown;
            }
            GameController.Main.statusBars.CoolDowns(2, cloudTimer / combatSettings.playerCloud.cooldown);
        }
        else if (Input.GetKeyDown(KeyCode.Q) && Mana >= combatSettings.playerCloud.cost)
        {
            Vector2 mouseAim = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - colbox.Center).normalized; //direction the projectile will move
            switch (Utils.GetDirection(mouseAim))
            {
                case Direction.Up:
                    animator.SetTrigger("magicUp");
                    break;
                case Direction.Down:
                    animator.SetTrigger("magicDown");
                    break;
                case Direction.Left:
                    animator.SetTrigger("magicLeft");
                    break;
                case Direction.Right:
                    animator.SetTrigger("magicRight");
                    break;
            }
            //poof here later
            Mana -= combatSettings.playerCloud.cost;
            cloudTimer = 0;
            GameController.Main.statusBars.CoolDowns(2, 0f);
            Cloud cloud = CloudPooler.instance.GetCloud(); //cloud projectiles are pooled. this is used instead of Instantiate 
            cloud.Activate(colbox.Center, mouseAim); //enables pooled projectile
            audioLibrary.PlayerSounds(playerEffects.RootWall);
        }
        if (healTimer < combatSettings.playerHeal.cooldown)
        {
            //Update Timer
            healTimer += Time.deltaTime;
            if (healTimer > combatSettings.playerHeal.cooldown)
            {
                healTimer = combatSettings.playerHeal.cooldown;
            }
            GameController.Main.statusBars.CoolDowns(3, healTimer / combatSettings.playerHeal.cooldown);
        }
        else if (Input.GetKeyDown(KeyCode.E) && Mana >= combatSettings.playerHeal.cost)
        {
            animator.SetTrigger("magicDown");
            //poof here later
            Health = Mathf.Min(maxHealth, Health + combatSettings.playerHeal.damage);
            Mana -= combatSettings.playerHeal.cost;
            healTimer = 0;
            GameController.Main.statusBars.CoolDowns(3, 0f);
            audioLibrary.PlayerSounds(playerEffects.Cure);
        }
        //Audio
        if (movement != Vector2.zero)
        {
            t -= Time.deltaTime;
            if (t < 0)
            {
                t = timeToWalk;
                audioLibrary.WalkingSounds(walk.WalkLight, 0.01f);
            }
        }

    }

    protected virtual void OnAnimatorMove()
    {
        //update flash mask to match animation frame
        spriteMask.sprite = spriteRenderer.sprite;
    }

    protected void TriggerFlash()
    {
        flash.enabled = true;
        flash.color = Color.white;
        flashTimer = combatSettings.player.flashLength;
    }
    protected void UpdateFlash()
    {
        if (flashTimer > 0f)
        {
            flash.color = new Color(1f, 1f, 1f, flashTimer / combatSettings.player.flashLength);
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                flash.enabled = false;
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
