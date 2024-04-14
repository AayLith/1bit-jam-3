using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Health;
using UnityEngine.U2D;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Unity.VisualScripting;

public class PlayerControls : ResetableObject
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
    public float shellCarrySpeedMult = 0.5f;

    [Header ( "Jump" )]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    public float shellCarryJumpMult = 0.5f;

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
    [SerializeField]
    private float invulnerabilityPeriodAfterTakingDamageSeconds = 0.5f;
    private Stopwatch invulnerabilityTimer = new Stopwatch ();

    [Header ( "Colliders" )]
    public BoxCollider2D physicsCollider;
    public Vector2 defaultColliderSize;
    public Vector2 smallShellColliderSize;
    public Vector2 mediumShellColliderSize;
    public Vector2 tallShellColliderSize;
    public Vector2 defaultCollideOffset;
    public Vector2 smallShellColliderOffset;
    public Vector2 mediumShellColliderOffset;
    public Vector2 tallShellColliderOffset;

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
        // bail out on plain old ground hits cause they aren't very interesting
        if ( hit.normal.y == 1f )
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent ( Collider2D collision )
    {
        Debug.Log ( "Trigger enter: " + collision.gameObject.name );
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer> ();
        if ( damageDealer != null && damageDealer.damageToPlayer > 0 )
        {
            bool invulnerable = CheckAndGetIsInvulnerable ();
            if ( !invulnerable )
            {
                Debug.Log ( $"Damage taken {damageDealer.damageToPlayer}" );
                healthComponent.TakeDamage ( damageDealer.damageToPlayer );
                invulnerabilityTimer = Stopwatch.StartNew ();
            }

        }
        Debug.Log ( "onTriggerEnterEvent: " + collision.gameObject.name );

    }


    void onTriggerExitEvent ( Collider2D col )
    {
        UnityEngine.Debug.Log ( "onTriggerExitEvent: " + col.gameObject.name );
    }

    #endregion


    // the Update loop contains a very simple example of moving the character around and controlling the animation
    void Update ()
    {
        if ( Input.GetButtonDown ( "Jump" ) )
        {
            jumpPressed = true;
        }
        else if ( Input.GetButtonUp ( "Jump" ) )
        {
            jumpReleased = true;
        }

        if ( _controller.isGrounded )
        {
            _velocity.y = 0;
            coyoteTimeCounter = coyoteTime;
            _animator.SetBool ( "isJumping" , false );
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            _animator.SetBool ( "isJumping" , true );
        }

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

        _animator.SetFloat ( "xVelocity" , Mathf.Abs ( _velocity.x ) );
        _animator.SetFloat ( "yVelocity" , _velocity.y );

    }

    void HandleHorizontal ()
    {
        if ( Input.GetAxis ( "Horizontal" ) > inputDeadZoneAmount )
        {
            direction = playerDirection.right;
            normalizedHorizontalSpeed = 1 * ( shell ? shellCarrySpeedMult : 1 );
            if ( transform.localScale.x < 0f )
                transform.localScale = new Vector3 ( -transform.localScale.x , transform.localScale.y , transform.localScale.z );

        }
        else if ( Input.GetAxis ( "Horizontal" ) < -inputDeadZoneAmount )
        {
            direction = playerDirection.left;
            normalizedHorizontalSpeed = -1 * ( shell ? shellCarrySpeedMult : 1 );
            if ( transform.localScale.x > 0f )
                transform.localScale = new Vector3 ( -transform.localScale.x , transform.localScale.y , transform.localScale.z );


        }
        else
        {
            normalizedHorizontalSpeed = 0;

            /*if ( _controller.isGrounded )
            {
                SetAnimationState ( AnimationState.Idle );
                PlayAnimation ( "Idle" );
            }
            else if ( _velocity.y < 0 )
            {
                SetAnimationState ( AnimationState.Falling );
                PlayAnimation ( "Fall" );
            }*/
        }
    }

    void HandleJump ()
    {
        if ( jumpPressed && coyoteTimeCounter > 0f )
        {
            _velocity.y = Mathf.Sqrt ( 2f * jumpHeight * -gravity * ( shell ? shellCarryJumpMult : 1 ) );
            coyoteTimeCounter = 0;
            _animator.SetBool ( "isJumping" , true );
        }

        // apply horizontal speed smoothing it. don't really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp ( _velocity.x , normalizedHorizontalSpeed * runSpeed , Time.deltaTime * smoothedMovementFactor );
        // apply gravity before moving
        _velocity.y += gravity * Time.deltaTime;

        // Short jumps
        if ( Input.GetButtonUp ( "Jump" ) )
        {
            jumpReleased = true;
        }
        if ( jumpReleased && _velocity.y > 0 )
        {
            _velocity = new Vector2 ( _velocity.x , _velocity.y * 0.5f );
        }

        jumpReleased = false;
        jumpPressed = false;
    }

    void HandleVertical ()
    {
        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if ( _controller.isGrounded && Input.GetAxis ( "Vertical" ) < -inputDeadZoneAmount )
        {
            //_velocity.y *= 3f; // superjump
            _controller.ignoreOneWayPlatformsThisFrame = true;
        }
    }

    void HandleActions ()
    {
        // Pickup or throw shell
        if ( !lockInput )
        {
            if ( Input.GetAxis ( "Pickup Shell" ) > inputDeadZoneAmount )
            {
                Shell s = shell;
                if ( shell )
                    dropShell ();
                if ( groundShell.collisions.Count > 0 )
                {
                    foreach ( Collider2D c in groundShell.collisions )
                        if ( c.transform.parent.GetComponent<Shell> () != s )
                        {
                            pickUpShell ( c.transform.parent.GetComponent<Shell> () );
                            break;
                        }
                }
                StartCoroutine ( lockInputsDelay () );
            }
            else if ( Input.GetAxis ( "Throw Shell" ) > inputDeadZoneAmount && shell != null )
            {
                ThrowShell ();
                dropShell ();
                StartCoroutine ( lockInputsDelay () );
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
                sprite.color = new Color ( 0 , 0 , 0 , 0 );
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
        s.transform.parent = shellSlot.transform;
        s.transform.position = shellSlot.transform.position;
        s.GetComponent<Rigidbody2D> ().gravityScale = 0;
        //s.playerCollision.GetComponent<Collider2D> ().isTrigger = true;
        s.playerCollision.enabled = false;
        s.groundCollision.enabled = false;
        shellCollider ();
    }

    void dropShell ()
    {
        shell.transform.parent = null;
        shell.GetComponent<Rigidbody2D> ().gravityScale = 2;
        shell.groundCollision.enabled = true;
        shell = null;
        disableShellColliders ();
    }

    void ThrowShell ()
    {
        shell.GetComponent<Rigidbody2D> ().velocity = new Vector2 ( direction == playerDirection.right ? throwStrength.x : -throwStrength.x , throwStrength.y ) + ( addPlayerVelocityToThrow ? GetComponent<Rigidbody2D> ().velocity : Vector2.zero );
    }

    void shellCollider ()
    {
        switch ( shell.shellHeight )
        {
            case Shell.shellHeights.small:
                physicsCollider.size = smallShellColliderSize;
                physicsCollider.offset = smallShellColliderOffset;
                break;
            case Shell.shellHeights.medium:
                physicsCollider.size = mediumShellColliderSize;
                physicsCollider.offset = mediumShellColliderOffset;
                break;
            case Shell.shellHeights.tall:
                physicsCollider.size = tallShellColliderSize;
                physicsCollider.offset = tallShellColliderOffset;
                break;
        }
    }

    void disableShellColliders ()
    {
        physicsCollider.size = defaultColliderSize;
        physicsCollider.offset = defaultCollideOffset;
    }

    private void OnDeath ()
    {
        UnityEngine.Debug.Log ( "Player death" );
        NotificationCenter.instance.PostNotification ( this , Notification.notifications.resetlevel );
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

    public void SetAnimationState ( AnimationState state )
    {
        // Debug.Log("Setting animation state to " + System.Enum.GetName(typeof(AnimationState), state));
        _animator.SetInteger ( "state" , ( int ) state ); // This always breaks when input are pressed too fast
    }

    public void PlayAnimation ( string name )
    {
        _animator.Play ( Animator.StringToHash ( name ) );
    }

    public override void receiveNotification ( Notification notification )
    {
        switch ( notification.name )
        {
            case Notification.notifications.resetlevel:
                reset ();
                break;
        }
    }

    protected override void reset ()
    {
        base.reset ();
        if ( shell )
            dropShell ();
        _velocity = Vector3.zero;
        // Check if there is a last activated checkpoint.
        if ( Checkpoint.lastCheckpoint != null )
        {
            // Teleport the player to the last activated checkpoint.
            transform.position = Checkpoint.lastCheckpoint.position;
            Debug.Log ( "Teleported to the last activated checkpoint at: " + Checkpoint.lastCheckpoint.position );
        }
        else
        {
            transform.position = startPosition;
            Debug.LogWarning ( "No checkpoint has been activated yet." );
        }
    }
}
