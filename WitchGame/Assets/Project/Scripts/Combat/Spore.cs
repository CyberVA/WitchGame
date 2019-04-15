using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;

public class Spore : MonoBehaviour
{
    const float lifespan = 10f;
    float speed;
    float age;
    Box box = new Box(0f, 0f, 1f, 1f);
    Vector2 trajectory;

    IEnumerable<Box> staticColliders;

    public void Setup(IEnumerable<Box> staticColliders)
    {
        this.staticColliders = staticColliders;
    }
    /// <summary>
    /// Enable and launch this spore
    /// </summary>
    /// <param name="position">Position to start at</param>
    /// <param name="direction">Normalized direction the spore will travel</param>
    public void Activate(Vector2 position, Vector2 direction, float speed)
    {
        this.speed = speed;
        age = 0f;
        box.Center = position;
        transform.position = position;
        trajectory = direction;
        gameObject.SetActive(true);
    }
    void Deactivate()
    {
        SporePooler.instance.Retire(this);
        gameObject.SetActive(false);
    }

    public void Update()
    {
        age += Time.deltaTime;
        if(age > lifespan)
        {
            Deactivate();
            return;
        }

        box.Center += trajectory * speed * Time.deltaTime;
        transform.position = box.Center;

        foreach (IHurtable h in GlobalData.instance.combatTriggers)
        {
            if (!h.Friendly && Intersects(box, h.HitBox))
            {
                //explode? particles? AOE?
                h.Hurt(3, DamageTypes.Shroom, trajectory);
                Deactivate();
                return;
            }
        }
        foreach(Box b in staticColliders)
        {
            if(Intersects(box, b))
            {
                //explode? particles? AOE?
                Deactivate();
                return;
            }
        }

    }
}
