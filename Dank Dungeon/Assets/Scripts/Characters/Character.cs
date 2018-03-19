using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    public Rigidbody2D body;
    public BoxCollider2D mainCollider;
    public LayerMask environmentalDamage;

    protected Vector2 lastValidPosition;

    private bool isFalling;

    protected IEnumerator UpdateLastValidPosition()
    {
        while (true)
        {
            if (!mainCollider.IsTouchingLayers(environmentalDamage))
            {
                lastValidPosition = transform.position;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void FallInPit(Collider2D pit)
    {
        if (!isFalling)
        {
            isFalling = true;
            FallInPit_Start();
            Vector2 lastVelocity = body.velocity;
            Vector2 fallTo = (Vector2)transform.position + lastVelocity.normalized;
            StartCoroutine(Falling());
        }
    }

    private IEnumerator Falling()
    {
        Debug.Log("falling");
        float endTime = Time.time + 0.5f;
        while (Time.time < endTime)
        {
            float reduceBy = 0.05f;
            Vector2 scale = (Vector2)transform.localScale - Vector2.one * reduceBy;

            if (scale.x <= reduceBy || scale.y <= reduceBy)
                break;

            transform.localScale = (Vector2)transform.localScale - Vector2.one * reduceBy;
            body.AddForce(-body.velocity * 8);
            yield return new WaitForFixedUpdate();
        }
        FallInPit_End();
        isFalling = false;
    }
    
    protected abstract void FallInPit_Start();

    protected abstract void FallInPit_End();
}
