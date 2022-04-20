using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuOptions : MonoBehaviour
{
    public void LoadGame()
    {
        PointSystem.ResetPoints();
        SceneManager.LoadScene("MovementScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
