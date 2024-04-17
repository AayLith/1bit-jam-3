using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moray : ResettableObject
{
    [SerializeField] protected Collision2DTriggerRecorder wallDetector;
    [SerializeField] protected PlayerDetector playerDetector;
    protected bool wallHit = false;

    [SerializeField] protected Transform moraySprite;
    [SerializeField] protected float morayRange = 3;
    [SerializeField] protected float exitSpeed = 1;
    [SerializeField] protected float enterSpeed = 1;
    [SerializeField] protected Vector3 morayStartPos;
    [SerializeField] protected Vector3 morayEndPos;
    [SerializeField] protected float moveTime = 0;
    [SerializeField] protected float recoveryDelay = 0.5f;
    protected float enterTime = 0;

    protected override void Awake ()
    {
        base.Awake ();
        morayStartPos = moraySprite.position;
        morayEndPos = morayStartPos + Vector3.right * morayRange;
    }

    void Update ()
    {
        if ( wallDetector.collisions.Count > 0 )
            wallHit = true;

        if ( wallHit )
            backInFully ();
        else if ( playerDetector.hitPlayer )
            jumpOut ();
        else
            backIn ();
    }

    void jumpOut ()
    {
        moveTime = Mathf.Min ( moveTime + Time.deltaTime * exitSpeed , 1 );
        moraySprite.position = Vector3.Lerp ( morayStartPos , morayEndPos , moveTime );
    }

    void backIn ()
    {
        moveTime = Mathf.Max ( moveTime - Time.deltaTime * enterSpeed , 0 );
        moraySprite.position = Vector3.Lerp ( morayStartPos , morayEndPos , moveTime );
    }

    void backInFully ()
    {
        backIn ();
        if ( moraySprite.position == morayStartPos )
        {
            enterTime += Time.deltaTime;
            if ( enterTime > recoveryDelay )
            {
                enterTime = 0;
                wallHit = false;
            }
        }
    }

    public override void receiveNotification ( Notification notification )
    {
        switch ( notification.name )
        {
            case Notification.notifications.resetlevel:
                break;
        }
    }

    protected override void reset ()
    {
        base.reset ();
        moveTime = 0;
        moraySprite.position = morayStartPos;
        wallHit = false;
    }
}
