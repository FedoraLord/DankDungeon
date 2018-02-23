using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mirror2D : MonoBehaviour {

    public GameObject bodyPrefab;

    public void CreateMirror()
    {
        GameObject body3D = Instantiate(bodyPrefab, CoordinatesTo3D(), Quaternion.identity, GameController.GetLevel3D().transform);
        Mirror3D mirror3D = body3D.GetComponent<Mirror3D>();
        if (mirror3D != null)
            mirror3D.mirror = gameObject;
        //tm.
    }
        public Tilemap tm;

    private Vector3 CoordinatesTo3D()
    {
        return transform.position + new Vector3(0, 0, GameController.Z_OffsetLevel3D);
    }
}
