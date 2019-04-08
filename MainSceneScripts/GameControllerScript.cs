using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Class to store treatment data for when a new treatment is discovered
public class TreatmentData {
    
    public int cost;     // The cost of the new treatment
    public int efficacy; // The efficacy of the new treatment

    // Constructor
    public TreatmentData(int cost, int efficacy) {
        this.cost = cost;
        this.efficacy = efficacy;
    }
}

// Class to store information about how the player won or lost
public class WinData {

    public int status;    // 0 = still playing, 1 = win, 2 = lose
    public string reason; // The reason the player won or lost

    // Constructor
    public WinData(int status, string reason) {
        this.status = status;
        this.reason = reason;
    }
}

// Main controller class
public class GameControllerScript : MonoBehaviour {

    // State of this scene, game becomes inactive if false
    public static bool sceneActive = true;

    // The settings this game was loaded with
    public static Settings settings;
    public Settings SETTINGS{
        get { return settings; }
    }

    // The main camera
    public static Camera cam;

    // The canvas for the tutorial
    public GameObject tutorial;

    // Whether ot not the tutorial is running
    public static bool inTutorial = false;
    public bool INTUTORIAL{
        get { return inTutorial; }
    }

    // Whether or not people can reproduce
    public static bool canReproduce = false;

    // Whether or not the disease can evolve
    public static bool canEvolve = true;

    // Whether or not treatments grant resistance
    public static bool treatmentsAsVaccines = false;

    // Whether or not new treatments can be researched
    public static bool canResearch = true;

    // Whether hospitals offering two treatments should alternate them or give double doses
    public static bool alternateTreatments = false;

    // Amount of money rewarded per uninfected person
    public static int moneyPerWorker = 5;

    // A penalty cost for having infected people
    public static int infectedPenalty = 2;

    public static int sickPenalty = 0;

    // Counter for daily income every update. 
    public static int dailyIncome = 0;

    public int INFECTEDPENALTY{
        get { return infectedPenalty; }
    }

    // Array of colors to use for the different illness strains
    public Color[] strainColors = new Color[20];

    // The highest strain number in the game right now
    public static int maxStrain = 1;

    // The chance of the first strain mutating (chance of further strains decreases)
    [Range(0f, 100f)] public static float baseMutationChance = 10f;

    // Budget variables
    public static int budget = 5000;
    public static int researchBudget;
    public static int newTreatmentGuaranteed = 15000;
    public static TreatmentData newTreatment = null;

    // Overall budget data
    public static int totalEarned;
    public static int totalSpentOnResearch;

    // GUI components
    public static Text budgetText;
    public static Text winText;
    public GameObject gameOverImage;
    public GameObject returnButton;
    public GameObject replayButton;
    public GameObject submitDataButton;
    public InputField playerNameInput;
    //Additional GUI components
    public static Text populationText;
    public static Text dayCounter;
    public static Text infectedCounter;

    public static float infectedMultiplier = 1f;

    // WinData tracker for Data Export
    public static int state;
    public int STATE{
        get { return state; }
    }

    public int score;

    // GameObject storing all the people
    public static GameObject people;

    // GameObject storing all the structures
    public static GameObject structures;

    // Update variables
    int frame;
    public static int updateFrequency = 200;
    public static int totalUpdates;
    public int TOTALUPDATES{
        get { return totalUpdates; }
    }

    // Population variables
    const int maxPopulation = 5000;
    [Range(300, maxPopulation)] public static int initialPopulation = 300;
    [Range(0,   maxPopulation)] public static int totalPopulation;
    [Range(0,   maxPopulation)] public static int numUninfected;
    [Range(0,   maxPopulation)] public static int numInfected = 50;

    // Population and budget stats per game update
    public static List<float> totalPopList      = new List<float>();
    public static List<float> uninfectedPopList = new List<float>();
    public static List<float> infectedPopList   = new List<float>();
    public static List<float> budgetList        = new List<float>();

    // Binomial Randomizer Object (needed in order to have only
    // one instance of the binomial dictionary of probs.)
    [SerializeField]
    public int variance;
    public static Randomize rand;

