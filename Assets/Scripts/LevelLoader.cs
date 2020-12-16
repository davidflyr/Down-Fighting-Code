using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator anim;

    public float transitionTime = 1f;

    public void LoadNextLevel(string scene)
    {
        StartCoroutine(LoadLevel(scene));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        anim.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(transitionTime);

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
