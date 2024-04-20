using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshadeShell : Shell
{
    public float fallSpeed;
    public float gravityFallSpeed = 1;
    public AudioClip fallsound;
    public AudioSource fallSource;
    public float volume = 0.3f;
    PlayerControls rb;
    bool play = false;

    protected override void Awake ()
    {
        base.Awake ();
        rb = FindObjectOfType<PlayerControls> ();
        fallSource.volume = volume;
    }

    private void Update ()
    {
        if ( rb._velocity.y > 0 || rb._controller.isGrounded)
        {
            fallSource.volume = 0;
        }
        else
        {
            fallSource.volume = volume;
        }
    }

    public override void onEquip ( PlayerControls p )
    {
        base.onEquip ( p );
        p.fallSpeedModifier = fallSpeed;
        p.gravityFallModifier = gravityFallSpeed;
        fallSource.clip = fallsound;
        fallSource.Play ();
        play = true;
        //StartCoroutine ( playSound () );
    }

    public override void onUnequip ( PlayerControls p )
    {
        base.onUnequip ( p );
        p.fallSpeedModifier = 1;
        p.gravityFallModifier = 1;
        //StopAllCoroutines ();
        play = false;
        fallSource.Stop ();
    }

    IEnumerator playSound ()
    {
        yield return null;
        Debug.Log ( rb._velocity.y );
        if ( rb._velocity.y > 0 )
        {
            fallSource.Stop ();
            fallSource.volume = 0;
        }
        else
        {
            fallSource.Play ();
            fallSource.volume = volume;
        }
    }
}
