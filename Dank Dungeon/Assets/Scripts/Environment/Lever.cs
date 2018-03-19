using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {
    
    public Barrier barrier;
    public Sprite triggeredSprite;

    private bool isTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") /*|| collision.CompareTag("Dagger")*/)
        {
            if (!isTriggered)
            {
                isTriggered = true;
                GetComponent<SpriteRenderer>().sprite = triggeredSprite;
                barrier.DisableBarrier();
            }
        }
    }
    
}
