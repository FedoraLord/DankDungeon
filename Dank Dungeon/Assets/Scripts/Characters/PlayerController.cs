using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : Character
{
    public static Transform lastRoom;

    public Transform weaponPivot;
    public Weapon weapon;
    public Transform topEnemyDetector;
    public Transform bottomEnemyDetector;
    public Transform leftEnemyDetector;
    public Transform rightEnemyDetector;
    public LayerMask enemyLayer;
    public AudioSource attackSound;
    public AudioSource walkSound;
    public AudioSource hitSound;
    public AudioSource craftingSound;
    public AudioSource drinkingSound;
    public AudioSource cough1Sound;
    public AudioSource cough2Sound;
    public AudioSource cough3Sound;
    public CraftingMenu menu;
    
    [NonSerialized]
    public Dictionary<CraftingMaterial, int> Inventory;

    private SpriteRenderer spriteR;
    private bool hasControl = true;
    private float speed = 5;
    private float hordeMovementSpeed = 1;
    private int health;
    private int maxHealth = 100;
    private int damageFromPits = 20;
    private int damageFromLava = 2;
    private int damageFromPoison = 5;
    public int burnTimer = 5;
    public int poisonTimer = 5;
    public int healingAmount;
    public int effectInvincTimer;
    public int physInvincTimer;
    public int damageUp = 10;
    public float potCooldown = 5;
    public bool canUsePotion = true;
    public bool isBluePotionActive = false;
    public bool isGreenPotionActive = false;
    private IEnumerator physicalDamageRoutine;
    private Vector2 enemyDetectorSize = new Vector2(0.5f, 0.1f);
    
	void Start () {
        InitializeCharacter();
        health = maxHealth;
        SetWeapon(weapon);
        Mirror mirror = GetComponent<Mirror>();
        mirror.mirror3D.GetComponent<NavMeshAgent>().Warp(mirror.Coordinates3D());
        anim = animationObject.GetComponent<Animator>();
        spriteR = animationObject.GetComponent<SpriteRenderer>();
    }

    public void SetWeapon(Weapon newWeapon)
    {
        if (weapon == null)
        {
            throw new Exception("PlayerController needs a reference to a Weapon prefab!");
        }
        else
        {
            weapon = Instantiate(newWeapon, weaponPivot);
            weapon.SetSprite();
        }
    }

    void Update () {
        if (!menu.IsOpen)
        {
            if (health <= 0)
            {
                SceneManager.LoadScene("Lose");
            }
            if (hasControl)
            {
                MovePlayer();
                Attack();
            }

            // Maybe put check to see if they have avaliable potions here?
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(RedPotion());
            if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(GreenPotion());
            if (Input.GetKeyDown(KeyCode.Alpha3))
                StartCoroutine(BluePotion());
            if (Input.GetKeyDown(KeyCode.Alpha4))
                StartCoroutine(YellowPotion(damageUp));           
        }
        else
        {
            GameController.PlayerCtrl.walkSound.Stop();
        }
    }

    private void MovePlayer()
    {
        Vector3 velocity = new Vector3();
        bool movingThroughEnemy = false;

        if (Input.GetKey(KeyCode.W))
        {
            if (Physics2D.OverlapBox(topEnemyDetector.position, enemyDetectorSize, 0, enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += transform.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (Physics2D.OverlapBox(leftEnemyDetector.position, enemyDetectorSize, 90, enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += -transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (Physics2D.OverlapBox(rightEnemyDetector.position, enemyDetectorSize, 90, enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += -transform.up;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (Physics2D.OverlapBox(bottomEnemyDetector.position, enemyDetectorSize, 0, enemyLayer))
            {
                movingThroughEnemy = true;
            }
            velocity += transform.right;
        }

        if (velocity.x < 0)
            spriteR.flipX = true;

        if (velocity.x > 0)
            spriteR.flipX = false;

        if (velocity.x == 0 && velocity.y == 0)
        {
            anim.SetInteger("Direction", 0);
        }
        else
            anim.SetInteger("Direction", 1);

        if (velocity.magnitude > 0)
        {
            if(!GameController.PlayerCtrl.walkSound.isPlaying)
                GameController.PlayerCtrl.walkSound.Play();

            if (movingThroughEnemy && !isBluePotionActive)
                velocity = velocity.normalized * hordeMovementSpeed;
            else
                velocity = velocity.normalized * speed;
        }
        else
        {
            GameController.PlayerCtrl.walkSound.Stop();
        }
         
        body.velocity = velocity;
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

    protected override void FallInPit_Start()
    {
        hasControl = false;
    }

    protected override void FallInPit_End()
    {
        StartCoroutine(TakePitDamage());

        hasControl = true;
        transform.localScale = Vector3.one;
        Vector2 awayFromPit = (Vector2)transform.position + (lastValidPosition - (Vector2)transform.position) * 2f;

        if (Physics2D.OverlapPoint(awayFromPit, environmentalDamage) == null)
        {
            transform.position = awayFromPit;
        }
        else
        {
            transform.position = lastValidPosition;
        }
    }

    public void TakePhysicalDamage(Enemy sender)
    {
        //TODO: anything special going to happen if its a fire slime or something?
        GameController.PlayerCtrl.hitSound.Play();
        int damage = sender.damage;
        TakePhysicalDamage(damage);
    }

    protected override void StandingInLava()
    {
        if (!isGreenPotionActive)
            StartCoroutine(TakeLavaDamage());
    }

    protected override void StandingInPoison()
    {
        if (!isGreenPotionActive)
            StartCoroutine(TakePoisonDamage());
    }

    public IEnumerator TakePitDamage()
    {
        yield return new WaitForSeconds(0.1f);
        TakePhysicalDamage(damageFromPits, true);
    }

    public IEnumerator TakeLavaDamage()
    {
        yield return new WaitForSeconds(3f);
        TakePhysicalDamage(damageFromLava, true);
    }

    public IEnumerator TakePoisonDamage()
    {
        yield return new WaitForSeconds(3f);
        TakePhysicalDamage(damageFromPoison, true);

    }

    private void TakePhysicalDamage(int damage, bool interrupt = false)
    {
        if (interrupt)
        {
            if (physicalDamageRoutine != null)
            {
                StopCoroutine(physicalDamageRoutine);
                physicalDamageRoutine = null;
            }
        }

        if (physicalDamageRoutine == null)
        {
            //TODO: armor resistance
            health -= damage;
            physicalDamageRoutine = PhysicalDamageCooldown();
            StartCoroutine(physicalDamageRoutine);
        }
    }

    private IEnumerator PhysicalDamageCooldown()
    {
        float showTime = 0.5f;
        float hideTime = 0.2f;
        SpriteRenderer r = animationObject.GetComponent<SpriteRenderer>();

        for (int blinks = 0; blinks < 3; blinks++)
        {
            r.enabled = false;
            yield return new WaitForSeconds(hideTime);
            r.enabled = true;
            yield return new WaitForSeconds(showTime);
        }

        physicalDamageRoutine = null;
    }

    private IEnumerator RedPotion()
    {
        canUsePotion = false;
       
        health = maxHealth;

        yield return new WaitForSeconds(potCooldown);

        canUsePotion = true;
    }

    private IEnumerator GreenPotion()
    {
        canUsePotion = false;
        isGreenPotionActive = true;
        
        yield return new WaitForSeconds(potCooldown);

        isGreenPotionActive = false;
        canUsePotion = true;
    }

    private IEnumerator BluePotion()
    {
        canUsePotion = false;
        isBluePotionActive = true;

        yield return new WaitForSeconds(potCooldown);

        canUsePotion = true;

        yield return new WaitForSeconds(3f);
    }

    private IEnumerator YellowPotion(int damage)
    {
        canUsePotion = false;
        int oldDamage = weapon.stats.damage;
        weapon.stats.damage += 10;

        yield return new WaitForSeconds(potCooldown);

        weapon.stats.damage = oldDamage;
        canUsePotion = true;
    }
}
