using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Tooltip("Assign GameObjects with components that implement the IActivatable interface. Can assign up to 4 objects.")]
    public GameObject[] activatableObjects = new GameObject[4];

    private IsActivatable[] activatables = new IsActivatable[4];
    private int numColliders = 0;

    private void Start()
    {
        // get the IsActivatable component from each assigned GameObject
        for (int i = 0; i < activatableObjects.Length; i++)
        {
            if (activatableObjects[i] != null)
            {
                activatables[i] = activatableObjects[i].GetComponent<IsActivatable>();
                if (activatables[i] == null)
                {
                    Debug.LogWarning("Assigned GameObject at index " + i + " does not contain a component implementing IActivatable");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)  // Assuming layer 8 is the player and shells layer
        {
            numColliders++;
            if (numColliders == 1)
            {
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
        if (other.gameObject.layer == 8)
        {
            numColliders--;
            if (numColliders == 0)
            {
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