using UnityEngine;
using System.Collections.Generic;
using TwoStepCollision;
using static TwoStepCollision.Func;
using static Enemy.AISTATES;
using System;

public class ArmShroom : Enemy
{
    //Editor Ref
    public GameObject shockWavePrefab;

    //Runtime Ref
    GameObject shockWave;

    //Timers
    float attackTimer;
    float attackPrepTimer;
    float shockWaveTimer;

    float shockWaveLength = 1f;

    protected override float Speed
    {
        get => combatSettings.armShroom.moveSpeed * speedMultiplier;
    }
    protected override float FlashLength
    {
        get => combatSettings.armShroom.flashLength;
    }
    protected override float Inertia
    {
        get => combatSettings.armShroom.inertia;
    }

    private void Start()
    {
        //set starting health
        health = combatSettings.armShroom.hp;
    }

    private void Update()
    {
        if(deathFlag) //stuff done next frame after death because of enumeration issues
        {
            deathFlag = false;
            roomController.enemies.Remove(this);
        }

        //Timers
        if (shockWaveTimer > 0f)
        {
            shockWaveTimer -= Time.deltaTime;
            if (shockWaveTimer <= 0f)
            {
                Destroy(shockWave);
            }
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
            if (aiState == ATTACKING)
            {
                if(attackPrepTimer > 0f)
                {
                    attackPrepTimer -= Time.deltaTime;
                    if(attackPrepTimer <= 0f)
                    {
                        TriggerAttack();
                    }
                }
                else if(attackTimer > 0f)
                {
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0)
                    {
                        if (PlayerInRange)
                        {
                            PrepAttack();
                        }
                        else
                        {
                            aiState = SEEKING;
                            PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
                            requestPathTimer += requestPathFreq;
                        }
                    }
                }
            }
            if (aiState == ATTACKING && attackTimer > 0f) //after attacking, the shroom waits, then attacks again or starts following the player again
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    if (PlayerInRange)
                    {
                        PrepAttack();
                    }
                    else
                    {
                        aiState = SEEKING;
                        PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
                        requestPathTimer += requestPathFreq;
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

            if (movement != Vector2.zero)
            {
                animator.SetBool("isWalking", true);
                switch (Utils.GetDirection(movement))
                {
                    case Direction.Up:
                        animator.SetTrigger("walkUp");
                        spriteRenderer.flipX = false;
                        spriteMask.transform.localScale = Vector3.one;
                        break;
                    case Direction.Down:
                        animator.SetBool("walkDown", true);
                        spriteRenderer.flipX = false;
                        spriteMask.transform.localScale = Vector3.one;
                        break;
                    case Direction.Left:
                        animator.SetBool("walkSide", true);
                        spriteRenderer.flipX = true;
                        spriteMask.transform.localScale = new Vector3(-1f, 1f, 1f);
                        break;
                    case Direction.Right:
                        animator.SetBool("walkSide", true);
                        spriteRenderer.flipX = false;
                        spriteMask.transform.localScale = Vector3.one;
                        break;
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
                spriteRenderer.flipX = false;
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
            if(aiState != ATTACKING && attackTimer < combatSettings.armShroomAttackCooldown && PlayerInRange)
            {
                aiState = ATTACKING;
                PrepAttack();
            }
        }
    }

    void PrepAttack()
    {
        animator.SetTrigger("attack");
        spriteRenderer.flipX = false;
        spriteMask.transform.localScale = Vector3.one;
        attackPrepTimer = combatSettings.armShroomAttackPrep;
    }
    void TriggerAttack()
    {
        if (shockWave != null) Destroy(shockWave);

        shockWave = Instantiate(shockWavePrefab);
        shockWave.transform.localScale = new Vector3(combatSettings.armShroomAttackRange * 2, combatSettings.armShroomAttackRange * 2, 1f);
        shockWave.transform.position = GameController.Main.pixelPerfect.PixSnapped(pos);
        shockWaveTimer = shockWaveLength;

        Vector2 toPlayer = playerHurt.HitBox.Center - pos;
        float mag = toPlayer.magnitude;
        if (mag < combatSettings.armShroomAttackRange)
        {
            playerHurt.Hurt(combatSettings.armShroomAttackDamage, DamageTypes.Shockwave, toPlayer / mag);
        }
        attackTimer += combatSettings.armShroomAttackCooldown;
    }

    bool PlayerInRange => Vector2.Distance(box.Center, playerHurt.HitBox.Center) < combatSettings.armShroomAttackTriggerRange;
    
    private void OnDestroy()
    {
        if (shockWave != null) Destroy(shockWave);
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
                    if(attackPrepTimer > 0) PrepAttack();
                    //set knockback
                    velocity = vector * combatSettings.armShroom.vMulitplierMelee;
                    if (TakeDamage(damage))
                    {
                        StartDying();
                        return true;
                    }
                    //start hit flash
                    TriggerFlash();

                    iTimer = combatSettings.armShroom.invincibleLength;
                    return true;
                }
            }
            else if(damageType == DamageTypes.Shroom)
            {
                //set knockback
                velocity = vector * combatSettings.armShroom.vMultiplierRange;
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
