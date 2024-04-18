using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PressurePlate : ResettableObject
{
    public GameObject[] activatableObjects = new GameObject[4];
    private IsActivatable[] activatables = new IsActivatable[4];

    public Sprite inactiveSprite; // Sprite when the plate is not pressed
    public Sprite activeSprite;   // Sprite when the plate is pressed
    private SpriteRenderer spriteRenderer;

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
            Debug.Log("Pressure Plate Initialized: Sprite set to initial.");
        }
    }

    private void Start()
    {
        base.Start();
        // Initialize state
        isPressed = false;
        Debug.Log("Pressure Plate Start: isPressed set to false.");

        for (int i = 0; i < activatableObjects.Length; i++)
        {
            if (activatableObjects[i] != null)
            {
                activatables[i] = activatableObjects[i].GetComponent<IsActivatable>();
                Debug.Log("Activatable object [" + i + "] set.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 23)
        {
            UpdateObjectsOnPlate(1);
            Debug.Log("Object entered: Total objects on plate now = " + objectsOnPlate);
            if (!isPressed && objectsOnPlate > 0)
            {
                TogglePlate(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 23)
        {
            UpdateObjectsOnPlate(-1);
            Debug.Log("Object exited: Total objects on plate now = " + objectsOnPlate);
            if (isPressed && objectsOnPlate == 0)
            {
                TogglePlate(false);
            }
        }
    }

    private void UpdateObjectsOnPlate(int change)
    {
        objectsOnPlate += change;
        if (objectsOnPlate < 0)
        {
            objectsOnPlate = 0; // Prevent objectsOnPlate from going negative
        }
        Debug.Log("UpdateObjectsOnPlate: Change = " + change + ", New Count = " + objectsOnPlate);
    }

    public int GetObjectsOnPlate()
    {
        return objectsOnPlate;
    }

    private void TogglePlate(bool pressed)
    {
        if (isPressed != pressed)  // Check if there is a state change
        {
            isPressed = pressed;
            spriteRenderer.sprite = isPressed ? activeSprite : inactiveSprite;
            Debug.Log("Toggle Plate: isPressed = " + isPressed);

            foreach (var activatable in activatables)
            {
                if (activatable != null)
                {
                    activatable.ToggleActivation();
                }
            }
        }
    }

    protected override void reset()
    {
        base.reset();
        isPressed = false;
        objectsOnPlate = 0;
        spriteRenderer.sprite = initialSprite; // Reset to initial sprite
        TogglePlate(false); // Ensure the plate is visually and functionally reset
        Debug.Log("Pressure Plate Reset: State reset successfully.");
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