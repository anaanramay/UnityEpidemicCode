using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreatmentButtonScript : MonoBehaviour {
    
    // State of this scene, player cannot treat manually when the scene is paused
    bool sceneActive = true;

    // Various components
    Button button;
    Text text;

    // Purple color
    Color purple = new Color32(145, 50, 220, 255);

    // Mouse attach object
    public GameObject mouseAttach;

    // The toggle group of treatments
    GameObject treatments;

	// Treatment mode is on or off
	bool treatmentOn = false;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        // Get the button and text components
        button = GetComponent<Button>();
        text = transform.GetComponentInChildren<Text>();

        // Set button color to black and white
        text.color = Color.black;
        GetComponent<Image>().color = Color.white;

        // Find the treatments
        treatments = transform.parent.Find("TreatmentToggles").gameObject;

        // Add listener for onClick
        button.onClick.AddListener(delegate { OnClick(); });
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) && treatmentOn) {
            if (sceneActive) {
                // The player clicked during treatment mode, find what they clicked on
                Vector2 mousePos = new Vector2(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y
                );

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);

                if (hit) {
                    // The mouse clicked on something
                    if (hit.transform.gameObject.tag == "infected") {
                        // The mouse clicked on an infected person
                        GameObject person = hit.transform.gameObject;
                        treatments.BroadcastMessage("OnTreatManual", person);
                    }
                }
            }
        }
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the button is clicked
    void OnClick() {
        // Toggle treatment mode
        treatmentOn = !treatmentOn;

        if (treatmentOn) {
            // Enter treatment mode
            text.text = "Treatment Mode: On";

            // Set Button color to purple and white
            text.color = Color.white;
            GetComponent<Image>().color = purple;

            // Set mouse text
            mouseAttach.SendMessage("ChangeText", "Click to treat");
        } else {
            // Exit treatment mode
            text.text = "Treatment Mode: Off";

            // Set color back to black and white
            text.color = Color.black;
            GetComponent<Image>().color = Color.white;

            // Set mouse text
            mouseAttach.SendMessage("ChangeText", "");
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Sets the activeness of the scene
    void SetSceneActive(bool active) {
        sceneActive = active;
    }
}
