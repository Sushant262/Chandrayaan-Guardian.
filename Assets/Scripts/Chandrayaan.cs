using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chandrayaan : MonoBehaviour

{
    public bool JPressed;
    public float respawnTime = 3.0f;
    public float respawnInvulnerabilityTime = 3.0f;
    public Player player;
    public bool Player ;
    //public float bulletspawncount;
    private bool _thrusting;
    public float thrustSpeed = 1.0f;
    public float turnspeed = 1.0f;
    private float _turnDirection;
    private Rigidbody2D _rigidbody;
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public ParticleSystem explosion;
    public Bullet bulletPrefab;
    [SerializeField] private AudioSource ShootingSound;
    [SerializeField] private AudioSource ChandAlienCollision;
    [SerializeField] private AudioSource PlayerDeathSound;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }


    private void Shoot()
    {
        ShootingSound.Play();
        Bullet bullet = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);
        bullet.Project(this.transform.up);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Alien")
        {
            ChandAlienCollision.Play();
            TakeDamage(20);
           Destroy(collision.gameObject);

            if(currentHealth <= 0)
            {
                this.explosion.Play();
                Destroy(gameObject);
                FindObjectOfType<GameManager>().GameOver();
            }
             
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (Player == true) 
        {
            _rigidbody.angularVelocity = 0;
            transform.rotation= Quaternion.identity;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            this.player.gameObject.SetActive(false);
            Player = false;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {       
                 JPressed = true;
            if(JPressed == true && Player == false)
            {
                //this.player.gameObject.SetActive(true);
                Player = true;
                PlayerDeathSound.Play();
                Invoke(nameof(Respawn), this.respawnTime);


            }




        }
        if(Input.GetKeyUp(KeyCode.J))
        {
            JPressed= false;
        }


        if (Player==false)
        {
            _thrusting = Input.GetKey(KeyCode.W);

            if (Input.GetKey(KeyCode.A)) { _turnDirection = 1.0f; }
            else if (Input.GetKey(KeyCode.D)) { _turnDirection = -1.0f; }
            else { _turnDirection = 0.0f; }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Shoot();

            }
        }
    }
    private void FixedUpdate()
    {
   
        if (_thrusting)
        {
            _rigidbody.AddForce(this.transform.up * this.thrustSpeed);
        }


        if (_turnDirection != 0.0f)
        {
            _rigidbody.AddTorque(_turnDirection * this.turnspeed);
        }
    }
    public void Respawn()
    {

        this.player.transform.position = Vector3.zero;
        this.player.gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
        this.player.gameObject.SetActive(true);

        Invoke(nameof(TurnOnCollisions), this.respawnInvulnerabilityTime);
    }

    private void TurnOnCollisions()
    {
        this.player.gameObject.layer = LayerMask.NameToLayer("Player");
    }
}