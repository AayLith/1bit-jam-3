using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision2DRecorder : MonoBehaviour
{
    public List<Collision2D> collisions = new List<Collision2D> ();

    private void OnCollisionEnter2D ( Collision2D collision )
    {
        collisions.Add ( collision );
    }
    private void OnCollisionExit2D ( Collision2D collision )
    {
        collisions.Remove ( collision );
    }
}
