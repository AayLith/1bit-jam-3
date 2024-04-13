using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Detects Collisions between a gameobject tagger as "Player" and checkpoints
public class Checkpoint : MonoBehaviour
{
    // Variable to keep track of the last activated checkpoint's transform.
    public static Transform lastCheckpoint;

    [SerializeField]
    private bool isStartingCheckpoint = false;  // Identifies the starting checkpoint. Set this in Unity's Inspector.

    void Start()
    {
        // If this checkpoint is set as the starting one, initialize it as the last checkpoint.
        if (isStartingCheckpoint)
        {
            lastCheckpoint = transform;  // Set this checkpoint as the last active one.
            // Debug position of the default starting checkpoint.
            Debug.Log($"Default checkpoint set at: {transform.position}");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is tagged as "Player".
        if (other.CompareTag("Player"))
        {
            lastCheckpoint = transform;  // Update the lastCheckpoint to this checkpoint's transform.
            // Debug when a checkpoint is activated.
            Debug.Log($"Checkpoint activated at: {transform.position} by {other.gameObject.name}");
        }
    }
}