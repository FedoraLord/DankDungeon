using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour {

    public Button playAgainButton;
    public Button closeButton;

	// Use this for initialization
	void Start () {
        
	}
	
	public void PlayAgain()
    {
        SceneManager.LoadScene("FinalMap");
    }

    public void Close()
    {
        Application.Quit();
    }
}
