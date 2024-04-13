using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Detects collisions with game objects in the "Trap" layer (11).
public class TrapCollisionDetector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        // If the game object's layer is "Trap" layer (11).
        if (collider.gameObject.layer == 11)
        {
            Debug.Log("Collision with a Trap!");

            // Check if there is a last activated checkpoint.
            if (Checkpoint.lastCheckpoint != null)
            {
                // Teleport the player to the last activated checkpoint.
                transform.position = Checkpoint.lastCheckpoint.position;
                Debug.Log("Teleported to the last activated checkpoint at: " + Checkpoint.lastCheckpoint.position);
            }
            else
            {
                Debug.LogWarning("No checkpoint has been activated yet.");
            }
        }
    }
}