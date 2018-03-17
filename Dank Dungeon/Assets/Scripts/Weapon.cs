using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    public SwordType type;
    public new SpriteRenderer renderer;
    public new BoxCollider2D collider;
    public int damage = 1;
    public int hitsPerSwing = 1;

    protected bool isSwinging;
    protected bool canDamage;
    protected int remainingHits;

    protected bool IsInWall
    {
        get
        {
            return collider.IsTouchingLayers(type.swingInterruptionLayers);
        }
    }

    private IEnumerator currentAttack;
    private bool validAttack;
    private Vector2 cursorDirection;
    private List<Func<Vector2, IEnumerator>> attackMethods = new List<Func<Vector2, IEnumerator>>();
    private int attackIndex;
    
    private void Start()
    {
        attackMethods = new List<Func<Vector2, IEnumerator>>() { TryClockwiseSlash, TryCounterClockwiseSlash, TryStab };
    }

    public void AttemptSwing(Vector2 direction)
    {
        if (!isSwinging)
        {
            remainingHits = hitsPerSwing;
            cursorDirection = direction;
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
        yield return AttackIfValid(Quaternion.Euler(0, 0, type.attackRadius / 2) * cursorDirection);
    }

    private IEnumerator TryCounterClockwiseSlash(Vector2 cursorDirection)
    {
        currentAttack = Slash(false);
        yield return AttackIfValid(Quaternion.Euler(0, 0, -type.attackRadius / 2) * cursorDirection);
    }

    private IEnumerator TryStab(Vector2 cursorDirection)
    {
        currentAttack = Stab();
        yield return AttackIfValid(cursorDirection);
    }

    private IEnumerator AttackIfValid(Vector2 direction)
    {
        transform.parent.up = direction;
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
        canDamage = false;
        renderer.enabled = false;
        ResetRestingPosition();

        if (type.attackLatency > 0)
            yield return new WaitForSeconds(type.attackLatency);

        isSwinging = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (renderer.enabled && collider.IsTouchingLayers(type.swingInterruptionLayers))
        {
            StopCoroutine(currentAttack);
            StartCoroutine(EndSwing());
        }
        else if (canDamage)
        {
            if (remainingHits > 0)
            {
                if (collision.CompareTag("Enemy"))
                {
                    //TODO damage enemy
                    remainingHits--;
                }
                //TODO break crates
                //else if (collision.CompareTag("Crate"))
                //{

                //}
            }
        
        }
    }

    public void ResetRestingPosition()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    public void Hit()
    {
        remainingHits--;
    }
}
