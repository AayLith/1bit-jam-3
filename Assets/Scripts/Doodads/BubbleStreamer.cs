using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BublleStreamer : MonoBehaviour
{
    public float push = 10;

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        try
        {
            Debug.Log ( collision.name );
            PlayerControls pc = collision.GetComponent<PlayerControls> ();
            pc.gravity = Mathf.Abs ( push );
            return;
        }
        catch { }

        try
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D> ();
            rb.gravityScale = Mathf.Abs ( rb.gravityScale ) * -1;
            return;
        }
        catch { }
    }

    private void OnTriggerExit2D ( Collider2D collision )
    {
        try
        {
            PlayerControls pc = collision.GetComponent<PlayerControls> ();
            pc.gravity = pc.defaultGravity;
            return;
        }
        catch { }

        try
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D> ();
            rb.gravityScale = Mathf.Abs ( rb.gravityScale );
            return;
        }
        catch { }
    }
}
