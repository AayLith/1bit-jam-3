using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum EnemyState
{
    Idle,
    SearchingForPlayer,
    ChasingPlayer,
    FleeingFromPlayer,
    Hiding
}


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float secondsPerState = 2;
    private Animator animator;
    [SerializeField]
    private EnemyState currentState = EnemyState.Idle;
    [SerializeField]
    private float visionRange = 50;
    private float secondsInCurrentState = 0;
    private PlayerController player;
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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SetAnimationState(AnimationState.Idle);
        player = FindObjectOfType<PlayerController>();
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

    }

    private void Flee()
    {

    }
    private void ChasePlayer()
    {
        var hit = FindPlayer();
        if (hit != null)
        {
            var deltaPos = hit.Value.point - (Vector2) transform.position;
            facingLeft = Mathf.Sign(deltaPos.x) == -1;
            var xSpeed = Mathf.Sign(deltaPos.x) * movementSpeed;
            rb.velocity =   Vector3.Lerp(rb.velocity,new Vector2(xSpeed,rb.velocity.y),0.8f);
        }
    }
    private void SearchForPlayer()
    {

    }

    private void BeIdle()
    {

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
                var hitPlayer = hits[0].collider.gameObject.GetComponentInParent<PlayerController>();
                if(hitPlayer !=null)
                {
                    return hits[0];
                }
            }
        }
        return null;
        
    }
    [SerializeField]
    private float fleeHealthFraction = 0.25f;
    private void UpdateState()
    {   FindPlayer();
        secondsInCurrentState += Time.fixedDeltaTime;
        if(secondsInCurrentState > secondsPerState)
        {
            var hit = FindPlayer();
            if(hit != null)
            {
                UnityEngine.Debug.Log("Found player");
                if(health.CurrentHealth / health.MaxHealth > fleeHealthFraction)
                {   // can see player, healthy, chase
                    currentState = EnemyState.ChasingPlayer;
                }
                else
                {   // can see player, low health, flee
                    currentState = EnemyState.FleeingFromPlayer;
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
