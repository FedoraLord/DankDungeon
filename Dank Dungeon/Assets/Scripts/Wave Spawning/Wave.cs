using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Wave : ScriptableObject {
    
    public EnemyList registeredEnemies;
    [HideInInspector]
    public List<WaveCommand> commandQueue = new List<WaveCommand>();
    public float timeBeforeWave = 30;
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