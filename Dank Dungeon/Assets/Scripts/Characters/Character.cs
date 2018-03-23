using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    public Rigidbody2D body;
    public BoxCollider2D mainCollider;
    public LayerMask environmentalDamage;

    [System.NonSerialized]
    public Vector2 lastValidPosition;
    private Vector2 previousValidPosition;

    protected bool IsFalling { get; private set; }

    private EnvironmentalDamage[] envDamageScripts;

    protected IEnumerator UpdateLastValidPosition()
    {
        envDamageScripts = GetComponents<EnvironmentalDamage>();
        while (true)
        {
            if (!IsFalling && envDamageScripts.All(x => !x.isTakingDamage))
            {
                previousValidPosition = lastValidPosition;
                lastValidPosition = transform.position;
            }
            else
            {
                lastValidPosition = previousValidPosition;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void FallInPit(Collider2D pit)
    {
        if (!IsFalling)
        {
            IsFalling = true;
            FallInPit_Start();
            Vector2 lastVelocity = body.velocity;
            StartCoroutine(Falling());
        }
    }

    private IEnumerator Falling()
    {
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
        IsFalling = false;
    }
    
    protected abstract void FallInPit_Start();

    protected abstract void FallInPit_End();
}
