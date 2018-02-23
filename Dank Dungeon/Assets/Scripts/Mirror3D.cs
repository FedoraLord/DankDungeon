using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror3D : MonoBehaviour {

    public GameObject mirror;

    void Start()
    {
        if (mirror == null)
            Debug.Log(gameObject.name + " is missing a 2D mirror");
    }

    private void Update()
    {
        mirror.transform.position = (Vector2)transform.position;
    }
}
