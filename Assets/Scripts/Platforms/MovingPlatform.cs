using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IsActivatable
{
    // Offset from starting point to destination point
    public Vector3 destinationOffset;

    // Constant speed of platform movement
    public float speed = 2.0f;

    // platform activation toggle
    public bool isActive = true;

    private Vector3 startingPoint;
    private Vector3 destinationPoint;

    // Reference to the player object
    private GameObject playerObject;

    void Start()
    {
        // Initialize the starting and destination points
        startingPoint = transform.position;
        destinationPoint = transform.position + destinationOffset;
    }

    void Update()
    {
        if (!isActive)
            return;

        // If the player is on the platform, parent it to move with the platform
        if (playerObject != null)
        {
            playerObject.transform.parent = transform;
        }
        else // If the player is not on the platform, unparent it
        {
            if (playerObject != null && playerObject.transform.parent == transform)
            {
                playerObject.transform.parent = null;
            }
        }

        Vector3 direction = (destinationPoint - startingPoint).normalized;
        float distanceToDestination = Vector3.Distance(transform.position, destinationPoint);

        float movementAmount = speed * Time.deltaTime;
        if (movementAmount >= distanceToDestination)
        {
            transform.position = destinationPoint;
            // Swap starting and destination points to create continuous movement
            Vector3 temp = startingPoint;
            startingPoint = destinationPoint;
            destinationPoint = temp;
        }
        else // Move towards the destination with constant speed
        {
            transform.position += direction * movementAmount;
        }
    }

    // Draw representation of colliders and pathing in the editor
    void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            // green cube for the starting point
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);

            Vector3 destinationCenter = collider.bounds.center + destinationOffset;

            // red cube for the destination point
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(destinationCenter, collider.bounds.size);

            // yellow line for the pathing
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(collider.bounds.center, destinationCenter);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Set the player object reference
            playerObject = other.gameObject;
        }
    }

    // When a collider exits the platform
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // If the player was previously parented by the platform, unparent it
            if (playerObject != null && playerObject.transform.parent == transform)
            {
                playerObject.transform.parent = null;
            }
            // Reset the player object reference
            playerObject = null;
        }
    }

    // Toggle platform activation as defined by the IsActivatable interface
    public void ToggleActivation()
    {
        isActive = !isActive;
    }
}