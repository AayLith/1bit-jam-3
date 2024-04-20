using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int levelNumber = scene.buildIndex;

        switch (levelNumber)
        {
            case 0:
                // Optionally handle music for the main menu, if any
                break;
            case 1:
            case 2:
            case 3:
                // Play music for levels 1-3
                AudioManager.instance.PlayMusic(AudioManager.instance.track1);
                break;
            case 4:
            case 5:
                // Play music for levels 4-5
                AudioManager.instance.PlayMusic(AudioManager.instance.track2);
                break;
            default:
                // Optionally handle any other cases or default music
                break;
        }
    }
}