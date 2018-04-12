using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour {

    public string mainMenuScene = "StartMenu";
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject[] contentArray = new GameObject[7];
    private float fIndex;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused) {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        fIndex = contentArray[1].transform.position.y;
        AllButton();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        
    }

    public void AllButton()
    {
        float y = fIndex; 
        contentArray[0].transform.DetachChildren();

        for (int count = 1; count < contentArray.Length; count++)
        {
            contentArray[count].transform.position = new Vector3(contentArray[count].transform.position.x, y, contentArray[count].transform.position.z);
            y -= 31f;
        }

        for (int count = 1; count < contentArray.Length; count++)
            contentArray[count].transform.SetParent(contentArray[0].transform);
    }

    public void OnlyWeapons()
    {
        float y = fIndex;
        contentArray[0].transform.DetachChildren();

        for (int count = 1; count < 4; count++)
        {
            contentArray[count].transform.position = new Vector3(contentArray[count].transform.position.x, y, contentArray[count].transform.position.z);
            y -= 31f;
        }

        for (int count = 1; count < 4; count++)
            contentArray[count].transform.SetParent(contentArray[0].transform);
    }

    public void OnlyOthers()
    {
        float y = fIndex;
        contentArray[0].transform.DetachChildren();

        for (int count = 4; count < contentArray.Length; count++)
        {
            contentArray[count].transform.position = new Vector3(contentArray[count].transform.position.x, y, contentArray[count].transform.position.z);
            y -= 31f;
        }

        for (int count = 4; count < contentArray.Length; count++)
            contentArray[count].transform.SetParent(contentArray[0].transform);
    }

    public void SettingsButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
