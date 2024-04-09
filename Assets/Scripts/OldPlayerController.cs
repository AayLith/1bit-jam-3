using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class OldPlayerController : MonoBehaviour
{/*
    [SerializeField]
    private const float inputDeadZoneAmount = 0.1f;
    [SerializeField]
    private const float airMovementFactor = 0.75f;
    [SerializeField]
    private bool flashWhileInvulnerable = true;
    public static PlayerController instance;

    [SerializeField] bool isGrounded = false;
    int groundCollisions = 0;
    bool lockInput = false;


    [Header("Stats")]
    [SerializeField] float defaultSpeed;
    public float speed;
    [SerializeField] float defaultJump;
    public float jump;

    public GameObject shellSlot;
    public Shell shell;
    [SerializeField] ItemCollision groundShell;
    private Health healthComponent;
    private SpriteRenderer sprite;

    private void Awake()
    {
        //instance = this;
        speed = defaultSpeed;
        jump = defaultJump;
        healthComponent = GetComponent<Health>();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
        if (healthComponent != null)
        {
            healthComponent.onDeath += OnDeath;
        }
    }

    private void Update()
    {
        // Update positions
        transform.rotation = Quaternion.identity;
        if (shell)
        {
            shell.transform.position = shellSlot.transform.position;
            shell.transform.rotation = Quaternion.identity;
        }
        var xAxis = Input.GetAxis("Horizontal");
        if (!isGrounded)
        {
            xAxis *= airMovementFactor;
        }
        // Walk
        if (xAxis > inputDeadZoneAmount || xAxis < -inputDeadZoneAmount)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(xAxis * speed, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x), 1, 1);
        }

        // Jump
        if (isGrounded && Input.GetAxis("Jump") > inputDeadZoneAmount)
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jump);

        // Pickup shell
        if (!lockInput && groundShell.item != null && Input.GetAxis("Submit") > inputDeadZoneAmount)
        {
            // if already has a shell
            if (shell != null)
            {
                shell.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(transform.localScale.x) * 0.5f, 1);
                dropShell();
            }

            // pickup the shell
            pickUpShell(groundShell.item);

            StartCoroutine(lockInputsDelay());
        }

        // Throw shell
        if (!lockInput && shell != null && Input.GetAxis("Fire1") > 0.1f)
        {
            shell.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(transform.localScale.x) * 10f, 2f);
            dropShell();
            StartCoroutine(lockInputsDelay());
        }

        FlashIfInvulnerable();
    }
    float flashesPerSecond = 4;
    void FlashIfInvulnerable()
    {
        bool invulnerable = CheckAndGetIsInvulnerable();
        if (invulnerable)
        {
            //bool black = sprite.color==Color.black;
            float seconds = (float)invulnerabilityTimer.Elapsed.TotalSeconds * flashesPerSecond;
            // bool shouldBeBlack = Mathf.RoundToInt(seconds) > Mathf.FloorToInt(seconds);
            float period = 1 / flashesPerSecond;
            float mod = seconds % (2 * period);
            bool shouldBeBlack = mod < period;
            if (shouldBeBlack)
            {
                sprite.color = Color.black;
            }
            else
            {
                sprite.color = Color.white;
            }
        }
    }
    void pickUpShell(Shell s)
    {
        shell = s;
        shell.transform.parent = shellSlot.transform;
        shell.transform.position = shellSlot.transform.position;
        shell.transform.rotation = Quaternion.identity;
        shell.GetComponent<Rigidbody2D>().gravityScale = 0;
        shell.playerCollision.GetComponent<Collider2D>().isTrigger = true;
    }
    private void OnDeath()
    {
        UnityEngine.Debug.Log("Player death");
    }
    void dropShell()
    {
        shell.transform.parent = null;
        shell.GetComponent<Rigidbody2D>().gravityScale = 1;
        shell = null;
    }

    IEnumerator lockInputsDelay()
    {
        lockInput = true;
        yield return new WaitForSeconds(0.1f);
        lockInput = false;
    }
    [SerializeField]
    private float invulnerabilityPeriodAfterTakingDamageSeconds = 0.5f;
    private Stopwatch invulnerabilityTimer = new Stopwatch();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            groundCollisions++;
        if (groundCollisions > 0)
            isGrounded = true;
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (damageDealer != null && damageDealer.damageToPlayer > 0)
        {
            bool invulnerable = CheckAndGetIsInvulnerable();
            if (!invulnerable)
            {
                healthComponent.TakeDamage(damageDealer.damageToPlayer);
                invulnerabilityTimer = Stopwatch.StartNew();
            }

        }
    }

    private bool CheckAndGetIsInvulnerable()
    {
        if (invulnerabilityTimer.IsRunning && invulnerabilityTimer.Elapsed.TotalSeconds > invulnerabilityPeriodAfterTakingDamageSeconds)
        {
            invulnerabilityTimer.Stop();
        }
        return invulnerabilityTimer.IsRunning;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            groundCollisions--;
        if (groundCollisions == 0)
            isGrounded = false;
    }*/
}