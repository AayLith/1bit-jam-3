using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleStreamer : MonoBehaviour
{
    public float push = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerControls = collision.GetComponent<PlayerControls>();
        if (playerControls != null)
        {
            playerControls.gravity = Mathf.Abs(push);
        }
        else
        {
            var rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = Mathf.Abs(rb.gravityScale) * -1;
            }
            else
            {
                Debug.LogError("No PlayerControls or Rigidbody2D found on the colliding object.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var playerControls = collision.GetComponent<PlayerControls>();
        if (playerControls != null)
        {
            playerControls.gravity = playerControls.defaultGravity;
        }
        else
        {
            var rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = Mathf.Abs(rb.gravityScale);
            }
            else
            {
                Debug.LogError("No PlayerControls or Rigidbody2D found on the colliding object.");
            }
        }
    }
}