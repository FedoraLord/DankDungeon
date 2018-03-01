using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour {

    public static float Z_OffsetLevel3D = 10;

    private static GameObject _level3D;
    public static GameObject Level3D
    {
        get { return _level3D; }        
    }

    private static Transform _player3DPosition;
    public static Vector3 Player3DPosition
    {
        get { return _player3DPosition.position; }
    }

    void Start()
    {
        _level3D = GameObject.FindGameObjectWithTag("Level3D");
        _player3DPosition = GameObject.FindGameObjectWithTag("Player3DPosition").transform;
    }
    
}
