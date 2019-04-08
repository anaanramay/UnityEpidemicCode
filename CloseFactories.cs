using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CloseFactories : MonoBehaviour
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
        GameObject[] factories = GameObject.FindGameObjectsWithTag("factory");
        foreach(GameObject factory in factories)
        {
            factory.BroadcastMessage("CloseFactory");

        }

    }

}
