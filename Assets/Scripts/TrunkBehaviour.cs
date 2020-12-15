using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float runForce;
    public Rigidbody2D rigidbody2D;
    public Transform lookInFrontPoint;
    public Transform lookAheadPoint;
    public LayerMask collisionGroundLayer;
    public LayerMask collisionWallLayer;
    public bool isGroundAhead;

    [Header("AI")]
    public LOS trunkLOS;

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

        _LookInFront();
        _LookAhead();
        _Move();
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
        if (trunkLOS.colliders.Count > 0)
        {
            if (trunkLOS.collidesWith.gameObject.name == "Player" || trunkLOS.colliders[0].gameObject.name == "Player")
            {
                return true;
            }
        }
        return false;
    }

    private void _LookInFront()
    {
        var wallHit = Physics2D.Linecast(transform.position, lookInFrontPoint.position, collisionWallLayer);
        if (wallHit)
        {
            if (!wallHit.collider.CompareTag("Ramps"))
            {
                _FlipX();
            }
         
        }

        Debug.DrawLine(transform.position, lookInFrontPoint.position, Color.red);
    }

    private void _LookAhead()
    {
        var groundHit = Physics2D.Linecast(transform.position, lookAheadPoint.position, collisionGroundLayer);
        if (groundHit)
        {
            isGroundAhead = true;
        }
        else
        {
            isGroundAhead = false;
        }

        Debug.DrawLine(transform.position, lookAheadPoint.position, Color.green);
    }

    private void _Move()
    {
        if (isGroundAhead)
        {
            rigidbody2D.AddForce(Vector2.left * runForce * Time.deltaTime * transform.localScale.x);


            rigidbody2D.velocity *= 0.90f;
        }

        else
        {
            _FlipX();
        }

    }
    private void _FlipX()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
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
