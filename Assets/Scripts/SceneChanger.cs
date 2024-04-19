using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public float secondsToWait;
    public bool oneShot = true;
    public bool victoryJump = true;
    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.gameObject.tag == "Player")
        {
            triggered = oneShot;
            PlayerControls playerControls = collision.gameObject.GetComponent<PlayerControls>();
            
            StartCoroutine(playerControls.lockInputsDelay(secondsToWait));
            if(victoryJump)
            {
                playerControls.jumpHeight *= 2;
                playerControls.Jump(true);
                playerControls.jumpHeight /= 2;
            }
            
            StartCoroutine(SwitchSceneAfterWait());
        }
    }

    private IEnumerator SwitchSceneAfterWait()
    {
        yield return new WaitForSeconds(secondsToWait);
        SceneManager.LoadScene(sceneToLoad);
    }
}