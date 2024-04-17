using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResettableObject : MonoBehaviour, NotificationReceiver
{
    protected Vector3 startPosition;
    protected Quaternion startRotation;
    protected Vector2 startVelocity;

    protected virtual void Awake ()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        var rb = GetComponent<Rigidbody2D> ();
        if (rb != null)
        {
            startVelocity = rb.velocity;
        }
        
    }

    protected void Start ()
    {
        NotificationCenter.instance.AddObserver ( this , Notification.notifications.resetlevel );
    }

    protected virtual void reset ()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        var rb = GetComponent<Rigidbody2D> ();
        if( rb != null )
        {
            rb.velocity = startVelocity;
        }
        
    }

    public abstract void receiveNotification ( Notification notification );
}
