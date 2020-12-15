using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

[System.Serializable]
public enum ImpulseSounds
{
    JUMP,
    HIT,
    DIE,
    THROW
}

public enum PlayerState { GROUNDED, RUNNING, JUMPING };
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Controls")]
    public Joystick joystick;
    public float joystickHorizontalSensitivity;
    public float joystickVerticalSensitivity;
    public float horizontalForce;
    public float verticalForce;

    [Header("Platform Detection")]
    public PlayerState state;
    public Transform spawnPoint;
    public Transform lookBelowPoint;
    public LayerMask collisionGroundLayer;
    public LayerMask collisionWallLayer;

    [Header("Player Abilities")]
    public int health = 100;
    public int lives;
    public int score;
    public float throwRate;
    public Transform spikeBallSpawn;
    public BarController healthBar;

    [Header("Impulse Sounds")]
    public AudioSource[] sounds;

    [Header("Special FX")]
    public CinemachineVirtualCamera vCam;
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeTime;
    public float maxShakeTime;
    public float shakeIntensity;
    public bool isCameraShaking = false;

    [Header("HUD")]
    public TMPro.TextMeshProUGUI Lives;
    public TMPro.TextMeshProUGUI Score;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private RaycastHit2D groundHit;
    private ParticleSystem dustTrail;
    

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        lives = 5;
        score = 0;
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        dustTrail = GetComponentInChildren<ParticleSystem>();
        vCam = FindObjectOfType<CinemachineVirtualCamera>();
        perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lookBelow();
        Move();

        if (isCameraShaking)
        {
            shakeTime -= Time.deltaTime;
            if(shakeTime <= 0.0)
            {
                perlin.m_AmplitudeGain = 0.0f;
                isCameraShaking = false;
                shakeTime = maxShakeTime;
            }
        }
    }

    void lookBelow()
    {
        groundHit = groundHit = Physics2D.CircleCast(transform.position - new Vector3(0.0f, 0.65f, 0.0f), 0.4f, Vector2.down, 0.4f, collisionGroundLayer);

        if (groundHit)
        {
            state = PlayerState.GROUNDED;
        }
    }

    void Move()
    {
        if (state == PlayerState.GROUNDED || state == PlayerState.RUNNING)
        {
            if (joystick.Horizontal > joystickHorizontalSensitivity)
            {
                // move right
                rigidBody2D.AddForce(Vector2.right * horizontalForce * Time.deltaTime);
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                state = PlayerState.RUNNING;
                CreateDustTrail();
            }

            else if (joystick.Horizontal < -joystickHorizontalSensitivity)
            {
                // move left
                rigidBody2D.AddForce(Vector2.left * horizontalForce * Time.deltaTime);
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                state = PlayerState.RUNNING;
                CreateDustTrail();
            }

            else
            {
                state = PlayerState.GROUNDED;
            }

            if ((joystick.Vertical > joystickVerticalSensitivity))
            {
                // jump
                state = PlayerState.JUMPING;
                rigidBody2D.AddForce(Vector2.up * verticalForce);
                sounds[(int)ImpulseSounds.JUMP].Play();
                CreateDustTrail();
                ShakeCamera();
            }
            else if ((joystick.Vertical < -joystickVerticalSensitivity && state == PlayerState.GROUNDED))
            {
                //delay bullet firing
                if (Time.frameCount % 10 == 0 && BulletManager.Instance().HasBullets(PoolType.PLAYER))
                {
                    var firingDirection = Vector3.up + Vector3.right * transform.localScale.x;
                    sounds[(int)ImpulseSounds.THROW].Play();
                    BulletManager.Instance().GetBullet(PoolType.PLAYER, spikeBallSpawn.position, firingDirection);

                }
            }
        }

        animator.SetInteger("AnimState", (int)state);
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Damage Delay
            if (Time.frameCount % 20 == 0)
            {
                TakeDamage(1);
                sounds[(int)ImpulseSounds.HIT].Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TriggerEnter");
        if (other.gameObject.CompareTag("DeathPlane"))
        {
            LoseLife();
        }
        else if (other.gameObject.CompareTag("Points"))
        {
            score += 50;
            Score.SetText(": " + score);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            var CheckpointLocaton = other.GetComponent<CheckpointBehaviour>().checkpointLocation;
            spawnPoint = CheckpointLocaton;
        }
        else if (other.gameObject.CompareTag("JumpingPlat"))
        {
            Debug.Log("HitJumingPlat");
            // jump
            state = PlayerState.JUMPING;
            rigidBody2D.AddForce(Vector2.up * verticalForce * 1.2f);
            CreateDustTrail();
            ShakeCamera();
        }
        else if (other.gameObject.CompareTag("Goal"))
        {
            FindObjectOfType<ScoreboardBehaviour>().score = score;
            SceneManager.LoadScene("GameOver");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10);
            sounds[(int)ImpulseSounds.HIT].Play();
        }
    }

    private void LoseLife()
    {
        lives -= 1;

        if (lives > 0)
        {
            health = 100;
            Lives.SetText(": " + lives);
            transform.position = spawnPoint.position;
            sounds[(int)ImpulseSounds.DIE].Play();
        }
        else
        { 
            FindObjectOfType<ScoreboardBehaviour>().score = score;
            SceneManager.LoadScene("GameOver");
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetValue(health);

        //PlayRandomHitSound();

        ShakeCamera();

        if (health <= 0)
        {
            LoseLife();
        }
    }
    private void CreateDustTrail()
    {
        dustTrail.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        dustTrail.Play();
    }
    private void ShakeCamera()
    {
        perlin.m_AmplitudeGain = shakeIntensity;
        isCameraShaking = true;
        shakeTime = maxShakeTime;
    }
}



