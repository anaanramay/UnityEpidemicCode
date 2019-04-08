using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextMouseOverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    // The description to display for this setting
    public string description;

    // Use this for initialization
    void Start() {
        description = string.Join("\n", description.Split('*'));
    }

    // Called when the player's mouse enters this setting
    public void OnPointerEnter(PointerEventData data) {
        GameObject.Find("Descriptions").SendMessage("OpenDesc", description);
    }

    // Called when the player's mouse leaves this setting
    public void OnPointerExit(PointerEventData data) {
        GameObject.Find("Descriptions").SendMessage("CloseDesc", description);
    }
}
