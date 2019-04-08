using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BridgeToggle : MonoBehaviour
{


    public Text text;
    public Button button;
    public GameObject[] bridgeObjects;
    public BridgeScript[] bridges;
    public bool hasOpen = true;

    void Start()
    {
        // Add listeners for onClick
        button.onClick.AddListener(delegate { OnClick(); });
        bridgeObjects = GameObject.FindGameObjectsWithTag("bridge");
        bridges = new BridgeScript[bridgeObjects.Length];
        int n = 0;
        foreach(GameObject brid in bridgeObjects)
        {
            BridgeScript bridge = brid.GetComponent(typeof(BridgeScript)) as BridgeScript;
            bridges[n] = bridge;
            n++;

        }
    }

	// Update is called once per frame
	void Update()
	{
        bool tempHasOpen = false;
        foreach(BridgeScript brid in bridges)
        {
            if (brid.open) 
            { 
                tempHasOpen = true; 
                //brid.text.gameObject.SetActive(false);
            }
            else
            {
                brid.text.gameObject.SetActive(true);
            }
        }
        hasOpen = tempHasOpen;
        if(!hasOpen)
        {
            GameObject.Find("CloseBridges").GetComponentInChildren<Text>().text = "Open Bridges";
        }
        else
        {
            GameObject.Find("CloseBridges").GetComponentInChildren<Text>().text = "Close Bridges";
        }

	}

    void OnClick()
    {
        foreach (BridgeScript brid in bridges)
        {
            if (brid.open && hasOpen)
            {
                brid.BroadcastMessage("CloseBridge");
            }
            else if(!hasOpen)
            {
                
                brid.BroadcastMessage("OpenBridge");
            }
        }
    }

}
