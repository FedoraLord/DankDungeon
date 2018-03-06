using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public SwordType type;
    public float damage = 1;
    public int hitsPerSwing = 1;

    private bool isSwinging;
    private bool canDamage;
    private int remainingHits;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Swing(Vector2 direction)
    {
        if (!isSwinging)
        {
            Vector2 directionFrom = Quaternion.Euler(0, 0, type.attackRadius / 2) * direction;
            gameObject.SetActive(true);
            remainingHits = 3;
            StartCoroutine(Co_Swing(directionFrom));
        }
    }

    private IEnumerator Co_Swing(Vector2 from)
    {
        isSwinging = true;
        transform.parent.up = from;

        if (type.chargeTime > 0)
            yield return new WaitForSeconds(type.chargeTime);

        canDamage = true;
        for (float angleTraveled = 0; angleTraveled < type.attackRadius; angleTraveled += type.attackSpeed)
        {
            transform.parent.Rotate(new Vector3(0, 0, 1), -type.attackSpeed);
            yield return new WaitForEndOfFrame();
        }
        canDamage = false;

        if (type.chargeOutTime > 0)
            yield return new WaitForSeconds(type.chargeOutTime);

        gameObject.SetActive(false);
        isSwinging = false;

        if (type.attackLatency > 0)
            yield return new WaitForSeconds(type.attackLatency);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (remainingHits > 0)
        {
            if (collision.CompareTag("Enemy"))
            {
                remainingHits--;
                //TODO damage enemy
            }
            //TODO break crates
            //else if (collision.CompareTag("Crate"))
            //{

            //}
        }
        
        //TODO if collision is the wall, stop the swing, maybe add a spark where it hit the wall
    }

}
