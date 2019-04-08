using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionsScript : MonoBehaviour {
    
    // Canvas object
    Canvas canvas;

    // Text component
    public Text text;

    // Background panel
    public RectTransform background;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        // Get the canvas object
        canvas = transform.parent.GetComponent<Canvas>();
	}
	
	// Update is called once per frame
	void Update () {
        // Align the mouse atttach object to the mouse
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out pos
        );

        transform.position = canvas.transform.TransformPoint(pos);
	}

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Opens the given description
    void OpenDesc(string desc) {
        text.text = desc;
        background.GetComponent<Image>().color = new Color(30f/255f, 30f/255f, 30f/255f, 200f/255f);
    }

    // Closes the given description if it is still open
    void CloseDesc(string desc) {
        if (text.text == desc) {
            text.text = "";
            background.GetComponent<Image>().color = new Color(30f/255f, 30f/255f, 30f/255f, 0f);
        }
    }
}
