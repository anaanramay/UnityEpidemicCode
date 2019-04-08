using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinTextShadow : MonoBehaviour {

    // The WinText object
    public Text winText;
	
	// Update is called once per frame
	void Update () {
        GetComponent<Text>().text = winText.text;
	}
}
