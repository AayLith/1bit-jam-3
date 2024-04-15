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
        if (playerObject != null)
        {
            playerObject.transform.parent = isActive ? transform : null;
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
        if (other.CompareTag("Player"))
        {
            playerObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerObject.transform.parent = null;
            playerObject = null;
        }
    }
}