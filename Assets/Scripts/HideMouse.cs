using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMouse : MonoBehaviour
{
    private bool isGamePaused = false;
    //Attach this to any game object in the scene
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            Time.timeScale = 0; // Stops the game time
            // Any additional pause logic here, like showing a pause menu
            Debug.Log("Game Paused");
        }
    }

    void ResumeGame()
    {
        if (isGamePaused)
        {
            isGamePaused = false;
            Time.timeScale = 1; // Resumes the game time
            // Any additional resume logic here, like hiding a pause menu
            Debug.Log("Game Resumed");
        }
    }
}