    public static bool gameStarted = false;

	// +--------------------------+-------------------------------------------------------------------------------------------------------------------------------
	// | Awake, Start, and Update |
	// +--------------------------+

	// Called before any Start functions
	void Awake() {
        // Get the settings
        GameObject settingsWindow = GameObject.Find("SettingsWindow");
        if (settingsWindow) {
            settingsWindow.name = "GameSettings";
            settings = settingsWindow.GetComponent<SettingsScript>().settings;
        } else {
            settingsWindow = GameObject.Find("GameSettings");
            if (settingsWindow) {
                settings = settingsWindow.GetComponent<SettingsScript>().settings;
            } else {
                settings = new Settings();
            }
        }

        // Turn on the tutorial if necessary
        inTutorial = settings.inTutorial;
        if (inTutorial) {
            tutorial.SetActive(true);
        }

        // Set all of the game controller script settings
        canReproduce = settings.canReproduce;
        canEvolve = settings.canEvolve;
        treatmentsAsVaccines = settings.treatmentsAsVaccines;
        canResearch = settings.canResearch;
        alternateTreatments = settings.alternateTreatments;

        budget = settings.initBudget;
        initialPopulation = settings.initPopulation;
        numInfected = (int)(initialPopulation * (settings.initPercentInfected / 100f));

        moneyPerWorker = settings.moneyPerWorker;
        infectedPenalty = settings.infectedPenalty;

        baseMutationChance = settings.baseMutationChance;

        // Reset all the variables that may have been left over from the last game
        maxStrain = 1;
        researchBudget = 0;
        totalEarned = 0;
        totalSpentOnResearch = 0;
        newTreatment = null;
        newTreatmentGuaranteed = 15000;
        totalUpdates = 0;
        totalPopList.Clear();
        uninfectedPopList.Clear();
        infectedPopList.Clear();
        budgetList.Clear();

	}

	// Use this for initialization
	void Start() {
        
        // Get the people and structures container objects
        people = GameObject.Find("People");
        structures = GameObject.Find("Structures");

        // Get the budget and win texts
        budgetText = GameObject.Find("BudgetText").GetComponent<Text>();
        winText = GameObject.Find("WinText").GetComponent<Text>();

        //Get population and other texts
        populationText = GameObject.Find("TotalPopulation").GetComponent<Text>();
        dayCounter = GameObject.Find("DayCounter").GetComponent<Text>();
        infectedCounter = GameObject.Find("InfectedCounter").GetComponent<Text>();

        // Get the camera
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        // Set budget text to the initial 500 dollars
        SetBudgetText();
        SetPopulationText();
        SetDayText();
        SetInfectedText();

        gameOverImage.SetActive(false);

        numUninfected = initialPopulation - numInfected;
        totalPopulation = initialPopulation;

        // Initialize the IB randomization/create the dictionary
        rand = new Randomize(variance);
        //GameObject.Find("PauseButton").BroadcastMessage("PauseGame", false);
        sceneActive = false;
        gameStarted = false;
	}
	
	// Update is called once per frame
	void Update() {
        GameObject[] hospitals = GameObject.FindGameObjectsWithTag("hospital");
        Boolean allTreatmentsSet = true;
        foreach(GameObject hosp in hospitals)
        {
            HospitalScript hos = hosp.GetComponent(typeof(HospitalScript)) as HospitalScript;
            if(hos.treatmentName1.Equals("none") && hos.treatmentName2.Equals("none"))
            {
                allTreatmentsSet = false;
            }
        }
        if(allTreatmentsSet)
        { 
            removeTreatmentMessege();

        }

        if(!sceneActive && allTreatmentsSet && !gameStarted)
        {
            sceneActive = true;
            gameStarted = true;
        }

        if (sceneActive) {

            // Run a game update every updateFrequency frames
            if (frame >= updateFrequency - 1) {
                frame = 0;
                totalUpdates++;

                // Add relevant data to DataModel
                if (totalUpdates % 10 == 0)
                {
                    DataModel.dataModel.AddData();
                }

                //Increases infectiveness over time if needed
                infectedMultiplier = infectedMultiplier * 1.00f;

                GameUpdate();
            }

            if (frame >= 0) {
                frame++;
            }
        }
	}

