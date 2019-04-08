using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMovementScript : MonoBehaviour {
    
    // State of this scene, person becomes inactive if false
    bool sceneActive = true;

    // Rate at which the person moves
    [Range(0.0001f, 2f)] public float speed = 0.125f;

    // The current direction of this person
    public Vector3 direction;

    // The path this person is currently following
    public List<GameObject> path = null;

    // The index of the last road intersection in the path that this person crossed
    int nodeIndex;

    // The last road intersection that this person crossed
    public GameObject lastNode;

    // The probabilities that a person will walk to each building type (index 0 = uninfected, index 1 = infected)
    // Adding these up cannot exceed 100, any chance leftover is put toward walking at random
    [Range(0f, 100f)] public float[] walkToHouseChance = { 33f, 25f };
    [Range(0f, 100f)] public float[] walkToFactoryChance = { 33f, 0f };
    [Range(0f, 100f)] public float[] walkToHospitalChance = { 0f, 75f };

    // An array of all intersections
    GameObject[] intersections;

    // Whether or not the path needs to be redrawn
    bool redraw = false;

    // The frame number
    int frame;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        // Get all the intersections
        intersections = GameObject.FindGameObjectsWithTag("road node");

        // Vary the speed
        speed += Random.Range(-0.0625f, 0.0625f);
        if (speed < 0.0001f) {
            speed = 0.0001f;
        } else if (speed > 2f) {
            speed = 2f;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (sceneActive && GameControllerScript.sceneActive) {
            if (frame > 10) {
                if (path != null) {
                    // If we are on a path, follow it
                    FollowPath();
                } else {
                    // Otherwise find a new path to follow
                    FindNewPath();
                }
            }
            frame++;
        }
	}

    // +-------------+--------------------------------------------------------------------------------------------------------------------------------------------
    // | Pathfinding |
    // +-------------+

    // Moves the person along the path they are following
    void FollowPath() {
        if (nodeIndex >= path.Count - 1) {
            // We are done with this path
            path = null;
        } else {
            // Continue on this path
            Vector3 newLocation = transform.position + (direction * speed * (200 / GameControllerScript.updateFrequency));

            // Get the location of the next intersection
            Vector3 nextNodeLocation = path[nodeIndex + 1].transform.position;

            // Check if moving to this new location pushed the person past the next intersection
            bool xPast = ((nextNodeLocation.x - newLocation.x) < 0 && (nextNodeLocation.x - transform.position.x) >= 0) ||
                          ((nextNodeLocation.x - newLocation.x) >= 0 && (nextNodeLocation.x - transform.position.x) < 0);
            bool yPast = ((nextNodeLocation.y - newLocation.y) < 0 && (nextNodeLocation.y - transform.position.y) >= 0) ||
                          ((nextNodeLocation.y - newLocation.y) >= 0 && (nextNodeLocation.y - transform.position.y) < 0);
            
            if (xPast || yPast) {
                // The next movement will bring this person past the next intersection, so move them to the intersection location instead
                newLocation = nextNodeLocation;
                nodeIndex++;
                lastNode = path[nodeIndex];

                if (redraw) {
                    // The path needs to be redrawn
                    redraw = false;
                    path = null;
                } else {
                    if (nodeIndex < path.Count - 1) {
                        // There are still intersections left in this path
                        direction = Vector3.Normalize(
                            path[nodeIndex + 1].transform.position - lastNode.transform.position
                        );
                        SendMessage("FlipAnimation", direction.x, SendMessageOptions.DontRequireReceiver);
                    } else {
                        // We are done with this path
                        path = null;

                        // Add this person to the building they entered if they ended at an "In" node
                        if (lastNode.transform.name.ToLower() == "in") {
                            lastNode.transform.parent.SendMessage("AddPerson", gameObject);
                        }
                    }
                }
            }

            // Check if this person is stuck
            if ((newLocation - transform.position).magnitude < 0.1f) {
                // Rerun pathfinding
                path = null;
            }

            // Move the person to the next location
            transform.position = newLocation;
        }
    }

    // Finds a new path for the person to follow
    void FindNewPath() {
        // Reset the node index
        nodeIndex = 0;

        // Choose an intersection to walk to
        GameObject destination = SelectAction();

        // Get a path to that intersection
        path = destination.GetComponent<PathfindingScript>().GetPath(lastNode);

        // If the path could not be found, pick a path to a random intersection on this island instead
        if (path == null) {
            // Find the island this person is on
            GameObject island = GameObject.Find("Island1");
            foreach (Transform i in GameObject.Find("LandMasses").transform) {
                if ((i.position - transform.position).magnitude < (island.transform.position - transform.position).magnitude) {
                    island = i.gameObject;
                }
            }

            // Get the island's intersections
            List<GameObject> islandIntersections = new List<GameObject>();
            foreach (GameObject inter in intersections) {
                if ((inter.transform.position - island.transform.position).magnitude < 200) {
                    islandIntersections.Add(inter);
                }
            }

            // Find an intersection
            destination = islandIntersections[Random.Range(0, islandIntersections.Count - 1)];
            path = destination.GetComponent<PathfindingScript>().GetPath(lastNode);
        }

        // Initialize the direction
        if (path != null && nodeIndex + 1 < path.Count) {
            direction = Vector3.Normalize(
                path[nodeIndex + 1].transform.position - lastNode.transform.position
            );
            SendMessage("FlipAnimation", direction.x, SendMessageOptions.DontRequireReceiver);
        }
    }

    // Reruns the pathfinding to find new paths
    void RedrawPaths() {
        redraw = true;
    }

    // +----------------+-----------------------------------------------------------------------------------------------------------------------------------------
    // | Action Changes |
    // +----------------+

    // Selects an action for this person, returns the GameObject that this person will walk to
    GameObject SelectAction() {

        // Select an action
        string action = "random";
        int index = (transform.tag == "infected") ? 1 : 0;
        float rand = Random.value;
        if (rand < walkToHouseChance[index] / 100f) {
            action = "house";
        } else if (rand < (walkToFactoryChance[index] + walkToHouseChance[index]) / 100f) {
            action = "factory";
        } else if (rand < (walkToHospitalChance[index] + walkToFactoryChance[index] + walkToHouseChance[index]) / 100f) {
            action = "hospital";
        }

        // Select a destination intersection
        GameObject destination = intersections[Random.Range(0, intersections.Length - 1)];

        if (action != "random") {
            // Move to the nearest building with a tag matching the action
            GameObject building = FindClosestStructure(action);

            if (building != null) {
                destination = building.transform.Find("In").gameObject;
            }
        }

        return destination;
    }

    // Finds the nearest instance of a particular structure
    GameObject FindClosestStructure(string structType) {

        GameObject close = null;
        float distance = float.MaxValue;

        // Iterate through the structures
        foreach (Transform t in GameObject.Find("Structures").transform) {
            GameObject st = t.gameObject;
            if (st.tag == structType) {
                
                float newDist = (st.transform.position - transform.position).magnitude;
                if (newDist < distance) {

                    switch (structType) {
                        case "hospital":
                            // Only choose a hospital if it offers at least one treatment

                            if (st.GetComponent<HospitalScript>().treatmentName1.ToLower() != "none" ||
                            st.GetComponent<HospitalScript>().treatmentName2.ToLower() != "none") {
                                distance = newDist;
                                close = st;
                            }
                            break;

                        case "house":
                            // Only choose a house if it has vacancies

                            if (st.GetComponent<HouseScript>().vacancies > 0 &&
                            !st.GetComponent<HouseScript>().quarantined) {
                                distance = newDist;
                                close = st;
                            }
                            break;

                        default:
                            // Choose the closest structure

                            distance = newDist;
                            close = st;
                            break;
                    }
                }
            }
        }

        // If the nearest factory is closed, choose the nearest house instead
        if (structType == "factory" && close != null &&
            (!close.GetComponent<FactoryScript>().open || close.GetComponent<FactoryScript>().vacancies <= 0)) {
            close = FindClosestStructure("house");
        }

        // If no hospital was found, choose the nearest house instead
        if (structType == "hospital" && close == null) {
            close = FindClosestStructure("house");
        }

        return close;
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Sets the lastNode variable
    void SetLastNode(GameObject node) {
        lastNode = node;
        direction = new Vector3();
        path = null;
    }

    // Sets the activeness of the scene
    void SetSceneActive(bool active) {
        sceneActive = active;
    }
}
