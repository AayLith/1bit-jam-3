using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject[] activatableObjects = new GameObject[4];
    private IsActivatable[] activatables = new IsActivatable[4];
    private int numColliders = 0;

    public Sprite inactiveSprite; // Sprite when the plate is not pressed
    public Sprite activeSprite;   // Sprite when the plate is pressed
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No SpriteRenderer component found on the pressure plate object.");
            return;
        }

        spriteRenderer.sprite = inactiveSprite; // Set the initial sprite to inactive

        // get the IsActivatable component from each assigned GameObject
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
        if (other.gameObject.layer == 8 || other.gameObject.layer == 23) // Check for layer 8 or layer 23
        {
            numColliders++;
            if (numColliders == 1)
            {
                spriteRenderer.sprite = activeSprite; // Change to active sprite
                foreach (var activatable in activatables)
                {
                    if (activatable != null)
                    {
                        activatable.ToggleActivation();
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 23) // Check for layer 8 or layer 23
        {
            numColliders--;
            if (numColliders == 0)
            {
                spriteRenderer.sprite = inactiveSprite; // Revert to inactive sprite
                foreach (var activatable in activatables)
                {
                    if (activatable != null)
                    {
                        activatable.ToggleActivation();
                    }
                }
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