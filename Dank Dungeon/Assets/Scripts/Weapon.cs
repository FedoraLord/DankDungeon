using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    public SwordType type;
    public new SpriteRenderer renderer;
    public new BoxCollider2D collider;
    public float damage = 1;
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
    private List<AttackDirection> attacks = new List<AttackDirection>();
    private int attackIndex;
    
    private class AttackDirection
    {
        public IEnumerator attackRoutine;
        private Func<Vector2> directionAccessor;

        public AttackDirection(IEnumerator attack, Func<Vector2> getDirection)
        {
            attackRoutine = attack;
            directionAccessor = getDirection;
        }

        public Vector2 GetDirection()
        {
            return directionAccessor();
        }
    }

    private Vector2 GetClockwiseSlashDirection()
    {
        return Quaternion.Euler(0, 0, type.attackRadius / 2) * cursorDirection;
    }

    private Vector2 GetCounterClockwiseSlashDirection()
    {
        return Quaternion.Euler(0, 0, -type.attackRadius / 2) * cursorDirection;
    }

    private Vector2 GetCursorDirection()
    {
        return cursorDirection;
    }

    private void Start()
    {
        attacks.Add(new AttackDirection(Slash(true), GetClockwiseSlashDirection));
        attacks.Add(new AttackDirection(Slash(false), GetCounterClockwiseSlashDirection));
        attacks.Add(new AttackDirection(Stab(), GetCursorDirection));
    }

    public void AttemptSwing(Vector2 direction)
    {
        if (!isSwinging)
        {
            StartCoroutine(FindValidSwing(direction));
        }
    }

    private IEnumerator FindValidSwing(Vector2 direction)
    {
        isSwinging = true;
        validAttack = false;
        
        //TODO randomize which slash is first
        for (int i = 0; i < attacks.Count && !validAttack; i++)
        {
            int index = (attackIndex + i) % attacks.Count;
            currentAttack = attacks[index].attackRoutine;
            yield return AttackIfValid(attacks[index].GetDirection());
        }
        //currentAttack = Slash(true);
        //yield return AttackIfValid(Quaternion.Euler(0, 0, type.attackRadius / 2) * direction);

        //if (!validAttack)
        //{
        //    currentAttack = Slash(false);
        //    yield return AttackIfValid(Quaternion.Euler(0, 0, -type.attackRadius / 2) * direction);
        //}

        //if (!validAttack)
        //{
        //    currentAttack = Stab();
        //    yield return AttackIfValid(direction);
        //}

        if (!validAttack)
        {
            isSwinging = false;
        }
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

}
