using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TutorialScript : MonoBehaviour {

    // The main camera
    public Camera cam;

    // The continue and exit buttons
    public Button cont;
    public Button exit;

    // The container for all the main scene objects
    public GameObject game;

    // The example buildings used in the tutorial
    public GameObject hospital;
    public GameObject factory;
    public GameObject bridge;
    public GameObject house;

    // The number of steps in the tutorial
    int numSteps;

    // The current step number
    int step;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get the number of steps
        foreach (Transform t in transform) {
            if (t.name.StartsWith("Step", System.StringComparison.CurrentCulture)) {
                numSteps++;
            }
        }

        // Start the tutorial
        NextStep(step++);

        // Add listeners to the buttons
        cont.onClick.AddListener(delegate { NextStep(step++); });
        exit.onClick.AddListener(delegate { ExitTutorial(); });
	}

	// +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
	// | Events |
	// +--------+

    // Continue to the next step of the tutorial
    void NextStep(int next) {
        if (next < numSteps) {
            // Disable the last step's UI, if it exists
            if (transform.Find("Step" + (next - 1))) {
                transform.Find("Step" + (next - 1)).gameObject.SetActive(false);
            }

            // Activate this step's UI
            GameObject stepUI = transform.Find("Step" + next).gameObject;
            stepUI.SetActive(true);

            // Set the camera position if necessary
            if (stepUI.transform.Find("CameraPosition")) {
                cam.GetComponent<CameraControlScript>().enabled = false;
                cam.transform.position = stepUI.transform.Find("CameraPosition").localPosition;
                cam.orthographicSize = stepUI.transform.Find("CameraPosition").localScale.z;
            } else {
                cam.GetComponent<CameraControlScript>().enabled = true;
            }

            // Run any code associated with this step
            gameObject.SendMessage("Step" + next, SendMessageOptions.DontRequireReceiver);
        } else {
            ExitTutorial();
        }
    }

	// Exit the tutorial and go back to the start scene
    void ExitTutorial() {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single); // Start scene
    }

    // +----------------+-----------------------------------------------------------------------------------------------------------------------------------------
    // | Step Functions |
    // +----------------+

    // Each step function is named "Step[step number]"
    // NextStep calls each step function with SendMessage("Step" + next)

    // The step functions allow the tutorial to run bits of code during the tutorial

    void Step10() {
        hospital.SendMessage("OnPointerEnter", new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>()));
    }

    void Step11() {
        hospital.SendMessage("OnClick");
    }

    void Step12() {
        hospital.SendMessage("OnPointerExit", new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>()));
        hospital.SendMessage("OnClick");
    }

    void Step13() {
        factory.SendMessage("OnPointerEnter", new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>()));
    }

    void Step14() {
        factory.SendMessage("OnClick");
    }

    void Step15() {
        factory.SendMessage("OnPointerExit", new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>()));
        factory.SendMessage("OnClick");
    }

    void Step16() {
        house.SendMessage("OnPointerEnter", new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>()));
    }

    void Step17() {
        house.SendMessage("OnClick");
    }

    void Step19() {
        house.SendMessage("OnPointerExit", new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>()));
        house.SendMessage("OnClick");
    }

    void Step20() {
        bridge.SendMessage("OnPointerEnter", new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>()));
    }

    void Step21() {
        bridge.SendMessage("OnClick");
    }

    void Step22() {
        bridge.SendMessage("OnPointerExit", new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>()));
        bridge.SendMessage("OnClick");
    }

    void Step23() {
        StartCoroutine(GameControllerScript.LoadSceneNumber(2)); // Stats scene
    }

    void Step29() {
        GameObject.Find("BackButton").SendMessage("OnClick");
    }

    void Step30() {
        StartCoroutine(GameControllerScript.LoadSceneNumber(3)); // Management scene
    }

    void Step33() {
        GameObject.Find("CenterWindow").GetComponent<CenterWindowScript>().newTreatment.SetActive(true);
    }

    void Step35() {
        GameObject.Find("CenterWindow").GetComponent<CenterWindowScript>().newTreatment.SetActive(false);
    }

    void Step36() {
        GameObject.Find("CenterWindow").SendMessage("OpenTreatmentData", GameObject.Find("MenuButton0").gameObject);
    }

    void Step38() {
        GameObject.Find("BackButton").SendMessage("OnClick");
    }
}
