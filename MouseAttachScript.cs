using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseAttachScript : MonoBehaviour {

    // Canvas object
    Canvas canvas;

    // Text component
    Text text;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        // Get the canvas object
        canvas = transform.parent.GetComponent<Canvas>();

        // Get the text component
        text = GetComponentInChildren<Text>();
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

    void ChangeText(string str) {
        text.text = str;
    }

    public IEnumerator ChangePopupText(string str, int duration = 5) {
        ChangeText(str);

        yield return new WaitForSeconds(duration);

        if (text.text == str) {
            ChangeText("");
        }
    }
}
