using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Barrier
{
    public Direction expandingDirection;
    public enum Direction { X, Y }

    public override void DisableBarrier()
    {
        GameController.Spawner.UpdateValidRooms();
        StartCoroutine(Raise());
    }

    private IEnumerator Raise()
    {
        float raiseSpeed = 0.03f * PitScale;

        while ((PitScale = PitScale - raiseSpeed) > 0)
        {
            yield return new WaitForEndOfFrame();
        }

        //TODO temporarily change mirror state of enemies on these meshes if they jitter
        GameController.GameCtrl.RebuildNavMeshes();

        Destroy(gameObject);
    }

    private float PitScale
    {
        get
        {
            if (expandingDirection == Direction.X)
            {
                return transform.localScale.x;
            }
            else
            {
                return transform.localScale.y;
            }
        }

        set
        {
            Vector3 scale = transform.localScale;
            if (expandingDirection == Direction.X)
            {
                scale.x = value;
            }
            else
            {
                scale.y = value;
            }
            transform.localScale = scale;
        }
    }
}
