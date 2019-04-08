using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonInstantiation : MonoBehaviour {

    // The game controller
    public GameControllerScript gameController;

    // Whether or not all people have been instantiated yet
    public static bool peopleLoaded = false;

    // The person prefab
    public GameObject prefab;

    // A List of all intersections on this island
    public List<GameObject> intersections = new List<GameObject>();

    // The total population
    int initialPopulation;

    // The initial number of infected people
    int numInfected;

    // The number of infected and uninfected people that have been instantiated so far
    int infected;
    int uninfected;

    // Arrays of the houses and factories in the world
    GameObject[] houses;
    GameObject[] factories;

    // The indices we are currently at for filling houses and factories
    int houseIndex;
    int factoryIndex;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

    // Use this for initialization
    void Start() {
        initialPopulation = GameControllerScript.initialPopulation;
        numInfected = GameControllerScript.numInfected;

        // Get all the intersections in the world
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("road node");
        foreach (GameObject node in nodes) {
            if (node.name.ToLower() != "in" && node.name.ToLower() != "out") {
                intersections.Add(node);
            }
        }

        // Get all the houses and factories
        houses = GameObject.FindGameObjectsWithTag("house");
        factories = GameObject.FindGameObjectsWithTag("factory");
    }

    // Update is called once per frame
    void Update() {
        if (uninfected + infected < initialPopulation) {
            // Generate uninfected people
            int uninfectedToMake = uninfected + 15;
            for (; uninfected < uninfectedToMake && uninfected < initialPopulation - numInfected; uninfected++) {
                GameObject person = InstantiatePerson(false);

                // Possibly place this person in a house or factory
                if (houseIndex < houses.Length &&
                    houses[houseIndex].GetComponent<HouseScript>().vacancies > houses[houseIndex].GetComponent<HouseScript>().capacity / 2) {
                    houses[houseIndex].SendMessage("AddPerson", person);
                } else {
                    houseIndex++;

                    if (factoryIndex < factories.Length &&
                        factories[factoryIndex].GetComponent<FactoryScript>().vacancies > factories[factoryIndex].GetComponent<FactoryScript>().capacity / 4) {
                        factories[factoryIndex].SendMessage("AddPerson", person);
                    } else {
                        factoryIndex++;
                    }
                }
            }

            // Generate infected people
            int infectedToMake = infected + 15;
            for (; infected < infectedToMake && infected < numInfected; infected++) {
                GameObject person = InstantiatePerson(true);

                // Possibly place this person in a house or factory
                float rand = Random.value;
                if (rand < 1f / 3f) {
                    houses[Random.Range(0, houses.Length - 1)].SendMessage("AddPerson", person);
                } else if (rand < 2f / 3f) {
                    factories[Random.Range(0, factories.Length - 1)].SendMessage("AddPerson", person);
                }
            }
        } else {
            peopleLoaded = true;
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Instantiate a person, returns the person that was created
    GameObject InstantiatePerson(bool isInfected) {
        // Find a random intersection
        GameObject startNode = intersections[Random.Range(0, intersections.Count - 1)];

        // Find a location on an adjacent road to the intersection to place this person
        List<NodeData> adjacent = startNode.GetComponent<PathfindingScript>().GetAdjacent();
        GameObject nextNode = adjacent[Random.Range(0, adjacent.Count - 1)].node;
        Vector3 direction = Vector3.Normalize(nextNode.transform.position - startNode.transform.position);
        Vector3 pos = startNode.transform.position +
                               direction *
                               Random.Range(0f, (nextNode.transform.position - startNode.transform.position).magnitude);

        // Instantiate a person
        GameObject newPerson = Instantiate(prefab, pos, Quaternion.identity);

        Settings settings = GameControllerScript.settings;

        PersonBehaviourScript behaviourScript = newPerson.GetComponent<PersonBehaviourScript>();
        behaviourScript.strainNumber = isInfected ? 1 : 0;
        behaviourScript.chanceOfSpreadOnContact = (GameControllerScript.settings.spreadOnContact * GameControllerScript.infectedMultiplier);
        behaviourScript.chanceOfRecovery = settings.recoveryChance;
        behaviourScript.chanceOfDeath = settings.deathChance;
        behaviourScript.chanceOfRandomInfection = settings.randomInfectionChance;

        PersonMovementScript movementScript = newPerson.GetComponent<PersonMovementScript>();
        movementScript.walkToHouseChance[0] = settings.walkToHouseU;
        movementScript.walkToFactoryChance[0] = settings.walkToFactoryU;
        movementScript.walkToHospitalChance[0] = settings.walkToHospitalU;
        movementScript.walkToHouseChance[1] = settings.walkToHouseI;
        movementScript.walkToFactoryChance[1] = settings.walkToFactoryI;
        movementScript.walkToHospitalChance[1] = settings.walkToHospitalI;
        movementScript.speed = GameControllerScript.settings.speed / 100f;

        // Generate a basic starting path from startNode to nextNode or vise versa
        List<GameObject> path = new List<GameObject>();
        if (Random.value < 0.5f) {
            movementScript.lastNode = startNode;
            movementScript.direction = direction;
            path.Add(startNode);
            path.Add(nextNode);
        } else {
            movementScript.lastNode = nextNode;
            movementScript.direction = -direction;
            path.Add(nextNode);
            path.Add(startNode);
        }
        movementScript.path = path;

        newPerson.transform.parent = GameObject.Find("People").transform;
        newPerson.tag = isInfected ? "infected" : "susceptible";
        newPerson.name = "Person";

        return newPerson;
    }
}
