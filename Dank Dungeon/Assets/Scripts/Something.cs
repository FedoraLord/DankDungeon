using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Something : MonoBehaviour {

    public GameObject prefab;
    public GameObject objectReference;
    public NavMeshAgent navigation;
    public Transform destination;

    public void DoThing()
    {
        objectReference = Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        navigation.SetDestination(destination.position);
    }
}
