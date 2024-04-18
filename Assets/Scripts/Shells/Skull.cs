using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Shell
{
    [Header("Hitbox")]
    public Collider2D hitbox; // Assign this in the Unity editor

    new void Start()
    {
        base.Start();
        if (hitbox == null)
        {
            Debug.LogError("Hitbox not assigned in the inspector");
        }
        else
        {
            hitbox.enabled = false;  // Initially disabled
        }
    }

    // Properly overriding the abstract method from ResettableObject
    public void ActivateHitbox()
    {
        if (hitbox == null)
        {
            Debug.LogError("Hitbox not assigned");
        }
        else
        {
            hitbox.enabled = true;  // Enable the hitbox when the skull is thrown
        }
    }

    protected new void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D (collision);
        if (isThrown) // Ensure 'isThrown' is properly declared and managed
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                if (hitbox != null)
                {
                    hitbox.enabled = false; // Disable the hitbox when the skull hits the ground
                }
                isThrown = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitbox != null && hitbox.enabled && other.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            other.GetComponent<EnemyController> ().kill ();
            //Destroy(other.gameObject); // Destroy enemy on contact
        }
    }
}