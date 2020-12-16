using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LevelLoader load;

    public void PlayGame()
    {
        load = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        load.LoadNextLevel("Fight");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
