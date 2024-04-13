using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Detects collisions with game objects in the "Trap" layer (11).
public class TrapCollisionDetector : MonoBehaviour
{
    //replaced with damage dealing, moved to PlayerControls.OnDeath. Use damagedealer with damageToPlayer set instead
    // void OnTriggerEnter2D(Collider2D collider)
    // {
    //     // If the game object's layer is "Trap" layer (11).
    //     if (collider.gameObject.layer == 11)
    //     {
    //         Debug.Log("Collision with a Trap!");

            
    //     }
    // }
}