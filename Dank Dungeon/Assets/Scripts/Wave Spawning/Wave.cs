using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu]
public class Wave : ScriptableObject {
    
    public EnemyList registeredEnemies;
    public List<WaveCommand> commandQueue = new List<WaveCommand>();
    public float spawnRate = 0.5f;

    public Enemy[] Enemies
    {
        get
        {
            if (registeredEnemies == null)
                return new Enemy[0]; 
            return registeredEnemies.enemies;
        }
    }
}

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //DrawDefaultInspector();

        Wave wave = (Wave)target;
        wave.registeredEnemies = (EnemyList)EditorGUILayout.ObjectField("Enemy List Object", wave.registeredEnemies, typeof(EnemyList), true);
        wave.spawnRate = EditorGUILayout.FloatField("Spawn Rate", wave.spawnRate);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("Add Command"))
        {
            wave.commandQueue.Add(new WaveCommand());
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < wave.commandQueue.Count; i++)
        {
            WaveCommand command = wave.commandQueue[i];
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Command " + (i + 1));
            if (GUILayout.Button("Delete"))
            {
                wave.commandQueue.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            if (i > 0)
            {
                if (GUILayout.Button("Move Up"))
                {
                    Swap(wave.commandQueue, i, i - 1);
                }
            }
            if (i < wave.commandQueue.Count - 1)
            {
                if (GUILayout.Button("Move Down"))
                {
                    Swap(wave.commandQueue, i, i + 1);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            command.type = (WaveCommand.CommandType)EditorGUILayout.EnumPopup(command.type);

            EditorGUILayout.BeginHorizontal();
            command.n = EditorGUILayout.IntField(command.n);

            string[] commandList;
            if (command.type == WaveCommand.CommandType.Spawning)
            {
                commandList = wave.Enemies.Select(x => x.name).ToArray();
            }
            else
            {
                commandList = WaveSpawner.alternateWaveCommands.ToArray();
            }
            command.CurrentIndex = EditorGUILayout.Popup(command.CurrentIndex, commandList.ToArray());
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void Swap(List<WaveCommand> list, int i, int j)
    {
        WaveCommand tmp = list[i];
        list[i] = list[j];
        list[j] = tmp;
    }
}

//EditorGUILayout.BeginHorizontal();
//EditorGUILayout.EndHorizontal();

//EditorGUILayout.BeginVertical();
//EditorGUILayout.EndVertical();