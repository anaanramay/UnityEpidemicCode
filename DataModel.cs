using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;

public class DataModel : MonoBehaviour
{

    public static DataModel dataModel;
    public int count = 0;
    public int level;
    public DateTime dateTime;
    public string playerID;
    public string groupID;
    public string winLose;


    public List<int> day = new List<int>();
    public List<int> population = new List<int>();
    public List<int> healthy = new List<int>();
    public List<int> sick = new List<int>();
    public List<int> trtANum = new List<int>();
    public List<int> trtBNum = new List<int>();
    public List<int> trtCNum = new List<int>();
    public List<double> trtAPercent = new List<double>();
    public List<double> trtBPercent = new List<double>();
    public List<double> trtCPercent = new List<double>();
    public List<int> curedA = new List<int>();
    public List<int> curedB = new List<int>();
    public List<int> curedC = new List<int>();
    public List<int> curedDay = new List<int>();
    public List<int> newSick = new List<int>();
    public List<int> costTrtA = new List<int>();
    public List<int> costTrtB = new List<int>();
    public List<int> costTrtC = new List<int>();
    public List<int> costResearch = new List<int>();
    public List<int> sickCost = new List<int>();
    public List<int> dailyIncome = new List<int>();
    public List<int> costDay = new List<int>();
    public List<int> budget = new List<int>();


    // Singleton method. If there is no instance of this object, persist it to the scene. If there is already an instance of this object, destroy it and use this instance.
    void Awake ()
    {
        if (dataModel == null) {
            DontDestroyOnLoad (gameObject);
            dataModel = this;
        } else if (dataModel != this) {
            Destroy (gameObject);
        }
    }

    public int Count(){
        return count;
    }

    public void AddData()
    {
        GameControllerScript gc = GameObject.Find("Game Controller").GetComponent<GameControllerScript>();
            
        Debug.Log("AddData called: Day " + gc.TOTALUPDATES/10);

        this.day.Add(gc.TOTALUPDATES/10);
        this.population.Add(GameControllerScript.totalPopulation);
        this.budget.Add(GameControllerScript.budget);
        this.healthy.Add(GameControllerScript.numUninfected);
        this.sick.Add(GameControllerScript.totalPopulation - GameControllerScript.numUninfected);
        this.dailyIncome.Add(GameControllerScript.dailyIncome);

        TreatmentScript trtA = GameObject.Find("TreatmentA").GetComponent<TreatmentScript>();
        TreatmentScript trtB = GameObject.Find("TreatmentB").GetComponent<TreatmentScript>();
        TreatmentScript trtC = GameObject.Find("TreatmentC").GetComponent<TreatmentScript>();
        int trtATreats = trtA.dailyTreated;
        int trtBTreats = trtB.dailyTreated;
        int trtCTreats = trtC.dailyTreated;

        int totalTreats = trtATreats + trtBTreats + trtCTreats;

        this.trtANum.Add(trtATreats);
        this.trtBNum.Add(trtBTreats);
        this.trtCNum.Add(trtCTreats);


        this.curedA.Add(trtA.dailySuccesses);
        this.curedB.Add(trtB.dailySuccesses);
        this.curedC.Add(trtC.dailySuccesses);

        this.trtAPercent.Add(totalTreats == 0 ? 0.0 : trtATreats / totalTreats);
        this.trtBPercent.Add(totalTreats == 0 ? 0.0 : trtBTreats / totalTreats);
        this.trtCPercent.Add(totalTreats == 0 ? 0.0 : trtCTreats / totalTreats);

        this.curedDay.Add(trtA.successes + trtB.successes + trtC.successes);

        if (this.newSick.Count == 0) {
            this.newSick.Add(GameControllerScript.totalPopulation - GameControllerScript.numUninfected);
        } else {
            this.newSick.Add((GameControllerScript.totalPopulation - GameControllerScript.numUninfected) - this.newSick[this.newSick.Count - 1]);
        }

        int costA = trtA.dailyTreated * trtA.cost;
        int costB = trtB.dailyTreated * trtB.cost;
        int costC = trtC.dailyTreated * trtC.cost;

        this.costTrtA.Add(costA);
        this.costTrtB.Add(costB);
        this.costTrtC.Add(costC);

        this.costDay.Add(costA + costB + costC + GameControllerScript.totalSpentOnResearch);

        this.sickCost.Add(GameControllerScript.sickPenalty);

        if (this.costResearch.Count == 0)
        {
            this.costResearch.Add(GameControllerScript.totalSpentOnResearch);
        }
        else
        {
            this.costResearch.Add(GameControllerScript.totalSpentOnResearch - this.costResearch[this.costResearch.Count - 1]);
        }

        count++;

    }

}
