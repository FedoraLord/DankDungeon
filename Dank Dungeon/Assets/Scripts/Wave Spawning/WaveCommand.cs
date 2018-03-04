using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class WaveCommand{
    
    public int enemyIndex;
    //public int desiredQueueIndex;
    public enum CommandType { Spawning, Utility }
    public CommandType type;
    public enum UtilityCommand { WaitNSeconds, WaitUntilNAlive }
    public UtilityCommand utility;

    public int N { get { return n; } }

    private int n = 5;

    //public WaveCommand(int queueIndex)
    //{
    //    desiredQueueIndex = queueIndex;
    //}

    /// <summary>
    /// FOR THE LOVE OF GOD DO NOT USE THIS METHOD UNLESS YOU ARE WORKING IN THE EDITOR SCRIPT. You will overwrite the Wave scriptable object at runtime.
    /// </summary>
    /// <param name="n"></param>
    public void SetNFromEditorScript(int n)
    {
        this.n = n;
    }
}