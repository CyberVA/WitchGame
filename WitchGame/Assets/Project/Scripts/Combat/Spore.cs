using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;

public class Spore : MonoBehaviour
{
    const float lifespan = 10f;
    float age;
    CombatSettings combatSettings;
    Box box = new Box(0f, 0f, 1f, 1f);
    Vector2 trajectory;

    IEnumerable<Box> staticColliders;

    public void Setup(IEnumerable<Box> staticColliders, CombatSettings combatSettings)
    {
        this.staticColliders = staticColliders;
        this.combatSettings = combatSettings;
    }
    /// <summary>
    /// Enable and launch this spore
    /// </summary>
    /// <param name="position">Position to start at</param>
    /// <param name="direction">Normalized direction the spore will travel</param>
    public void Activate(Vector2 position, Vector2 direction)
    {
        //this.speed = speed;
        //this.damage = damage;
        age = 0f;
        box.Center = position;
        transform.position = position;
        trajectory = direction;
        gameObject.SetActive(true);
    }
    /// <summary>
    /// disable object and return to pool
    /// </summary>
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

        box.Center += trajectory * combatSettings.playerShroomSpeed * Time.deltaTime;
        transform.position = box.Center;

        //Hurt enemies
        foreach (IHurtable h in GameController.Main.roomController.enemies)
        {
            if (!h.Friendly && Intersects(box, h.HitBox))
            {
                //explode? particles? AOE?
                if(h.Hurt(combatSettings.playerShroom.damage, DamageTypes.Shroom, trajectory))
                {
                    Deactivate();
                    return;
                }
            }
        }
        //Deactivate if hit wall
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
