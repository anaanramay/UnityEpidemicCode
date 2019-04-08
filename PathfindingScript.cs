using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingScript : MonoBehaviour {

    // The dictionary of distances between this node and all other nodes
    public List<NodeData> distances;

    // The list of adjacent nodes
    public List<GameObject> adjacent;

    // The list of adjacent nodes with their distances
    public List<NodeData> adjDist;

    // The array of all nodes in the game
    GameObject[] nodes;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start() {
        nodes = GameObject.FindGameObjectsWithTag("road node");

        FindPaths();
	}

    // Initialize the distances
    void Initialize() {
        distances = new List<NodeData>();
        adjDist = null;

        foreach (GameObject node in nodes) {
            if (node == gameObject) {
                // Set this node's distance to itself to 0
                distances.Add(new NodeData(node, 0, node));
            } else {
                // This node is not adjacent, set it to infinity
                distances.Add(new NodeData(node, float.PositiveInfinity, node));
            }
        }
    }

    // +------------+---------------------------------------------------------------------------------------------------------------------------------------------
    // | Dijkstra's |
    // +------------+

    // Finds the shortest paths from this node to all others
    void FindPaths() {
        // Reset the paths
        Initialize();

        // Get all edges in the game
        List<NodeData> edges = new List<NodeData>();
        foreach (GameObject node in nodes) {
            List<NodeData> newEdges = node.GetComponent<PathfindingScript>().GetAdjacent();

            // Add non-duplicates
            foreach (NodeData edge in newEdges) {
                if (!edges.Contains(edge)) {
                    edges.Add(edge);
                }
            }
        }

        // Find new paths
        Dijkstras();

        //PrintPath(GameObject.Find("Intersection11").gameObject);
    }

    // Runs Dijkstra's algorithm with this node as the starting point
    void Dijkstras() {
        // Set up discovered and explored lists
        List<NodeData> discovered = new List<NodeData>();
        List<GameObject> explored = new List<GameObject>();
        discovered.Add(new NodeData(gameObject, 0, gameObject));

        // Continue finding paths until we run out of nodes to explore
        while (discovered.Count > 0) {
            // Sort the nodes by distance and get the closest one
            discovered.Sort();
            NodeData current = discovered[0];

            // Search through all the edges adjacent to current
            List<NodeData> edges = current.node.GetComponent<PathfindingScript>().GetAdjacent();
            foreach (NodeData edge in edges) {
                GameObject otherNode = edge.OtherNode(current.node);
                if (otherNode != null) {
                    if (!explored.Contains(otherNode)) {
                        // The other node on this edge has not been explored yet, get the known distance
                        float newDist = distances.Find(n => n.node == current.node).totalDist + edge.totalDist;

                        // Update the known distance if this new edge makes it shorter
                        NodeData other = distances.Find(n => n.node == otherNode);
                        if (newDist < other.totalDist) {
                            other.totalDist = newDist;
                            other.parent = current.node;
                        }

                        // Add the other node on this edge to discovered if it is not already there
                        if (!discovered.Contains(other)) {
                            discovered.Add(other);
                        }
                    }
                }
            }

            // Add current to explored and remove it from discovered
            explored.Add(current.node);
            discovered.RemoveAt(0);
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Returns the adjacent nodes to this node
    public List<NodeData> GetAdjacent() {
        if (adjDist != null) {
            return adjDist;
        } else {
            adjDist = new List<NodeData>();

            foreach (GameObject node in adjacent) {
                float dist = (transform.position - node.transform.position).magnitude;
                adjDist.Add(new NodeData(node, dist, gameObject));
            }
            return adjDist;
        }
    }

    // Reruns Dijkstra's to find new paths
    void RedrawPaths() {
        FindPaths();
        adjDist = null;
    }

    // Prints a path found for debuging
    void PrintPath(GameObject goal) {
        string toPrint = "";

        NodeData node = distances.Find(n => n.node == goal);
        toPrint += goal.transform.name + " ";
        while (node.parent != node.node) {
            toPrint += node.parent.transform.name + " ";
            node = distances.Find(n => n.node == node.parent);
        }

        Debug.Log(toPrint);
    }

    // Finds the shortest path from this node to the goal node, returns null if no path exists
    // HOW TO USE: When person needs to navigate from A to B, call B.GetPath(A) and follow that array in order
    public List<GameObject> GetPath(GameObject goal) {
        List<GameObject> path = new List<GameObject>();

        NodeData node = distances.Find(n => n.node == goal);
        if (float.IsInfinity(node.totalDist)) { return null; }

        path.Add(goal);
        while (node.parent != node.node) {
            path.Add(node.parent);
            node = distances.Find(n => n.node == node.parent);
        }

        return path;
    }
}
