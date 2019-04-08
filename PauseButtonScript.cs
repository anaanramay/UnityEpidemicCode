using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonScript : MonoBehaviour {

    // Various components
    Image image;
    Button button;

    // Whether or not the game is paused
    bool paused = false;

    // Whether the game auto-paused while the game was already paused (for saving between scene changes)
    bool alreadyPaused = false;

    // Sprites
    public Sprite pauseSprite;
    public Sprite playSprite;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get the image and button components
        image = transform.Find("Image").GetComponent<Image>();
        button = transform.GetComponent<Button>();

        // Add listener for onClick
        button.onClick.AddListener(delegate { PauseGame(!paused); });
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks the button
    void PauseGame(bool pause) {
        if (!GameObject.Find("GameOverImage") && PersonInstantiation.peopleLoaded) {
            if (pause != paused && !alreadyPaused) {
                paused = pause;

                // Pause or unpause all objects
                transform.parent.transform.parent.BroadcastMessage("SetSceneActive", !pause, SendMessageOptions.DontRequireReceiver);

                // Change the button icon
                if (paused) {
                    image.sprite = playSprite;
                } else {
                    image.sprite = pauseSprite;
                }
            } else {
                alreadyPaused = !alreadyPaused;
            }
        }
    }
}
