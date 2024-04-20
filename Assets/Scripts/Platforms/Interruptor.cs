using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interruptor : ResettableObject
{
    public GameObject[] activatableObjects = new GameObject[4];
    private IsActivatable[] activatables = new IsActivatable[4];

    public Sprite inactiveSprite; // Sprite when the interruptor is not pressed
    public Sprite activeSprite;   // Sprite when the interruptor is pressed
    private SpriteRenderer spriteRenderer;
    private bool isActive = false; // Tracks the active state of the interruptor
    private bool isPlayerPresent = false; // Tracks if the player is currently on the interruptor

    // Initial states to reset to
    private Sprite initialSprite;
    private bool initialActiveState;

    public AudioClip activationSound; // Sound when the interruptor is activated
    public AudioClip deactivationSound; // Sound when the interruptor is deactivated
    public AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialSprite = inactiveSprite; // Initially inactive
            spriteRenderer.sprite = initialSprite;
        }

        // Save the initial active state
        initialActiveState = isActive;
    }

    private new void Start()
    {
        base.Start();

        for (int i = 0; i < activatableObjects.Length; i++)
        {
            if (activatableObjects[i] != null)
            {
                activatables[i] = activatableObjects[i].GetComponent<IsActivatable>();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.layer == 8 || other.gameObject.layer == 23) && !isPlayerPresent) // Check for layer 8 or layer 23
        {
            isPlayerPresent = true;
            TogglePlate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 23) // Check for layer 8 or layer 23
        {
            isPlayerPresent = false;
        }
    }

    private void TogglePlate()
    {
        isActive = !isActive;
        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;

        audioSource.clip = isActive ? activationSound : deactivationSound; // Set the appropriate sound
        audioSource.Play();

        foreach (var activatable in activatables)
        {
            if (activatable != null)
            {
                activatable.ToggleActivation();
            }
        }
    }

    protected override void reset()
    {
        base.reset();
        spriteRenderer.sprite = initialSprite;
        isActive = initialActiveState;
        isPlayerPresent = false;

    }

    public override void receiveNotification(Notification notification)
    {
        if (notification.name == Notification.notifications.resetlevel)
        {
            reset();
        }
    }

    void OnDrawGizmos()
    {
        foreach (var obj in activatableObjects)
        {
            if (obj != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, obj.transform.position);
            }
        }
    }
}