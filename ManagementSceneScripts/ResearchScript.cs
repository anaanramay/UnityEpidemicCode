using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchScript : MonoBehaviour {

    // The add money buttons
    public Button addTen;
    public Button addHundred;
    public Button addThousand;
    public Button addTenThou;
    public Button reduceSpread;

    // The research and budget text
    Text budgetText;
    Text researchText;

    public int price = 100000;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {

        // Get the text components
        budgetText = transform.Find("BudgetLabel").Find("Budget").GetComponent<Text>();
        researchText = transform.Find("MoneyLabel").Find("Money").GetComponent<Text>();

        // Add listeners for the add buttons onClicks
        addTen.onClick.AddListener(     delegate { OnClick(10);    });
        addHundred.onClick.AddListener( delegate { OnClick(100);   });
        addThousand.onClick.AddListener(delegate { OnClick(1000);  });
        addTenThou.onClick.AddListener(delegate { OnClick(10000); });
        reduceSpread.onClick.AddListener(delegate { OnClick(); });

}
	
	// Update is called once per frame
	void Update () {
        budgetText.text = GameControllerScript.budget.ToString("c0");
        researchText.text = GameControllerScript.researchBudget.ToString("c0");
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks on one of the add to research buttons
    void OnClick(int amount) {
        if (!GameObject.Find("CenterWindow").transform.Find("NewTreatment").gameObject.activeInHierarchy) {
            GameControllerScript.AddToResearch(amount);
        }
    }

    void OnClick() 
    {
        if(GameControllerScript.budget >= price )
        {
            GameControllerScript.budget = GameControllerScript.budget - price;
            GameControllerScript.infectedMultiplier = GameControllerScript.infectedMultiplier * .75f;
        }
    }
}
