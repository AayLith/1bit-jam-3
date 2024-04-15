using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshadeShell : Shell
{
    public float gravity;

    public override void onEquip ( PlayerControls p )
    {
        base.onEquip ( p );
        p.gravity = p.defaultGravity * gravity;
    }

    public override void onUnequip ( PlayerControls p )
    {
        base.onUnequip ( p );
        p.gravity = p.defaultGravity;
    }
}
