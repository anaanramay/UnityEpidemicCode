using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageMenuButtonScript : MonoBehaviour {

    // The name of this treatment
    public string treatmentName = "A";

    // Various components
    Button button;
    Text text;

    // The scrolling treatment menu
    public GameObject menu;

    // The center window of the management scene
    public GameObject centerWindow;

    // The active indicator on this button
    public GameObject activeIndicator;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get the button component
        button = GetComponent<Button>();
        text = transform.Find("Text").GetComponent<Text>();

        // Initialize text
        text.text = "Treatment " + treatmentName;

        // Add listener for onClick
        button.onClick.AddListener(delegate { OnClick(); });
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks on this treatment
    void OnClick() {
        centerWindow.SendMessage("OpenTreatmentData", gameObject);
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Called when the player has renamed a treatment
    void Rename(string[] names) {
        if (treatmentName == names[0]) {
            treatmentName = names[1];
            text.text = "Treatment " + names[1];
        }
    }

    // Called when the player activates/deactivates a treatment
    void ToggleActivate(string treatment) {
        if (treatmentName == treatment) {
            activeIndicator.SetActive(!activeIndicator.activeInHierarchy);
        }
    }

    // Called when the player discards a treatment
    void Discard(string treatment) {
        if (treatmentName == treatment) {
            menu.BroadcastMessage("MoveUp", transform.localPosition.y);
            Destroy(gameObject);
        }
    }

    // Called when a menu button has been deleted in order to fill the button's gap
    void MoveUp(float yPos) {
        if ((transform as RectTransform).anchoredPosition.y < yPos) {
            (transform as RectTransform).anchoredPosition += new Vector2(0, 30);
        }
    }
}
