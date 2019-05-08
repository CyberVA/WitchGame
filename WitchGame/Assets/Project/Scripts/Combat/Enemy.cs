using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;
using static Enemy.AISTATES;
using System;

public abstract class Enemy : MonoBehaviour, IMover, IHurtable, ICallbackReciever
{
    public static int enemyCount;

    protected const float requestPathFreq = 0.7f;

    //Editor Values
    public Box box;

    //Editor Ref
    public SpriteRenderer flash;
    public SpriteMask spriteMask;

    //Auto Ref
    protected CombatSettings combatSettings;
    protected RoomController roomController;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected IEnumerable<Box> staticColliders;
    protected IHurtable playerHurt;

    //Timers
    protected float iTimer;
    protected float flashTimer;
    protected float requestPathTimer;

    //Runtime Values
    [NonSerialized]
    public float speedMultiplier = 1f;
    protected float health;
    protected bool alive = true;
    protected bool deathFlag = false;
    protected Vector2 velocity = Vector2.zero;
    protected Vector2 movement;

    //AI
    protected Vector3[] path; // The path associated with this unit
    protected Vector2 currentWaypoint;
    protected Vector2 pathTrajectory; // normalized direction to path waypoint
    protected int targetIndex; //index of our target
    
    protected byte aiState = SEEKING;

    //IHurtable
    public abstract bool Hurt(float damage, DamageTypes damageType, Vector2 vector);
    public Box HitBox => box;
    public bool Friendly => false;
    //IMover (used for collision)
    Box IMover.box => box;
    void IMover.SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    protected abstract float Speed { get; }
    protected abstract float FlashLength { get; }
    protected abstract float Inertia { get; }

    protected Vector2 pos
    {
        get => box.Center;
        set
        {
            box.Center = value;
            transform.position = value;
        }
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

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

    protected void FollowPath()
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
        if (aiState != SEEKING)
        {
            //Moves the unit towards the waypoint
            movement += (currentWaypoint - box.Center).normalized * Speed;
        }
    }
    protected void TriggerFlash()
    {
        flash.enabled = true;
        flash.color = Color.white;
        flashTimer = FlashLength;
    }
    protected void UpdateVelocity()
    {
        if (velocity != Vector2.zero)
        {
            //Apply velocity to movement
            movement += velocity;

            //reduce velocity
            velocity *= 1f - (Time.deltaTime * Inertia);

            //set velocity to zero if below minimum
            if (velocity.magnitude < combatSettings.minVelocity)
            {
                velocity = Vector2.zero;
            }
        }
    }
    protected void UpdateFlash()
    {
        if (flashTimer > 0f)
        {
            flash.color = new Color(1f, 1f, 1f, flashTimer / FlashLength);
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
    }
    protected void UpdatePathTimer()
    {
        requestPathTimer -= Time.deltaTime;
        if (requestPathTimer <= 0)
        {
            PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
            requestPathTimer += requestPathFreq;
        }
    }

    /// <summary>
    /// returns true if dead
    /// </summary>
    protected bool TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void StartDying()
    {
        alive = false;
        deathFlag = true;
        StartCoroutine(Effects.FadeAway(spriteRenderer, 0.5f, new Vector2(0f, 1.5f), this));
        GetComponent<Animator>().enabled = false;
        spriteMask.enabled = false;
        flash.gameObject.SetActive(false);
    }

    public virtual void Callback(uint callBackCode)
    {
        if (callBackCode == Effects.fadeCallback) //death fade callback
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void OnAnimatorMove()
    {
        //update flash mask to match animation frame
        spriteMask.sprite = spriteRenderer.sprite;
    }

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

    public static class AISTATES
    {
        public const byte SEEKING = 0;
        public const byte FOLLOWING = 1;
        public const byte ATTACKING = 2;
    }
}
