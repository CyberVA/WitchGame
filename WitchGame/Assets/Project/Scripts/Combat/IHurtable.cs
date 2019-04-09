using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoStepCollision;

public enum DamageTypes { Melee, Shroom}

public interface IHurtable
{
    Box HurtBox { get; }
    bool Friendly { get; }
    void Hurt(int damage, DamageTypes damageType);
}