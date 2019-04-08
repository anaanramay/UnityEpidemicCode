using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxisMenuScript : MonoBehaviour {

    // Dropdown component
    Dropdown dropdown;

    // Axis info
    public Text axisText;
    public string axis = "Y";

    // Background object
    public GameObject background;

    // Graph object
    GameObject graph;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get the graph object and dropdown component
        graph = transform.parent.parent.gameObject;
        dropdown = GetComponent<Dropdown>();

        // Instantiate the treatment efficacy options
        foreach (Transform toggle in GameObject.Find("TreatmentToggles").transform) {
            AddEfficacy(toggle.GetComponent<TreatmentScript>().treatmentName);
        }

		// Add listener for onValueChanged
        dropdown.onValueChanged.AddListener(delegate { OnValueChanged(dropdown); });
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player picks a new y axis label
    void OnValueChanged(Dropdown change) {
        axisText.text = change.captionText.text;

        // Update the graph info
        graph.SendMessage("UpdateInfo" + axis, change.captionText.text);

        // Destroy the old points and add the new ones
        background.SendMessage("DestroyPoints");
        background.SendMessage("InstantiatePoints");
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Add a treatment efficacy option to the dropdown menu
    void AddEfficacy(string treatmentName) {
        dropdown.options.Add(
            new Dropdown.OptionData("Treatment " + treatmentName + " Efficacy")
        );
    }
}
