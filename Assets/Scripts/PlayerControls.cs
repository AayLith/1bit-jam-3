using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Health;
using UnityEngine.U2D;
using System.Diagnostics;

public class PlayerControls : MonoBehaviour
{
    enum playerDirection { right, left }
    private playerDirection direction = playerDirection.right;

    [Header ( "Movement" )]
    public float gravity = -25;
    public float runSpeed = 8;
    public float groundDamping = 20; // how fast do we change direction? higher means faster
    public float inAirDamping = 5;
    public float jumpHeight = 3;
    public float maxFallSpeed = -9;
    private float normalizedHorizontalSpeed = 0;

    [Header ( "Jump" )]
    public float coyoteTime = 0.1f;
    private float coyoteTimeCounter = 0.1f;
    public float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    [Header ( "Shell" )]
    public Vector2 throwStrength = new Vector2 ( 10 , 3 );

    [Header ( "Other" )]
    [SerializeField]
    private const float inputDeadZoneAmount = 0.1f;
    [SerializeField]
    private bool flashWhileInvulnerable = true;
    [SerializeField]
    private bool addPlayerVelocityToThrow = true;
    [SerializeField]
    private float flashesPerSecond = 4;
    private float invulnerabilityPeriodAfterTakingDamageSeconds = 0.5f;
    private Stopwatch invulnerabilityTimer = new Stopwatch ();

    [Header ( "Objects" )]
    private CharacterController2D _controller;
    private Animator _animator;
    [SerializeField] private Collision2DTriggerRecorder groundShell;
    public GameObject shellSlot;
    public Shell shell;
    private Health healthComponent;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    private RaycastHit2D _lastControllerColliderHit;
    private Vector3 _velocity;

    // Inputs
    private bool jumpPressed = false;
    private bool jumpReleased = false;
    private float horizontalInput;
    private bool pickupInput;
    private bool throwInput;
    private bool lockInput = false;


    void Awake ()
    {
        _animator = GetComponent<Animator> ();
        _controller = GetComponent<CharacterController2D> ();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;

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
    }


    #region Event Listeners

    void onControllerCollider ( RaycastHit2D hit )
    {
        // bail out on plain old ground hits cause they arent very interesting
        if ( hit.normal.y == 1f )
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent ( Collider2D col )
    {
        UnityEngine.Debug.Log ( "onTriggerEnterEvent: " + col.gameObject.name );
    }


    void onTriggerExitEvent ( Collider2D col )
    {
        UnityEngine.Debug.Log ( "onTriggerExitEvent: " + col.gameObject.name );
    }

    #endregion


    // the Update loop contains a very simple example of moving the character around and controlling the animation
    void Update ()
    {
        if ( _controller.isGrounded )
            _velocity.y = 0;

        HandleHorizontal ();
        HandleJump ();
        HandleVertical ();
        ClampFallSpeed ();
        FlashIfInvulnerable ();
        HandleActions ();
        if ( shell != null )
            HandleShell ();

        _controller.move ( _velocity * Time.deltaTime );

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }

    void HandleHorizontal ()
    {
        if ( Input.GetAxis ( "Horizontal" ) > inputDeadZoneAmount )
        {
            direction = playerDirection.right;
            normalizedHorizontalSpeed = 1;
            if ( transform.localScale.x < 0f )
                transform.localScale = new Vector3 ( -transform.localScale.x , transform.localScale.y , transform.localScale.z );

            if ( _controller.isGrounded )
                _animator.Play ( Animator.StringToHash ( "Run" ) );
        }
        else if ( Input.GetAxis ( "Horizontal" ) < -inputDeadZoneAmount )
        {
            direction = playerDirection.left;
            normalizedHorizontalSpeed = -1;
            if ( transform.localScale.x > 0f )
                transform.localScale = new Vector3 ( -transform.localScale.x , transform.localScale.y , transform.localScale.z );

            if ( _controller.isGrounded )
                _animator.Play ( Animator.StringToHash ( "Run" ) );
        }
        else
        {
            normalizedHorizontalSpeed = 0;

            if ( _controller.isGrounded )
                _animator.Play ( Animator.StringToHash ( "Idle" ) );
        }
    }

    void HandleJump ()
    {
        if ( _controller.isGrounded && Input.GetAxis ( "Jump" ) > inputDeadZoneAmount )//&& coyoteTimeCounter > 0f )
        {
            _velocity.y = Mathf.Sqrt ( 2f * jumpHeight * -gravity );
            _animator.Play ( Animator.StringToHash ( "Jump" ) );
        }

        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp ( _velocity.x , normalizedHorizontalSpeed * runSpeed , Time.deltaTime * smoothedMovementFactor );
        // apply gravity before moving
        _velocity.y += gravity * Time.deltaTime;

        if ( _controller.isGrounded )
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void HandleVertical ()
    {
        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if ( _controller.isGrounded && Input.GetKey ( KeyCode.DownArrow ) )
        {
            _velocity.y *= 3f;
            _controller.ignoreOneWayPlatformsThisFrame = true;
        }
    }

    void HandleActions ()
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

    void HandleShell ()
    {
        shell.transform.position = shellSlot.transform.position;
    }

    private void ClampFallSpeed ()
    {
        if ( rb.velocity.y < maxFallSpeed )
        {
            rb.velocity = new Vector2 ( rb.velocity.x , maxFallSpeed );
        }
    }

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

    private bool CheckAndGetIsInvulnerable ()
    {
        if ( invulnerabilityTimer.IsRunning && invulnerabilityTimer.Elapsed.TotalSeconds > invulnerabilityPeriodAfterTakingDamageSeconds )
        {
            invulnerabilityTimer.Stop ();
        }
        return invulnerabilityTimer.IsRunning;
    }

    IEnumerator lockInputsDelay ()
    {
        lockInput = true;
        yield return new WaitForSeconds ( 0.1f );
        lockInput = false;
    }
}
