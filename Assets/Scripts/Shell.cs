using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : ResetableObject
{
    public Collider2D playerCollision;
    public Collider2D groundCollision;
    public enum shellHeights { small,medium,tall}
    public shellHeights shellHeight;

    public void outlineOn ()
    {

    }

    public void outlineOff ()
    {

    }

    public override void receiveNotification ( Notification notification )
    {
        switch ( notification.name )
        {
            case Notification.notifications.resetlevel:
                reset ();
                break;
        }
    }
}
