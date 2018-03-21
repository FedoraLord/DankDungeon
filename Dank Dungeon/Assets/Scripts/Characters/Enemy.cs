using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    public Mirror mirror;
    public int health = 5;
    public int damage;
    public float attackRange;
    public float attackLatency;

    private bool isAttacking;
    private NavMeshAgent navigator;
    private IEnumerator knockbackRoutine;
    private IEnumerator attackRoutine;

    void Start()
    {
        GameObject obj = mirror.mirror3D;
        if (obj != null)
            navigator = obj.GetComponent<NavMeshAgent>();

    }

    void Update () {
        if (navigator == null)
            navigator = mirror.mirror3D.GetComponent<NavMeshAgent>();

        if (IsAtDestination())
        {
            StartAttacking();
        }
        else
        {
            navigator.SetDestination(GameController.Player3DTransform.position);
        }
    }

    private bool IsAtDestination()
    {
        if (navigator.hasPath && navigator.pathStatus == NavMeshPathStatus.PathComplete && !navigator.pathPending)
        {
            if (navigator.remainingDistance <= attackRange)
            {
                return true;
            }
        }
        return false;
    }

    private void StartAttacking()
    {
        if (!isAttacking)
        {
            attackRoutine = Attack();
            StartCoroutine(attackRoutine);
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        bool hitPlayer = false;
        mirror.Mirror2DObject();
        PlayerController player = GameController.PlayerCtrl;
        
        while (!hitPlayer)
        {
            body.velocity = (player.transform.position - transform.position).normalized * navigator.speed * 2;
            yield return new WaitForEndOfFrame();
            hitPlayer = mainCollider.IsTouching(player.mainCollider);
        }

        mirror.Mirror3DObject();
        yield return new WaitForSeconds(attackLatency);
        isAttacking = false;
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {

    //    }
    //}

    public void TakeDamage(Weapon wpn)
    {
        health -= wpn.stats.damage;
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
            knockbackRoutine = Knockback(wpn.stats.knockbackForce, wpn.stats.knockbackTime);
            StartCoroutine(knockbackRoutine);
        }
    }

    public IEnumerator Knockback(float force, float duration)
    {
        Vector2 player = GameController.PlayerCtrl.transform.position;
        Vector2 enemy = transform.position;
        Vector2 direction = enemy - player;

        mirror.Mirror2DObject();
        body.velocity = direction.normalized * force;

        yield return new WaitForSeconds(duration);

        mirror.Mirror3DObject();
    }

    void Die()
    {
        GameController.Spawner.EnemyDied(this);
        Destroy(gameObject);
    }

    protected override void FallInPit_Start()
    {
        mirror.Mirror2DObject();
        StopCoroutine(knockbackRoutine);
    }

    protected override void FallInPit_End()
    {
        Die();
    }
}
