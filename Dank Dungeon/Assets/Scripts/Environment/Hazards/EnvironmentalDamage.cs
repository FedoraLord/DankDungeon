using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EnvironmentalDamage : MonoBehaviour {

    public List<Transform> overlapPoints;
    public LayerMask damageLayer;
    public int minContacts = 1;
    [System.NonSerialized]
    public bool isTakingDamage;

    protected string tagFilter;

    protected IEnumerator CheckCollisions()
    {
        while (true)
        {
            int numContacts = 0;
            Collider2D damageFrom = null;
            for (int i = 0; i < overlapPoints.Count; i++)
            {
                Collider2D[] colliders = Physics2D.OverlapPointAll(overlapPoints[i].position, damageLayer);
                var environmentColliders = colliders.Where(x => x.CompareTag(tagFilter));
                if (environmentColliders.Any())
                {
                    numContacts++;
                    damageFrom = environmentColliders.FirstOrDefault();
                }
            }

            if (numContacts >= minContacts)
            {
                isTakingDamage = true;
                TriggerEnvironmentalDamage(damageFrom);
            }
            else
            {
                isTakingDamage = false;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    protected abstract void TriggerEnvironmentalDamage(Collider2D damageFrom);
}