using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPit : MonoBehaviour {

    public GameObject pit;
    public Transform point;
    public bool dewIt;
    
	void Update () {
		if (dewIt && pit != null)
        {
            Instantiate(pit, point.position, Quaternion.identity, null);
            dewIt = false;
        }
	}
}
