using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    PlayerControls player;

    public bool hitPlayer = false;

    void Start()
    {
        player = FindObjectOfType<PlayerControls> ();
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if ( collision.gameObject == player.gameObject )
            hitPlayer = true;
    }

    private void OnTriggerExit2D ( Collider2D collision )
    {
        if ( collision.gameObject == player.gameObject )
            hitPlayer = false;
    }
}
