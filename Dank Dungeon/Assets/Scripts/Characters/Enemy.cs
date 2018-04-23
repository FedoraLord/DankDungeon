using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    public Mirror mirror;
    public int health = 5;
    public int damage;
    public float speed;
    public float attackRange;
    public float attackLatency;
    public LayerMask attackLineOfSight;

    private bool isAttacking;
    private NavMeshAgent navigator;
    private IEnumerator knockbackRoutine;
    private IEnumerator attackRoutine;
    private SpriteRenderer spriteR;

    void Start()
    {
        InitializeCharacter();
        GameObject obj = mirror.mirror3D;
        if (obj != null)
            navigator = obj.GetComponent<NavMeshAgent>();
        StartCoroutine(SetAgentSpeed());
        anim = animationObject.GetComponent<Animator>();
        spriteR = animationObject.GetComponent<SpriteRenderer>();
    }

    private IEnumerator SetAgentSpeed()
    {
        yield return new WaitUntil(() => mirror.mirror3D != null);
        mirror.mirror3D.GetComponent<NavMeshAgent>().speed = speed;
    }

    void Update () {
        if (navigator == null)
            navigator = mirror.mirror3D.GetComponent<NavMeshAgent>();
        navigator.SetDestination(GameController.Player3DTransform.position);

        if (navigator.velocity.x < 0)
            spriteR.flipX = false;
        if (navigator.velocity.x > 0)
            spriteR.flipX = true;



        if (!IsFalling)
        {
            if (!isAttacking && IsAtDestination())
            {
                RaycastHit2D hit = Physics2D.RaycastAll(transform.position, (GameController.PlayerCtrl.transform.position - transform.position), attackRange, attackLineOfSight)
                    .Where(x => x.collider != mainCollider).OrderBy(x => x.distance).FirstOrDefault();
                if (hit.collider != null && hit.collider.gameObject == GameController.PlayerCtrl.gameObject)
                {
                    StartAttacking();
                }
            }
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
        if (knockbackRoutine == null)
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

        float attackTimeout = Time.time + 2;
        while (!hitPlayer && Vector2.Distance(player.transform.position, transform.position ) < attackRange + 0.1f && attackTimeout > Time.time)
        {
            body.velocity = (player.transform.position - transform.position).normalized * navigator.speed * 2;
            yield return new WaitForEndOfFrame();
            hitPlayer = mainCollider.IsTouching(player.mainCollider);
        }

        if (hitPlayer)
        {
            player.TakePhysicalDamage(this);
        }

        float backOffTime = Time.time;
        while (Vector2.Distance(player.transform.position, transform.position) < attackRange && Time.time < backOffTime + attackLatency)
        {
            body.velocity = (transform.position - player.transform.position).normalized * navigator.speed;
            yield return new WaitForFixedUpdate();
        }

        mirror.Mirror3DObject();
        float remainingLatency = attackLatency - (Time.time - backOffTime);
        if (remainingLatency > 0)
            yield return new WaitForSeconds(remainingLatency);
        isAttacking = false;
    }

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
        knockbackRoutine = null;
    }

    void Die()
    {
        GameController.Spawner.EnemyDied(this);
        Destroy(gameObject);
    }

    protected override void FallInPit_Start()
    {
        mirror.Mirror2DObject();
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }
    }

    protected override void FallInPit_End()
    {
        Die();
    }

    protected override void StandingInLava()
    {
        Die(); // for testing
    }
}
