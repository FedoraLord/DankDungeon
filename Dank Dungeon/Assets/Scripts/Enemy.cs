using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    public Mirror mirror;
    public int health = 5;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //pit
    }

    public void TakeDamage(Weapon wpn)
    {
        health -= wpn.damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameController.Spawner.EnemyDied(this);
        Destroy(gameObject);
    }
}
