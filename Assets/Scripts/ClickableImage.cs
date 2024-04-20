using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableImage : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            LoadLevel();
        }
    }

    void LoadLevel()
    {
        SceneManager.LoadScene(1); 
    }
}
