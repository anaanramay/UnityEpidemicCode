using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(delegate { ExitGame(); });
	}
	
    // Exit the tutorial and go back to the start scene
    void ExitGame() {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single); // Start scene
    }
}
