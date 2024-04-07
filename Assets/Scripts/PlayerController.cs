using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] bool isGrounded = false;
    int groundCollisions = 0;
    bool lockInput = false;


    [Header ( "Stats" )]
    [SerializeField] float defaultSpeed;
    public float speed;
    [SerializeField] float defaultJump;
    public float jump;

    public GameObject shellSlot;
    public Shell shell;
    [SerializeField] ItemCollision groundShell;

    private void Awake ()
    {
        instance = this;
        speed = defaultSpeed;
        jump = defaultJump;
    }

    private void Update ()
    {
        // Update positions
        transform.rotation = Quaternion.identity;
        if ( shell )
        {
            shell.transform.position = shellSlot.transform.position;
            shell.transform.rotation = Quaternion.identity;
        }

        // Walk
        if ( isGrounded && Input.GetAxis ( "Horizontal" ) > 0.1f || isGrounded && Input.GetAxis ( "Horizontal" ) < -0.1f )
        {
            GetComponent<Rigidbody2D> ().velocity = new Vector2 ( Input.GetAxis ( "Horizontal" ) * speed , GetComponent<Rigidbody2D> ().velocity.y );
            transform.localScale = new Vector3 ( Mathf.Sign ( GetComponent<Rigidbody2D> ().velocity.x ) , 1 , 1 );
        }

        // Jump
        if ( isGrounded && Input.GetAxis ( "Jump" ) > 0.1f )
            GetComponent<Rigidbody2D> ().velocity = new Vector2 ( GetComponent<Rigidbody2D> ().velocity.x , jump );

        // Pickup shell
        if ( !lockInput && groundShell.item != null && Input.GetAxis ( "Submit" ) > 0.1f )
        {
            // if already has a shell
            if ( shell != null )
            {
                shell.GetComponent<Rigidbody2D> ().velocity = new Vector2 ( Mathf.Sign ( transform.localScale.x ) * 0.5f , 1 );
                dropShell ();
            }

            // pickup the shell
            pickUpShell ( groundShell.item );

            StartCoroutine ( lockInputsDelay () );
        }

        // Throw shell
        if ( !lockInput && shell != null && Input.GetAxis ( "Fire1" ) > 0.1f )
        {
            shell.GetComponent<Rigidbody2D> ().velocity = new Vector2 ( Mathf.Sign ( transform.localScale.x ) * 10f , 2f );
            dropShell ();
            StartCoroutine ( lockInputsDelay () );
        }
    }

    void pickUpShell ( Shell s )
    {
        shell = s;
        shell.transform.parent = shellSlot.transform;
        shell.transform.position = shellSlot.transform.position;
        shell.transform.rotation = Quaternion.identity;
        shell.GetComponent<Rigidbody2D> ().gravityScale = 0;
        shell.playerCollision.GetComponent<Collider2D> ().isTrigger = true;
    }

    void dropShell ()
    {
        shell.transform.parent = null;
        shell.GetComponent<Rigidbody2D> ().gravityScale = 1;
        shell = null;
    }

    IEnumerator lockInputsDelay ()
    {
        lockInput = true;
        yield return new WaitForSeconds ( 0.1f );
        lockInput = false;
    }

    private void OnCollisionEnter2D ( Collision2D collision )
    {
        if ( collision.gameObject.layer == LayerMask.NameToLayer ( "Ground" ) )
            groundCollisions++;
        if ( groundCollisions > 0 )
            isGrounded = true;
    }

    private void OnCollisionExit2D ( Collision2D collision )
    {
        if ( collision.gameObject.layer == LayerMask.NameToLayer ( "Ground" ) )
            groundCollisions--;
        if ( groundCollisions == 0 )
            isGrounded = false;
    }
}
