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
            for (int i = 0; i < overlapPoints.Count; i++)
            {
                Collider2D[] colliders = Physics2D.OverlapPointAll(overlapPoints[i].position, damageLayer);
                if (colliders.Where(x => x.CompareTag(tagFilter)).Any())
                {
                    numContacts++;
                }
            }

            if (numContacts >= minContacts)
            {
                TriggerEnvironmentalDamage();
            }
            yield return new WaitForFixedUpdate();
        }
    }

    protected abstract void TriggerEnvironmentalDamage();
}