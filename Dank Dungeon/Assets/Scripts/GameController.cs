using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static float Z_OffsetLevel3D = 10;

    private static GameObject level3D;

    public static GameObject GetLevel3D()
    {
        if (level3D == null)
            level3D = GameObject.FindGameObjectWithTag("Level3D");
        return level3D;
    }

}
