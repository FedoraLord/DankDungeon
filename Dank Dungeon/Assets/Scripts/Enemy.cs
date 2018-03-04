using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    public Mirror mirror;

    private NavMeshAgent navigator;

	void Start () {
        GameObject obj = mirror.GetMirror();
        if (obj != null)
            navigator = obj.GetComponent<NavMeshAgent>();
        
	}
	
	void Update () {
        if (navigator == null)
            navigator = mirror.GetMirror().GetComponent<NavMeshAgent>();
        navigator.SetDestination(GameController.Player3DTransform.position);
	}

    void OnDestroy()
    {
        GameController.Spawner.EnemyDied(this);
    }
}
