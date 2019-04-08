using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CenterWindowScript : MonoBehaviour {

    // The treatment data window
    public GameObject treatmentData;

    // The new treatment discovered window
    public GameObject newTreatment;

    // The number of active treatments text
    public Text activeTreatments;

    // The menu of treatments
    public GameObject treatmentMenu;

    // The treatment menu button prefab
    public GameObject treatmentPrefab;

    // Variables for sliding animation
    readonly int slideRate = 10;
    float canvasHeight;
    RectTransform background;
    bool slideIn = false;
    bool slideOut = false;

    // The frame number
    int frame;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        // Start the scene above the camera
        canvasHeight = GameObject.FindWithTag("main overlay").GetComponent<RectTransform>().rect.height;
        background = GameObject.FindWithTag("main overlay").transform.Find("Background").GetComponent<RectTransform>();
        background.anchoredPosition = new Vector2(0, canvasHeight);
        slideIn = true;

        // Instantiate the treatment menu buttons
        treatmentMenu.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10);
        int toggleNum = 0;
        int numActive = 0;
        foreach (Transform toggle in GameObject.Find("TreatmentToggles").transform) {
            InstantiateMenuButton(
                toggle.GetComponent<TreatmentScript>().treatmentName,
                toggleNum,
                toggle.GetComponent<Toggle>().interactable
            );
            numActive = toggle.GetComponent<Toggle>().interactable ? numActive + 1 : numActive;
            toggleNum++;
        }

        // Display the new treatment window if a new treatment was discovered
        if (GameControllerScript.newTreatment != null) {
            newTreatment.SetActive(true);
        }

        // Initialize the number of active treatments text
        activeTreatments.text = "Active Treatments: " + numActive + "/8";
	}

    // Update is called once per frame
    void Update() {
        // Slide the scene in or out
        if (slideIn && frame < slideRate) {
            background.anchoredPosition -= new Vector2(0, canvasHeight / slideRate);
            frame++;
        } else if (slideOut && frame < slideRate) {
            background.anchoredPosition += new Vector2(0, canvasHeight / slideRate);
            frame++;
        }

        // Reset the frame number and slide bools
        if (frame >= slideRate) {
            frame = 0;
            slideIn = false;

            if (slideOut) {
                // Unpause the main scene
                GameObject.Find("PauseButton").SendMessage("PauseGame", false);
                GameObject.FindWithTag("MainCamera").SendMessage("ToggleCameraActive");

                // Set the main scene as active
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));

                // Unload this scene
                SceneManager.UnloadSceneAsync(3);
            }
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Instantiate a treatment menu button, returns the button that was created
    public GameObject InstantiateMenuButton(string treatmentName, int buttonNum, bool active) {
        // Expand the scrolling menu
        treatmentMenu.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 30);

        // Create a new button
        GameObject button = Instantiate(treatmentPrefab);

        // Set the script variables
        ManageMenuButtonScript script = button.GetComponent<ManageMenuButtonScript>();
        script.treatmentName = treatmentName;
        script.menu = treatmentMenu;
        script.centerWindow = gameObject;
        script.activeIndicator.SetActive(active);

        // Set the button's properties
        button.transform.name = "MenuButton" + buttonNum;
        button.transform.SetParent(treatmentMenu.transform, false);
        button.transform.localPosition += new Vector3(0, -30 * buttonNum, 0);

        return button;
    }

    // Called when the player views the data for a treatment
    void OpenTreatmentData(GameObject treatmentButton) {
        if (!newTreatment.activeInHierarchy) {
            treatmentData.SetActive(true);
            treatmentData.SendMessage("DisplayData", treatmentButton);
        }
    }

    // Closes the scene
    void CloseScene() {
        slideOut = true;
    }
}
