using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshadeShell : Shell
{
    public float fallSpeed;
    public float gravityFallSpeed = 1;

    public override void onEquip ( PlayerControls p )
    {
        base.onEquip ( p );
        p.fallSpeedModifier = fallSpeed;
        p.gravityFallModifier = gravityFallSpeed;
    }

    public override void onUnequip ( PlayerControls p )
    {
        base.onUnequip ( p );
        p.fallSpeedModifier = 1;
        p.gravityFallModifier = 1;
    }
}
