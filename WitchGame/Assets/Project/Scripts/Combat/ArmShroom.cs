using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;
using static ArmShroom.AISTATES;
using System;

public class ArmShroom : MonoBehaviour, IHurtable, IMover, ICallbackReciever
{
    public static int enemyCount = 0; //static counter for consistant layering

    //Editor Ref
    public SpriteRenderer flash;
    public GameObject shockWavePrefab;

    //Editor Values
    public Box box;
    
    //Auto Ref
    CombatSettings combatSettings;
    RoomController roomController;
    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;
    IEnumerable<Box> staticColliders;
    IHurtable playerHurt;

    //Timers
    float iTimer;
    float attackTimer;
    float attackPrepTimer;
    float flashTimer;
    float requestPathTimer;
    float shockWaveTimer;

    float shockWaveLength = 0.2f;

    const float requestPathFreq = 0.5f;

    //Runtime Values
    [NonSerialized]
    public float speedMultiplier = 1f;
    float health;
    bool alive = true;
    bool deathFlag = false;
    GameObject shockWave;
    Vector2 velocity = Vector2.zero;
    Vector2 movement;

    //AI
    Vector3[] path; // The path associated with this unit
    Vector2 currentWaypoint;
    Vector2 pathTrajectory; // normalized direction to path waypoint
    int targetIndex; //index of our target

    byte aiState = SEEKING;

    public static class AISTATES
    {
        public const byte SEEKING = 0;
        public const byte FOLLOWING = 1;
        public const byte ATTACKING = 2;
    }


    float Speed
    {
        get => combatSettings.armShroom.moveSpeed * speedMultiplier;
    }
    Vector2 pos
    {
        get => box.Center;
        set
        {
            box.Center = value;
            transform.position = value;
        }
    }
    
    #region IHurtable
    public Box HitBox => box;
    public bool Friendly => false;
    #endregion

    #region IMover
    Box IMover.box => box;
    void IMover.SetPosition(Vector2 position)
    {
        transform.position = position;
    }
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMask = GetComponent<SpriteMask>();

        roomController = GameController.Main.roomController;
        combatSettings = GameController.Main.combatSettings;
        playerHurt = GameController.Main.player;

        staticColliders = roomController.wallColliders;

        //Layer management
        spriteRenderer.sortingOrder = enemyCount;
        flash.sortingOrder = enemyCount + 1;
        spriteMask.frontSortingOrder = enemyCount + 1;
        spriteMask.backSortingOrder = enemyCount;
        //-

        //add self to collision list
        roomController.enemies.Add(this);

        //AI
        // Requests a path from the PathRequestManager
        PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
        requestPathTimer = requestPathFreq + 0.1f * enemyCount;

