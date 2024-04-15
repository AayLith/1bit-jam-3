using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum EnemyState
{
    Idle,
    SearchingForPlayer,
    Alert,
    ChasingPlayer,
    FleeingFromPlayer,
    Hiding
}


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private const int waitBeforeChaseOrFleeSeconds = 1;
    [SerializeField]
    private const int alertJumpVelocity = 3;
    [SerializeField]
    private float secondsPerState = 2;
    private Animator animator;
    [SerializeField]
    private EnemyState currentState = EnemyState.Idle;
    [SerializeField]
    private float visionRange = 50;
    [SerializeField]
    private float playerSpottedDelay = 2;
    private float secondsInCurrentState = 0;
    private PlayerControls player;
    private SpriteRenderer spriteRenderer;
    private bool facingLeft = false;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private LayerMask visionLayers;
    [SerializeField]
    private Vector3 playerCentreOffset = new Vector2(0,-0.25f);
    private Health health;
    private Rigidbody2D rb;
    private Vector2?  lastKnownPlayerPosition = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SetAnimationState(AnimationState.Idle);
        player = FindObjectOfType<PlayerControls>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SetAnimationState(AnimationState animationState)
    {
        animator.SetInteger("state", (int) animationState);
    }

    void FixedUpdate()
    {
        UpdateState();
        // Debug.Log("Current enemy state: "+Enum.GetName(typeof(EnemyState), currentState));
        switch (currentState)
        {
            case EnemyState.Idle:
                BeIdle();
                break;
            case EnemyState.SearchingForPlayer:
                SearchForPlayer();
                break;
            case EnemyState.ChasingPlayer:
                ChasePlayer();
                break;
            case EnemyState.FleeingFromPlayer:
                Flee();
                break;
            case EnemyState.Hiding:
                Hide();
                break;
        }
    }

    private void Hide()
    {
        SetAnimationState(AnimationState.Hiding);
    }

    private void Flee()
    {
        var hit = FindPlayer();
        if (hit != null || lastKnownPlayerPosition != null)
        {
            var deltaPos = lastKnownPlayerPosition.Value - (Vector2) transform.position;
            deltaPos *= -1;  //flip, we want to get away from player
            facingLeft = Mathf.Sign(deltaPos.x) == -1;
            var xSpeed = Mathf.Sign(deltaPos.x) * movementSpeed;
            rb.velocity =   Vector3.Lerp(rb.velocity,new Vector2(xSpeed,rb.velocity.y),0.8f);
            SetAnimationState(AnimationState.Running);
        }
    }
    private void ChasePlayer()
    {
        FindPlayer();
        UnityEngine.Debug.Log($"player spotted while chasing? {lastKnownPlayerPosition}");
        if ( lastKnownPlayerPosition != null)
        {
            var deltaPos = lastKnownPlayerPosition.Value - (Vector2) transform.position;
            facingLeft = Mathf.Sign(deltaPos.x) == -1;
            var xSpeed = Mathf.Sign(deltaPos.x) * movementSpeed;
            rb.velocity = Vector3.Lerp(rb.velocity,new Vector2(xSpeed,rb.velocity.y),0.8f);
            SetAnimationState(AnimationState.Running);
        }
    }
    private void SearchForPlayer()
    {

    }

    private void Alerted()
    {
        UnityEngine.Debug.Log("Alerted!");
        rb.velocity = new Vector2(rb.velocity.x, alertJumpVelocity);
    }

    private void BeIdle()
    {
        SetAnimationState(AnimationState.Idle);
        rb.velocity = Vector3.Lerp(rb.velocity,new Vector2(0,rb.velocity.y),0.9f);
        lastKnownPlayerPosition = null;
    }

    private RaycastHit2D? FindPlayer()
    {
        var deltaPosition = player.transform.position + playerCentreOffset - gameObject.transform.position;
        var distance = deltaPosition.magnitude;

        if (distance < visionRange)
        {
            
            
            var hits = Physics2D.RaycastAll(transform.position,deltaPosition.normalized,visionRange,visionLayers);
            Debug.DrawRay(transform.position,deltaPosition.normalized*visionRange,Color.red);
            if(hits.Length > 0)
            {
                var hitPlayer = hits[0].collider.gameObject.GetComponentInParent<PlayerControls>();
                if(hitPlayer !=null)
                {
                    lastKnownPlayerPosition = hitPlayer.transform.position;
                    return hits[0];
                }
            }
        }
        return null;
        
    }
    [SerializeField]
    private float fleeHealthFraction = 0.25f;

    private IEnumerator SwitchStates(EnemyState stateNow, float waitBeforeActionSeconds, EnemyState nextState)
    {
        switchingState = true;
        SwitchState(stateNow);
        yield return new WaitForSeconds(waitBeforeActionSeconds);
        SwitchState(nextState);
        Debug.Log("Switched state to "+Enum.GetName(typeof(EnemyState), nextState));
        switchingState = false;
    }

    private void SwitchState(EnemyState state)
    {
        currentState = state;
        //Default to walking
        AnimationState animationState = AnimationState.Running;
        switch (state)
        {
            case EnemyState.Idle:
                animationState = AnimationState.Idle;
                break;
            case EnemyState.Hiding:
                animationState = AnimationState.Hiding;
                break;
        }
        SetAnimationState(animationState);
    }
    bool switchingState = false;
    private void UpdateState()
    {   FindPlayer();

        
        secondsInCurrentState += Time.fixedDeltaTime;
        if(!switchingState && secondsInCurrentState > secondsPerState)
        {
            var hit = FindPlayer();
            if(hit != null)
            {
                
                // flee from player by default
                var newState = EnemyState.FleeingFromPlayer;
                if(health.CurrentHealth / health.MaxHealth > fleeHealthFraction)
                {   // healthy, chase player
                    newState = EnemyState.ChasingPlayer;
                }
                if(newState != currentState && !switchingState)
                {   // don't alert if we're not changing state
                    Alerted();
                    Debug.Log("Found player, new state "+Enum.GetName(typeof(EnemyState), newState));
                    StartCoroutine(SwitchStates(EnemyState.Alert, waitBeforeChaseOrFleeSeconds, newState));
                }
                
            }
            else
            {   // can't see player
                currentState = EnemyState.Idle;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.flipX = facingLeft;
    }
}
