using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    public Animator transition;
    private string sceneToLoad;
    [SerializeField]
    private float secondsToWait;
    [SerializeField]
    private AudioClip onTriggerSound;
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

            LoadNextLevel();
        }
    }

    private IEnumerator SwitchSceneAfterWait()
    {
        yield return new WaitForSeconds(secondsToWait);
        SceneManager.LoadScene(sceneToLoad);
    }

    void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(secondsToWait);
        SceneManager.LoadScene(levelIndex);
    }
}