        enemyCount++;
    }
    private void FirstInit() //may be used for pooling in the future
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMask = GetComponent<SpriteMask>();

        roomController = GameController.Main.roomController;
        combatSettings = GameController.Main.combatSettings;

        staticColliders = roomController.wallColliders;
    }

    private void Start()
    {
        //set starting health
        health = combatSettings.armShroom.hp;
    }

    private void Update()
    {
        if(deathFlag) //stuff done next frame after death because of enumeration issues
        {
            deathFlag = false;
            roomController.enemies.Remove(this);
            //transform.position = GameController.Main.pixelPerfect.PixSnapped(pos);
        }

        //Timers
        if (shockWaveTimer > 0f)
        {
            shockWaveTimer -= Time.deltaTime;
            if (shockWaveTimer <= 0f)
            {
                Destroy(shockWave);
            }
        }

        if (alive)
        {
            //Living Timers
            if (iTimer > 0f)
            {
                iTimer -= Time.deltaTime;
            }
            if (flashTimer > 0f)
            {
                flash.color = new Color(1f, 1f, 1f, flashTimer / combatSettings.armShroom.flashLength);
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0)
                {
                    flash.enabled = false;
                    if (!alive)
                    {
                        enabled = false;
                    }
                }
            }
            if (aiState == ATTACKING)
            {
                if(attackPrepTimer > 0f)
                {
                    attackPrepTimer -= Time.deltaTime;
                    if(attackPrepTimer <= 0f)
                    {
                        TriggerAttack();
                    }
                }
                else if(attackTimer > 0f)
                {
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0)
                    {
                        if (PlayerInRange)
                        {
                            PrepAttack();
                        }
                        else
                        {
                            aiState = SEEKING;
                            PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
                            requestPathTimer += requestPathFreq;
                        }
                    }
                }
            }
            if (aiState == ATTACKING && attackTimer > 0f) //after attacking, the shroom waits, then attacks again or starts following the player again
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    if (PlayerInRange)
                    {
                        TriggerAttack();
                    }
                    else
                    {
                        aiState = SEEKING;
                        PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
                        requestPathTimer += requestPathFreq;
                    }
                }
            }
            if ((aiState == FOLLOWING || aiState == SEEKING) && requestPathTimer > 0f) //if enemy is looking for or following player, request a path at a regular interval
            {
                requestPathTimer -= Time.deltaTime;
                if (requestPathTimer <= 0)
                {
                    PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
                    requestPathTimer += requestPathFreq;
                }
            }

            //Calculate movement
            movement = Vector2.zero;

            //Knockback velocity-
            if (velocity != Vector2.zero)
            {
                //Apply velocity to movement
                movement += velocity;

                //reduce velocity
                velocity *= 1f - (Time.deltaTime * combatSettings.armShroom.inertia);

                //set velocity to zero if below minimum
                if (velocity.magnitude < combatSettings.minVelocity)
                {
                    velocity = Vector2.zero;
                }
            }
            //PathFinding

            //Folow Path--
            if (aiState == FOLLOWING && flashTimer <= 0f)
            {
                // If the positon of our unit is the same position as it's target
                if (Vector2.Distance(box.Center, currentWaypoint) < 0.1f)
                {
                    // Increments the target index
                    targetIndex++;
                    // If the targetIndex is greater than the length of our path, stop following path
                    if (targetIndex >= path.Length)
                    {
                        aiState = SEEKING;
                    }
                    else
                    {
                        currentWaypoint = path[targetIndex];
                    }
                }
                if (aiState == FOLLOWING)
                {
                    //Moves the unit towards the waypoint
                    movement += (currentWaypoint - box.Center).normalized * Speed;
                }
            }

            //Apply movement
            if(movement != Vector2.zero)
            {
                SuperTranslate(this, movement * Time.deltaTime, staticColliders);
            }
            else
            {
                transform.position = GameController.Main.pixelPerfect.PixSnapped(pos);
            }

            //Post movement
            if(aiState != ATTACKING && attackTimer < combatSettings.armShroomAttackCooldown && PlayerInRange)
            {
                aiState = ATTACKING;
                PrepAttack();
            }
        }
    }

    void PrepAttack()
    {
        attackPrepTimer = combatSettings.armShroomAttackPrep;
    }
    void TriggerAttack()
    {
        if (shockWave != null) Destroy(shockWave);

        shockWave = Instantiate(shockWavePrefab);
        shockWave.transform.localScale = new Vector3(combatSettings.armShroomAttackRange * 2, combatSettings.armShroomAttackRange * 2, 1f);
        shockWave.transform.position = pos;
        shockWaveTimer = shockWaveLength;

        Vector2 toPlayer = playerHurt.HitBox.Center - pos;
        float mag = toPlayer.magnitude;
        if (mag < combatSettings.armShroomAttackRange)
        {
            playerHurt.Hurt(combatSettings.armShroomAttackDamage, DamageTypes.Shockwave, toPlayer / mag);
        }
        attackTimer += combatSettings.armShroomAttackCooldown;
    }

    bool PlayerInRange => Vector2.Distance(box.Center, playerHurt.HitBox.Center) < combatSettings.armShroomAttackTriggerRange;

    public void OnPathFound(Vector3[] newPath, bool pathSuccesfull)
    {
        if (!alive) return;
        if (pathSuccesfull)
        {
            if (aiState == SEEKING) aiState = FOLLOWING;
            else if (aiState == ATTACKING) return;
            //Sets path to the new Path
            path = newPath;
            //Sets the targets index to 0
            targetIndex = 0;
            currentWaypoint = path[targetIndex];
            //make path following run in update
        }
        else
        {
            aiState = SEEKING;
        }
    }

    private void OnAnimatorMove()
    {
        //update flash mask to match animation frame
        spriteMask.sprite = spriteRenderer.sprite;
    }

    //Taking damage
    public bool Hurt(float damage, DamageTypes damageType, Vector2 vector)
    {
        if(alive)
        {
            if(damageType == DamageTypes.Melee)
            {
                if (iTimer <= 0)
                {
                    if(attackPrepTimer > 0) PrepAttack();
                    //set knockback
                    velocity = vector * combatSettings.armShroom.vMulitplierMelee;
                    health -= damage;
                    if (health <= 0)
                    {
                        StartDying(vector);
                        return true;
                    }
                    //start hit flash
                    flash.enabled = true;
                    flash.color = Color.white;
                    flashTimer = combatSettings.armShroom.flashLength;

                    iTimer = combatSettings.armShroom.invincibleLength;
                    return true;
                }
            }
            else if(damageType == DamageTypes.Shroom)
            {
                //set knockback
                velocity = vector * combatSettings.armShroom.vMultiplierRange;
                health -= damage;
                if (health <= 0)
                {
                    StartDying(vector);
                    return true;
                }

                //start hit flash
                flash.enabled = true;
                flash.color = Color.white;
                flashTimer = combatSettings.armShroom.flashLength;
                return true;
            }
        }

        //return true before here if attack lands
        return false;
    }

    //static Color fadeColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);

    void StartDying(Vector2 v)
    {
        alive = false;
        deathFlag = true;
        StartCoroutine(Effects.FadeAway(spriteRenderer, 0.5f, v * 1.5f, this));
        GetComponent<Animator>().enabled = false;
        spriteMask.enabled = false;
        flash.gameObject.SetActive(false);
    }

    void ICallbackReciever.Callback(uint callBackCode)
    {
        if(callBackCode == Effects.fadeCallback)
        {
            gameObject.SetActive(false);
        }
    }
}
