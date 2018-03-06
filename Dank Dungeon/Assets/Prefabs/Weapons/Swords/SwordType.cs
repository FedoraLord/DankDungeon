using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SwordType : ScriptableObject {

    public float attackRadius = 90;
    public float attackSpeed = 20;
    public float attackLatency = 0.25f;
    public float chargeInTime = 0.1f;
    public float chargeOutTime = 0.1f;
    public float stabDistance = 1;
    public float stabSpeed = 0.05f;
    public LayerMask swingInterruptionLayers;
}
