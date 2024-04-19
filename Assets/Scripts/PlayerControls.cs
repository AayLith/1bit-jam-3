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

public class PlayerControls : ResettableObject
{
    enum playerDirection { right, left }
    private playerDirection direction = playerDirection.right;

    [Header ( "Movement" )]
    public float defaultGravity = -25;
    public float gravity = -25;
    public float runSpeed = 8;
    public float groundDamping = 20; // how fast do we change direction? higher means faster
    public float inAirDamping = 5;
    public float jumpHeight = 3;
    public float maxFallSpeed = -9;
    private float normalizedHorizontalSpeed = 0;
    public float shellCarrySpeedMult = 0.5f;
    public float fallSpeedModifier = 1;
    public float gravityFallModifier = 1;

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
    public Vector3 _velocity;


    [Header ( "Audio" )]
    public AudioClip jumpSound;
    public AudioClip throwSound;
    public AudioClip pickupSound;
    public AudioClip dropSound;
    public AudioClip deathSound;
    private AudioSource _audioSource;

    [Header ( "Inputs" )]
    private bool jumpPressed = false;
    private bool jumpReleased = false;
    private bool lockInput = false;
    private bool wasPickupPressedLastFrame = false;
    private bool wasThrowPressedLastFrame = false;
    private bool pickupPressed = false;
    private bool throwPressed = false;

    protected override void Awake ()
    {
        base.Awake ();

        _animator = GetComponent<Animator> ();
        _controller = GetComponent<CharacterController2D> ();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;

        healthComponent = GetComponent<Health> ();
        sprite = GetComponent<SpriteRenderer> ();
        rb = GetComponent<Rigidbody2D> ();
        _audioSource = GetComponentInChildren<AudioSource> ();
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

       

        
        
        
        
        
        pickupPressed = Input.GetAxis("Pickup Shell") > inputDeadZoneAmount;
        throwPressed = Input.GetAxis("Throw Shell") > inputDeadZoneAmount;



        _animator.SetFloat ( "xVelocity" , Mathf.Abs ( _velocity.x ) );
        _animator.SetFloat ( "yVelocity" , _velocity.y );

    }

    void FixedUpdate()
    {
        if (_controller.isGrounded)
        {
            _velocity.y = 0;
            coyoteTimeCounter = coyoteTime;
            _animator.SetBool("isJumping", false);
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
            _animator.SetBool("isJumping", true);
        }

        if (shell != null)
            HandleShell();
        if (_velocity.y < 0)
            _velocity.y *= fallSpeedModifier;
        
        HandleHorizontal();
        HandleJump();
        HandleActions();
        ClampFallSpeed();
        FlashIfInvulnerable();
        HandleVertical();

        _controller.move(_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
        jumpPressed = false;
        jumpReleased = false;

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
            Jump();
        }

        // apply horizontal speed smoothing it. don't really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp ( _velocity.x , normalizedHorizontalSpeed * runSpeed , Time.deltaTime * smoothedMovementFactor );
        // apply gravity before moving

        if ( _velocity.y < 0 )
            _velocity.y += gravity * gravityFallModifier * Time.deltaTime ;
        else
            _velocity.y += gravity * Time.deltaTime;

        // Short jumps
        
        if ( jumpReleased && _velocity.y > 0 )
        {
            _velocity = new Vector2 ( _velocity.x , _velocity.y * 0.5f );
        }

    }

    public void DisableInput(bool disable)
    {

    }

    public void Jump(bool force = false)
    {
        if (jumpSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(jumpSound);
        }
        else
        {
            Debug.LogError("Jump sound or AudioSource is not set properly");
        }
        if(force)
        {
            jumpPressed = true;
        }
        _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity * (shell ? shellCarryJumpMult : 1));
        coyoteTimeCounter = 0;

        _animator.SetBool("isJumping", true);
    }

    void HandleVertical ()
    {
        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if ( _controller.isGrounded && Input.GetAxis ( "Vertical" ) < -inputDeadZoneAmount )
        {
            //_velocity.y *= 3f; // super jump
            _controller.ignoreOneWayPlatformsThisFrame = true;
        }
    }

    void HandleActions ()
    {

        if ( !lockInput )
        {
            // Handle pickup action: only if the button was not pressed last frame but is pressed now
            if ( pickupPressed && !wasPickupPressedLastFrame )
            {
                if ( shell )
                {
                    dropShell ();
                }
                else if ( groundShell.collisions.Count > 0 )
                {
                    foreach ( Collider2D c in groundShell.collisions )
                    {
                        Shell potentialShell = c.transform.parent.GetComponent<Shell> ();
                        if ( potentialShell != null && potentialShell.canBePickedUp )
                        {
                            pickUpShell ( potentialShell );
                            break;
                        }
                    }
                }
            }

            if ( throwPressed && !wasThrowPressedLastFrame && shell != null )
            {
                if ( shell is Skull )
                {
                    ( shell as Skull ).ActivateHitbox ();
                }
                ThrowShell ();
                dropShell ();
            }
        }

        // Update last frame state for next frame's comparison
        wasPickupPressedLastFrame = pickupPressed;
        wasThrowPressedLastFrame = throwPressed;
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
        if ( invulnerable && flashWhileInvulnerable)
        {
            //bool black = sprite.color==Color.black;
            float seconds = ( float ) invulnerabilityTimer.Elapsed.TotalSeconds * flashesPerSecond;
            // bool shouldBeBlack = Mathf.RoundToInt(seconds) > Mathf.FloorToInt(seconds);
            float period = 1 / flashesPerSecond;
            float mod = seconds % ( 2 * period );
            bool shouldBeBlack = mod < period;
            if ( shouldBeBlack )
            {
                sprite.enabled = false;
                sprite.color = new Color ( 1 , 1 , 1 , 0 );
            }
            else
            {
                sprite.enabled = true;
                sprite.color = Color.white;
            }
        }
    }

    void pickUpShell ( Shell s )
    {
        if ( !s.canBePickedUp )
            return;

        s.onEquip ( this );
        shell = s;
        if (pickupSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(pickupSound);
        }
        s.transform.parent = shellSlot.transform;
        s.transform.position = shellSlot.transform.position;
        s.GetComponent<Rigidbody2D> ().gravityScale = 0;
        s.playerCollision.enabled = false;
        s.groundCollision.enabled = false;
        shellCollider ();
    }

    void dropShell ()
    {
        if ( shell == null )
            return;

        if (dropSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(dropSound);
        }
        shell.onUnequip ( this );
        shell.transform.parent = null;
        shell.GetComponent<Rigidbody2D> ().gravityScale = 2;
        shell.groundCollision.enabled = true;
        StartCoroutine ( shell.Cooldown () );  // Start the cooldown
        shell = null;
        disableShellColliders ();
    }

    void ThrowShell ()
    {
        if ( shell == null )
            return;

        if ( throwSound != null && _audioSource != null )
        {
            _audioSource.PlayOneShot ( throwSound );
        }
        else
        {
            Debug.LogError ( "Throw sound or AudioSource is not set properly" );
        }
        shell.onUnequip ( this );
        shell.isThrown = true;
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
        if (deathSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(deathSound);
        }
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

    public IEnumerator lockInputsDelay (float delay = 0.1f)
    {
        lockInput = true;
        yield return new WaitForSeconds ( delay );
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
        if ( shell )
            dropShell ();
        base.reset ();
        _velocity = Vector3.zero;
        healthComponent.ResetHealth ();
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
