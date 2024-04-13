using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 destinationOffset; // Offset from starting point to destination point
    public float speed = 2.0f; // Constant speed of platform movement

    private Vector3 startingPoint; // Starting point of the platform
    private Vector3 destinationPoint; // Destination point of the platform
    private GameObject playerObject; // Reference to the player object

    void Start()
    {
        // Initialize the starting and destination points
        startingPoint = transform.position;
        destinationPoint = transform.position + destinationOffset;
    }

    void Update()
    {
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

        // Calculate the direction and distance to the destination
        Vector3 direction = (destinationPoint - startingPoint).normalized;
        float distanceToDestination = Vector3.Distance(transform.position, destinationPoint);

        // Calculate the movement amount based on the constant speed
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

    void OnDrawGizmos()
    {
        // Access the BoxCollider2D component
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        // Draw a visual representation of the collider in the editor
        if (collider != null)
        {
            // Draw a green cube for the starting point
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);

            Vector3 destinationCenter = collider.bounds.center + destinationOffset;

            // Draw a red cube for the destination point
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(destinationCenter, collider.bounds.size);

            // Draw a yellow line between both points
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(collider.bounds.center, destinationCenter);
        }
    }

    // On collision with another collider
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
}