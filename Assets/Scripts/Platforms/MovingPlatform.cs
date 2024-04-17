using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : ResettableObject, IsActivatable
{
    public Vector3 destinationOffset;
    public float speed = 2.0f;
    public bool isActive = true;

    private Vector3 startingPoint;
    private Vector3 destinationPoint;
    private bool initialActiveState;
    private Vector3 initialPosition;
    private List<GameObject> objectsOnPlatform = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        initialPosition = transform.position;
        startingPoint = initialPosition;
        destinationPoint = initialPosition + destinationOffset;
        initialActiveState = isActive;
    }

    void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (!isActive)
            return;

        foreach (var obj in objectsOnPlatform)
        {
            obj.transform.parent = transform;
        }

        Vector3 direction = (destinationPoint - startingPoint).normalized;
        float distanceToDestination = Vector3.Distance(transform.position, destinationPoint);
        float movementAmount = speed * Time.deltaTime;

        if (movementAmount >= distanceToDestination)
        {
            transform.position = destinationPoint;
            Vector3 temp = startingPoint;
            startingPoint = destinationPoint;
            destinationPoint = temp;
        }
        else
        {
            transform.position += direction * movementAmount;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            objectsOnPlatform.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            other.transform.parent = null;
            objectsOnPlatform.Remove(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            objectsOnPlatform.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            collision.gameObject.transform.parent = null;
            objectsOnPlatform.Remove(collision.gameObject);
        }
    }

    public void ToggleActivation()
    {
        isActive = !isActive;
    }

    protected override void reset()
    {
        base.reset();
        transform.position = initialPosition;
        startingPoint = initialPosition;
        destinationPoint = initialPosition + destinationOffset;
        isActive = initialActiveState;
    }

    public override void receiveNotification(Notification notification)
    {
        if (notification.name == Notification.notifications.resetlevel)
        {
            reset();
        }
    }

    void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            Vector3 destinationCenter = collider.bounds.center + destinationOffset;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(destinationCenter, collider.bounds.size);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(collider.bounds.center, destinationCenter);
        }
    }
}