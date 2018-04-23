using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class WeaponStats : Craftable
{
    [Header("Weapon Stats")]
    public float attackRadius;
    public float attackSpeed;
    public float attackLatency;
    public float chargeInTime;
    public float chargeOutTime;
    public float stabDistance;
    public float stabSpeed;
    public float knockbackForce;
    public float knockbackTime;
    public int damage;
    public int hitsPerSwing;
    public LayerMask swingInterruptionLayers;
    public Sprite sprite;

    //protected override void OnCraft()
    //{
    //    GameController.PlayerCtrl.weapon.ApplyUpgrade(this);
    //}
}

#if UNITY_EDITOR
[CustomEditor(typeof(WeaponStats))]
public class WeaponStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WeaponStats copy = EditorGUILayout.ObjectField("Copy Values", null, typeof(WeaponStats), false) as WeaponStats;

        if (copy != null)
        {
            Undo.RegisterCompleteObjectUndo(target, "Copy WeaponStats");
            WeaponStats target1 = target as WeaponStats;
            target1.attackRadius = copy.attackRadius;
            target1.attackSpeed = copy.attackSpeed;
            target1.attackLatency = copy.attackLatency;
            target1.chargeInTime = copy.chargeInTime;
            target1.chargeOutTime = copy.chargeOutTime;
            target1.stabDistance = copy.stabDistance;
            target1.stabSpeed = copy.stabSpeed;
            target1.knockbackForce = copy.knockbackForce;
            target1.knockbackTime = copy.knockbackTime;
            target1.damage = copy.damage;
            target1.hitsPerSwing = copy.hitsPerSwing;
            target1.swingInterruptionLayers = copy.swingInterruptionLayers;
            target1.sprite = copy.sprite;
        }

        DrawDefaultInspector();
    }
}
#endif