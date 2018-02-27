using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class EnemyList : ScriptableObject {

    public List<Enemy> enemies;
    public static List<Enemy> globalEnemies = new List<Enemy>();
}

[CustomEditor(typeof(EnemyList))]
public class EnemyListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //if (GUILayout.Button("Register As Global"))
        //{
        EnemyList enemyList = (EnemyList)target;
        EnemyList.globalEnemies = enemyList.enemies;
        //}
    }
} 