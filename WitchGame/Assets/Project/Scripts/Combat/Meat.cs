using UnityEngine;
using System.Collections;
using TwoStepCollision;
using static TwoStepCollision.Func;

public class Meat : MonoBehaviour, IHurtable
{
    //Editor Data
    public Box box;

    //i frames
    public float iLength;
    float iTimer;

    //Gizmos
    public Material glMaterial;
    public Color glColor;

    //Hurtable properties
    public Box HurtBox => box;
    public bool Friendly => false;

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
