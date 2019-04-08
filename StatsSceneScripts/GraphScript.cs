using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GraphScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GraphInfo graphInfo;
    public GameObject xAxis;
    public GameObject yAxis;

    // Various components
    Text text;
    RectTransform line;

    // The update number associated with this point
    public int updateNum;

    // Whether or not the last fifty toggle is switched on
    public bool onlyLastFifty = false;

    // The efficacy at this point
    float efficacy;

    // The data to display in the text
    float displayData;

    // The frame number
    int frame;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        // Get the text and line components
        text = transform.Find("Text").GetComponent<Text>();
        line = transform.Find("Line").GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (frame >= updateNum / 12f) {
            // Place the point
            PlacePoint();

            // Set the text
            text.text = updateNum + " : " + displayData.ToString("n2");

            // Set the frame to -1 so this is not run again
            frame = -1;
        } else if (frame >= 0) {
            frame++;
        }
	}

    // +-----------------+----------------------------------------------------------------------------------------------------------------------------------------
    // | Point Placement |
    // +-----------------+

    // Place the point on the graph, returns whether the point was successfully placed
    void PlacePoint() {
        // Set the efficacy
        efficacy = graphInfo.treatments[updateNum];
        displayData = graphInfo.displayData[updateNum];

        // Get the previous efficacy to position the line
        int prevUpdate = (updateNum - 1 < 0) ? 0 : (updateNum - 1);
        float prevEfficacy = graphInfo.treatments[prevUpdate];

        // Get the lengths of the axes
        if (!xAxis || !yAxis) {
            xAxis = GameObject.Find("XAxis").gameObject;
            yAxis = GameObject.Find("YAxis").gameObject;
        }
        float xAxisLength = xAxis.GetComponent<RectTransform>().rect.width;
        float yAxisLength = yAxis.GetComponent<RectTransform>().rect.height;

        // Reset the update number if in only last fifty mode
        updateNum = (onlyLastFifty ? updateNum - (graphInfo.totalUpdates - 50) : updateNum);
        prevUpdate = (onlyLastFifty ? prevUpdate - (graphInfo.totalUpdates - 50) : prevUpdate);

        // Place the point
        Vector3 oldPos = transform.localPosition;
        Vector3 thisPos = oldPos;
        Vector3 prevPos = oldPos;

        if (graphInfo.flipAxes) {
            // Plot this point with the axes flipped
            float spaceBetweenPointsX = xAxisLength / 100;
            float spaceBetweenPointsY = yAxisLength / (onlyLastFifty ? 49 : (graphInfo.totalUpdates - 1));
            thisPos = new Vector3(efficacy * spaceBetweenPointsX, updateNum * spaceBetweenPointsY, 0);
            prevPos = new Vector3(prevEfficacy * spaceBetweenPointsX, prevUpdate * spaceBetweenPointsY, 0);

            // Make sure this point is being placed in a valid location
            if (IsValid(thisPos)) {
                transform.localPosition += thisPos;
            }
        } else {
            // Plot this point the usual way
            float spaceBetweenPointsX = xAxisLength / (onlyLastFifty ? 49 : (graphInfo.totalUpdates - 1));
            float spaceBetweenPointsY = yAxisLength / 100;
            thisPos = new Vector3(updateNum * spaceBetweenPointsX, efficacy * spaceBetweenPointsY, 0);
            prevPos = new Vector3(prevUpdate * spaceBetweenPointsX, prevEfficacy * spaceBetweenPointsY, 0);

            // Make sure this point is being placed in a valid location
            if (IsValid(thisPos)) {
                transform.localPosition += thisPos;
            }
        }

        // Place the line
        if (updateNum != 0) {
            if (IsValid(prevPos) && IsValid(thisPos)) {
                PlaceLine(prevPos, thisPos);
            }
        }
    }

    // Place the line between this point and the previous one
    void PlaceLine(Vector3 prevPos, Vector3 thisPos) {
        // Position the line and scale it
        Vector3 avgPos = (prevPos + thisPos) / 2 - thisPos;
        line.localScale = new Vector3((prevPos - thisPos).magnitude, 2, 0);
        line.localPosition = avgPos;

        // Rotate the line
        float angle = Mathf.Atan2((transform.position.y - line.position.y), (transform.position.x - line.position.x)) * Mathf.Rad2Deg;
        line.localEulerAngles = new Vector3(0, 0, angle);

        // Set the line's parent
        line.SetParent(transform.parent);
        line.SetSiblingIndex(1);
    }

    // Determines if a Vector3 is valid (no NaNs or infinities)
    bool IsValid(Vector3 vec) {
        bool isNaN = (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z));
        bool isInf = (float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z));

        if (isNaN || isInf) {
            return false;
        } else {
            return true;
        }
    }

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the mouse enters this point
    public void OnPointerEnter(PointerEventData eventData) {
        text.gameObject.SetActive(true);
    }

    // Called when the mouse exits this point
    public void OnPointerExit(PointerEventData eventData) {
        text.gameObject.SetActive(false);
    }
}
