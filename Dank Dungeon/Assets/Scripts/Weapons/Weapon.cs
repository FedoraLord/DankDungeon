﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    
    public WeaponStats stats;
    public new SpriteRenderer renderer;
    public new BoxCollider2D collider;
    public int level = 1;

    protected bool isSwinging;
    protected bool canDamage;
    protected int remainingHits;

    protected bool IsInWall
    {
        get
        {
            return collider.IsTouchingLayers(stats.swingInterruptionLayers);
        }
    }

    private IEnumerator currentAttack;
    private bool validAttack;
    private List<Func<Vector2, IEnumerator>> attackMethods = new List<Func<Vector2, IEnumerator>>();
    private int attackIndex;

    private void Start()
    {
        attackMethods = new List<Func<Vector2, IEnumerator>>() { TryClockwiseSlash, TryCounterClockwiseSlash, TryStab };
        ResetRestingPosition();
    }

    public void SetSprite()
    {
        renderer.sprite = stats.sprite;
        collider.size = stats.sprite.bounds.size;
        collider.offset = stats.sprite.bounds.center;
    }

    public void ApplyUpgrade(WeaponStats upgrade)
    {
        stats = upgrade;
        level++;
        SetSprite();
    }

    public void AttemptSwing(Vector2 direction)
    {
        if (!isSwinging)
        {
            remainingHits = stats.hitsPerSwing;
            StartCoroutine(FindValidSwing(direction));
        }
    }

    private IEnumerator FindValidSwing(Vector2 direction)
    {
        isSwinging = true;
        validAttack = false;

        int i = 0;
        for (; i < attackMethods.Count && !validAttack; i++)
        {
            yield return attackMethods[(attackIndex + i) % attackMethods.Count](direction);
        }

        if (!validAttack)
        {
            isSwinging = false;
            collider.enabled = false;
            attackIndex = 0;
        }
        else
        {
            attackIndex = (attackIndex + 1) % attackMethods.Count;
        }
    }

    private IEnumerator TryClockwiseSlash(Vector2 cursorDirection)
    {
        currentAttack = Slash(true);
        yield return AttackIfValid(Quaternion.Euler(0, 0, stats.attackRadius / 2) * cursorDirection);
    }

    private IEnumerator TryCounterClockwiseSlash(Vector2 cursorDirection)
    {
        currentAttack = Slash(false);
        yield return AttackIfValid(Quaternion.Euler(0, 0, -stats.attackRadius / 2) * cursorDirection);
    }

    private IEnumerator TryStab(Vector2 cursorDirection)
    {
        currentAttack = Stab();
        yield return AttackIfValid(cursorDirection);
    }

    private IEnumerator AttackIfValid(Vector2 direction)
    {
        transform.parent.up = direction;
        collider.enabled = true;
        yield return new WaitForFixedUpdate();

        if (!IsInWall)
        {
            validAttack = true;
            StartCoroutine(currentAttack);
        }
    }

    protected abstract IEnumerator Slash(bool clockwise);

    protected abstract IEnumerator Stab();
    
    protected IEnumerator EndSwing()
    {
        collider.enabled = false;
        renderer.enabled = false;
        canDamage = false;
        ResetRestingPosition();

        if (stats.attackLatency > 0)
            yield return new WaitForSeconds(stats.attackLatency);

        isSwinging = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (renderer.enabled && collider.IsTouchingLayers(stats.swingInterruptionLayers))
        {
            StopCoroutine(currentAttack);
            StartCoroutine(EndSwing());
        }
        else if (remainingHits > 0)
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy hit = collision.GetComponent<Enemy>();
                hit.TakeDamage(this);
                remainingHits--;
            }
            //TODO break crates
            //else if (collision.CompareTag("Crate"))
            //{

            //}
        
        }
    }

    public void ResetRestingPosition()
    {
        transform.localPosition = new Vector3(0, 1f, 0);
    }
}
