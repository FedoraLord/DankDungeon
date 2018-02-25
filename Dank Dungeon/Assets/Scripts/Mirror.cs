using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour {

    public GameObject prefab3D;
    public Parent3D parent3D;
    public MirrorMode independentMover;

    public enum Parent3D { StaticObstacles, DynamicObstacles, EnemyBodies }
    public enum MirrorMode { Object2D, Object3D, DontMirror }

    private GameObject mirror3D;

    public GameObject GetMirror()
    {
        return mirror3D;
    }

    void Start() {
        GameObject obj = GameObject.FindWithTag("Level3D");
        Transform parent = null;
        switch (parent3D)
        {
            case Parent3D.StaticObstacles:
                parent = obj.transform.Find("Static Obstacles");
                break;
            case Parent3D.DynamicObstacles:
                parent = obj.transform.Find("Dynamic Obstacles");
                break;
            case Parent3D.EnemyBodies:
                parent = obj.transform.Find("Enemy Bodies");
                break;
        }

        if (parent == null)
        {
            parent = obj.transform;
        }
            
        mirror3D = Instantiate(prefab3D, Coordinates3D(), Quaternion.identity, parent);
        mirror3D.transform.localScale = transform.localScale;
	}

    private Vector3 Coordinates3D()
    {
        Vector3 pos = transform.position;
        pos.z = GameController.Z_OffsetLevel3D;
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
}