    public void removeTreatmentMessege()
    {
        GameObject l = GameObject.FindWithTag("treatment messege");
        if (l != null)
        {
            l.SetActive(false);
        }
    }

    // +-------------+--------------------------------------------------------------------------------------------------------------------------------------------
    // | Game Update |
    // +-------------+

    // Called every updateFrequency frames and broadcasts OnGameUpdate to all other scripts
    void GameUpdate() {
        // Calculate population counts
        PopulationCounts();
        // Add to the budget
        budgetList.Add((float)budget);

        // Checks if it is end of day and if it is resets daily income counter to
        if (totalUpdates % 10 == 0)
        {
            dailyIncome = 0;
        }

        int income = CalculateIncome();
        dailyIncome += income;

        sickPenalty = CalculateInfectedCost();
        budget += income - sickPenalty;
        totalEarned += income;

        // Ensure budget does not go negative
        if (budget < 0) {
            budget = 0;
        }

        // Set the budget text
        SetBudgetText();
        SetPopulationText();
        SetDayText();
        SetInfectedText();

        transform.parent.BroadcastMessage("OnGameUpdate", SendMessageOptions.DontRequireReceiver);


       

        // Add to the population lists
        totalPopList.Add((float)totalPopulation);
        uninfectedPopList.Add((float)numUninfected / totalPopulation * 100);
        infectedPopList.Add((float)numInfected / totalPopulation * 100);

        // Research a new treatment
        Research();

        // Checks if game is over
        if (totalUpdates > 15) {
            GameOver();
        }
    }

    // Sets the budget text
    static void SetBudgetText() {
        if (!budgetText) {
            GameObject.Find("BudgetText").GetComponent<Text>().text = "Budget: " + budget.ToString("c0");
        } else {
            budgetText.text = "Budget: " + budget.ToString("c0");
        }
    }
    // Sets the population text
    static void SetPopulationText()
    {
        if (!populationText)
        {
            GameObject.Find("TotalPopulation").GetComponent<Text>().text = "Population: " + totalPopulation.ToString();
        }
        else
        {
            populationText.text = "Population: " + totalPopulation.ToString();
        }
    }
    // Sets the date text
    static void SetDayText()
    {
        if (!dayCounter)
        {
            GameObject.Find("DayCounter").GetComponent<Text>().text = "Day: " + (double)totalUpdates/10.0;
        }
        else
        {
            dayCounter.text = "Day: " + (double)totalUpdates/10.0;
        }
    }
    // Sets the infected text
    static void SetInfectedText()
    {
        if (!infectedCounter)
        {
            GameObject.Find("InfectedCounter").GetComponent<Text>().text = "Infected: " + numInfected;
        }
        else
        {
            infectedCounter.text = "Infected: " + numInfected + ", (" + Math.Round((float)numInfected * 100 / (float)totalPopulation) +"%)";
        }
    }


    // Calculates how much more money the player gets this update
    int CalculateIncome() {

        int totalWorkers = 0;

        foreach (Transform building in structures.transform) {
            if (building.tag == "factory") {
                totalWorkers += building.GetComponent<FactoryScript>().workers.Count;
            }
        }

        return moneyPerWorker * totalWorkers;
    }

    int CalculateInfectedCost(){
        return numInfected * infectedPenalty;
    }

    // Gets the population counts
    void PopulationCounts() {
        List<GameObject> infected = new List<GameObject>();
        List<GameObject> susceptible = new List<GameObject>();
        List<GameObject> resistant = new List<GameObject>();

        foreach (Transform person in people.transform) {
            if (person.tag == "infected") {
                infected.Add(person.gameObject);
            } else if (person.tag == "susceptible") {
                susceptible.Add(person.gameObject);
            } else if (person.tag == "resistant") {
                resistant.Add(person.gameObject);
            }
        }

        numInfected = infected.Count;
        numUninfected = susceptible.Count + resistant.Count;
        totalPopulation = numInfected + numUninfected;
    }

