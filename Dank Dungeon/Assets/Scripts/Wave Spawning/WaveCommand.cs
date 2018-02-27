using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class WaveCommand{
    
    public int n = 5;
    //public bool isSpawnCommand;
    public enum CommandType { Spawning, Utility }
    public CommandType type;

    public int CurrentIndex
    {
        get
        {
            if (type == CommandType.Spawning)
                return enemyIndex;
            return altIndex;
        }

        set
        {
            if (type == CommandType.Spawning)
                enemyIndex = value;
            else
                altIndex = value;
        }
    }

    private int enemyIndex;
    private int altIndex;
}