using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    public static Transform lastRoom;

    public float speed = 5;
    public float hordeMovementSpeed = 2;
    public BoxCollider2D top;
    public BoxCollider2D bottom;
    public BoxCollider2D left;
    public BoxCollider2D right;
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Mirror>().mirror3D.GetComponent<NavMeshAgent>().isStopped = true;
    }

    void Update () {
        MovePlayer();
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Room"))
        {
            lastRoom = collision.transform;
            GameController.Spawner.UpdateSpawnPoints();
        }
    }
}
