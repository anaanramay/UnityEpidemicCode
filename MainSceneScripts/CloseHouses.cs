using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CloseHouses : MonoBehaviour
{


    public Text text;
    public Button button;

    void Start()
    {
        // Add listeners for onClick
        button.onClick.AddListener(delegate { OnClick(); });
    }

	// Update is called once per frame
	void Update()
	{
			
	}

    void OnClick()
    {
        GameObject[] bridges = GameObject.FindGameObjectsWithTag("house");
        foreach(GameObject brid in bridges)
        {
            brid.BroadcastMessage("CloseHouse");

        }

    }

}
