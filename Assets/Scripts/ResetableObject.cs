using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResetableObject : MonoBehaviour, NotificationReceiver
{
    protected Vector3 startPosition;
    protected Quaternion startRotation;
    protected Vector2 startVelocity;

    private void Awake ()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startVelocity = GetComponent<Rigidbody2D> ().velocity;
    }

    private void Start ()
    {
        NotificationCenter.instance.AddObserver ( this , Notification.notifications.resetlevel );
    }

    protected virtual void reset ()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        GetComponent<Rigidbody2D> ().velocity = startVelocity;
    }

    public abstract void receiveNotification ( Notification notification );
}
