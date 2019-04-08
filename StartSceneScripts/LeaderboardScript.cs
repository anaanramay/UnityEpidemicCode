using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour {

    // The settings canvas
    GameObject canvas;

    // The height of the canvas
    float canvasHeight;

    // Back button
    public Button backButton;

    // Leaderboard places array
    public GameObject[] places;

    // Whether or not the window is sliding into view or out of view
    bool slideIn = false;
    bool slideOut = false;

    // The rate at which the settings window slides in and out
    readonly int slideRate = 15;

    // The frame number
    int frame = 0;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {

        // Get the settings canvas
        canvas = transform.parent.gameObject;
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        // Initialize the position of the window
        (transform as RectTransform).anchoredPosition = new Vector2(0, canvasHeight);

        // Initialize the leaderboard text
        StartCoroutine(InitializeLeaderboard());

        // Add button listeners
        backButton.onClick.AddListener(delegate { slideOut = true; });
	}
	
	// Update is called once per frame
	void Update () {
        // Slide the window in or out
        if (slideIn && frame < slideRate) {
            (transform as RectTransform).anchoredPosition -= new Vector2(0, canvasHeight / slideRate);
            frame++;
        } else if (slideOut && frame < slideRate) {
            (transform as RectTransform).anchoredPosition += new Vector2(0, canvasHeight / slideRate);
            frame++;
        }

        // Reset the frame number and slide bools
        if (frame >= slideRate) {
            frame = 0;
            slideIn = false;
            slideOut = false;
        }
	}

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Initializes the leaderboard UI text
    IEnumerator InitializeLeaderboard() {

        WWW getLeaderboard = new WWW("https://stat2games.sites.grinnell.edu/php/getepidemicleaderboard.php");
        yield return getLeaderboard;

        string[] playerScores = {};

        if (getLeaderboard.text == "No characters found") {
            Debug.Log("No players in leaderboard");
        } else {
            playerScores = getLeaderboard.text.Split(";"[0]);
        }

        getLeaderboard.Dispose();


        for (int i = 0; i < playerScores.Length; i++) {

            string[] split = playerScores[i].Split(":"[0]);
            string playerID = split[0];
            string score = split[1];

            places[i].transform.Find("Player").GetComponent<Text>().text = playerID;
            places[i].transform.Find("Score").GetComponent<Text>().text = score;
        }

        yield return null;
    }

    // Opens the settings window
    void OpenLeaderboard() {
        slideIn = true;
    }
}
