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
    public int health;
    public float inertia;
    public float minVelocity;

    //Auto Ref
    RoomController roomController;
    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;
    IEnumerable<Box> staticColliders;

    //i frames
    public float iLength;
    float iTimer;

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

        staticColliders = roomController.staticColliders;
        roomController.enemies.Add(this);
    }

    private void Update()
    {
        if(deathFlag)
        {
            deathFlag = false;
            roomController.enemies.Remove(this);
        }

        movement = Vector2.zero;
        if(iTimer > 0)
        {
            flash.color = new Color(1f, 1f, 1f, iTimer / iLength);
            iTimer -= Time.deltaTime;
            if(iTimer <= 0)
            {
                flash.enabled = false;
            }
        }

        //Calculate movement
        //Knockback velocity-
        if(velocity != Vector2.zero)
        {
            movement += velocity * Time.deltaTime;
            velocity *= 1f - (Time.deltaTime * inertia);
            if(velocity.magnitude < minVelocity)
            {
                velocity = Vector2.zero;
            }
        }
        //Pathfinding-

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
    public bool Hurt(int damage, DamageTypes damageType, Vector2 vector)
    {
        if(alive && iTimer <= 0)
        {
            velocity = vector * 2;
            health -= damage;
            if (health <= 0)
            {
                StartDying();
            }
            flash.enabled = true;
            flash.color = Color.white;

            iTimer += iLength;
            return true;
        }
        else
        {
            return false;
        }
    }

    void StartDying()
    {
        alive = false;
        deathFlag = true;
        spriteRenderer.color = Color.black;
        GetComponent<Animator>().enabled = false;
    }

}
