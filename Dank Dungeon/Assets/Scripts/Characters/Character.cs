using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    public bool takeFireDamage;
    public bool takePitDamage;
    public bool takePoisonDamage;
    public BoxCollider2D mainCollider;
    public LayerMask environmentalDamage;
    public List<Transform> environmentOverlapPoints;
    public Rigidbody2D body;
    public GameObject animationObject;

    [NonSerialized]
    public Animator anim;
    [NonSerialized]
    public Vector2 lastValidPosition;

    protected bool IsFalling { get; private set; }

    public int lavaDamage;
    public int effectTimer = 10;
    public int poisonDamage;

    private bool isTakingEnvironmentDamage;
    private int lavaContacts = 1;
    private int pitContacts = 4;
    private int poisonContacts = 1;
    private Vector2 previousValidPosition;
    private int coughCounter = 0;

    protected void InitializeCharacter()
    {
        if (takeFireDamage || takePitDamage || takePoisonDamage)
        {
            StartCoroutine(UpdateLastValidPosition());
            StartCoroutine(CheckCollisions());
        }
    }

    private IEnumerator UpdateLastValidPosition()
    {
        while (true)
        {
            if (!isTakingEnvironmentDamage)
            {
                previousValidPosition = lastValidPosition;
                lastValidPosition = transform.position;
            }
            else
            {
                lastValidPosition = previousValidPosition;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator CheckCollisions()
    {
        while (true)
        {
            Collider2D damageFrom = null;
            isTakingEnvironmentDamage = false;

            if (takePitDamage && (damageFrom = GetDamagingCollider("Pit", pitContacts)))
            {
                isTakingEnvironmentDamage = true;
                FallInPit(damageFrom);
            }

            if (takeFireDamage && (damageFrom = GetDamagingCollider("Lava", lavaContacts)))
            {
                isTakingEnvironmentDamage = true;
                Lava(lavaDamage);
            }

            if (takePoisonDamage && (damageFrom = GetDamagingCollider("Poison", poisonContacts)))
            {
                isTakingEnvironmentDamage = true;
                Poison(poisonDamage);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private Collider2D GetDamagingCollider(string tagFilter, int minContacts)
    {
        int numContacts = 0;
        Collider2D damageFrom = null;
        for (int i = 0; i < environmentOverlapPoints.Count; i++)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(environmentOverlapPoints[i].position, environmentalDamage);
            var environmentColliders = colliders.Where(x => x.CompareTag(tagFilter));
            if (environmentColliders.Any())
            {
                numContacts++;
                damageFrom = environmentColliders.FirstOrDefault();
            }
        }

        if (numContacts >= minContacts)
        {
            return damageFrom;
        }
        return null;
    }

    private void FallInPit(Collider2D pit)
    {
        if (!IsFalling)
        {
            IsFalling = true;
            FallInPit_Start();
            StartCoroutine(Falling());
        }
    }

    private IEnumerator Falling()
    {
        float endTime = Time.time + 0.5f;
        while (Time.time < endTime)
        {
            float reduceBy = 0.05f;
            Vector2 scale = (Vector2)transform.localScale - Vector2.one * reduceBy;

            if (scale.x <= reduceBy || scale.y <= reduceBy)
                break;

            transform.localScale = (Vector2)transform.localScale - Vector2.one * reduceBy;
            body.AddForce(-body.velocity * 8);
            yield return new WaitForFixedUpdate();
        }
        FallInPit_End();
        IsFalling = false;
    }

    private IEnumerator Lava(int damage)
    {
        float timer = 0;

        if(timer >= effectTimer)
        {
            timer -= effectTimer;
            // Deal Damage... Probs make it a override in PlayerController
        }
        timer += Time.deltaTime;
        yield return new WaitForFixedUpdate();
    }

    private IEnumerator Poison(int damage)
    {
        float timer = 0;
        
        //switch(coughCounter)
        //{
        //    case 0:
        //        PlayCoughOne();
        //        break;
        //    case 1:
        //        PlayCoughTwo();
        //        break;
        //    case 2:
        //        PlayCoughThree();
        //        break;
        //}

        if (timer >= effectTimer)
        {
            timer -= effectTimer;
            // Deal Damage... Probs make it a override in PlayerController
        }
        timer += Time.deltaTime;
        yield return new WaitForFixedUpdate();
    }

    private void PlayCoughOne()
    {
        if(!GameController.PlayerCtrl.cough3Sound.isPlaying)
        {
            GameController.PlayerCtrl.cough1Sound.Play();
            coughCounter++;
        }
    }

    private void PlayCoughTwo()
    {
        if (!GameController.PlayerCtrl.cough1Sound.isPlaying)
        {
            GameController.PlayerCtrl.cough2Sound.Play();
            coughCounter++;
        }
    }

    private void PlayCoughThree()
    {
        if (!GameController.PlayerCtrl.cough2Sound.isPlaying)
        {
            GameController.PlayerCtrl.cough3Sound.Play();
            coughCounter++;
        }
    }


    protected abstract void FallInPit_Start();

    protected abstract void FallInPit_End();
}
