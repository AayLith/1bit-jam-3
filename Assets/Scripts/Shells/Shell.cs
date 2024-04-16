using System.Collections;
using UnityEngine;

public class Shell : ResettableObject
{
    public Collider2D playerCollision;
    public Collider2D groundCollision;
    public enum shellHeights { small, medium, tall }
    public shellHeights shellHeight;

    [Header("Cooldown")]
    public bool canBePickedUp = true;
    public float pickupCooldown = 0.15f;

    [Header("Audio")]
    public AudioClip landingSound;
    protected AudioSource audioSource;

    [Header("Throwing")]
    public bool isThrown = false;

    protected void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {  // Ensure there is an AudioSource component
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected override void Awake ()
    {
        base.Awake ();
        NotificationCenter.instance.AddObserver ( this , Notification.notifications.resetlevel );
    }

    public IEnumerator Cooldown()
    {
        canBePickedUp = false;
        yield return new WaitForSeconds(pickupCooldown);
        canBePickedUp = true;
    }

    public virtual void onEquip ( PlayerControls p )
    {

    }

    public virtual void onUnequip ( PlayerControls p )
    {

    }

    public void outlineOn()
    {
        // Implementation for outline on
    }

    public void outlineOff()
    {
        // Implementation for outline off
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        int wallLayer = LayerMask.NameToLayer("Wall");
        int trapLayer = LayerMask.NameToLayer("Trap");

        // Use a layer mask to check if the collision object is on one of these layers
        int layerMask = (1 << groundLayer) | (1 << wallLayer) | (1 << trapLayer);

        if (isThrown && (((1 << collision.gameObject.layer) & layerMask) != 0 && landingSound != null))
        {
            audioSource.PlayOneShot(landingSound);
            isThrown = false;
        }
    }

    public override void receiveNotification(Notification notification)
    {
        switch (notification.name)
        {
            case Notification.notifications.resetlevel:
                reset();
                break;
        }
    }

    protected override void reset()
    {
        base.reset();
        isThrown = false;
        // Reset specific properties if needed
    }
}