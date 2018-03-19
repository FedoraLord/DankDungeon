using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitDamage : EnvironmentalDamage
{
    void Start()
    {
        tagFilter = "Pit";
        StartCoroutine(CheckCollisions());
    }

    protected override void TriggerEnvironmentalDamage(Collider2D damageFrom)
    {
        Character actor = GetComponent<Character>();
        if (actor != null)
        {
            actor.FallInPit(damageFrom);
        }
    }
}
