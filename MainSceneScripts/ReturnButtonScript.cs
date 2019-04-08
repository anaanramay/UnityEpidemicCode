using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnButtonScript : MonoBehaviour {

    // The button component
    Button button;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

    // Use this for initialization
    void Start() {
        // Get the button component
        button = GetComponent<Button>();

        // Add listener for onClick
        button.onClick.AddListener(delegate { OnClick(); });
    }

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks this button
    void OnClick() {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single); // Start scene
    }
}
