using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour {

    public static float Z_OffsetLevel3D = 10;
    
    public static GameController GameCtrl { get; set; }
    public static WaveSpawner Spawner { get; set; }
    public static GameObject Level3D { get; set; }
    public static Transform Player3DTransform { get; set; }    

    void Start()
    {
        GameCtrl = this;
        Spawner = GetComponent<WaveSpawner>();
        Level3D = GameObject.FindGameObjectWithTag("Level3D");
        Player3DTransform = GameObject.FindGameObjectWithTag("Player3DPosition").transform;
    }
    
}
