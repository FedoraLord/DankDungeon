using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour {

    public Rigidbody2D rigid;

	void Start () {
        rigid.velocity = transform.up * 7;
        Invoke("KillMePlease", 3);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().Die();
            KillMePlease();
        }
    }

    private void KillMePlease()
    {
        Destroy(gameObject);
    }
}
