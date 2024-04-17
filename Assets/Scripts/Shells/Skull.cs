using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Shell
{
    [Header("Hitbox")]
    public Collider2D hitbox; // Assign this in the Unity editor

    void Start()
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

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (isThrown) // Ensure 'isThrown' is properly declared and managed
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
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
            Destroy(other.gameObject); // Destroy enemy on contact
        }
    }
}