using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    public Mirror mirror;
    public Rigidbody2D body;
    public int health = 5;

    private NavMeshAgent navigator;
    private IEnumerator knockbackRoutine;

	void Start () {
        GameObject obj = mirror.mirror3D;
        if (obj != null)
            navigator = obj.GetComponent<NavMeshAgent>();
        
	}
	
	void Update () {
        if (navigator == null)
            navigator = mirror.mirror3D.GetComponent<NavMeshAgent>();
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
        else
        {
            if (knockbackRoutine != null)
            {
                StopCoroutine(knockbackRoutine);
            }
            knockbackRoutine = Knockback(wpn.knockbackForce, wpn.knockbackTime);
            StartCoroutine(knockbackRoutine);
        }
    }

    public IEnumerator Knockback(float force, float duration)
    {
        Vector2 player = GameController.PlayerCtrl.transform.position;
        Vector2 enemy = transform.position;
        Vector2 direction = enemy - player;

        mirror.Mirror2DObject();
        body.velocity = (direction).normalized * force;

        yield return new WaitForSeconds(duration);

        mirror.Mirror3DObject();
    }

    void Die()
    {
        GameController.Spawner.EnemyDied(this);
        Destroy(gameObject);
    }

    protected override void FallInPit()
    {
        mirror.Mirror2DObject();
    }
}
