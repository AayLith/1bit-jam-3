using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : ResettableObject, IsActivatable
{
    public Vector3 destinationOffset;
    public float speed = 2.0f;
    public bool isActive = true;

    private Vector3 startingPoint;
    private Vector3 destinationPoint;
    private List<GameObject> objectsOnWall = new List<GameObject>();
    private bool initialActiveState;

    protected override void Awake()
    {
        base.Awake();
        startingPoint = transform.position;
        destinationPoint = transform.position + destinationOffset;
        initialActiveState = isActive;
    }

    new void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        Vector3 targetPoint = isActive ? destinationPoint : startingPoint;

        if (Vector3.Distance(transform.position, targetPoint) > 0.01f)
        {
            MoveWall(targetPoint);
        }

        UpdatePlayerParenting();
    }

    private void MoveWall(Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPoint) <= 0.01f)
        {
            transform.position = targetPoint;
        }
    }

    private void UpdatePlayerParenting()
    {
        foreach (var obj in objectsOnWall)
        {
            obj.transform.parent = transform;
        }
    }

    public void ToggleActivation()
    {
        isActive = !isActive;
    }

    protected override void reset()
    {
        base.reset();
        transform.position = startingPoint;
        destinationPoint = startingPoint + destinationOffset;
        isActive = initialActiveState;
    }

    public override void receiveNotification(Notification notification)
    {
        if (notification.name == Notification.notifications.resetlevel)
        {
            reset();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            objectsOnWall.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            other.transform.parent = null;
            objectsOnWall.Remove(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            objectsOnWall.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            collision.gameObject.transform.parent = null;
            objectsOnWall.Remove(collision.gameObject);
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