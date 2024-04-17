using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum playerDirection { right, left }
    private playerDirection direction = playerDirection.right;

    [SerializeField]
    bool addPlayerVelocityToThrow = true;
    [SerializeField]
    private const float inputDeadZoneAmount = 0.1f;
    [SerializeField]
    private const float airMovementFactor = 0.75f;
    public static PlayerController instance;

    public bool isGrounded = false;
    bool lockInput = false;

    public float maxFallSpeed = -9f;
    public Vector2 throwStrength = new Vector2 ( 10 , 3 );


    [Header ( "Stats" )]
    [SerializeField] float defaultSpeed;
    public float speed;
    [SerializeField] float defaultJump;
    public float jump;

    public GameObject shellSlot;
    public Shell shell;
    [SerializeField] private Collision2DRecorder physicsShape;
    [SerializeField] private Collision2DTriggerRecorder groundShell;
    [SerializeField] private Collision2DTriggerRecorder groundCollider;
    [SerializeField] private Collision2DTriggerRecorder hitCollider;
    private Health healthComponent;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Animator animationController;

    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    private bool jumpPressed = false;
    private bool jumpReleased = false;

    private float horizontalInput;
    private bool pickupInput;
    private bool throwInput;

    private void Awake ()
    {
        instance = this;
        speed = defaultSpeed;
        jump = defaultJump;
        healthComponent = GetComponent<Health> ();
        sprite = GetComponent<SpriteRenderer> ();
        rb = GetComponent<Rigidbody2D> ();
        if ( sprite == null )
        {
            sprite = GetComponentInChildren<SpriteRenderer> ();
        }
        if ( healthComponent != null )
        {
            healthComponent.onDeath += OnDeath;
        }
        animationController = GetComponentInChildren<Animator>();
    }

    private void Update ()
    {
        HandleInput ();
        FlashIfInvulnerable ();
        HandleActions ();
        if ( shell != null )
            HandleShell ();
    }

    private void FixedUpdate ()
    {
        HandleGrounded ();
        HandleMovement ();

        if ( jumpPressed )
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        //Handle Jumping
        if ( jumpPressed && ( coyoteTimeCounter > 0f && jumpBufferCounter > 0f ) )
        {
            HandleJump ();
        }

        if ( jumpReleased && rb.velocity.y > 0 )
        {
            rb.velocity = new Vector2 ( rb.velocity.x , rb.velocity.y * 0.5f );
        }
        ClampFallSpeed ();
        jumpPressed = false;
        jumpReleased = false;
    }

    private void HandleInput ()
    {
        if ( Input.GetAxis ( "Horizontal" ) > inputDeadZoneAmount || Input.GetAxis ( "Horizontal" ) < -inputDeadZoneAmount )
            horizontalInput = Input.GetAxis ( "Horizontal" );
        else horizontalInput = 0;
        if ( horizontalInput > 0 ) direction = playerDirection.right;
        else if ( horizontalInput < 0 ) direction = playerDirection.left;

        if ( Input.GetButtonDown ( "Jump" ) )
        {
            jumpPressed = true;
        }
        else if ( Input.GetButtonUp ( "Jump" ) )
        {
            jumpReleased = true;
        }

        if ( isGrounded )
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        pickupInput = Input.GetAxis ( "Pickup Shell" ) > inputDeadZoneAmount;
        throwInput = Input.GetAxis ( "Throw Shell" ) > inputDeadZoneAmount;
    }

    private void ClampFallSpeed ()
    {
        if ( rb.velocity.y < maxFallSpeed )
        {
            rb.velocity = new Vector2 ( rb.velocity.x , maxFallSpeed );
        }
    }

    private void HandleGrounded ()
    {
        if ( groundCollider.collisions.Count > 0 )
            isGrounded = true;
        else
            isGrounded = false;
    }

    private void HandleMovement ()
    {
        rb.velocity = new Vector2 ( horizontalInput * speed , rb.velocity.y );
        if(Mathf.Abs(horizontalInput) > float.Epsilon)
        {
            SetAnimationState(AnimationState.Running);
        }
        else if(isGrounded)
        {
            SetAnimationState(AnimationState.Idle);
        }
    }

    private void HandleShell ()
    {
        shell.transform.position = shellSlot.transform.position;
    }

    private void HandleJump ()
    {
        // Full jump if the jump button is held down
        rb.velocity = new Vector2 ( rb.velocity.x , jump );
        coyoteTimeCounter = 0;
        jumpBufferCounter = 0;
        isGrounded = false;
    }

    private void HandleActions ()
    {
        // Pickup or throw shell
        if ( !lockInput )
        {
            if ( pickupInput )
            {
                if ( shell != null )
                    dropShell ();
                if ( groundShell.collisions.Count > 0 )
                    pickUpShell ( groundShell.collisions[ 0 ].transform.parent.GetComponent<Shell> () );
            }
            else if ( throwInput && shell != null )
            {
                ThrowShell ();
            }
        }
    }
    float flashesPerSecond = 4;
    void FlashIfInvulnerable ()
    {
        bool invulnerable = CheckAndGetIsInvulnerable ();
        if ( invulnerable )
        {
            //bool black = sprite.color==Color.black;
            float seconds = ( float ) invulnerabilityTimer.Elapsed.TotalSeconds * flashesPerSecond;
            // bool shouldBeBlack = Mathf.RoundToInt(seconds) > Mathf.FloorToInt(seconds);
            float period = 1 / flashesPerSecond;
            float mod = seconds % ( 2 * period );
            bool shouldBeBlack = mod < period;
            if ( shouldBeBlack )
            {
                sprite.color = Color.black;
            }
            else
            {
                sprite.color = Color.white;
            }
        }
    }
    void pickUpShell ( Shell s )
    {
        shell = s;
        shell.transform.parent = shellSlot.transform;
        shell.transform.position = shellSlot.transform.position;
        shell.transform.rotation = Quaternion.identity;
        shell.GetComponent<Rigidbody2D> ().gravityScale = 0;
        shell.playerCollision.GetComponent<Collider2D> ().isTrigger = true;
    }

    void dropShell ()
    {
        shell.transform.parent = null;
        shell.GetComponent<Rigidbody2D> ().gravityScale = 2;
        shell = null;
    }

    void ThrowShell ()
    {
        shell.transform.parent = null;
        shell.GetComponent<Rigidbody2D> ().gravityScale = 2;
        shell.GetComponent<Rigidbody2D> ().velocity = new Vector2 ( direction == playerDirection.right ? throwStrength.x : -throwStrength.x , throwStrength.y ) + ( addPlayerVelocityToThrow ? GetComponent<Rigidbody2D> ().velocity : Vector2.zero );
        shell = null;
    }

    private void OnDeath ()
    {
        UnityEngine.Debug.Log ( "Player death" );
    }

    IEnumerator lockInputsDelay ()
    {
        lockInput = true;
        yield return new WaitForSeconds ( 0.1f );
        lockInput = false;
    }

    [SerializeField]
    private float invulnerabilityPeriodAfterTakingDamageSeconds = 0.5f;
    private Stopwatch invulnerabilityTimer = new Stopwatch ();
    private void OnCollisionEnter2D ( Collision2D collision )
    {
        
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer> ();
        if ( damageDealer != null && damageDealer.damageToPlayer > 0 )
        {
            bool invulnerable = CheckAndGetIsInvulnerable ();
            if ( !invulnerable )
            {
                healthComponent.TakeDamage ( damageDealer.damageToPlayer );
                invulnerabilityTimer = Stopwatch.StartNew ();
            }

        }
    }

    private bool CheckAndGetIsInvulnerable ()
    {
        if ( invulnerabilityTimer.IsRunning && invulnerabilityTimer.Elapsed.TotalSeconds > invulnerabilityPeriodAfterTakingDamageSeconds )
        {
            invulnerabilityTimer.Stop ();
        }
        return invulnerabilityTimer.IsRunning;
    }

    private void SetAnimationState(AnimationState state)
    {
        UnityEngine.Debug.Log("Setting animation state to " + Enum.GetName(typeof(AnimationState), state));
        animationController.SetInteger("state",(int) state);
    }
}
