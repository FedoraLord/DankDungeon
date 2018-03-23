using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : Character
{
    public static Transform lastRoom;

    public Transform weaponPivot;
    public Weapon weapon;
    public Transform topEnemyDetector;
    public Transform bottomEnemyDetector;
    public Transform leftEnemyDetector;
    public Transform rightEnemyDetector;
    public LayerMask enemyLayer;

    private bool hasControl = true;
    private float speed = 5;
    private float hordeMovementSpeed = 5;
    private int health;
    private int maxHealth = 100;
    private int damageFromPits = 20;
    private IEnumerator physicalDamageRoutine;
    
	void Start () {
        health = maxHealth;
        SetWeapon(weapon);
        StartCoroutine(UpdateLastValidPosition());
        Mirror mirror = GetComponent<Mirror>();
        mirror.mirror3D.GetComponent<NavMeshAgent>().Warp(mirror.Coordinates3D());
    }

    public void SetWeapon(Weapon newWeapon)
    {
        if (weapon == null)
        {
            throw new System.Exception("PlayerController needs a reference to a Weapon prefab!");
        }
        else
        {
            weapon = Instantiate(newWeapon, weaponPivot);
            weapon.SetSprite();
        }
    }

    void Update () {
        if (hasControl)
        {
            MovePlayer();
            Attack();
        }
    }

    private void MovePlayer()
    {
        Vector3 velocity = new Vector3();
        bool movingThroughEnemy = false;

        if (Input.GetKey(KeyCode.W))
        {
            //TODO these conditions will be "if(touching || !hasBluePotionEffect)"
            //also change mass to 100 or something on the rigidbody
            if (Physics2D.OverlapPoint(topEnemyDetector.position, enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += transform.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (Physics2D.OverlapPoint(leftEnemyDetector.position, enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += -transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (Physics2D.OverlapPoint(rightEnemyDetector.position, enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += -transform.up;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (Physics2D.OverlapPoint(bottomEnemyDetector.position, enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += transform.right;
        }

        if (velocity.magnitude > 0)
        {
            if (movingThroughEnemy)
                velocity = velocity.normalized * hordeMovementSpeed;
            else
                velocity = velocity.normalized * speed;
        }
        body.velocity = velocity;
    }

    private void Attack()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 direction = GameController.MainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            weapon.AttemptSwing(direction);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Room"))
        {
            lastRoom = collision.transform;
            GameController.Spawner.UpdateSpawnPoints();
        }
    }

    protected override void FallInPit_Start()
    {
        hasControl = false;
    }

    protected override void FallInPit_End()
    {
        StartCoroutine(TakePitDamage());

        hasControl = true;
        transform.localScale = Vector3.one;
        Vector2 awayFromPit = (Vector2)transform.position + (lastValidPosition - (Vector2)transform.position) * 2f;

        if (Physics2D.OverlapPoint(awayFromPit, environmentalDamage) == null)
        {
            transform.position = awayFromPit;
        }
        else
        {
            transform.position = lastValidPosition;
        }
    }

    public void TakePhysicalDamage(Enemy sender)
    {
        //TODO: anything special going to happen if its a fire slime or something?
        int damage = sender.damage;
        TakePhysicalDamage(damage);
    }

    public IEnumerator TakePitDamage()
    {
        yield return new WaitForSeconds(0.1f);
        TakePhysicalDamage(damageFromPits, true);
    }

    private void TakePhysicalDamage(int damage, bool interrupt = false)
    {
        if (interrupt)
        {
            if (physicalDamageRoutine != null)
            {
                StopCoroutine(physicalDamageRoutine);
                physicalDamageRoutine = null;
            }
        }

        if (physicalDamageRoutine == null)
        {
            //TODO: armor resistance
            health -= damage;
            physicalDamageRoutine = PhysicalDamageCooldown();
            StartCoroutine(physicalDamageRoutine);
        }
    }

    private IEnumerator PhysicalDamageCooldown()
    {
        float showTime = 0.5f;
        float hideTime = 0.2f;
        SpriteRenderer r = GetComponent<SpriteRenderer>();

        for (int blinks = 0; blinks < 3; blinks++)
        {
            r.enabled = false;
            yield return new WaitForSeconds(hideTime);
            r.enabled = true;
            yield return new WaitForSeconds(showTime);
        }

        physicalDamageRoutine = null;
    }
}
