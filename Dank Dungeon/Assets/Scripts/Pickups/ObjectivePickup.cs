using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePickup : MonoBehaviour {

    public static List<ObjectivePickup> pickups;

	void Start () {
        if (pickups == null)
            pickups = new List<ObjectivePickup>();
        pickups.Add(this);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickups.Remove(this);
            Destroy(gameObject);

            if (pickups.Count <= 0)
            {
                GameController.Win();
            }
        }
    }
}
