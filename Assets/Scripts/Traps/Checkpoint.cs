using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Detects Collisions between a gameobject tagger as "Player" and checkpoints
public class Checkpoint : MonoBehaviour
{
    // Variable to keep track of the last activated checkpoint's transform.
    public static Transform lastCheckpoint;
    private AudioSource checkpointAudio;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private bool isStartingCheckpoint = false;
    [SerializeField] 
    private Color activationColor = new Color(0.870588235f, 0.862745098f, 0.694117647f, 1f);// Identifies the starting checkpoint. Set this in Unity's Inspector.

    void Start()
    {
        checkpointAudio = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (other.CompareTag("Player") && transform != lastCheckpoint && isStartingCheckpoint == false)
        {
            lastCheckpoint = transform;  // Update the lastCheckpoint to this checkpoint's transform.
            checkpointAudio.Play();  // Play the sound effect for activating the checkpoint.
            Debug.Log($"Checkpoint activated at: {transform.position} by {other.gameObject.name}");
        }
    }
}