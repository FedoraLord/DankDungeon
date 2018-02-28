using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb;
    private float speed = 10;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKey)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                rb.velocity = new Vector2(0, 1) * speed;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                rb.velocity = new Vector2(-1, 0) * speed;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                rb.velocity = new Vector2(0, -1) * speed;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                rb.velocity = new Vector2(1, 0) * speed;
            }
        }
        else
        {
            rb.velocity = new Vector2();
        }
		
    }
}
