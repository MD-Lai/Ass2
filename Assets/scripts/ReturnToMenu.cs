using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnToMenu : MonoBehaviour {

    public void BackToMenu() {
        SceneManager.LoadScene("MainMenu");
        GameObject settings = GameObject.Find("Persistent Settings");
        DontDestroyOnLoad(settings);
    }
}
