using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnStart : MonoBehaviour {

    public Behaviour[] enable;

	void Start () {
        foreach (Behaviour b in enable)
        {
            b.enabled = true;
        }
	}
}
