using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {

    }

    public void checkIfPaused()
    {
        if(gameIsPaused)
        {
            Resume();
        }
        else if(gameIsPaused == false)
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
        mouselook mouse = gameObject.GetComponentInParent(typeof(mouselook)) as mouselook;
        mouse.enabled = true;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
    }

    public void ResumeGame()
    {
        Debug.Log("resume");
    }

    public bool getGameIsPaused()
    {
        return gameIsPaused;
    }
}
