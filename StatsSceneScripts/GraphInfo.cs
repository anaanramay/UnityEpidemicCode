using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GraphInfo : MonoBehaviour {
    // Class that stores a bunch of public variables used to assemble graphs

    public int totalUpdates;

    // Name of the current treatment
    public string treatmentName = "A";

    // Whether or not to flip the axes for graphing
    public bool flipAxes = false;

    // Current axis labels
    string currentLabelY;
    string currentLabelX;

    // Array of the current treatment's efficacies
    public float[] treatments = new float[100];

    // Array of the data to display in mouse over text
    public float[] displayData = new float[100];

    // Variables for sliding animation
    readonly int slideRate = 10;
    float canvasHeight;
    RectTransform background;
    bool slideIn = false;
    bool slideOut = false;

    // The frame number
    int frame;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

    // Use this for initialization
    void Start() {
        // Start the scene above the camera
        canvasHeight = transform.GetComponent<RectTransform>().rect.height;
        background = transform.Find("Background").GetComponent<RectTransform>();
        background.anchoredPosition = new Vector2(0, canvasHeight);
        slideIn = true;

        UpdateInfo("Time", "Total Population");
        transform.Find("Background").SendMessage("InstantiatePoints");
    }

    // Update is called once per frame
    void Update() {
        // Slide the scene in or out
        if (slideIn && frame < slideRate) {
            background.anchoredPosition -= new Vector2(0, canvasHeight / slideRate);
            frame++;
        } else if (slideOut && frame < slideRate) {
            background.anchoredPosition += new Vector2(0, canvasHeight / slideRate);
            frame++;
        }

        // Reset the frame number and slide bools
        if (frame >= slideRate) {
            frame = 0;
            slideIn = false;

            if (slideOut) {
                // Unpause the main scene
                GameObject.Find("PauseButton").SendMessage("PauseGame", false);
                GameObject.FindWithTag("MainCamera").SendMessage("ToggleCameraActive");

                // Set the main scene as active
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));

                // Unload this scene
                SceneManager.UnloadSceneAsync(2);
            }
        }
    }

    // +--------------+-------------------------------------------------------------------------------------------------------------------------------------------
    // | Display Data |
    // +--------------+                                                         COULD USE SOME SIMPLIFICATION

    void SetDisplayData(string label) {
        List<float> points = null;
        int startPoint = 0;

        if (label.ToLower() == "budget") {
            // Get the budget list
            points = GameControllerScript.budgetList;

        } else if (label.Length == 1 || label.Substring(0, 9).ToLower() == "treatment") {
            // Set the treatment name
            treatmentName = (label.Length == 1) ? label : label.Split(' ')[1];

            TreatmentScript script = GameObject.Find("Treatment" + treatmentName).GetComponent<TreatmentScript>();
            startPoint = script.startingUpdate;

            // Get the efficacyList of the treatment we are looking at
            points = script.efficacyList;

        } else if (label.ToLower() == "total population") {
            // Get the total population list
            points = GameControllerScript.totalPopList;

        } else if (label.Substring(0, 10).ToLower() == "population") {
            // Get the population list we are looking at
            points = (label.Substring(11, 2).ToLower() == "un") ?
                GameControllerScript.uninfectedPopList : GameControllerScript.infectedPopList;
        }

        if (points != null) {
            // Add the points to the graphInfo
            int index = startPoint;
            foreach (float p in points) {
                if (index >= displayData.Length) { break; }
                displayData[index] = p;
                index++;
            }
        } else {
            for (int i = 0; i < totalUpdates; i++) {
                displayData[i] = i;
            }
        }
    }

    // +----------------+-----------------------------------------------------------------------------------------------------------------------------------------
    // | Get Point Data |
    // +----------------+                                                       COULD USE SOME SIMPLIFICATION

    // Gets the list of points for the given axis label
    List<float> GetPoints(string label) {
        List<float> points = null;

        if (label.ToLower() != "time") {
            if (label.ToLower() == "budget") {
                // The player is viewing the budget
                // Get the budget list
                List<float> budget = GameControllerScript.budgetList;

                // Get the max value
                float max = budget[0];
                foreach (float b in budget) {
                    if (b > max) {
                        max = b;
                    }
                }

                // Divide everything by the max and multiply by 100 for percents
                points = new List<float>();
                foreach (float b in budget) {
                    points.Add(b / max * 100);
                }
            } else if (label.Length == 1 || label.Substring(0, 9).ToLower() == "treatment") {
                // The player is viewing a treatment
                // Set the treatment name
                treatmentName = (label.Length == 1) ? label : label.Split(' ')[1];

                TreatmentScript script = GameObject.Find("Treatment" + treatmentName).GetComponent<TreatmentScript>();

                // Default the first treatment.startingUpdate spots in the points to 0
                points = new List<float>();
                for (int i = 0; i < script.startingUpdate; i++) {
                    points.Add(0);
                }

                // Get the efficacyList of the treatment we are looking at
                foreach (float f in script.efficacyList) {
                    points.Add(f);
                }

            } else if (label.ToLower() == "total population") {
                // The player is viewing the total population
                // Get the total population list
                List<float> pop = GameControllerScript.totalPopList;

                // Get the max value
                float max = pop[0];
                foreach (float p in pop) {
                    if (p > max) {
                        max = p;
                    }
                }

                // Divide everything by the max and multiply by 100 for percents
                points = new List<float>();
                foreach (float p in pop) {
                    points.Add(p / max * 100);
                }

            } else if (label.Substring(0, 10).ToLower() == "population") {
                // The player is viewing a different population
                // Get the population list we are looking at
                points = (label.Substring(11, 2).ToLower() == "un") ?
                    GameControllerScript.uninfectedPopList : GameControllerScript.infectedPopList;
            }
        }

        return points;
    }

    // +-------------+--------------------------------------------------------------------------------------------------------------------------------------------
    // | Update Info |
    // +-------------+                                                          COULD USE SOME SIMPLIFICATION

    // Update the graph info on the y axis
    void UpdateInfo(string xAxis, string yAxis) {
        currentLabelX = xAxis;
        currentLabelY = yAxis;

        totalUpdates = GameControllerScript.totalUpdates;
        treatments   = new float[totalUpdates];
        displayData  = new float[totalUpdates];

        if (xAxis == yAxis) {
            // Plot a straight y = x line
            for (int i = 0; i < totalUpdates; i++) {
                treatments[i]  = (100f / totalUpdates) * i;
                displayData[i] = i;
            }

        } else {
            // Plot the points appropriately
            List<float> points = null;
            List<float> pointsX = GetPoints(xAxis);
            List<float> pointsY = GetPoints(yAxis);

            if (pointsX == null) {
                // x axis is time
                points = pointsY;
                SetDisplayData(yAxis);
                flipAxes = false;
            } else if (pointsY == null) {
                // y axis is time
                points = pointsX;
                SetDisplayData(xAxis);
                flipAxes = true;
            } else {
                // Average the two point lists
                points = new List<float>();

                for (int i = 0; i < pointsX.Count && i < pointsY.Count; i++) {
                    points.Add((pointsX[i] + pointsY[i]) / 2);
                }
                SetDisplayData(yAxis);

                flipAxes = false;
            }

            if (points != null) {
                // Add the points to the graphInfo
                int index = 0;
                foreach (float p in points) {
                    if (index >= treatments.Length) { break; }
                    treatments[index] = p;
                    index++;
                }
            }
        }
    }

    // Only update the x axis
    void UpdateInfoX(string xAxis) {
        UpdateInfo(xAxis, currentLabelY);
    }

    // Only update the y axis
    void UpdateInfoY(string yAxis) {
        UpdateInfo(currentLabelX, yAxis);
    }

    // Update the graph, but keep the axes the same
    void UpdateInfoNA() {
        UpdateInfo(currentLabelX, currentLabelY);
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Closes the scene
    void CloseScene() {
        slideOut = true;
    }
}
