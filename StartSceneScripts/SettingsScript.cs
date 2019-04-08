using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour {

    // The settings
    public Settings settings = null;

    // The settings canvas
    GameObject canvas;

    // The height of the canvas
    float canvasHeight;
    
    // Buttons
    public Button backButton;
    public Button defaultsButton;

    // Toggles
    public Toggle evolve;
    public Toggle vaccines;
    public Toggle research;
    public Toggle doubleTrts;

    // Input Fields
    public InputField budget;
    public InputField population;
    public InputField infected;
    public InputField spreadChance;
    public InputField recoveryChance;
    public InputField deathChance;
    public InputField randomInfection;
    public InputField mutation;
    public InputField money;
    public InputField penalty;
    public InputField personSpeed;

    // Treatment toggles and input fields
    public Toggle treatmentA;
    public Toggle treatmentB;
    public Toggle treatmentC;
    public InputField costA;
    public InputField costB;
    public InputField costC;
    public InputField efficacyA;
    public InputField efficacyB;
    public InputField efficacyC;

    // Walk-to location input fields
    public InputField houseU;
    public InputField factoryU;
    public InputField hospitalU;
    public InputField noneU;
    public InputField houseI;
    public InputField factoryI;
    public InputField hospitalI;
    public InputField noneI;

    // Whether or not the window is sliding into view or out of view
    bool slideIn = false;
    bool slideOut = false;

    // The rate at which the settings window slides in and out
    readonly int slideRate = 15;

    // The frame number
    int frame = 0;

    // +--------------------------+-------------------------------------------------------------------------------------------------------------------------------
    // | Awake, Start, and Update |
    // +--------------------------+

    // Called before any Start functions
    void Awake() {
        // Check if settings were left over from the last game
        GameObject prevSettings = GameObject.Find("GameSettings");
        if (prevSettings) {
            settings = Settings.Clone(prevSettings.GetComponent<SettingsScript>().settings);
        } else {
            settings = new Settings();
        }

        // Set the toggles
        evolve.isOn = settings.canEvolve;
        vaccines.isOn = settings.treatmentsAsVaccines;
        research.isOn = settings.canResearch;
        doubleTrts.isOn = settings.alternateTreatments;
        treatmentA.isOn = settings.treatmentA;
        treatmentB.isOn = settings.treatmentB;
        treatmentC.isOn = settings.treatmentC;

        // Set the input fields
        budget.text = settings.initBudget.ToString();
        population.text = settings.initPopulation.ToString();
        infected.text = settings.initPercentInfected.ToString();
        spreadChance.text = settings.spreadOnContact.ToString();
        recoveryChance.text = settings.recoveryChance.ToString();
        deathChance.text = settings.deathChance.ToString();
        randomInfection.text = settings.randomInfectionChance.ToString();
        mutation.text = settings.baseMutationChance.ToString();
        mutation.interactable = settings.canEvolve;
        money.text = settings.moneyPerWorker.ToString();
        penalty.text = settings.infectedPenalty.ToString();
        personSpeed.text = settings.speed.ToString();
        costA.text = settings.costA.ToString();
        costB.text = settings.costB.ToString();
        costC.text = settings.costC.ToString();
        efficacyA.text = settings.efficacyA.ToString();
        efficacyB.text = settings.efficacyB.ToString();
        efficacyC.text = settings.efficacyC.ToString();
        houseU.text = settings.walkToHouseU.ToString();
        factoryU.text = settings.walkToFactoryU.ToString();
        hospitalU.text = settings.walkToHospitalU.ToString();
        noneU.text = (100 - settings.walkToHouseU - settings.walkToFactoryU - settings.walkToHospitalU).ToString();
        houseI.text = settings.walkToHouseI.ToString();
        factoryI.text = settings.walkToFactoryI.ToString();
        hospitalI.text = settings.walkToHospitalI.ToString();
        noneI.text = (100 - settings.walkToHouseI - settings.walkToFactoryI - settings.walkToHospitalI).ToString();

        if (prevSettings) {
            Destroy(prevSettings.transform.parent.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        // Initialize the settings
        if (settings == null) {
            settings = new Settings();
        }

        // Get the settings canvas
        canvas = transform.parent.gameObject;
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        // Initialize the position of the window
        (transform as RectTransform).anchoredPosition = new Vector2(0, canvasHeight);

        // Add button listeners
        backButton.onClick.AddListener(     delegate { slideOut = true; });
        defaultsButton.onClick.AddListener( delegate { Awake(); });

        // Add toggle listeners
        evolve.onValueChanged.AddListener(     delegate {
            settings.canEvolve = evolve.isOn;
            mutation.interactable = evolve.isOn;
        });
        vaccines.onValueChanged.AddListener(   delegate { settings.treatmentsAsVaccines = vaccines.isOn; });
        research.onValueChanged.AddListener(   delegate { settings.canResearch          = research.isOn; });
        doubleTrts.onValueChanged.AddListener( delegate { settings.alternateTreatments  = doubleTrts.isOn; });

        // Add input field listeners
        budget.onEndEdit.AddListener(          delegate { OnRoundedIntEntered( budget,          out settings.initBudget,     0, 100000); });
        population.onEndEdit.AddListener(      delegate { OnRoundedIntEntered( population,      out settings.initPopulation, 100, 3000);   });
        infected.onEndEdit.AddListener(        delegate { OnPercentEntered(    infected,        out settings.initPercentInfected); });
        spreadChance.onEndEdit.AddListener(    delegate { OnPercentEntered(    spreadChance,    out settings.spreadOnContact);     });
        recoveryChance.onEndEdit.AddListener(  delegate { OnPercentEntered(    recoveryChance,  out settings.recoveryChance);      });
        deathChance.onEndEdit.AddListener(     delegate { OnPercentEntered(    deathChance,     out settings.deathChance);         });
        randomInfection.onEndEdit.AddListener( delegate { OnPercentEntered(    randomInfection, out settings.randomInfectionChance);   });
        mutation.onEndEdit.AddListener(        delegate { OnPercentEntered(    mutation,        out settings.baseMutationChance); });
        money.onEndEdit.AddListener(           delegate { OnRoundedIntEntered( money,           out settings.moneyPerWorker,  0, 100); });
        penalty.onEndEdit.AddListener(         delegate { OnRoundedIntEntered( penalty,         out settings.infectedPenalty, 0, 100); });
        personSpeed.onEndEdit.AddListener(     delegate { OnRoundedIntEntered( personSpeed,     out settings.speed, 1, 200);           });

        // Add listeners for treatment settings
        treatmentA.onValueChanged.AddListener( delegate { settings.treatmentA = treatmentA.isOn; });
        treatmentB.onValueChanged.AddListener( delegate { settings.treatmentB = treatmentB.isOn; });
        treatmentC.onValueChanged.AddListener( delegate { settings.treatmentC = treatmentC.isOn; });
        costA.onEndEdit.AddListener(           delegate { OnRoundedIntEntered(costA,  out settings.costA, 5, 200); });
        costB.onEndEdit.AddListener(           delegate { OnRoundedIntEntered(costB,  out settings.costB, 5, 200); });
        costC.onEndEdit.AddListener(           delegate { OnRoundedIntEntered(costC,  out settings.costC, 5, 200); });
        efficacyA.onEndEdit.AddListener(       delegate { OnPercentEntered(efficacyA, out settings.efficacyA); });
        efficacyB.onEndEdit.AddListener(       delegate { OnPercentEntered(efficacyB, out settings.efficacyB); });
        efficacyC.onEndEdit.AddListener(       delegate { OnPercentEntered(efficacyC, out settings.efficacyC); });

        // Add listeners for walk-to location settings
        InputField[] groupU = { houseU, factoryU, hospitalU, noneU };
        InputField[] groupI = { houseI, factoryI, hospitalI, noneI };
        houseU.onEndEdit.AddListener(delegate {
            AdjustFieldsInGroup(houseU,    groupU, out settings.walkToHouseU, out settings.walkToFactoryU, out settings.walkToHospitalU);
        });
        factoryU.onEndEdit.AddListener(delegate {
            AdjustFieldsInGroup(factoryU,  groupU, out settings.walkToHouseU, out settings.walkToFactoryU, out settings.walkToHospitalU);
        });
        hospitalU.onEndEdit.AddListener(delegate {
            AdjustFieldsInGroup(hospitalU, groupU, out settings.walkToHouseU, out settings.walkToFactoryU, out settings.walkToHospitalU);
        });
        noneU.onEndEdit.AddListener(delegate {
            AdjustFieldsInGroup(noneU,     groupU, out settings.walkToHouseU, out settings.walkToFactoryU, out settings.walkToHospitalU);
        });
        houseI.onEndEdit.AddListener(delegate {
            AdjustFieldsInGroup(houseI,    groupI, out settings.walkToHouseI, out settings.walkToFactoryI, out settings.walkToHospitalI);
        });
        factoryI.onEndEdit.AddListener(delegate {
            AdjustFieldsInGroup(factoryI,  groupI, out settings.walkToHouseI, out settings.walkToFactoryI, out settings.walkToHospitalI);
        });
        hospitalI.onEndEdit.AddListener(delegate {
            AdjustFieldsInGroup(hospitalI, groupI, out settings.walkToHouseI, out settings.walkToFactoryI, out settings.walkToHospitalI);
        });
        noneI.onEndEdit.AddListener(delegate {
            AdjustFieldsInGroup(noneI,     groupI, out settings.walkToHouseI, out settings.walkToFactoryI, out settings.walkToHospitalI);
        });
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

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Rounds an int entered into an input field
    void OnRoundedIntEntered(InputField input, out int writeTo, int lower, int upper) {
        int entered = int.Parse(input.text);

        // Round the int to be between lower and upper inclusive
        if (entered < lower) {
            entered = lower;
            input.text = lower.ToString();
        } else if (entered > upper) {
            entered = upper;
            input.text = upper.ToString();
        }

        writeTo = entered;
    }

    // Rounds a percentage entered into an input field
    void OnPercentEntered(InputField input, out float writeTo) {
        float percent = float.Parse(input.text);

        // Round the percent to be between 0 and 100
        if (percent < 0) {
            percent = 0;
            input.text = "0";
        } else if (percent > 100) {
            percent = 100;
            input.text = "100";
        }

        writeTo = percent;
    }

    // Adjusts the input fields in the group according to the new value for this input field
    // The fields array must be in this order: house, factory, hospital, none
    void AdjustFieldsInGroup(InputField input, InputField[] fields, out float out0, out float out1, out float out2) {
        // Bound the new value
        float newVal;
        OnPercentEntered(input, out newVal);

        // Find the index of input in fields
        int inputIndex = 0;
        while (inputIndex < fields.Length) {
            if (fields[inputIndex] == input) {
                break;
            }
            inputIndex++;
        }

        // Get the values of each field
        float[] values = new float[4];
        for (int i = 0; i < 4; i++) {
            values[i] = float.Parse(fields[i].text);
        }

        if (inputIndex != 3) {
            // Adjust noneVal first, then others
            float totalNoNone = values[0] + values[1] + values[2];
            float totalTwoOthers = totalNoNone - newVal;

            if (totalNoNone <= 100) {
                // Assign the remainder to none
                values[3] = 100 - totalNoNone;
            } else {
                // Adjust the two fields that are not the edited one
                for (int i = 0; i < 3; i++) {
                    if (i != inputIndex) {
                        float share = (values[i] / totalTwoOthers) * (totalNoNone - 100);
                        values[i] -= share;
                    }
                }

                // Set none to 0
                values[3] = 0;
            }
        } else {
            // Adjust the other three values
            float totalNoNone = values[0] + values[1] + values[2];
            float total = totalNoNone + values[3];

            for (int i = 0; i < 3; i++) {
                float share = (values[i] / totalNoNone) * (total - 100);
                values[i] -= share;
            }
        }

        // Update field text
        for (int i = 0; i < 4; i++) {
            fields[i].text = values[i].ToString();
        }

        // Assign out variables
        out0 = values[0];
        out1 = values[1];
        out2 = values[2];
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Opens the settings window
    void OpenSettings() {
        slideIn = true;
    }

    void setLevel1() {
        settings.canEvolve = false;
        settings.treatmentsAsVaccines = true;
        settings.canResearch = false;
        settings.alternateTreatments = true;
        settings.treatmentA = true;
        settings.treatmentB = true;
        settings.treatmentC = true;

        settings.initBudget = 5001;
        settings.initPopulation = 1000;
        settings.initPercentInfected = 2f;
        settings. moneyPerWorker = 5;
        settings. infectedPenalty = 2;

        settings. baseMutationChance = 0.1f;

        settings.treatmentA = true;
        settings.treatmentB = true;
        settings.treatmentC = true;

        settings. costA = 26;
        settings. costB = 26;
        settings. costC = 100;

        settings. efficacyA = 70;
        settings. efficacyB = 40;
        settings. efficacyC = 75;

        settings.spreadOnContact = 5f;
        settings.recoveryChance = 0f;
        settings.deathChance = 5f;
        settings.randomInfectionChance = 0f;

        settings.walkToHouseU = 33.33f;
        settings.walkToFactoryU = 33.33f;
        settings.walkToHospitalU = 0f;
        settings.walkToHouseI = 25f;
        settings.walkToFactoryI = 0f;
        settings.walkToHospitalI = 75f;

        settings.speed = 50;
    }
    void setLevel2()
    {
        settings.canEvolve = false;
        settings.treatmentsAsVaccines = true;
        settings.canResearch = true;
        settings.alternateTreatments = false;
        settings.treatmentA = true;
        settings.treatmentB = true;
        settings.treatmentC = true;

        settings.initBudget = 5002;
        settings.initPopulation = 1000;
        settings.initPercentInfected = 5f;
        settings.moneyPerWorker = 5;
        settings.infectedPenalty = 2;

        settings.baseMutationChance = 0.1f;

        settings.treatmentA = true;
        settings.treatmentB = true;
        settings.treatmentC = true;

        settings.costA = 25;
        settings.costB = 25;
        settings.costC = 100;

        settings.efficacyA = 70;
        settings.efficacyB = 40;
        settings.efficacyC = 75;

        settings.spreadOnContact = 5f;
        settings.recoveryChance = 0f;
        settings.deathChance = 5f;
        settings.randomInfectionChance = 0f;

        settings.walkToHouseU = 33.33f;
        settings.walkToFactoryU = 33.33f;
        settings.walkToHospitalU = 0f;
        settings.walkToHouseI = 25f;
        settings.walkToFactoryI = 0f;
        settings.walkToHospitalI = 75f;

        settings.speed = 50;
    }
    void setLevel3()
    {
        settings.canEvolve = false;
        settings.treatmentsAsVaccines = true;
        settings.canResearch = true;
        settings.alternateTreatments = false;
        settings.treatmentA = true;
        settings.treatmentB = true;
        settings.treatmentC = true;

        settings.initBudget = 5003;
        settings.initPopulation = 1000;
        settings.initPercentInfected = 2f;
        settings.moneyPerWorker = 5;
        settings.infectedPenalty = 2;

        settings.baseMutationChance = 0.1f;

        settings.treatmentA = true;
        settings.treatmentB = true;
        settings.treatmentC = true;

        settings.costA = 25;
        settings.costB = 25;
        settings.costC = 100;

        settings.efficacyA = 70;
        settings.efficacyB = 40;
        settings.efficacyC = 75;

        settings.spreadOnContact = 10f;
        settings.recoveryChance = 0f;
        settings.deathChance = 10f;
        settings.randomInfectionChance = 0f;

        settings.walkToHouseU = 33.33f;
        settings.walkToFactoryU = 33.33f;
        settings.walkToHospitalU = 0f;
        settings.walkToHouseI = 25f;
        settings.walkToFactoryI = 0f;
        settings.walkToHospitalI = 75f;

        settings.speed = 50;
    }
}
