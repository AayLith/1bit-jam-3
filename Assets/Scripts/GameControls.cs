using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControls : MonoBehaviour
{
    private void Update ()
    {
        if ( Input.GetButtonUp ( "Reset" ) )
            NotificationCenter.instance.PostNotification ( this , Notification.notifications.resetlevel );
    }
}
