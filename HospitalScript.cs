using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HospitalScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    // State of this scene, person becomes inactive if false
    bool sceneActive = true;

    // Various components
    public Text text1;
    public Text text2;
    public Button button;
    public Dropdown treatment1;
    public Dropdown treatment2;

    // The treatments this hospital offers
    public string treatmentName1 = "none";
    public string treatmentName2 = "none";

    // The capacity of this hospital
    public int capacity = 200;

    // Bool value that determines which treatment to use if alternateTreatments is true
    bool alternate = true;

    // The queue of people at this hospital
    Queue<GameObject> patients = new Queue<GameObject>();

    // Frame number
    int frame;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

    // Use this for initialization
    void Start() {
        // Add listeners for onClick and onValueChanged
        button.onClick.AddListener(delegate { OnClick(); });
        treatment1.onValueChanged.AddListener(delegate { OnValueChanged(treatment1, text1, 1); });
        treatment2.onValueChanged.AddListener(delegate { OnValueChanged(treatment2, text2, 2); });

    }

    // Update is called once per frame
    void Update() {
        if (sceneActive) {
            if (patients.Count > 0 && frame >= 30 * (GameControllerScript.updateFrequency / 200f)) {
                // Release a patient
                ReleasePatient();
                frame = 0;
            }

            frame++;
        }
    }

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks on the hospital
    void OnClick() {
        if (treatment1.GetComponentInChildren<Canvas>()) {
            Destroy(treatment1.GetComponentInChildren<Canvas>().gameObject);
        }

        if (treatment2.GetComponentInChildren<Canvas>()) {
            Destroy(treatment2.GetComponentInChildren<Canvas>().gameObject);
        }

        treatment1.gameObject.SetActive(!treatment1.gameObject.activeInHierarchy);
        treatment2.gameObject.SetActive(!treatment2.gameObject.activeInHierarchy);
    }

    // Called when the player selects a treatment
    void OnValueChanged(Dropdown change, Text text, int treatmentNum) {
        if (change.gameObject.activeInHierarchy && (treatmentNum == 1 || treatmentNum == 2)) {
            
            treatment1.gameObject.SetActive(false);
            treatment2.gameObject.SetActive(false);

            // Workaround for a Unity dropdown menu bug
            if (treatment1.GetComponentInChildren<Canvas>()) {
                Destroy(treatment1.GetComponentInChildren<Canvas>().gameObject);
            }
            if (treatment2.GetComponentInChildren<Canvas>()) {
                Destroy(treatment2.GetComponentInChildren<Canvas>().gameObject);
            }

            string selection = change.captionText.text;

            if (selection.ToLower() == "none") {
                // This hospital will not treat patients
                text.text = "N/A";
                if (treatmentNum == 1) {
                    treatmentName1 = "none";
                } else {
                    treatmentName2 = "none";
                }
            } else {
                // Set the treatment name
                if (treatmentNum == 1) {
                    treatmentName1 = selection.Substring(5);
                    text.text = treatmentName1;
                } else {
                    treatmentName2 = selection.Substring(5);
                    text.text = treatmentName2;
                }
            }
        }
    }

    // Called when the mouse enters this building
    public void OnPointerEnter(PointerEventData eventData) {
        text1.gameObject.SetActive(true);
        text2.gameObject.SetActive(true);
    }

    // Called when the mouse exits this building
    public void OnPointerExit(PointerEventData eventData) {
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
    }

    // +----------+-----------------------------------------------------------------------------------------------------------------------------------------------
    // | Patients |
    // +----------+

    // Called when a person collides with this hospital
    void AddPerson(GameObject person) {
        if (patients.Count < capacity && person.tag == "infected") {

            if ((treatmentName1 == "none" && treatmentName2 != "none") ||
                (treatmentName2 == "none" && treatmentName1 != "none")) {
                // This hospital only offers one treatment, administer it
                string treatmentName = (treatmentName1 == "none") ? treatmentName2 : treatmentName1;

                patients.Enqueue(person);

                GameObject treatment = GameObject.Find("Treatment" + treatmentName);
                if (treatment) {
                    treatment.SendMessage("OnTreatAuto", person);
                }

                person.SetActive(false);
            } else if (treatmentName1 != "none" && treatmentName2 != "none") {
                // This hospital offers two treatments, administer treatment 1 first, then treatment 2 if alternateTreatments is false
                // Otherwise administer one or the other depending on the value of alternate

                patients.Enqueue(person);

                if (!GameControllerScript.alternateTreatments) {
                    // Double dose
                    GameObject treat1 = GameObject.Find("Treatment" + treatmentName1);
                    GameObject treat2 = GameObject.Find("Treatment" + treatmentName2);
                    if (treat1 && treat2) {
                        GameObject[] param = { person, treat2 };
                        treat1.SendMessage("DoubleTreatAuto", param);
                    }

                } else {
                    // Alternate single dose
                    string treatmentName = treatmentName2;

                    if (alternate) {
                        treatmentName = treatmentName1;
                    }

                    alternate = !alternate;

                    GameObject treatment = GameObject.Find("Treatment" + treatmentName);
                    if (treatment) {
                        treatment.SendMessage("OnTreatAuto", person);
                    }
                }

                person.SetActive(false);
            }
        }
    }

    // Called when the hospital treats a patient
    void ReleasePatient() {
        GameObject person = patients.Dequeue();
        if (person) {
            person.transform.position = transform.Find("Out").position;
            person.SetActive(true);
            person.SendMessage("SetLastNode", transform.Find("Out").gameObject);
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Renames a treatment in a dropdown menu
    void RenameInDropdown(Dropdown treatment, string[] names) {
        // Change dropdown captions
        if (treatment.captionText.text == "Trt. " + names[0]) {
            treatment.captionText.text = "Trt. " + names[1];
        }

        // Change dropdown options
        for (int i = 0; i < treatment.options.Count; i++) {
            if (treatment.options[i].text == "Trt. " + names[0]) {
                treatment.options[i].text = "Trt. " + names[1];
            }
        }
    }

    // Called when the player renames a treatment
    void RenameTreatment(string[] names) {
        // Change treatment names
        if (treatmentName1 == names[0]) {
            treatmentName1 = names[1];
            text1.text = treatmentName1;
        }

        if (treatmentName2 == names[0]) {
            treatmentName2 = names[1];
            text2.text = treatmentName2;
        }

        RenameInDropdown(treatment1, names);
        RenameInDropdown(treatment2, names);
    }

    // Removes an option from a dropdown
    void RemoveDropdownOption(Dropdown treatments, string tname, int treatmentNum) {
        // Locate the option
        int i = 0;
        for (; i < treatments.options.Count; i++) {
            if (treatments.options[i].text == "Trt. " + tname) {
                break;
            }
        }

        // Default the hospital to no treatment if it is offering the one we are removing
        if (treatmentNum == 1) {
            if (treatmentName1 == tname) {
                treatmentName1 = "none";
                text1.text = "N/A";
            }
        } else {
            if (treatmentName2 == tname) {
                treatmentName2 = "none";
                text2.text = "N/A";
            }
        }

        // Reset the hospital to none if necessary
        if (treatments.value == i) {
            treatments.value = 0;
        }

        // Remove the option
        treatments.options.RemoveAt(i);
    }

    // Called when the player deactivates a treatment
    void RemoveTreatment(string treatment) {
        RemoveDropdownOption(treatment1, treatment, 1);
        RemoveDropdownOption(treatment2, treatment, 2);
    }

    // Called when the player activates a treatment
    void AddTreatment(string treatment) {
        treatment1.options.Add(new Dropdown.OptionData("Trt. " + treatment));
        treatment2.options.Add(new Dropdown.OptionData("Trt. " + treatment));
    }

    // Calls OnClick if the meu for this building is open
    void CloseMenus() {
        if (treatment1.gameObject.activeInHierarchy) {
            OnClick();
        }
    }

    // Sets the activeness of the scene
    void SetSceneActive(bool active) {
        sceneActive = active;
    }
}
