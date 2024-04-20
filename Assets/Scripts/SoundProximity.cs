using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundProximity : MonoBehaviour
{
    private AudioSource audioSource; // Reference to the AudioSource component
    private Transform player; // Player's transform

    public float maxVolume = 1.0f; // Maximum volume of the audio source
    public float maxDistance = 5.0f; // Max distance at which the audio is at max volume

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        FindPlayer();
    }

    void Update()
    {
        if (player == null) // Check if player has been destroyed or needs to be reassigned
        {
            FindPlayer();
        }

        if (audioSource != null && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            float volume = Mathf.Clamp01(1 - (distance / maxDistance));
            audioSource.volume = volume * maxVolume;
        }
    }

    // Finds the player by tag and sets the player transform
    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found, please ensure your player has the 'Player' tag.");
        }
    }
}