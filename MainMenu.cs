using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    // Button components
    public GameObject PlayID;
    public GameObject GroupID;
    public Button play;
    public Button level1;
    public Button level2;
    public Button level3;
    public Button tutorial;
    public Button settings;
    public Button leaderboard;

    // The settings window
    public GameObject settingsWindow;

    // The leaderboard window
    public GameObject boardWindow;

    // Debounce to prevent the player from spam clicking buttons
    bool clicked = false;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

    // Use this for initialization
    void Start()
    {
        // Add listeners for the buttons

        play.onClick.AddListener(delegate { PlayGame(); });
        level1.onClick.AddListener(delegate { PlayLevel1(); });
        level2.onClick.AddListener(delegate { PlayLevel2(); });
        level3.onClick.AddListener(delegate { PlayLevel3(); });
        tutorial.onClick.AddListener(delegate { StartCoroutine(Tutorial()); });
        settings.onClick.AddListener(delegate { Settings(); });
        leaderboard.onClick.AddListener(delegate { Leaderboard(); });

    }

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called whenever the input fields change
    public void UpdatePlayerData() {

        PlayerData.playerdata.playerID = this.PlayID.GetComponent<InputField>().text;
        PlayerData.playerdata.groupID = this.GroupID.GetComponent<InputField>().text;
    
    }

    // Called when the player clicks the play button
    void PlayGame()
    {
        if (!clicked)
        {
            clicked = true;

            // Keep the settings window but destroy all of its children
            DontDestroyOnLoad(settingsWindow.transform.parent);
            foreach (Transform t in settingsWindow.transform)
            {
                Destroy(t.gameObject);
            }



            // Load the main scene
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        }
    }

    void PlayLevel1()
    {

        PlayerData.playerdata.level = 1;

        if (!clicked)
        {
            if (!(string.IsNullOrEmpty(PlayID.GetComponent<InputField>().text) || string.IsNullOrEmpty(GroupID.GetComponent<InputField>().text)))
            {
                clicked = true;
                settingsWindow.SendMessage("setLevel1");
                // Keep the settings window but destroy all of its children
                DontDestroyOnLoad(settingsWindow.transform.parent);
                foreach (Transform t in settingsWindow.transform)
                {
                    Destroy(t.gameObject);
                }


                // Load the main scene
                settingsWindow.SendMessage("setLevel1");
                SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
                settingsWindow.SendMessage("setLevel1");
            }
        }
    }

    void PlayLevel2()
    {

        PlayerData.playerdata.level = 2;

        if (!clicked)
        {
            clicked = true;
            settingsWindow.SendMessage("setLevel2");
            // Keep the settings window but destroy all of its children
            DontDestroyOnLoad(settingsWindow.transform.parent);
            foreach (Transform t in settingsWindow.transform)
            {
                Destroy(t.gameObject);
            }


            // Load the main scene
            settingsWindow.SendMessage("setLevel2");
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            settingsWindow.SendMessage("setLevel2");
        }
    }

    void PlayLevel3()
    {
        PlayerData.playerdata.level = 3;

        if (!clicked)
        {
            clicked = true;
            settingsWindow.SendMessage("setLevel3");
            // Keep the settings window but destroy all of its children
            DontDestroyOnLoad(settingsWindow.transform.parent);
            foreach (Transform t in settingsWindow.transform)
            {
                Destroy(t.gameObject);
            }

            // Send PlayerID and GroupID to PlayerData gameObject
            //PushData();

            // Load the main scene
            settingsWindow.SendMessage("setLevel3");
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            settingsWindow.SendMessage("setLevel3");
        }
    }

    // Called when the player clicks the tutorial button
    IEnumerator Tutorial()
    {
        if (!clicked)
        {
            clicked = true;

            // Turn on the tutorial in the settings
            settingsWindow.GetComponent<SettingsScript>().settings.inTutorial = true;

            // Load the main scene over the top of this scene
            AsyncOperation loading = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive); // Main scene
            while (!loading.isDone)
            {
                yield return null;
            }

            // Unload this scene now that the main scene is fully loaded
            SceneManager.UnloadSceneAsync(0);
        }
    }

    // Called when the player clicks the settings button
    void Settings()
    {
        settingsWindow.SendMessage("OpenSettings");
    }

    // Called when the player clicks the leaderboard button
    void Leaderboard()
    {
        boardWindow.SendMessage("OpenLeaderboard");
    }
}
    
