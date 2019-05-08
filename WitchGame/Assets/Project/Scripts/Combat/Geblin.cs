﻿using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;
using static Enemy.AISTATES;
using System;

public class Geblin : Enemy
{
    //Melee Attack
    public Sprite knife;
    float attackDelayTimer;
    float attackRecoverTimer;
    float meleeTimer;
    bool charging = false;
    Vector2 meleeVector; //position of melee attack hitbox relative to player and direction of knockback
    Vector2 toPlayer;
    float distanceToPlayer;
    Box attackBox = new Box(Vector2.zero, 1f, 1f);

    //anim
    string walkAnimBool = string.Empty;
    
    protected override float Speed
    {
        get => combatSettings.geblin.moveSpeed * speedMultiplier;
    }
    protected override float FlashLength
    {
        get => combatSettings.geblin.flashLength;
    }
    protected override float Inertia
    {
        get => combatSettings.geblin.inertia;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        //set starting health
        health = combatSettings.geblin.hp;
    }

    private void Update()
    {
        if(deathFlag) //stuff done next frame after death because of enumeration issues
        {
            deathFlag = false;
            roomController.enemies.Remove(this);
        }

        if (alive)
        {
            //Living Timers
            if (iTimer > 0f)
            {
                iTimer -= Time.deltaTime;
            }
            UpdateFlash();
            if ((aiState == FOLLOWING || aiState == SEEKING) && requestPathTimer > 0f) //if enemy is looking for or following player, request a path at a regular interval
            {
                UpdatePathTimer();
            }
            if(aiState == ATTACKING)
            {
                if(attackDelayTimer > 0f)
                {
                    attackDelayTimer -= Time.deltaTime;
                    if(attackDelayTimer <= 0f)
                    {
                        if(Intersects(attackBox, playerHurt.HitBox))
                        {
                            playerHurt.Hurt(combatSettings.geblinStabDamage, DamageTypes.Knife, meleeVector);
                        }
                        charging = false;
                        velocity = meleeVector;
                        attackRecoverTimer = combatSettings.geblinStabRecover;
                    }
                }
                else
                {
                    attackRecoverTimer -= Time.deltaTime;
                    if(attackRecoverTimer <= 0f)
                    {
                        CalculateToPlayer();
                        if (distanceToPlayer < combatSettings.geblinStabBeginRange)
                        {
                            PrepAttack();
                        }
                        else
                        {
                            aiState = SEEKING;
                        }
                    }
                }
            }

            //Calculate movement
            movement = Vector2.zero;

            //Knockback velocity-
            UpdateVelocity();

            //Pathfinding
            if (aiState == FOLLOWING && flashTimer <= 0f)
            {
                FollowPath();
            }
            else if(charging)
            {
                movement += meleeVector * Speed;
            }

            animator.SetBool("isWalkingUp", false);
            animator.SetBool("isWalkingDown", false);
            animator.SetBool("isWalkingSide", false);
            if (movement != Vector2.zero)
            {
                switch (Utils.GetDirection(movement))
                {
                    case Direction.Up:
                        animator.SetBool("isWalkingUp", true);
                        spriteRenderer.flipX = false;
                        break;
                    case Direction.Down:
                        animator.SetBool("isWalkingDown", true);
                        spriteRenderer.flipX = false;
                        break;
                    case Direction.Left:
                        animator.SetBool("isWalkingSide", true);
                        spriteRenderer.flipX = true;
                        break;
                    case Direction.Right:
                        animator.SetBool("isWalkingSide", true);
                        spriteRenderer.flipX = false;
                        break;
                }
            }

            //Apply movement
            if (movement != Vector2.zero)
            {
                SuperTranslate(this, movement * Time.deltaTime, staticColliders);
            }
            else
            {
                transform.position = GameController.Main.pixelPerfect.PixSnapped(pos);
            }

            //Post movement
            if(aiState == FOLLOWING)
            {
                CalculateToPlayer();
                if (distanceToPlayer < combatSettings.geblinStabBeginRange)
                {
                    //stab
                    //Debug.Log("lunge");
                    aiState = ATTACKING;
                    PrepAttack();
                }
            }
        }
    }

    /// <summary>
    /// only call if toPlayer has been calculated
    /// </summary>
    void PrepAttack()
    {
        meleeVector = toPlayer / distanceToPlayer;
        attackBox.Center = pos + meleeVector;
        attackDelayTimer = combatSettings.geblinStabDelay;
        charging = true;
        switch (Utils.GetDirection(meleeVector))
        {
            case Direction.Up:
                animator.SetTrigger("stabbyUp");
                spriteRenderer.flipX = false;
                break;
            case Direction.Down:
                animator.SetTrigger("stabbyDown");
                spriteRenderer.flipX = false;
                break;
            case Direction.Left:
                animator.SetTrigger("stabbySide");
                spriteRenderer.flipX = true;
                break;
            case Direction.Right:
                animator.SetTrigger("stabbySide");
                spriteRenderer.flipX = false;
                break;
        }
    }
    void TriggerAttack()
    {

    }
    void CalculateToPlayer()
    {
        toPlayer = playerHurt.HitBox.Center - pos;
        distanceToPlayer = toPlayer.magnitude;
    }
    
    //Taking damage
    public override bool Hurt(float damage, DamageTypes damageType, Vector2 vector)
    {
        if(alive)
        {
            if(damageType == DamageTypes.Melee)
            {
                if (iTimer <= 0)
                {
                    //set knockback
                    velocity = vector * combatSettings.geblin.vMulitplierMelee;
                    if(TakeDamage(damage))
                    {
                        StartDying();
                        return true;
                    }
                    //start hit flash
                    TriggerFlash();

                    iTimer = combatSettings.geblin.invincibleLength;
                    return true;
                }
            }
            else if(damageType == DamageTypes.Shroom)
            {
                //set knockback
                velocity = vector * combatSettings.geblin.vMultiplierRange;

                if (TakeDamage(damage))
                {
                    StartDying();
                    return true;
                }

                //start hit flash
                TriggerFlash();
                return true;
            }
        }

        //return true before here if attack lands
        return false;
    }
}
