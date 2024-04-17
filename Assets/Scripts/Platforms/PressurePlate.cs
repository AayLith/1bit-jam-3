using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : ResettableObject
{
    public GameObject[] activatableObjects = new GameObject[4];
    private IsActivatable[] activatables = new IsActivatable[4];

    public Sprite inactiveSprite; // Sprite when the plate is not pressed
    public Sprite activeSprite;   // Sprite when the plate is pressed
    private SpriteRenderer spriteRenderer;

    // Initial states to reset to
    private Sprite initialSprite;
    private bool isPressed;
    private int objectsOnPlate = 0;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialSprite = spriteRenderer.sprite; // Store the initial sprite
        }
    }

    private void Start()
    {
        base.Start();

        // Store the initial state
        isPressed = false;

        // Get the IsActivatable component from each assigned GameObject
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
        if (other.gameObject.layer == 8 || other.gameObject.layer == 23) // Check for layer 8 or layer 23
        {
            objectsOnPlate++;
            if (!isPressed)
            {
                TogglePlate(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 23) // Check for layer 8 or layer 23
        {
            objectsOnPlate--;
            if (isPressed && objectsOnPlate == 0)
            {
                TogglePlate(false);
            }
        }
    }

    private void TogglePlate(bool pressed)
    {
        isPressed = pressed;
        spriteRenderer.sprite = isPressed ? activeSprite : inactiveSprite;

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
        base.reset(); // Reset any required properties from base class
        spriteRenderer.sprite = initialSprite; // Reset to initial sprite
        isPressed = false; // Reset plate state
        objectsOnPlate = 0; // Reset object count
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