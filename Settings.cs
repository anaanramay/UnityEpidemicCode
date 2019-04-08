using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for storing game settings data
public class Settings {

    // ------ GAME CONTROLLER SCRIPT SETTINGS -----------------------------------------------------------------------

    public bool inTutorial = false;
    //Do not set canReproduce to true
    public bool canReproduce = false;
    public bool canEvolve = false;
    public bool treatmentsAsVaccines = true;
    public bool canResearch = true;
    public bool alternateTreatments = true;

    public int initBudget = 5000;
    public int initPopulation = 1000;
    public int moneyPerWorker = 5;
    public int infectedPenalty = 2;

    public float initPercentInfected = 2f;
    public float baseMutationChance = 0.1f;

    // ------ TREATMENT SCRIPT SETTINGS -----------------------------------------------------------------------------

    public bool treatmentA = true;
    public bool treatmentB = true;
    public bool treatmentC = true;

    public int costA = 25;
    public int costB = 25;
    public int costC = 100;

    public float efficacyA = 70;
    public float efficacyB = 40;
    public float efficacyC = 75;

    // ------ PERSON SCRIPT SETTINGS --------------------------------------------------------------------------------

    public float spreadOnContact = 5f;
    public float recoveryChance = 0f;
    public float deathChance = 5f;
    public float randomInfectionChance = 0f;

    public float walkToHouseU = 33.33f;
    public float walkToFactoryU = 33.33f;
    public float walkToHospitalU = 0f;
    public float walkToHouseI = 25f;
    public float walkToFactoryI = 0f;
    public float walkToHospitalI = 75f;

    public int speed = 50;

    // ------ CLONE METHOD ------------------------------------------------------------------------------------------

    public static Settings Clone(Settings toClone) {
        Settings settings = new Settings();

        settings.canReproduce = toClone.canReproduce;
        settings.canEvolve = toClone.canEvolve;
        settings.treatmentsAsVaccines = toClone.treatmentsAsVaccines;
        settings.canResearch = toClone.canResearch;
        settings.initBudget = toClone.initBudget;
        settings.initPopulation = toClone.initPopulation;
        settings.moneyPerWorker = toClone.moneyPerWorker;
        settings.infectedPenalty = toClone.infectedPenalty;
        settings.initPercentInfected = toClone.initPercentInfected;
        settings.baseMutationChance = toClone.baseMutationChance;

        settings.treatmentA = toClone.treatmentA;
        settings.treatmentB = toClone.treatmentB;
        settings.treatmentC = toClone.treatmentC;
        settings.costA = toClone.costA;
        settings.costB = toClone.costB;
        settings.costC = toClone.costC;
        settings.efficacyA = toClone.efficacyA;
        settings.efficacyB = toClone.efficacyB;
        settings.efficacyC = toClone.efficacyC;

        settings.spreadOnContact = toClone.spreadOnContact;
        settings.recoveryChance = toClone.recoveryChance;
        settings.deathChance = toClone.deathChance;
        settings.randomInfectionChance = toClone.randomInfectionChance;
        settings.walkToHouseU = toClone.walkToHouseU;
        settings.walkToFactoryU = toClone.walkToFactoryU;
        settings.walkToHospitalU = toClone.walkToHospitalU;
        settings.walkToHouseI = toClone.walkToHouseI;
        settings.walkToFactoryI = toClone.walkToFactoryI;
        settings.walkToHospitalI = toClone.walkToHospitalI;
        settings.speed = toClone.speed;

        return settings;
    }

    // ------ IS DEFAULT METHOD -------------------------------------------------------------------------------------

    // Determines if this settings object is the default settings
    public bool IsDefault() {
        Settings def = new Settings();

        bool[] checks = {
            inTutorial == false,

            canReproduce == def.canReproduce,
            canEvolve == def.canEvolve,
            treatmentsAsVaccines == def.treatmentsAsVaccines,
            canResearch == def.canResearch,
            initBudget == def.initBudget,
            initPopulation == def.initPopulation,
            moneyPerWorker == def.moneyPerWorker,
            infectedPenalty == def.infectedPenalty,
            Mathf.Approximately(initPercentInfected, def.initPercentInfected),
            Mathf.Approximately(baseMutationChance, def.baseMutationChance),

            treatmentA == def.treatmentA,
            treatmentB == def.treatmentB,
            treatmentC == def.treatmentC,
            costA == def.costA,
            costB == def.costB,
            costC == def.costC,
            Mathf.Approximately(efficacyA, def.efficacyA),
            Mathf.Approximately(efficacyB, def.efficacyB),
            Mathf.Approximately(efficacyC, def.efficacyC),

            Mathf.Approximately(spreadOnContact, def.spreadOnContact),
            Mathf.Approximately(recoveryChance, def.recoveryChance),
            Mathf.Approximately(deathChance, def.deathChance),
            Mathf.Approximately(randomInfectionChance, def.randomInfectionChance),
            Mathf.Approximately(walkToHouseU, def.walkToHouseU),
            Mathf.Approximately(walkToFactoryU, def.walkToFactoryU),
            Mathf.Approximately(walkToHospitalU, def.walkToHospitalU),
            Mathf.Approximately(walkToHouseI, def.walkToHouseI),
            Mathf.Approximately(walkToFactoryI, def.walkToFactoryI),
            Mathf.Approximately(walkToHospitalI, def.walkToHospitalI),
            Mathf.Approximately(speed, def.speed)
        };

        foreach (bool check in checks) {
            if (!check) {
                return false;
            }
        }

        return true;
    }

    // ------ TREATMENT SETTINGS GETTERS ----------------------------------------------------------------------------

    public bool GetActive(string treatment) {
        switch (treatment.ToLower()) {
            case "a":
                return treatmentA;
            case "b":
                return treatmentB;
            case "c":
                return treatmentC;
            default:
                return false;
        }
    }

    public int GetCost(string treatment) {
        switch (treatment.ToLower()) {
            case "a":
                return costA;
            case "b":
                return costB;
            case "c":
                return costC;
            default:
                return 50;
        }
    }

    public float GetEfficacy(string treatment) {
        switch (treatment.ToLower()) {
            case "a":
                return efficacyA;
            case "b":
                return efficacyB;
            case "c":
                return efficacyC;
            default:
                return 50f;
        }
    }
}
