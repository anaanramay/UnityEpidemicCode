using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Text;

//this script sends game data to the server
public class SubmitData : MonoBehaviour {



	//This Method respond to the click on the submit button
    public void SubmitUpload(){
        StartCoroutine (Upload());
	}

    IEnumerator Upload()
    {
        DataModel data = DataModel.dataModel;

        int gameNum = -1;

        WWW getGameNum = new WWW("https://stat2games.sites.grinnell.edu/php/getepidemicgamenum.php");
        yield return getGameNum;

        try
        {
            gameNum = int.Parse(getGameNum.text);
        }
        catch (System.Exception e)
        {
            Debug.Log("Fetching game number failed.  Error message: " + e.ToString());
        }
        for (int index = 0; index < data.day.Count; index++)
        {
            WWWForm form = new WWWForm();
            form.AddField("Game", gameNum);
            form.AddField("Level", PlayerData.playerdata.level);
            form.AddField("PlayerID", PlayerData.playerdata.playerID);
            form.AddField("GroupID", PlayerData.playerdata.groupID);
            form.AddField("Day", data.day[index]);

            form.AddField("Population", data.population[index]);

            form.AddField("Budget", data.budget[index]);
            Debug.Log("Budget: " + data.budget[index]);

            form.AddField("AvailToTreat", data.sick[index]);

            form.AddField("TreatA", data.trtANum[index]);

            form.AddField("CureA", data.curedA[index]);

            form.AddField("TreatB", data.trtBNum[index]);

            form.AddField("CureB", data.curedB[index]);

            form.AddField("TreatC", data.trtCNum[index]);

            form.AddField("CureC", data.curedC[index]);

            form.AddField("CostA", data.costTrtA[index]);

            form.AddField("CostB", data.costTrtB[index]);

            form.AddField("CostC", data.costTrtC[index]);

            Debug.Log("CostA: " + data.costTrtA[index]);
            Debug.Log("CostB: " + data.costTrtB[index]);
            Debug.Log("CostC: " + data.costTrtC[index]);

            form.AddField("TotalTreatCost", data.costDay[index]);
            
            form.AddField("SickCost", data.sickCost[index]);
            Debug.Log("SickCost: " + data.sickCost[index]);

            form.AddField("TotalWorkers", data.dailyIncome[index]);
            Debug.Log("DailyIncome: " + data.dailyIncome[index]);


            form.AddField("WinLose", data.winLose);

            WWW www = new WWW("https://stat2games.sites.grinnell.edu/php/sendepidemicgameinfo.php", form);
            yield return www;

            if (www.text == "0")
            {
                Debug.Log("Player data created successfully.");
            }
            else
            {
                Debug.Log("Player data creation failed. Error # " + www.text);
            }

        }

        //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    } 
}
