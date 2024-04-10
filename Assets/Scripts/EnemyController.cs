using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private float movementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SetAnimationState(AnimationState.Idle);
    }

    private void SetAnimationState(AnimationState animationState)
    {
        animator.SetInteger("state", (int) animationState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
