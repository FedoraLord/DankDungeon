using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class EnvironmentalDamage : MonoBehaviour {

    public List<Transform> overlapPoints;
    public LayerMask damageLayer;
    public int minContacts = 1;

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
                TriggerEnvironmentalDamage(damageFrom);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    protected abstract void TriggerEnvironmentalDamage(Collider2D damageFrom);
}