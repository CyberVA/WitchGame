using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;

public class Meat : MonoBehaviour, IHurtable, IMover
{
    //Editor Ref
    public SpriteRenderer flash;

    //Editor Data
    public Box box;
    public float inertia;
    public float minVelocity;

    //Auto Ref
    RoomController roomController;

    //i frames
    public float iLength;
    float iTimer;

    //Data
    Vector2 velocity = Vector2.zero;

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
        roomController = GameController.Main.roomController;
    }

    private void Start()
    {
       roomController.enemies.Add(this);
    }

    private void Update()
    {
        if(iTimer > 0)
        {
            flash.color = new Color(1f, 1f, 1f, iTimer / iLength);
            iTimer -= Time.deltaTime;
            if(iTimer <= 0)
            {
                flash.enabled = false;
            }
        }
        if(velocity != Vector2.zero)
        {
            SuperTranslate(this, velocity * Time.deltaTime, roomController.staticColliders);
            velocity *= 1f - (Time.deltaTime * inertia);
            if(velocity.magnitude < minVelocity)
            {
                velocity = Vector2.zero;
            }
        }
    }

    private void OnDrawGizmos()
    {
        BeginBoxesGL(glMaterial, glColor);
        DrawBoxGL(box);
        EndBoxesGL();
    }

    //Hurtable implementation
    public bool Hurt(float damage, DamageTypes damageType, Vector2 vector)
    {
        if(iTimer <= 0)
        {
            velocity = vector * 2;
            transform.localScale *= 0.9f;
            box.height *= 0.9f;
            box.width *= 0.9f;

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

}
