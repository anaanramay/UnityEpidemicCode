using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FactoryScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {



    // State of this scene, person becomes inactive if false
    bool sceneActive = true;

    // Various components
    public Text text;
    public Button button;
    public Toggle toggle;

    // Whether or not this factory is open
    public bool open = true;

    // Capacity of this factory
    public int capacity = 200;

    // The amount of space still available
    public int vacancies = 200;

    // Queue of people currently working
    public Queue<GameObject> workers = new Queue<GameObject>();

    // Frame number
    int frame;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {

        // Add listeners for onClick and onValueChanged
        button.onClick.AddListener(delegate { OnClick(); });
        toggle.onValueChanged.AddListener(delegate { OnValueChanged(toggle); });
	}
	
	// Update is called once per frame
	void Update () {
        if (sceneActive) {
            if (!open && workers.Count > 0) {
                EndWorkerShift();
            } else if (Random.Range(25, 800) * (GameControllerScript.updateFrequency / 200f) < frame) {
                EndWorkerShift();
                frame = 0;
            }

            frame++;
        }
	}

    void CloseFactory()
    {
        open = false;
        toggle.GetComponent<Toggle>().isOn = false;

    }

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks on the factory
    void OnClick() {
        toggle.gameObject.SetActive(!toggle.gameObject.activeInHierarchy);
    }

    // Called when the player switches the toggle
    void OnValueChanged(Toggle change) {
        toggle.gameObject.SetActive(false);
        open = change.isOn;

        if (open) {
            text.text = "Open";
        } else {
            text.text = "Closed";
        }
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

        // Count up the infected workers and find the highest strain in this factory
        foreach (GameObject worker in workers) {
            if (worker.tag == "infected") {
                numInfected++;
                int strain = worker.GetComponent<PersonBehaviourScript>().strainNumber;
                maxStrain = (strain > maxStrain) ? strain : maxStrain;
            }
        }

        // Make a list of workers not infected with the highest strain
        foreach (GameObject worker in workers) {
            if (worker.GetComponent<PersonBehaviourScript>().strainNumber < maxStrain) {
                people.Add(worker);
            }
        }

        // Infect a bunch of the workers
        for (int i = 0; i < numInfected && i < people.Count; i++) {
            if (people[i].GetComponent<PersonBehaviourScript>().strainNumber != maxStrain) {
                if (Random.value*1.5 < (GameControllerScript.settings.spreadOnContact * GameControllerScript.infectedMultiplier) / 100f && (GameControllerScript.settings.spreadOnContact * GameControllerScript.infectedMultiplier) > 0f&& GameControllerScript.sceneActive)
                {
                    people[i].tag = "infected";
                    people[i].GetComponent<PersonBehaviourScript>().strainNumber = maxStrain;
                }
            }
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Called when a person collides with this factory
    void AddPerson(GameObject person) {
        if (open && workers.Count < capacity) {
            workers.Enqueue(person);
            person.SetActive(false);

            vacancies = capacity - workers.Count;
        }
    }

    // Called when a person leaves work
    void EndWorkerShift() {
        if (workers.Count > 0) {
            GameObject person = workers.Dequeue();
            if (person) {
                person.transform.position = transform.Find("Out").position;
                person.SetActive(true);
                person.SendMessage("SetLastNode", transform.Find("Out").gameObject);
            }

            vacancies = capacity - workers.Count;
        }
    }

    // Calls OnClick if the meu for this building is open
    void CloseMenus() {
        if (toggle.gameObject.activeInHierarchy) {
            OnClick();
        }
    }

    // Sets the activeness of the scene
    void SetSceneActive(bool active) {
        sceneActive = active;
    }
}
