using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour {

    // The road prefab
    public GameObject roadPrefab;

    // Array of all nodes in the game
    GameObject[] nodes;

    // List of all intersection connections
    List<NodeData> edges;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get all the intersections
        nodes = GameObject.FindGameObjectsWithTag("road node");

        // Get all edges in the game
        edges = new List<NodeData>();
        foreach (GameObject node in nodes) {
            List<NodeData> newEdges = node.GetComponent<PathfindingScript>().GetAdjacent();

            // Add non-duplicates
            foreach (NodeData edge in newEdges) {
                if (!edges.Contains(edge)) {
                    edges.Add(edge);
                }
            }
        }

        // Generate the roads
        GenerateRoads();
	}

    // +-----------------+----------------------------------------------------------------------------------------------------------------------------------------
    // | Road Generation |
    // +-----------------+

    // Generates a road for every edge in the edges list
    void GenerateRoads() {
        foreach (NodeData edge in edges) {
            // Get the two intersections on this road
            GameObject inter1 = edge.node;
            GameObject inter2 = edge.parent;

            // Make the road
            GameObject road = Instantiate(roadPrefab);
            road.name = "Road";
            road.transform.position = (inter1.transform.position + inter2.transform.position) / 2;
            road.transform.localScale = new Vector3((inter1.transform.position - inter2.transform.position).magnitude, 13.1221f, 0);

            // Set the rotation of the road
            float angle = Mathf.Atan2(
                (inter1.transform.position.y - inter2.transform.position.y),
                (inter1.transform.position.x - inter2.transform.position.x)
            ) * Mathf.Rad2Deg;
            road.transform.localEulerAngles = new Vector3(0, 0, angle);

            // Set the road's parent
            road.transform.SetParent(transform, false);
        }
    }
}
