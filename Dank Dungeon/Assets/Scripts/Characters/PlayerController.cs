using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : Character
{
    public static Transform lastRoom;

    public Transform weaponPivot;
    public Weapon currentWeapon;
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
<<<<<<< HEAD
    public List<ActiveWeapon> allWeapons;
    public Dagger daggerPrefab;
    
=======
    public Text healthCounter;

    [NonSerialized]
    public Dictionary<CraftingMaterial, int> Inventory;

>>>>>>> 8f5de3111c9dd38a831bd2257bcb393a9b8fa503
    private SpriteRenderer spriteR;
    private bool hasControl = true;
    private float speed = 5;
    private float hordeMovementSpeed = 1;
    private int health;
    private int maxHealth = 100;
    private int damageFromPits = 20;
    private int damageFromLava = 1;
    private int damageFromPoison = 2;
    public int burnTimer = 3;
    public int poisonTimer = 3;
    public int healingAmount;
    public int effectInvincTimer;
    public int physInvincTimer;
    public int damageUp = 5;
    public float potCooldown = 5;
    public bool canUsePotion = true;
    public bool isBluePotionActive = false;
    private IEnumerator physicalDamageRoutine;
    private Vector2 enemyDetectorSize = new Vector2(0.5f, 0.1f);
    private int weaponIndex;
    
    [Serializable]
    public class ActiveWeapon
    {
        public bool isUnlocked;
        public Weapon weapon;
    }

	void Start () {
        InitializeCharacter();
        health = maxHealth;

        for (int i = 0; i < allWeapons.Count; i++)
        {
            allWeapons[i].weapon.SetSprite();
        }
        SetWeapon(allWeapons[0].weapon);

        Mirror mirror = GetComponent<Mirror>();
        mirror.mirror3D.GetComponent<NavMeshAgent>().Warp(mirror.Coordinates3D());
        anim = animationObject.GetComponent<Animator>();
        spriteR = animationObject.GetComponent<SpriteRenderer>();

        var a = GetWeapon(typeof(ShortSword));
        var b = currentWeapon.GetType();
        var c = allWeapons[0].weapon.GetType();
        var d = b == c;
    }

    public ActiveWeapon GetWeapon(Type weaponType)
    {
        return allWeapons.Where(x => x.weapon.GetType() == weaponType).FirstOrDefault();
    }

    public void SetWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
    }

    public void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentWeapon.Sheathe())
            {
                for (int i = (weaponIndex + 1) % allWeapons.Count; i != weaponIndex; i = (i + 1) % allWeapons.Count)
                {
                    if (allWeapons[i].isUnlocked)
                    {
                        weaponIndex = i;
                        currentWeapon = allWeapons[i].weapon;
                        break;
                    }
                }
            }
        }
    }

    void Update () {
        if (!menu.IsOpen)
        {
            healthCounter.text = "HEALTH: " + health;
            if (health <= 0)
            {
                SceneManager.LoadScene("Lose");
            }
            if (hasControl)
            {
                MovePlayer();
                Attack();
                SwitchWeapon();
                ThrowDagger();
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

    private void ThrowDagger()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (CraftingMenu.Instance.daggers.Number_inv > 0)
            {
                Vector2 cursorDirection = GameController.MainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                var a = Instantiate(daggerPrefab, null);
                a.transform.position = transform.position;
                a.transform.up = cursorDirection;
                CraftingMenu.Instance.daggers.Number_inv--;
            }
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
            currentWeapon.AttemptSwing(direction);           
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
<<<<<<< HEAD
        StartCoroutine(TakeLavaDamage(damageFromLava, burnTimer));
    }

    protected override void StandingInPoison()
    {
        StartCoroutine(TakePoisonDamage());
=======
        Debug.Log("Reeeeee");
        StartCoroutine(TakeLavaDamage());
>>>>>>> 08038af7d3017e61ab57b37be926107ae67d4f2a
    }

    public IEnumerator TakePitDamage()
    {
        yield return new WaitForSeconds(0.1f);
        TakePhysicalDamage(damageFromPits, true);
    }

    public IEnumerator TakeLavaDamage(int damageAmount, int duration)
    {
        float timer;
        bool isRunning = false;

        if (isRunning == false)
        {
            isRunning = true;
            do
            {
                if (isGreenPotionActive)
                {
                    isRunning = false;
                    break;
                }
                else
                {
                    timer = Time.time + burnTimer;
                    health -= damageFromLava;
                    Debug.Log(health);
                    yield return new WaitForSeconds(3);
                    isRunning = false;
                }
            }
            while (timer > Time.time);
        }
    }

    public IEnumerator TakePoisonDamage()
    {
<<<<<<< HEAD
        float timer;

        do
        {
            if (isGreenPotionActive)
                break;
            else
            {
                timer = Time.time + poisonTimer;
                health -= damageFromPoison;
                yield return new WaitForSeconds(3f);
            }
        }
        while (timer > Time.time);
=======
        yield return new WaitForSeconds(3f);
        TakePhysicalDamage(damageFromPoison, true);
>>>>>>> 08038af7d3017e61ab57b37be926107ae67d4f2a
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
        // Null Status Effects
        yield return new WaitForSeconds(potCooldown);

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
        int oldDamage = currentWeapon.stats.damage;
        currentWeapon.stats.damage += 10;

        yield return new WaitForSeconds(potCooldown);

        currentWeapon.stats.damage = oldDamage;
        canUsePotion = true;
    }
}
