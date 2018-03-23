using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshAgentMover : MonoBehaviour {

    public Transform destination;
    public UnityEngine.AI.NavMeshAgent agent;

    void Update () {
        agent.SetDestination(destination.position);
	}
}