    // +----------+-----------------------------------------------------------------------------------------------------------------------------------------------
    // | Research |
    // +----------+

    // Determines if a new treatment has been found this game update
    void Research() {
        int guaranteed = newTreatmentGuaranteed;
        float constant = guaranteed / Mathf.Sqrt((float)int.MaxValue);
        float chanceOfTreatment = ((researchBudget / constant) * (researchBudget / constant)) / int.MaxValue;

        if (UnityEngine.Random.value < chanceOfTreatment) {
            // Generate a new treatment
            NewTreatment(researchBudget);
            researchBudget = 0;
        }
    }

    // Creates a new treatment
    void NewTreatment(int money) {
        // Increase the amount of money to guarantee a new treatment
        newTreatmentGuaranteed += 2000;

        // Set the max efficacy
        // When variance != 0, high percentages tend to end up being too overpowered
        int maxEfficacy = (variance != 0) ? 80 : 95;

        // Choose an efficacy
        int efficacy = UnityEngine.Random.Range(5, maxEfficacy);

        if (money <= newTreatmentGuaranteed / 3) {
            efficacy -= UnityEngine.Random.Range(5, 15);
        } else if (money >= (newTreatmentGuaranteed / 3) * 2) {
            efficacy += UnityEngine.Random.Range(5, 15);
        }

        if (efficacy >= maxEfficacy) {
            efficacy = maxEfficacy - UnityEngine.Random.Range(0, 5);
        } else if (efficacy <= 5) {
            efficacy = 5 + UnityEngine.Random.Range(0, 5);
        }

        // Choose a cost
        int cost = UnityEngine.Random.Range(2, 20) * (5 * efficacy / 50);
        if (cost < 5) { cost = 5 + UnityEngine.Random.Range(0, 5); }

        // Generate the new treatment
        newTreatment = new TreatmentData(cost, efficacy);

        // Notify the player
        StartCoroutine(ChangePopupText("A new treatment has been discovered!"));
    }

    // +---------+------------------------------------------------------------------------------------------------------------------------------------------------
    // | Endgame |
    // +---------+

    // Calculates the player's score
    int CalculateScore(bool win) {
        if (win) {
            float constant = 75f;

            float score = ((float)totalPopulation / initialPopulation);
            score *= (constant / totalUpdates) * 100000f;
            score += (budget / 4f > 100000) ? 100000 : (budget / 4f);

            return (int)score;
        } else {
            float constant = 200f;

            float score = ((float)totalPopulation / initialPopulation);
            score *= (totalUpdates / constant) * 100000f;

            return (int)score / 4;
        }
    }

    // Checks if the player has won because the infection died off
    bool InfectionEradicated() {
        return numInfected < totalPopulation / 10;
    }

    // Checks if the player has lost because the population dwindled too low
    bool PopulationTooLow() {
        return totalPopulation <= 50;
    }

    bool TooManyInfected() {
        return numInfected > totalPopulation*.95f;
    }

    // Checks if the player has lost because they could not balance their budget (budget below 1000 for five updates)
    bool BudgetNotBalanced() {
        bool lose = true;

        for (int i = 1; i < 6; i++) {
            if (budgetList[budgetList.Count - i] > 1000) {
                lose = false;
                break;
            }
        }

        return lose;
    }

    // Determines if the player has won, lost, or is still playing
    WinData DetermineWin() {

        // Wins
        if (InfectionEradicated()) {
            state = 1;
            return new WinData(1, "the disease was sufficiently eradicated");
        }

        //Losses
        if (PopulationTooLow())
        {
            state = 2;
            return new WinData(2, "the population got too low");
        }
        else if (BudgetNotBalanced())
        {
            state = 2;
            return new WinData(2, "the budget was not balanced");
        }
        else if (TooManyInfected())
        {
            state = 2;
            return new WinData(2, "too many people were infected");
        }
        // Still playing
        state = 0;
        return new WinData(0, "");
    }

