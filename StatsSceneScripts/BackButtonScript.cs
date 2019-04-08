using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackButtonScript : MonoBehaviour {
    
    // Button component
    Button button;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get the button component
        button = GetComponent<Button>();

        // Add listener for onClick
        button.onClick.AddListener(delegate { OnClick(); });
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    void OnClick() {
        GameObject.FindWithTag("main overlay").BroadcastMessage("CloseScene", SendMessageOptions.DontRequireReceiver);
    }
}
