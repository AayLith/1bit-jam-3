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
    private GameObject playerObject = null;
    private bool initialActiveState; // Store the initial active state

    protected override void Awake()
    {
        base.Awake();
        startingPoint = transform.position;
        destinationPoint = transform.position + destinationOffset;
        initialActiveState = isActive; // Store initial active state at Awake
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

        UpdatePlayerParenting();  // Always update parenting regardless of isActive state
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
        if (playerObject != null)
        {
            playerObject.transform.parent = transform; // Always parent the playerObject
        }
    }

    public void ToggleActivation()
    {
        isActive = !isActive;
    }

    protected override void reset()
    {
        base.reset(); // Reset position
        destinationPoint = transform.position + destinationOffset; // Recalculate the destination
        isActive = initialActiveState; // Reset to initial active state
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