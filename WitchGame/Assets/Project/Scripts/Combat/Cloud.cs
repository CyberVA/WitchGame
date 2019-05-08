using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;

public class Cloud : MonoBehaviour
{
    float age;
    CombatSettings combatSettings;
    Box box = new Box(0f, 0f, 1f, 1f);
    Vector2 trajectory;

    public void Setup(CombatSettings combatSettings)
    {
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
        CloudPooler.instance.Retire(this);
        gameObject.SetActive(false);
    }

    public void Update()
    {
        age += Time.deltaTime;
        if (age > combatSettings.playerCloudLife)
        {
            Deactivate();
            return;
        }

        if (age < combatSettings.playerCloudMoveTime)
        {
            box.Center += trajectory * combatSettings.playerCloudSpeed * Time.deltaTime;
            transform.position = box.Center;
        }

        //Hurt enemies
        foreach (IHurtable h in GameController.Main.roomController.enemies)
        {
            if (!h.Friendly && Intersects(box, h.HitBox))
            {
                //explode? particles? AOE?
                if (h.Hurt(combatSettings.playerShroom.damage, DamageTypes.Slow, Vector2.zero))
                {

                }
            }
        }

    }
}
