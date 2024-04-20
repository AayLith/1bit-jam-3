using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Detects Collisions between a gameobject tagger as "Player" and checkpoints
public class Checkpoint : MonoBehaviour
{
    // Variable to keep track of the last activated checkpoint's transform.
    public static Transform lastCheckpoint;
    private AudioSource checkpointAudio;
    public SpriteRenderer spriteRenderer;

    public Sprite activeSprite;  
    public Sprite inactiveSprite;

    [SerializeField]
    private bool isStartingCheckpoint = false;
    

    void Start()
    {
        checkpointAudio = GetComponent<AudioSource>();

        // Set the initial sprite based on whether this is the starting checkpoint
        if (isStartingCheckpoint)
        {
            lastCheckpoint = transform;
            spriteRenderer.enabled = false;
            Debug.Log($"Default checkpoint set at: {transform.position}");
        }
        else
        {
            spriteRenderer.sprite = inactiveSprite;
            spriteRenderer.enabled = true;
        }
    }

    void Update()
    {
        // Check if this checkpoint is currently the active one and update sprite accordingly
        if (transform == lastCheckpoint)
        {
            spriteRenderer.sprite = activeSprite;
        }
        else
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && transform != lastCheckpoint && !isStartingCheckpoint)
        {
            lastCheckpoint = transform;  // Update the lastCheckpoint to this checkpoint's transform.
            checkpointAudio.Play();  // Play the sound effect for activating the checkpoint.
            Debug.Log($"Checkpoint activated at: {transform.position} by {other.gameObject.name}");

            // Inform all other checkpoints to update their state
            Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
            foreach (Checkpoint chk in allCheckpoints)
            {
                if (chk != this)
                {
                    chk.UpdateCheckpointState();
                }
            }
        }
    }

    public void UpdateCheckpointState()
    {
        if (transform != lastCheckpoint)
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }
}