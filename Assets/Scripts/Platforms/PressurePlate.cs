using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    // Reference to the MovingPlatform script
    public MovingPlatform movingPlatform;

    private int numColliders = 0;

    // Called when a collider enters the pressure plate collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is on layer 8 (playerphysics layer)
        if (other.gameObject.layer == 8)
        {
            numColliders++;
            // Toggle the activation of the moving platform and this is the first collider entering
            if (movingPlatform != null && numColliders == 1)
            {
                movingPlatform.ToggleActivation();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            numColliders--;
            // Untoggle the activation of the moving platform if there are no more colliders in contact
            if (movingPlatform != null && numColliders == 0)
            {
                movingPlatform.ToggleActivation();
            }
        }
    }

    //  // Draw a purple line in the editor between the pressure plate and the designated moving platform
    void OnDrawGizmos()
    {
        if (movingPlatform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, movingPlatform.transform.position);
        }
    }
}