using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClearHouses : MonoBehaviour
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
        GameObject[] houses = GameObject.FindGameObjectsWithTag("house");
        foreach (GameObject hous in houses)
        {
            hous.BroadcastMessage("OnClear");

        }

    }

}
