using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 10;

    private Rigidbody2D rb;
    private Transform lastRoom;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        MovePlayer();
		
    }

    private void MovePlayer()
    {
        Vector3 velocity = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            velocity += transform.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity += -transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity += -transform.up;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity += transform.right;
        }

        if (velocity.magnitude > 0)
        {
            velocity = velocity.normalized * speed;
        }
        rb.velocity = velocity;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Room"))
        {
            lastRoom = collision.transform;
        }
    }
}
