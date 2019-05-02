using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;

public class ArmShroom : MonoBehaviour, IHurtable, IMover
{
    public static int layerOrder = 0; //static counter for consistant layering
    public static int enemyCount = 0;

    //Editor Ref
    public SpriteRenderer flash;

    //Editor Values
    public Box box;
    
    //Auto Ref
    CombatSettings combatSettings;
    RoomController roomController;
    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;
    IEnumerable<Box> staticColliders;

    //Timers
    float iTimer;
    float flashTimer;
    float requestPathTimer;

    const float requestPathFreq = 0.5f;

    //Runtime Values
    float health;
    bool alive = true;
    bool deathFlag = false;
    Vector2 velocity = Vector2.zero;
    Vector2 movement;

    //AI
    Vector3[] path; // The path associated with this unit
    Vector2 currentWaypoint;
    Vector2 pathTrajectory; // normalized direction to path waypoint
    int targetIndex; //index of our target
    bool followingPath;
    
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

        staticColliders = roomController.wallColliders;

        //Layer management
        spriteRenderer.sortingOrder = layerOrder;
        flash.sortingOrder = layerOrder + 1;
        spriteMask.frontSortingOrder = layerOrder + 1;
        layerOrder++;
        //-

        //add self to collision list
        roomController.enemies.Add(this);

        //AI
        // Requests a path from the PathRequestManager
        PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
        requestPathTimer = requestPathFreq + 0.1f * enemyCount++;
    }
    private void FirstInit()
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
        }

        //Timers
        if(iTimer > 0f)
        {
            iTimer -= Time.deltaTime;
        }
        if(flashTimer > 0f)
        {
            flash.color = new Color(1f, 1f, 1f, flashTimer / combatSettings.armShroom.flashLength);
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                flash.enabled = false;
            }
        }

        if (alive)
        {
            //Living Timers
            if (requestPathTimer > 0f)
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
            //Pathfinding-
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
            }

            //Folow Path--
            if (followingPath && flashTimer <= 0f)
            {
                // If the positon of our unit is the same position as it's target
                if (Vector2.Distance(box.Center, currentWaypoint) < 0.1f)
                {
                    // Increments the target index
                    targetIndex++;
                    // If the targetIndex is greater than the length of our path, stop following path
                    if (targetIndex >= path.Length - 5)
                    {
                        followingPath = false;
                    }
                    else
                    {
                        currentWaypoint = path[targetIndex];
                    }
                }
                if (followingPath)
                {
                    //Moves the unit towards the waypoint
                    movement += (currentWaypoint - box.Center).normalized * combatSettings.armShroom.moveSpeed;
                }
            }

            //Apply movement
            SuperTranslate(this, movement * Time.deltaTime, staticColliders);

            //Post movement
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccesfull)
    {
        if (pathSuccesfull)
        {
            //Sets path to the new Path
            path = newPath;
            //Sets the targets index to 0
            targetIndex = 0;
            currentWaypoint = path[targetIndex];
            //make path following run in update
            followingPath = true;
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
                    //set knockback
                    velocity = vector * combatSettings.armShroom.vMulitplierMelee;
                    health -= damage;
                    if (health <= 0)
                    {
                        StartDying();
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
                velocity = vector * combatSettings.armShroom.vMultiplirRange;
                health -= damage;
                if (health <= 0)
                {
                    StartDying();
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

    void StartDying()
    {
        alive = false;
        deathFlag = true;
        spriteRenderer.color = Color.black;
        GetComponent<Animator>().enabled = false;
    }

}
