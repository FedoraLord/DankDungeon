using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Something : MonoBehaviour {

    public GameObject prefab;
    public GameObject objectReference;

    public void DoThing()
    {
        objectReference = Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
