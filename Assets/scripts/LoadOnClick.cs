using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadOnClick : MonoBehaviour {

	public void LoadScene() 
    {
        SceneManager.LoadScene("MainScene");
        GameObject settings = GameObject.Find("Persistent Settings");
        DontDestroyOnLoad(settings);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
