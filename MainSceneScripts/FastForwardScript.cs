using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastForwardScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Add a listener for onClick
        GetComponent<Button>().onClick.AddListener(delegate { OnClick(); });
	}

    // Called when the player clicks this button
    void OnClick() {
        if (GameControllerScript.updateFrequency == 200) {
            // Speed up the game
            GameControllerScript.updateFrequency = 25;
            GetComponentInChildren<RectTransform>().rotation = new Quaternion(0, 0, 180, 0);
        } else {
            // Return the game to normal speed
            GameControllerScript.updateFrequency = 200;
            GetComponentInChildren<RectTransform>().rotation = new Quaternion();
        }
    }
}
