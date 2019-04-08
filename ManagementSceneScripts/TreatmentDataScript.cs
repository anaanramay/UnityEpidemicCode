using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreatmentDataScript : MonoBehaviour {

    // Sections of the window
    public Text activeTreatmentsText;
    public Text title;
    public Button rename;
    public InputField nameTextBox;
    public Text costText;
    public Text efficacyText;
    public Text resistanceText;
    public GameObject strainPanel;

    // Main buttons
    public Button activateButton;
    public Button deactivateButton;
    public Button discardButton;

    // The name of the treatment the player is viewing
    string activeTreatment;

    // The treatment menu in this scene
    public GameObject treatmentMenu;

    // The strain label prefab
    public GameObject strainPrefab;

    // The array of strain colors
    Color[] strainColors;

    // Array of all hospitals in the game
    GameObject[] hospitals;

    // The treatment toggles in the main scene
    GameObject treatmentToggles;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get the hospitals
        hospitals = GameObject.FindGameObjectsWithTag("hospital");

        // Get the treatment toggles
        treatmentToggles = GameObject.Find("TreatmentToggles");

        // Get the strain colors
        strainColors = GameObject.FindWithTag("GameController").GetComponent<GameControllerScript>().strainColors;

        // Add listeners
        rename.onClick.AddListener(delegate { RenameButtonClick(); });
        activateButton.onClick.AddListener(delegate { Activate(true); });
        deactivateButton.onClick.AddListener(delegate { Activate(false); });
        discardButton.onClick.AddListener(delegate { Discard(); });
        nameTextBox.onEndEdit.AddListener(delegate { OnEditEnd(nameTextBox); });
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Rename |
    // +--------+

    // Called when the rename button is clicked
    void RenameButtonClick() {
        bool renaming = title.gameObject.activeInHierarchy;
        title.gameObject.SetActive(!renaming);
        nameTextBox.gameObject.SetActive(renaming);
        nameTextBox.transform.Find("Placeholder").GetComponent<Text>().text = "Enter name...";
    }

    // Called when the player stops typing a new name
    void OnEditEnd(InputField change) {
        string newName = activeTreatment;

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
                // The name is not in use, so rename the treatment
                newName = change.text;

                nameTextBox.text = "";
                title.gameObject.SetActive(true);
                nameTextBox.gameObject.SetActive(false);

                if (newName != activeTreatment) {
                    RenameTreatment(activeTreatment, newName);
                }
            }
        }
    }

    // +----------------------+-----------------------------------------------------------------------------------------------------------------------------------
    // | Activate and Discard |
    // +----------------------+

    // Called when the player presses the activate or deactivate button
    void Activate(bool active) {
        // Switch the active marker in the menu
        treatmentMenu.BroadcastMessage("ToggleActivate", activeTreatment);

        // Activate/deactivate the treatment in the main scene
        GameObject toggle = treatmentToggles.transform.Find("Treatment" + activeTreatment).gameObject;
        toggle.GetComponent<Toggle>().interactable = active;
        toggle.SendMessage("SetText");

        if (active) {
            EnableToggle(toggle);
        } else {
            DisableToggle(toggle);
        }

        // Switch the buttons
        ChangeVisibleButtons(active);
    }

    // Enables a toggle in the main scene treatment toggles
    void EnableToggle(GameObject toggle) {
        // Move the treatment to the bottom of the list
        RectTransform rectTrans = toggle.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(0, -10);
        foreach (RectTransform t in treatmentToggles.transform) {
            if (t.GetComponent<Toggle>().interactable && t != rectTrans) {
                rectTrans.anchoredPosition += new Vector2(0, -20);
            }
        }

        // Add this treatment to the hospital options
        foreach (GameObject hospital in hospitals) {
            hospital.SendMessage("AddTreatment", activeTreatment);
        }

        // Set the number active text
        activeTreatmentsText.text = "Active Treatments: " + (GetNumActive() + 1) + "/8";
    }

    // Disables a toggle in the main scene treatment toggles
    void DisableToggle(GameObject toggle) {
        // Move the treatments below this treatment in the list up
        float yPos = toggle.transform.localPosition.y;
        foreach (RectTransform t in treatmentToggles.transform) {
            // If toggle t was below the removed toggle, move it up to fill the gap
            if (t.GetComponent<Toggle>().interactable && t.localPosition.y < yPos) {
                t.anchoredPosition += new Vector2(0, 20);
            }

            // Make toggle t the active toggle if it is at the top and the toggle we removed was the active one
            if (Mathf.Abs(t.anchoredPosition.y + 10f) < .01 && toggle.GetComponent<Toggle>().isOn) {
                treatmentToggles.GetComponent<ToggleGroup>().NotifyToggleOn(toggle.GetComponent<Toggle>());
                t.GetComponent<Toggle>().isOn = true;
            }
        }

        // Move this toggle off screen and turn it off
        toggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
        toggle.GetComponent<Toggle>().isOn = false;

        // Remove this treatment from the hospital options
        foreach (GameObject hospital in hospitals) {
            hospital.SendMessage("RemoveTreatment", activeTreatment);
        }

        // Set the number active text
        activeTreatmentsText.text = "Active Treatments: " + (GetNumActive() - 1) + "/8";
    }

    // Called when the player presses the discard button
    void Discard() {
        // Delete the menu button for this treatment
        treatmentMenu.BroadcastMessage("Discard", activeTreatment);

        // Remove the toggle from the main scene
        Destroy(treatmentToggles.transform.Find("Treatment" + activeTreatment).gameObject);

        // Make this center window inactive
        gameObject.SetActive(false);
    }

    // +------------+---------------------------------------------------------------------------------------------------------------------------------------------
    // | Displaying |
    // +------------+

    // Changes which main buttons are visible (activate and discard, or deactivate)
    void ChangeVisibleButtons(bool active) {
        // Choose which buttons to display
        activateButton.transform.parent.gameObject.SetActive(!active);
        deactivateButton.transform.parent.gameObject.SetActive(active);

        // Turn off the activate button if there are no active slots, otherwise turn it on
        int numActive = GetNumActive();

        // Check if there are slots available for more active treatments
        if (numActive < 8) {
            // Allow the player to activate this treatment
            activateButton.GetComponentInChildren<Text>().text = "Activate";
            activateButton.interactable = true;
            activateButton.GetComponent<Image>().color = new Color(75f / 255f, 1, 75f / 255f);
        } else {
            // Do not allow this treatment to be activated
            activateButton.GetComponentInChildren<Text>().text = "No active slots available.";
            activateButton.interactable = false;
            activateButton.GetComponent<Image>().color = Color.white;
        }

        // Turn off the deactivate button if there is only one active treatment, otherwise turn it on
        if (numActive == 1) {
            deactivateButton.GetComponentInChildren<Text>().text = "Must have one treatment active.";
            deactivateButton.interactable = false;
        } else {
            deactivateButton.GetComponentInChildren<Text>().text = "Deactivate";
            deactivateButton.interactable = true;
        }
    }

    // Instantiates the resistant strain labels
    void InstantiateStrainLabels(int startStrain) {
        // Calculate how many labels can fit on one line
        int perLine = (int)strainPanel.GetComponent<RectTransform>().rect.width / 85;

        // Place the strain labels
        for (int i = startStrain; i <= GameControllerScript.maxStrain; i++) {
            GameObject strainLabel = Instantiate(strainPrefab);
            strainLabel.transform.SetParent(strainPanel.transform, false);
            strainLabel.GetComponent<Image>().color = strainColors[i - 1];
            strainLabel.GetComponentInChildren<Text>().text = "Strain " + i;
            strainLabel.transform.localPosition += new Vector3(85 * ((i - startStrain) % perLine), -22.5f * ((i - startStrain) / perLine), 0);
        }
    }

    // Displays the data for the treatment named treatmentName
    void DisplayData(GameObject treatmentButton) {
        // Get information about this treatment from the button
        string treatmentName = treatmentButton.GetComponent<ManageMenuButtonScript>().treatmentName;
        bool active = treatmentButton.transform.Find("Active").gameObject.activeInHierarchy;

        title.text = "Treatment " + treatmentName;
        activeTreatment = treatmentName;

        // Get the treatment script
        TreatmentScript treatment;
        if (treatmentToggles) {
            treatment = treatmentToggles.transform.Find("Treatment" + treatmentName).GetComponent<TreatmentScript>();
        } else {
            treatment = GameObject.Find("TreatmentToggles").transform.Find("Treatment" + treatmentName).GetComponent<TreatmentScript>();
        }

        // Set the cost text
        costText.text = treatment.cost.ToString("c0");

        // Set the efficacy label
        efficacyText.text = treatment.efficacy.ToString("n2") + "%";

        // Clear the old strains
        foreach (Transform strainLabel in strainPanel.transform) {
            Destroy(strainLabel.gameObject);
        }

        // Set the resistant strains
        int resistantStrain = treatment.resistantStrain;
        if (resistantStrain <= GameControllerScript.maxStrain) {
            resistanceText.text = "";

            InstantiateStrainLabels(resistantStrain);
        } else {
            resistanceText.text = "None";
        }

        // Choose which buttons should be active
        ChangeVisibleButtons(active);
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Renames the treatment called oldName to newName
    void RenameTreatment(string oldName, string newName) {
        title.text = "Treatment " + newName;

        string[] names = { oldName, newName };
        treatmentMenu.BroadcastMessage("Rename", names, SendMessageOptions.DontRequireReceiver);
        treatmentToggles.BroadcastMessage("Rename", names, SendMessageOptions.DontRequireReceiver);

        foreach (GameObject hospital in hospitals) {
            hospital.SendMessage("RenameTreatment", names);
        }
    }

    // Gets the number of currently active treatments
    int GetNumActive() {
        int slashIndex = activeTreatmentsText.text.IndexOf("/", System.StringComparison.CurrentCulture);
        int numActive = 0;
        int.TryParse(activeTreatmentsText.text.Substring(slashIndex - 2, 2), out numActive);

        return numActive;
    }
}
