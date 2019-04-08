using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreatmentScript : MonoBehaviour {

    // The probability that this treatment works
    [Range(0, 100)] public float chanceOfSuccess = 50;

    // Whether or not this is a starter treatment
    public bool starterTreatment = false;

    // The name of this treatment
    public string treatmentName = "A";

    // The cost of this treatment
    public int cost = 10;

    // The total amount spent on this treatment
    public int totalSpent;

    // Various components
    Toggle toggle;
    public Text text;
    public Text shadowText;

    // Mouse attach object
    public GameObject mouseAttach;

    // Number of successes and failures
    public int successes;
    public int failures;

    public int dailyTreated;
    public int dailySuccesses;

    // The current player-known efficacy
    public float efficacy;

    // List of player-known efficacies at each update
    public List<float> efficacyList = new List<float>();

    // The strain number that is resistant to this treatment
    public int resistantStrain = int.MaxValue;

    // The array of strain colors
    Color[] strainColors;

    // The first game update this treatment has existed for
    public int startingUpdate = 0;

    // The frame number

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Get the toggle and text components
        toggle = GetComponent<Toggle>();

        // Get the array of strain colors
        strainColors = GameObject.FindWithTag("GameController").GetComponent<GameControllerScript>().strainColors;

        // Set the settings
        if (starterTreatment) {
            Settings settings = GameControllerScript.settings;
            cost = settings.GetCost(treatmentName);
            chanceOfSuccess = settings.GetEfficacy(treatmentName);

            // Discard this treatment if set inactive
            if (!settings.GetActive(treatmentName)) {
                foreach (Transform hospital in GameObject.Find("Structures").transform) {
                    if (hospital.tag == "hospital") {
                        hospital.SendMessage("RemoveTreatment", treatmentName);
                    }
                }

                float yPos = transform.localPosition.y;
                foreach (RectTransform t in transform.parent.transform) {
                    // If toggle t was below the removed toggle, move it up to fill the gap
                    if (t.GetComponent<Toggle>().interactable && t.localPosition.y < yPos) {
                        t.anchoredPosition += new Vector2(0, 20);
                    }
                }

                Destroy(gameObject);
            }
        }

        // Initialize the text
        SetText();
	}

    // +-----------+----------------------------------------------------------------------------------------------------------------------------------------------
    // | Evolution |
    // +-----------+

    // Checks if this strain of illness has evolved to the next strain
    // Returns 0 if no mutation, 1 if mutated into an existing strain, 2 if mutated into a new strain
    public int NextStrain(int currentStrain, GameObject person) {
        // If evolution is turned off, then do not mutate
        if (!GameControllerScript.canEvolve || GameControllerScript.totalUpdates <= 50) {
            return 0;
        }

        int maxStrain = GameControllerScript.maxStrain;
        int maxNumStrains = strainColors.Length;

        if (currentStrain < strainColors.Length && currentStrain < resistantStrain) {
            float constant = GameControllerScript.baseMutationChance / ((maxNumStrains - 1) / (float)maxNumStrains);

            float chanceOfMutation = maxNumStrains - currentStrain;
            chanceOfMutation /= (float)maxNumStrains;
            chanceOfMutation *= constant;
            chanceOfMutation /= 5f;

            if (Random.value < chanceOfMutation) {
                person.SendMessage("Infect", currentStrain + 1);

                if (currentStrain == maxStrain) {
                    GameControllerScript.maxStrain = maxStrain + 1;

                    StartCoroutine(GameControllerScript.ChangePopupText(
                        "A new strain that is resistant to treatment " + treatmentName + " has appeared!"
                    ));

                    return 2;
                } else {
                    return 1;
                }
            } else if (resistantStrain > maxNumStrains && currentStrain < maxStrain && Random.value < 0.015625f) {
                StartCoroutine(GameControllerScript.ChangePopupText(
                    "Strain " + (currentStrain + 1) + " has become resistant to treatment " + treatmentName + "!"
                ));

                return 2;
            }
        }

        return 0;
    }

    // +-----------+----------------------------------------------------------------------------------------------------------------------------------------------
    // | Treatment |
    // +-----------+

    // Called when a person is treated with this treatment, returns 0 = failed treatment, 1 = successful treatment, 2 = insufficient funds
    public int OnTreat(GameObject person, bool manually, bool canMutate = true) {
        int success = 0;

        int strain = person.GetComponent<PersonBehaviourScript>().strainNumber;

        // Charge the budget
        bool enoughFunds = GameControllerScript.ReduceBudget(cost);

        GameControllerScript gc = GameObject.Find("Game Controller").GetComponent<GameControllerScript>();
        if (gc.TOTALUPDATES % 10 == 0)
        {
            dailyTreated = 0;
            dailySuccesses = 0;
        }

            if (enoughFunds) {
            

            if (strain < resistantStrain) {

                if (person.tag == "infected") {

                    if (Random.value <= chanceOfSuccess / 100f) {
                        // Treat the person
                        if (manually) { mouseAttach.SendMessage("ChangeText", "Treatment successful!"); }
                        person.SendMessage("OnTreat");

                        // Add to the total spent on this treatment
                        totalSpent += cost;

                        dailyTreated++;
                        dailySuccesses++;

                        successes++;
                        success = 1;

                    } else {
                        // Has this disease mutated?
                        int mutated = canMutate ? NextStrain(strain, person) : 0;

                        if (mutated != 0) {
                            // The strain has mutated resistance against this treatment
                            resistantStrain = (mutated == 2) ? strain + 1 : resistantStrain;
                            if (manually) { mouseAttach.SendMessage("ChangeText", "Strain mutated!"); }
                        } else {
                            if (manually) { mouseAttach.SendMessage("ChangeText", "Treatment failed!"); }
                        }

                        // Add to the total spent on this treatment
                        totalSpent += cost;
                        dailyTreated++;

                        failures++;
                    }
                } else {
                    // This person was not actuall infected, but we tried to treat them anyway

                    // Add to the total spent on this treatment
                    totalSpent += cost;
                    dailyTreated++;

                    success = 1;
                }
            } else {
                // This person's strain is resistant to the treatment
                if (manually) { mouseAttach.SendMessage("ChangeText", "Strain resistant!"); }

                // Add to the total spent on this treatment
                totalSpent += cost;
                dailyTreated++;

                failures++;
            }
        } else {
            // There wasn't enough money in the budget to use this treatment
            if (manually) { mouseAttach.SendMessage("ChangeText", "Insufficient funds."); }
            success = 2;
        }

        // Update the text
        SetText();

        return success;
    }

    // Called when a person visits a hospital using this treatment as the only dose or as the second dose
    void OnTreatAuto(GameObject person) {
        OnTreat(person, false);
    }

    // Called when a person visits a hospital using this treatment as the first dose
    void DoubleTreatAuto(GameObject[] param) {
        GameObject person = param[0];
        TreatmentScript treatment2 = param[1].GetComponent<TreatmentScript>();

        // Use both of the treatments
        int firstSuccess = OnTreat(person, false, false);
        int secondSuccess = treatment2.OnTreat(person, false, false);

        // Check if the first treatment succeeded
        if (firstSuccess != 1) {
            // Check if the second treatment succeeded
            if (secondSuccess != 1) {
                int strain = person.GetComponent<PersonBehaviourScript>().strainNumber;
                int mutated = 0;

                // The disease might mutate
                if (firstSuccess != 2 && Random.value < 0.5f) {
                    // Mutate resistance against the first treatment

                    mutated = NextStrain(strain, person);
                    if (mutated != 0) {
                        resistantStrain = (mutated == 2) ? strain + 1 : resistantStrain;
                    }
                } else if (secondSuccess != 2) {
                    // Mutate resistance against the second treatment

                    mutated = treatment2.NextStrain(strain, person);
                    if (mutated != 0) {
                        treatment2.resistantStrain = (mutated == 2) ? strain + 1 : treatment2.resistantStrain;
                    }
                }
            }
        }
    }

    // Called when the player treats a person manually
    void OnTreatManual(GameObject person) {
        if (toggle.isOn && person.tag == "infected") {
            OnTreat(person, true);
        }
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Called every game update
    void OnGameUpdate() {
        efficacyList.Add(efficacy);
    }

    // Sets the text of the toggle to include cost and efficacy
    void SetText() {
        if (toggle.interactable) {
            efficacy = (float)successes / (successes + failures);
            if (successes + failures == 0) {
                efficacy = 0;
            } else {
                efficacy *= 100;
            }

            if (successes == 0 && failures == 0) {
                text.text = "Treatment " + treatmentName + ": Cost: $" + cost + " Efficacy: ??? Total Treated: 0";
                shadowText.text = text.text;
            } else {
                text.text = "Treatment " + treatmentName + ": Cost: $" + cost + " Efficacy: " + efficacy.ToString("n2") + "% Total Treated: " + (successes + failures);
                shadowText.text = text.text;
            }
        }
    }

    // Renames this treatment if this was the treatment being renamed
    void Rename(string[] names) {
        if (treatmentName == names[0]) {
            treatmentName = names[1];
            gameObject.name = "Treatment" + names[1];
            SetText();
        }
    }
}
