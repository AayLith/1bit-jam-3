using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interruptor : MonoBehaviour
{
    public GameObject[] activatableObjects = new GameObject[4];
    private IsActivatable[] activatables = new IsActivatable[4];

    public Sprite inactiveSprite; // Sprite when the interruptor is not pressed
    public Sprite activeSprite;   // Sprite when the interruptor is pressed
    private SpriteRenderer spriteRenderer;
    private bool isActive = false; // Tracks the active state of the interruptor
    private bool isPlayerPresent = false; // Tracks if the player is currently on the interruptor

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = inactiveSprite; // Set the initial sprite to inactive

        // Get the IsActivatable component from each assigned GameObject
        for (int i = 0; i < activatableObjects.Length; i++)
        {
            if (activatableObjects[i] != null)
            {
                activatables[i] = activatableObjects[i].GetComponent<IsActivatable>();
                if (activatables[i] == null)
                {
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 && !isPlayerPresent) // Check layer and if player is not already present
        {
            isPlayerPresent = true;
            TogglePlate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            isPlayerPresent = false;
        }
    }

    private void TogglePlate()
    {
        isActive = !isActive;
        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;

        foreach (var activatable in activatables)
        {
            if (activatable != null)
            {
                activatable.ToggleActivation();
            }
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