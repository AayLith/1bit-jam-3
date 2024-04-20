using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    public Animator transition;
    private string sceneToLoad;
    [SerializeField]
    private float secondsToWait;
    [SerializeField]
    private AudioClip onTriggerSound;
    [SerializeField]
    private AudioSource audioSource;  // Ensure this is assigned via the Inspector
    public bool oneShot = true;
    public bool victoryJump = true;
    private bool triggered = false;

    private void Awake()
    {
        // Initialize AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.gameObject.tag == "Player")
        {
            triggered = oneShot;
            PlayTriggerSound();
            PlayerControls playerControls = collision.gameObject.GetComponent<PlayerControls>();

            StartCoroutine(playerControls.lockInputsDelay(secondsToWait));
            LoadNextLevel();
        }
    }

    private void PlayTriggerSound()
    {
        if (onTriggerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(onTriggerSound);
        }
        else
        {
            Debug.LogError("No audio source or trigger sound provided");
        }
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextLevelIndex = 0;  // Loop back to the first scene if there are no more scenes
        }

        StartCoroutine(LoadLevel(nextLevelIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(secondsToWait);
        SceneManager.LoadScene(levelIndex);
    }
}