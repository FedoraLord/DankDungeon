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
    public Vector2 customScale;
    public bool scaleWithSpriteRendererSize;

    public enum FindBy { Tag, Name }
    public enum MirrorMode { Object2D, Object3D, DontMirror }

    private SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();

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

            if (scaleWithSpriteRendererSize)
            {
                mirror3D.transform.localScale = sr.bounds.extents * 2;
            }
            else
            { 
                if (customScale == default(Vector2))
                {
                    mirror3D.transform.localScale = transform.localScale;
                }
                else
                {
                    mirror3D.transform.localScale = (Vector3)customScale + new Vector3(0, 0, 1);
                }
            }
        }
	}

    public Vector3 Coordinates3D()
    {
        if (scaleWithSpriteRendererSize)
        {
            return new Vector3(sr.bounds.center.x, sr.bounds.center.y, transform.position.z + GameController.Z_OffsetLevel3D);
        }
        else
        {
            Vector3 pos = transform.position;
            pos.z = GameController.Z_OffsetLevel3D;
            pos += (Vector3)positionOffset;
            return pos;
        }
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

    public void DontMirror()
    {
        independentMover = MirrorMode.DontMirror;
    }

    private void OnDestroy()
    {
        Destroy(mirror3D);
    }
}
