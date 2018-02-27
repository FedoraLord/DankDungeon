using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Wave : ScriptableObject {
    
    public List<WaveCommand> commandQueue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{ 
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        Wave wave = (Wave)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {

        }
        if (GUILayout.Button("Remove"))
        {

        }
        EditorGUILayout.EndHorizontal();

        List<string> commandList = EnemyList.globalEnemies.Select(x => x.name).ToList();
        commandList.Add("Wait N Seconds");
        commandList.Add("Wait Until N Alive");

        foreach (WaveCommand command in wave.commandQueue)
        {
            command.index = EditorGUILayout.Popup("Command", command.index, commandList.ToArray());
        }
    }
}