using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;

public class ArmShroom : MonoBehaviour, IHurtable, IMover
{
    public static int layerOrder = 0;

    //Editor Ref
    public SpriteRenderer flash;

    //Editor Data
    public Box box;
    float health;

    //Auto Ref
    CombatSettings combatSettings;
    RoomController roomController;
    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;
    IEnumerable<Box> staticColliders;

    //i frames
    float iTimer;

    //flash
    float flashTimer;

    //Data
    bool alive = true;
    bool deathFlag = false;
    Vector2 velocity = Vector2.zero;
    Vector2 movement;

    //Gizmos
    public Material glMaterial;
    public Color glColor;
    
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

        spriteRenderer.sortingOrder = layerOrder;
        flash.sortingOrder = layerOrder + 1;
        spriteMask.frontSortingOrder = layerOrder + 1;
        layerOrder++;

        roomController = GameController.Main.roomController;
        combatSettings = GameController.Main.combatSettings;

        staticColliders = roomController.staticColliders;
        roomController.enemies.Add(this);
    }

    private void Start()
    {
        health = combatSettings.armShroom.hp;
    }

    private void Update()
    {
        if(deathFlag)
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
            flashTimer -= Time.deltaTime; ;
            if (flashTimer <= 0)
            {
                flash.enabled = false;
            }
        }

        //Calculate movement
        movement = Vector2.zero;
        //Knockback velocity-
        if (velocity != Vector2.zero)
        {
            //Apply velocity to movement
            movement += velocity * Time.deltaTime;
            //reduce velocity
            velocity *= 1f - (Time.deltaTime * combatSettings.armShroom.inertia);
            //set velocity to zero if below minimum
            if(velocity.magnitude < combatSettings.minVelocity)
            {
                velocity = Vector2.zero;
            }
        }
        //Pathfinding-
        //movement += PATHVECTOR * moveSpeed * Time.deltaTime;

        //Apply movement
        SuperTranslate(this, movement, staticColliders);

        //Post movement
    }

    private void OnAnimatorMove()
    {
        spriteMask.sprite = spriteRenderer.sprite;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BeginBoxesGL(glMaterial, glColor);
        DrawBoxGL(box);
        EndBoxesGL();
    }
#endif

    //Hurtable implementation
    public bool Hurt(float damage, DamageTypes damageType, Vector2 vector)
    {
        if(alive)
        {
            if(damageType == DamageTypes.Melee)
            {
                if (iTimer <= 0)
                {
                    velocity = vector * 2;
                    health -= damage;
                    if (health <= 0)
                    {
                        StartDying();
                    }
                    flash.enabled = true;
                    flash.color = Color.white;
                    flashTimer = combatSettings.armShroom.flashLength;

                    iTimer = combatSettings.armShroom.invincibleLength;
                    return true;
                }
            }
            else if(damageType == DamageTypes.Shroom)
            {
                velocity = vector * 3;
                health -= damage;
                if (health <= 0)
                {
                    StartDying();
                }
                flash.enabled = true;
                flash.color = Color.white;
                flashTimer = combatSettings.armShroom.flashLength;
                return true;
            }
        }

        //return true before here if attack lands
        return false;
    }

    void StartDying()
    {
        alive = false;
        deathFlag = true;
        spriteRenderer.color = Color.black;
        GetComponent<Animator>().enabled = false;
    }

}
