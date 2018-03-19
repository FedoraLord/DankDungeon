using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Barrier
{
    public SpriteRenderer spriteRenderer;

    private float raiseHeight = 1;
    private float raiseSpeed = 0.03f;

    public override void DisableBarrier()
    {
        Destroy(GetComponent<Mirror>());
        GameController.Spawner.UpdateValidRooms();
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(Raise());
    }

    private IEnumerator Raise()
    {
        float stopAt = transform.position.y + raiseHeight;

        while (transform.position.y < stopAt)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + raiseSpeed);

            Color color = spriteRenderer.color;
            color.a = color.a - raiseSpeed;
            spriteRenderer.color = color;

            yield return new WaitForEndOfFrame();
        }
    }
}
