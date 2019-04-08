using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPrefabScript : MonoBehaviour {

    // The point prefab
    public GameObject pointPrefab;

    // The graph info script
    public GraphInfo graphInfo;

    // The last fifty points toggle
    public Toggle lastFifty;

    // The axis GUI objects
    public GameObject xAxis;
    public GameObject yAxis;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        lastFifty.onValueChanged.AddListener(delegate { LastFiftyOnly(); });
	}

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Called when the player switches the last fifty only toggle
    void LastFiftyOnly() {
        DestroyPoints();
        InstantiatePoints();
    }

    // Instantiate the points, as of now, this keeps track of updates like 1 update = 1 day. If this is to be added into a future version, you will need to change that to the current 10 updates = 1 day
    void InstantiatePoints() {
        // Get the total number of updates
        int totalUpdates = graphInfo.totalUpdates;

        // Set the starting index (i = 0 unless the player is only viewing the last fifty points)
        int i = 0;
        if (lastFifty.isOn && totalUpdates >= 50) {
            i = totalUpdates - 50;
        }

        for (; i < totalUpdates; i++) {
            // Create a new point
            GameObject newPoint = Instantiate(pointPrefab);
            GraphScript graphScript = newPoint.GetComponent<GraphScript>();
            graphScript.updateNum = i;
            graphScript.onlyLastFifty = (totalUpdates >= 50) ? lastFifty.isOn : false;
            graphScript.graphInfo = graphInfo;
            graphScript.xAxis = xAxis;
            graphScript.yAxis = yAxis;
            newPoint.transform.SetParent(transform, false);
        }
    }

    // Destroys all old stat points
    void DestroyPoints() {
        // Get all the points in the graph
        GameObject[] points = GameObject.FindGameObjectsWithTag("stat point");

        // Destroy every point
        foreach (GameObject point in points) {
            Destroy(point);
        }
    }
}
