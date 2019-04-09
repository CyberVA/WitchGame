using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;

public class Meat : MonoBehaviour, IHurtable
{
    //Editor Ref
    public RoomController roomController;

    //Editor Data
    public Box box;
    public float minVelocity;

    //Auto Ref
    IEnumerable<Box> staticColliders;

    //i frames
    public float iLength;
    float iTimer;

    //Data
    Vector2 velocity = Vector2.zero;

    //Gizmos
    public Material glMaterial;
    public Color glColor;

    //Hurtable properties
    public Box HitBox => box;
    public bool Friendly => false;

    private void Awake()
    {
        staticColliders = roomController.staticColliders;
    }

    private void Start()
    {
        GlobalData.instance.combatTriggers.Add(this);
    }

    private void Update()
    {
        if(iTimer > 0)
        {
            iTimer -= Time.deltaTime;
        }
        if(velocity != Vector2.zero)
        {
            SuperTranslate(box, velocity * Time.deltaTime, staticColliders);
            //velocity *= 1f - (Time.deltaTime * inertia);
        }
    }

    private void OnDrawGizmos()
    {
        BeginBoxesGL(glMaterial, glColor);
        DrawBoxGL(box);
        EndBoxesGL();
    }

    //Hurtable implementation
    public void Hurt(int damage, DamageTypes damageType)
    {
        if(iTimer <= 0)
        {
            Debug.Log("oof");
            iTimer += iLength;
        }
    }
}
