using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSceneScript : MonoBehaviour {

    // The button component
    Button button;

    // The scene number this button switches to
    public int sceneNumber;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get the button component
        button = GetComponent<Button>();

        // Add listener for onClick
        if (sceneNumber == 3) {
            if (GameControllerScript.canResearch) {
                button.onClick.AddListener(delegate { OnClick(); });
            } else {
                gameObject.SetActive(false);
            }
        } else {
            button.onClick.AddListener(delegate { OnClick(); });
        }
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks this button
    void OnClick() {
        StartCoroutine(GameControllerScript.LoadSceneNumber(sceneNumber));
    }
}
