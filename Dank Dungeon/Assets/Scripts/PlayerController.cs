using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    public static Transform lastRoom;

    public float speed = 5;
    public float hordeMovementSpeed = 2;
    public Transform weaponPivot;
    public Weapon weapon;
    public BoxCollider2D top;
    public BoxCollider2D bottom;
    public BoxCollider2D left;
    public BoxCollider2D right;
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        SetWeapon(weapon);
    }

    public void SetWeapon(Weapon newWeapon)
    {
        weapon = Instantiate(newWeapon, weaponPivot);
    }

    void Update () {
        MovePlayer();
        Attack();
    }

    private void MovePlayer()
    {
        Vector3 velocity = new Vector3();
        bool movingThroughEnemy = false;

        if (Input.GetKey(KeyCode.W))
        {
            //TODO these conditions will be "if(touching || !hasBluePotionEffect)"
            if (top.IsTouchingLayers(enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += transform.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (left.IsTouchingLayers(enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += -transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (right.IsTouchingLayers(enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += -transform.up;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (bottom.IsTouchingLayers(enemyLayer))
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
        rb.velocity = velocity;
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
}
