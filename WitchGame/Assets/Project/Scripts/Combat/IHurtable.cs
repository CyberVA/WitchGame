﻿using System;
using System.Collections.Generic;
using UnityEngine;
using TwoStepCollision;

public enum DamageTypes { Melee, Shroom}

public interface IHurtable
{
    Box HitBox { get; }
    bool Friendly { get; }
    void Hurt(int damage, DamageTypes damageType, Vector2 vector);
}