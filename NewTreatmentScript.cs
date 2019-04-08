using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewTreatmentScript : MonoBehaviour {

    // Sections of the window
    public Text costText;
    public InputField nameTextBox;

    // Main buttons
    public Button addButton;
    public Button discardButton;

    // The data for the new treatment
    TreatmentData newTreatment;

    // The treatment menu in this scene
    public GameObject treatmentMenu;

    // The treatment toggles in the main scene
    GameObject treatmentToggles;

    // The toggle prefab
    public GameObject togglePrefab;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {

        // Get the new treatment data
        newTreatment = GameControllerScript.newTreatment;

        // Get the treatment toggles
        treatmentToggles = GameObject.Find("TreatmentToggles");

        // Initialize the cost text
        if (newTreatment != null) {
            costText.text = newTreatment.cost.ToString("c0");
        }

        // Add listeners
        addButton.onClick.AddListener(delegate { AddButtonClick(); });
        discardButton.onClick.AddListener(delegate { Discard(); });
        nameTextBox.onEndEdit.AddListener(delegate { OnEditEnd(nameTextBox); });
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player chooses to add this treatment to their game
    void AddButtonClick() {
        discardButton.gameObject.SetActive(false);
        nameTextBox.gameObject.SetActive(true);
    }

    // Called when the player chooses to discard this treatment
    void Discard() {
        newTreatment = null;
        GameControllerScript.newTreatment = null;
        gameObject.SetActive(false);
    }

    // Called when the player stops typing a name for this treatment
    void OnEditEnd(InputField change) {
        string newName = "";

        if (change.text.Length > 0) {
            // Make sure this name is not already in use
            bool invalid = false;
            foreach (Transform menuButton in treatmentMenu.transform) {
                if (menuButton.GetComponent<ManageMenuButtonScript>().treatmentName == change.text) {
                    invalid = true;
                    break;
                }
            }

            if (invalid) {
                // The name is already in use, so reject it and reprompt
                nameTextBox.text = "";
                nameTextBox.transform.Find("Placeholder").GetComponent<Text>().text = "Name already in use!";
            } else {
                // The name is not in use, so add a new treatment
                newName = change.text;

                nameTextBox.text = "";
                nameTextBox.gameObject.SetActive(false);
                addButton.gameObject.SetActive(false);

                AddTreatment(newName);
            }
        }
    }

    // Adds the new treatment to the game
    void AddTreatment(string treatmentName) {
        // Create a new inactive toggle
        GameObject newToggle = Instantiate(togglePrefab);

        // Set the script variables
        TreatmentScript script = newToggle.GetComponent<TreatmentScript>();
        script.treatmentName = treatmentName;
        script.cost = newTreatment.cost;
        script.chanceOfSuccess = newTreatment.efficacy;
        script.startingUpdate = GameControllerScript.totalUpdates;
        script.mouseAttach = GameObject.Find("MouseAttach").gameObject;

        // Set the toggle properties
        newToggle.transform.name = "Treatment" + treatmentName;
        newToggle.transform.SetParent(treatmentToggles.transform, false);
        newToggle.GetComponent<Toggle>().group = treatmentToggles.GetComponent<ToggleGroup>();

        // Create a new management menu button
        int numButtons = 0;
        foreach (Transform menuButton in treatmentMenu.transform) {
            numButtons++;
        }

        // Reset newTreatment
        newTreatment = null;
        GameControllerScript.newTreatment = null;
        gameObject.SetActive(false);

        // Open the treatment data window for the new treatment
        transform.parent.SendMessage(
            "OpenTreatmentData",
            transform.parent.GetComponent<CenterWindowScript>().InstantiateMenuButton(treatmentName, numButtons, false)
        );
    }
}
