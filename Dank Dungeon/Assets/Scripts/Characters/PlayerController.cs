using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : Character
{
    public static Transform lastRoom;

    public bool hasControl = true;
    public float speed = 5;
    public float hordeMovementSpeed = 2;
    public int health;
    public int maxHealth;
    public Transform weaponPivot;
    public Weapon weapon;
    public Transform topEnemyDetector;
    public Transform bottomEnemyDetector;
    public Transform leftEnemyDetector;
    public Transform rightEnemyDetector;
    public LayerMask enemyLayer;
    
	void Start () {
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
        hasControl = true;
        //Take Damage
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
}
