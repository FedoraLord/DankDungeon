using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitDamage : EnvironmentalDamage
{
    public Rigidbody2D body;
    public bool isFalling;

    void Start()
    {
        tagFilter = "Pit";
        StartCoroutine(CheckCollisions());
    }

    protected override void TriggerEnvironmentalDamage()
    {
        if (!isFalling)
        {
            isFalling = true;
            Vector2 lastVelocity = body.velocity;
            
        }
    }
}
