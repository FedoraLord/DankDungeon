using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    
    public void PlayGame()
    {
        //Starts game if game is on a different scene
        SceneManager.LoadScene("FinalMap");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
