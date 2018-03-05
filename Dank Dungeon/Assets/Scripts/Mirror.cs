﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour {

    [Tooltip("Use this field if you don't want to instantiate a prefab at runtime.")]
    public GameObject mirror3D;
    public GameObject prefab3D;
    public string parentTagOrName;
    public FindBy findBy;
    public MirrorMode independentMover;
    public Vector2 positionOffset;
    
    public enum FindBy { Tag, Name }
    public enum MirrorMode { Object2D, Object3D, DontMirror }

    public GameObject GetMirror()
    {
        return mirror3D;
    }

    void Start() {
        if (mirror3D == null)
        {
            Transform parent;
            if (findBy == FindBy.Tag)
            {
                parent = GameObject.FindGameObjectWithTag(parentTagOrName).transform;
            }
            else
            {
                parent = GameObject.FindGameObjectWithTag("Level3D").transform.Find(parentTagOrName);
            }

            mirror3D = Instantiate(prefab3D, Coordinates3D(), Quaternion.identity, parent);
            mirror3D.transform.localScale = transform.localScale;
        }
	}

    private Vector3 Coordinates3D()
    {
        Vector3 pos = transform.position;
        pos.z = GameController.Z_OffsetLevel3D;
        pos += (Vector3)positionOffset;
        return pos;
    }
	
	void Update () {
		if (independentMover == MirrorMode.Object3D)
        {
            transform.position = (Vector2)mirror3D.transform.position;
        }
        else if (independentMover == MirrorMode.Object2D)
        {
            mirror3D.transform.position = Coordinates3D();
        }
	}

    public void Mirror3DObject()
    {
        independentMover = MirrorMode.Object3D;
    }

    public void Mirror2DObject()
    {
        independentMover = MirrorMode.Object2D;
    }

    private void OnDestroy()
    {
        Destroy(mirror3D);
    }
}
