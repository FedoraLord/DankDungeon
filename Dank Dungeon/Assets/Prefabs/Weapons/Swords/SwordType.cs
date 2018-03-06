using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SwordType : ScriptableObject {

    public float attackRadius = 60;
    public float attackSpeed = 10;
    public float attackLatency = 1;
    public float chargeTime = 0.5f;
    public float chargeOutTime = 0.5f;
}
