using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBirdBehaviour : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;

    [Header("AI")]
    public LOS birdLOS;

    [Header("Bullet Firing")]
    public Transform bulletSpawn;
    public float fireDelay;
    public PlayerBehaviour player;

    [Header("Abilities")]
    public int health;
    public BarController healthBar;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private AudioSource hitSound;
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        rigidbody2D = GetComponent<Rigidbody2D>();
        player = GameObject.FindObjectOfType<PlayerBehaviour>();
        hitSound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_hasLOS())
        {
            _FireBullet();
        }
    }

    private void _FireBullet()
    {
        //delay bullet firing
        if (Time.frameCount % fireDelay == 0 && BulletManager.Instance().HasBullets(PoolType.ENEMY))
        {
            var playerPosition = player.transform.position;
            var firingDirection = Vector3.Normalize(playerPosition - bulletSpawn.position);

            BulletManager.Instance().GetBullet(PoolType.ENEMY, bulletSpawn.position, firingDirection);
        }

    }

    private bool _hasLOS()
    {
        if (birdLOS.colliders.Count > 0)
        {
            if (birdLOS.collidesWith.gameObject.name == "Player" || birdLOS.colliders[0].gameObject.name == "Player")
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DeathPlane"))
        {
            Dead();
        }

        if (other.gameObject.CompareTag("SpikedBall"))
        {
            TakeDamage(50);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetValue(health);
        hitSound.Play();


        if (health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        player.score += 150;
        player.Score.SetText(": " + player.score);
        gameObject.SetActive(false);
    }
}
