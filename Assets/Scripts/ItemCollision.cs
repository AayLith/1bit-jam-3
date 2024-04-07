using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollision : MonoBehaviour
{
    public Shell item;

    private void OnCollisionEnter2D ( Collision2D collision )
    {
        if ( collision.gameObject.layer == LayerMask.NameToLayer ( "item x pickup" ) )
        {
            if ( item != null )
                item.outlineOff ();
            item = collision.transform.parent.gameObject.GetComponent<Shell> ();
            item.outlineOn ();
        }
    }

    private void OnCollisionExit2D ( Collision2D collision )
    {
        if ( collision.transform.parent.gameObject.layer == LayerMask.NameToLayer ( "item x pickup" ) )
        {
            item.outlineOff ();
            item = null;
        }
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if ( collision.gameObject.layer == LayerMask.NameToLayer ( "item x pickup" ) )
        {
            if ( item != null )
                item.outlineOff ();
            item = collision.transform.parent.gameObject.GetComponent<Shell> ();
            item.outlineOn ();
        }
    }

    private void OnTriggerExit2D ( Collider2D collision )
    {
        if ( collision.transform.parent.gameObject.layer == LayerMask.NameToLayer ( "item x pickup" ) )
        {
            item.outlineOff ();
            item = null;
        }
    }
}
