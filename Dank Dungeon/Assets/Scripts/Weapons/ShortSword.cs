using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortSword : Weapon {

    protected override IEnumerator Slash(bool clockwise)
    {
        renderer.enabled = true;
        remainingHits = 3;

        if (type.chargeInTime > 0)
            yield return new WaitForSeconds(type.chargeInTime);

        canDamage = true;
        for (float angleTraveled = 0; angleTraveled < type.attackRadius; angleTraveled += type.attackSpeed)
        {
            float rotation = type.attackSpeed;
            if (clockwise)
            {
                rotation *= -1;
            }

            transform.parent.Rotate(new Vector3(0, 0, 1), rotation);
            yield return new WaitForEndOfFrame();
        }
        canDamage = false;

        if (type.chargeOutTime > 0)
            yield return new WaitForSeconds(type.chargeOutTime);

        yield return EndSwing();
    }

    protected override IEnumerator Stab()
    {
        renderer.enabled = true;
        remainingHits = 3;

        transform.localPosition = new Vector3();

        if (type.chargeInTime > 0)
            yield return new WaitForSeconds(type.chargeInTime);

        canDamage = true;
        //thrust forward
        for (float distanceTraveled = 0; distanceTraveled < type.stabDistance; distanceTraveled += type.stabSpeed)
        {
            transform.localPosition = (transform.localPosition + new Vector3(0, type.stabSpeed, 0));
            yield return new WaitForEndOfFrame();
        }
        //come back
        for (float distanceTraveled = 0; distanceTraveled < type.stabDistance; distanceTraveled += type.stabSpeed)
        {
            transform.localPosition = (transform.localPosition + new Vector3(0, -type.stabSpeed, 0));
            yield return new WaitForEndOfFrame();
        }
        canDamage = false;

        if (type.chargeOutTime > 0)
            yield return new WaitForSeconds(type.chargeOutTime);

        yield return EndSwing();
    }
}
