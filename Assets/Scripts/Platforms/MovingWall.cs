using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour, IsActivatable
{
    public Vector3 destinationOffset;
    public float speed = 2.0f;
    public bool isActive = true;

    private Vector3 startingPoint;
    private Vector3 destinationPoint;
    private GameObject playerObject = null;

    void Start()
    {
        startingPoint = transform.position;
        destinationPoint = transform.position + destinationOffset;
    }

    void Update()
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8) // Check if object is on layer 8
        {
            playerObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            if (playerObject == other.gameObject)
            {
                playerObject.transform.parent = null;
                playerObject = null;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8) // Check if object is on layer 8
        {
            playerObject = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if (playerObject == collision.gameObject)
            {
                playerObject.transform.parent = null;
                playerObject = null;
            }
        }
    }
}