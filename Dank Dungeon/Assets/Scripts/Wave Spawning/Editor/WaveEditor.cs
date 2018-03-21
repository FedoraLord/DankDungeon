using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Wave wave = (Wave)target;
        if (wave.commandQueue.Count > 0)
        {
            if (wave.commandQueue.First().type == WaveCommand.CommandType.Utility)// || wave.commandQueue.Last().type == WaveCommand.CommandType.Utility)
            {
                EditorGUILayout.HelpBox("Utility commands at the beginning of the command queue will be ignored by the spawner.", MessageType.Warning);
            }
        }
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
            command.SetNFromEditorScript(EditorGUILayout.IntField(command.N));

            if (command.type == WaveCommand.CommandType.Spawning)
            {
                command.enemyIndex = EditorGUILayout.Popup(command.enemyIndex, wave.Enemies.Select(x => x.name).ToArray());
            }
            else
            {
                command.utility = (WaveCommand.UtilityCommand)EditorGUILayout.EnumPopup(command.utility);
            }

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