    // Checks if the player has won, lost, or is still in the game
    void GameOver(){
        if (!tutorial.activeInHierarchy && totalUpdates > 15) {
            WinData data = DetermineWin();

            int sc = 0;

            if (data.status == 1) {

                DataModel.dataModel.winLose = "Win";
                sc = CalculateScore(true);

            } else if (data.status == 2) {

                DataModel.dataModel.winLose = "Lose";
                sc = CalculateScore(false);

            }

            StartCoroutine(SubmitScore(sc));

            if (data.status == 1) {
                // The player wins
                this.score = CalculateScore(true);
                SaveMainScene();
                gameOverImage.SetActive(true);
                gameOverImage.transform.Find("TextIcon").gameObject.SetActive(false);
                gameOverImage.transform.Find("TextIconShadow").gameObject.SetActive(false);
                returnButton.SetActive(true);
                replayButton.SetActive(true);
                winText.text = "You won because " + data.reason +"!\nScore: " + sc;
                submitDataButton.SetActive(true);
                GameObject.Find("SubmitData").GetComponent<Button>().onClick.Invoke();
                submitDataButton.SetActive(false);

                frame = -1;

            } else if (data.status == 2) {
                // The player loses
                sceneActive = false;
                gameStarted = false;
                this.score = CalculateScore(false);
                SaveMainScene();
                gameOverImage.SetActive(true);
                returnButton.SetActive(true);
                replayButton.SetActive(true);
                winText.text = "You lost because " + data.reason + "!\nScore: " + sc;
                submitDataButton.SetActive(true);
                GameObject.Find("SubmitData").GetComponent<Button>().onClick.Invoke();
                submitDataButton.SetActive(false);

                frame = -1;
            }
        }
    }

    public IEnumerator SubmitScore(int score) {

        WWWForm form = new WWWForm();

        form.AddField("level", PlayerData.playerdata.level);
        form.AddField("score", score);

        form.AddField("PlayerID", PlayerData.playerdata.playerID);
        form.AddField("GroupID", PlayerData.playerdata.groupID);

        WWW www = new WWW("https://stat2games.sites.grinnell.edu/php/sendepidemicscore.php", form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("Score data created successfully.");
        }
        else
        {
            Debug.Log("Score data creation failed. Error # " + www.text);
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Called when the player uses a treatment, returns whether or not there are enough funds
    public static bool ReduceBudget(int amount) {
        if (budget - amount < 0) {
            return false;
        } else {
            budget -= amount;
            SetBudgetText();
            return true;
        }
    }

    // Called when the player puts money into research, returns whether or not there are enough funds
    public static bool AddToResearch(int amount) {
        if (budget - amount < 0) {
            return false;
        } else {
            budget -= amount;
            SetBudgetText();
            researchBudget += amount;
            totalSpentOnResearch += amount;
            return true;
        }
    }

    public static IEnumerator ChangePopupText(string text, int duration = 5) {
        winText.text = text;
        yield return new WaitForSeconds(duration);
        if (winText.text == text) {
            winText.text = "";
        }
    }

    // Saves the current state of the main scene by pausing it
    static void SaveMainScene() {
        GameObject.Find("PauseButton").SendMessage("PauseGame", true);

        if (!cam) {
            cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }
        cam.gameObject.SendMessage("ToggleCameraActive");

        foreach (Transform st in structures.transform) {
            st.SendMessage("CloseMenus");
        }
    }

    // Called when the player switches to a different scene
    public static IEnumerator LoadSceneNumber(int sceneNum) {
        SaveMainScene();

        // Load the new scene over the top of the main scene
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneNum, LoadSceneMode.Additive);
        while (!loading.isDone) {
            yield return null;
        }

        // Set the camera of the new scene and make the new scene active
        GameObject.FindWithTag("main overlay").GetComponent<Canvas>().worldCamera = cam;
        //GameObject.FindWithTag("main overlay").GetComponent<Canvas>().sortingLayerName = "OverlayScreens";
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneNum));
    }

    // Sets the activeness of the scene
    void SetSceneActive(bool active) {
        sceneActive = active;
    }

    public Boolean getSceneActive()
    {
        return sceneActive;
    }
}
