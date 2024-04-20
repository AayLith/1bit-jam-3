using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableImage : MonoBehaviour
{
    public Animator transition;
    public Sprite normalSprite; // Sprite when not hovered
    public Sprite hoverSprite;  // Sprite when hovered
    private SpriteRenderer spriteRenderer;

    public float transitionTime = 0.5f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the GameObject.");
        }
        // Assign the normal sprite initially if not done through the Inspector
        if (normalSprite == null)
        {
            normalSprite = spriteRenderer.sprite;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LoadNextLevel();
        }
    }

    void OnMouseEnter()
    {
        spriteRenderer.sprite = hoverSprite;
    }

    void OnMouseExit()
    {
        spriteRenderer.sprite = normalSprite;
    }

    void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}