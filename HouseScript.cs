using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HouseScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {



    // State of this scene, person becomes inactive if false
    bool sceneActive = true;

    // Various components
    public Text text;
    public Text label;
    public Button button;
    public Button clear;
    public Toggle quarantine;

    // Whether or not this house is being quarantined
    public bool quarantined = false;

    // Capacity of this house
    public int capacity = 50;

    // Vacancies currently left at this house
    public int vacancies = 50;

    // Queue of people in this house
    Queue<GameObject> tenants = new Queue<GameObject>();

    // Frame number
    int frame;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        // Add listeners for button onClick, clear onClick and quarantine onValueChanged
        button.onClick.AddListener(delegate { OnClick(); });
        clear.onClick.AddListener(delegate { OnClear(); });
        quarantine.onValueChanged.AddListener(delegate { OnQuarantine(); });
	}
	
	// Update is called once per frame
	void Update () {
        if (sceneActive) {
            if (!quarantined && Random.Range(40, 800) * (GameControllerScript.updateFrequency / 200f) < frame) {
                RemovePerson();
                frame = 0;
            }

            text.text = tenants.Count + "/" + capacity;

            frame++;
        }

        if(quarantined)
        {
            text.gameObject.SetActive(true);
        }
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks on this house
    void OnClick() {
        if (quarantined) {
            clear.gameObject.SetActive(false);
        } else {
            clear.gameObject.SetActive(!quarantine.gameObject.activeInHierarchy);
        }
        quarantine.gameObject.SetActive(!quarantine.gameObject.activeInHierarchy);

    }

    // Called when the player clears out this house
    void OnClear() {
        if (sceneActive) {
            OnClick();

            while (tenants.Count > 0) {
                RemovePerson();
            }
        }
    }

    // Called when the player quarantines this house
    void OnQuarantine() {
        OnClick();
        quarantined = !quarantined;

        if (quarantined) {
            label.text = "Quarantined:";
            label.color = Color.red;
            text.color = Color.red;
        } else {
            label.text = "Occupants:";
            label.color = Color.black;
            text.color = Color.black;
        }
    }

    void CloseHouse()
    {
        quarantine.gameObject.SetActive(!quarantine.gameObject.activeInHierarchy);

        if(!quarantined)
        {
            OnQuarantine();
        }
        quarantined = true;
        quarantine.GetComponent<Toggle>().isOn = true;
    } 


    // Called when the mouse enters this building
    public void OnPointerEnter(PointerEventData eventData) {
        text.gameObject.SetActive(true);
    }

    // Called when the mouse exits this building
    public void OnPointerExit(PointerEventData eventData) {
        text.gameObject.SetActive(false);
    }

    // +--------------+-------------------------------------------------------------------------------------------------------------------------------------------
    // | OnGameUpdate |
    // +--------------+

    void OnGameUpdate() {
        // Infect susceptible workers depending on how many infected workers there are
        int numInfected = 0;
        int maxStrain = 1;
        List<GameObject> people = new List<GameObject>();

        // Count up all the infected tenants and find the highest strain in this house
        foreach (GameObject tenant in tenants) {
            if (tenant.tag == "infected") {
                numInfected++;
                int strain = tenant.GetComponent<PersonBehaviourScript>().strainNumber;
                maxStrain = (strain > maxStrain) ? strain : maxStrain;
            }
        }

        // Make a list of tenants not infected with the highest strain
        foreach (GameObject tenant in tenants) {
            if (tenant.GetComponent<PersonBehaviourScript>().strainNumber < maxStrain) {
                people.Add(tenant);
            }
        }

        // Infect a bunch of the tenants
        for (int i = 0; i < numInfected && i < people.Count; i++) {
            if (people[i].GetComponent<PersonBehaviourScript>().strainNumber != maxStrain && Random.value < (GameControllerScript.settings.spreadOnContact * GameControllerScript.infectedMultiplier) / 100f && GameControllerScript.settings.spreadOnContact > 0f && GameControllerScript.sceneActive) {
                people[i].tag = "infected";
                people[i].GetComponent<PersonBehaviourScript>().strainNumber = maxStrain;
            }
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Called when a person collides with this house
    void AddPerson(GameObject person) {
        if (!quarantined && tenants.Count < capacity) {
            tenants.Enqueue(person);
            person.SetActive(false);

            vacancies = capacity - tenants.Count;
        }
    }

    // Called when a person leaves the house
    void RemovePerson() {
        if (tenants.Count > 0) {
            GameObject person = tenants.Dequeue();
            if (person) {
                person.transform.position = transform.Find("Out").position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                person.SetActive(true);
                person.SendMessage("SetLastNode", transform.Find("Out").gameObject);
            }

            vacancies = capacity - tenants.Count;
        }
    }
    // Calls OnClick if the meu for this building is open
    void CloseMenus() {
        if (quarantine.gameObject.activeInHierarchy) {
            OnClick();
        }
    }

    // Sets the activeness of the scene
    void SetSceneActive(bool active) {
        sceneActive = active;
    }
}
