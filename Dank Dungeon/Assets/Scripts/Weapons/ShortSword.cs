using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortSword : Weapon {

    protected override IEnumerator Slash(bool clockwise)
    {
        renderer.enabled = true;
        remainingHits = stats.hitsPerSwing;

        if (stats.chargeInTime > 0)
            yield return new WaitForSeconds(stats.chargeInTime);

        canDamage = true;
        for (float angleTraveled = 0; angleTraveled < stats.attackRadius; angleTraveled += stats.attackSpeed)
        {
            float rotation = stats.attackSpeed;
            if (clockwise)
            {
                rotation *= -1;
            }

            transform.parent.Rotate(new Vector3(0, 0, 1), rotation);
            yield return new WaitForEndOfFrame();
        }
        canDamage = false;

        if (stats.chargeOutTime > 0)
            yield return new WaitForSeconds(stats.chargeOutTime);

        yield return EndSwing();
    }

    protected override IEnumerator Stab()
    {
        renderer.enabled = true;
        remainingHits = 3;

        transform.localPosition = new Vector3();

        if (stats.chargeInTime > 0)
            yield return new WaitForSeconds(stats.chargeInTime);

        canDamage = true;
        //thrust forward
        for (float distanceTraveled = 0; distanceTraveled < stats.stabDistance; distanceTraveled += stats.stabSpeed)
        {
            transform.localPosition = (transform.localPosition + new Vector3(0, stats.stabSpeed, 0));
            yield return new WaitForEndOfFrame();
        }
        //come back
        for (float distanceTraveled = 0; distanceTraveled < stats.stabDistance; distanceTraveled += stats.stabSpeed)
        {
            transform.localPosition = (transform.localPosition + new Vector3(0, -stats.stabSpeed, 0));
            yield return new WaitForEndOfFrame();
        }
        canDamage = false;

        if (stats.chargeOutTime > 0)
            yield return new WaitForSeconds(stats.chargeOutTime);

        yield return EndSwing();
    }
}
