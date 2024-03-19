using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    bool paused = false;
    public GameObject pauseMenu;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                Time.timeScale = 0;
                paused = true;
                pauseMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                paused = false;
                pauseMenu.SetActive(false);
            }
        }
    }

    public void Continue()
    {
        Time.timeScale = 1;
        paused = false;
        pauseMenu.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
