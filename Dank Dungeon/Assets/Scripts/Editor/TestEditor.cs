using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Something))]
public class TestEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Something myScript = (Something)target;
        if (GUILayout.Button("Do Thing"))
        {
            if (myScript.objectReference == null)
                myScript.DoThing();
            else
                Debug.Log("Thing already done");
        }
    }
}
