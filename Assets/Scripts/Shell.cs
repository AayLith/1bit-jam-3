using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Collider2D playerCollision;
    public float maxFallSpeed = -3;
    Rigidbody2D rb;

    private void Awake ()
    {
        rb = GetComponent<Rigidbody2D> ();
    }

    private void Update ()
    {
        transform.rotation = Quaternion.identity;
        rb.velocity = new Vector2 ( rb.velocity.x , Mathf.Max ( rb.velocity.y , maxFallSpeed ) );
    }

    public void outlineOn ()
    {

    }

    public void outlineOff ()
    {

    }
}
