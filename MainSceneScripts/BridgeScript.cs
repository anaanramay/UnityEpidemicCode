using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BridgeScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    // Various components
    public Text text;
    public Button button;
    public Toggle toggle;

    // Whether or not this bridge is open
    public bool open = true;

    // The intersections at either end of this bridge
    public PathfindingScript end1;
    public PathfindingScript end2;

    // The game container
    public GameObject game;

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Start |
    // +-------+

	// Use this for initialization
	void Start () {
        // Add listeners for onClick and onValueChanged
        button.onClick.AddListener(delegate { OnClick(); });
        toggle.onValueChanged.AddListener(delegate { OnValueChanged(toggle); });
	}

    // +--------+-------------------------------------------------------------------------------------------------------------------------------------------------
    // | Events |
    // +--------+

    // Called when the player clicks on the factory
    void OnClick() {
        toggle.gameObject.SetActive(!toggle.gameObject.activeInHierarchy);
    }

    // Called when the player switches the toggle
    void OnValueChanged(Toggle change) {
        toggle.gameObject.SetActive(false);
        open = change.isOn;

        if (open) {
            text.text = "Open";
            OpenBridge();
        } else {
            text.text = "Closed";
            CloseBridge();
        }
    }

    // Called when the mouse enters this building
    public void OnPointerEnter(PointerEventData eventData) {
        text.gameObject.SetActive(true);
    }

    // Called when the mouse exits this building
    public void OnPointerExit(PointerEventData eventData) {
        text.gameObject.SetActive(false);
    }

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Opens the bridge
    void OpenBridge() {
        // Reconnect end1 and end2
        text.text = "Open";
        toggle.GetComponent<Toggle>().isOn = true;
        open = true;
        if (!end1.adjacent.Contains(end2.gameObject)) {
            end1.adjacent.Add(end2.gameObject);
            end1.adjDist = null;
        }

        if (!end2.adjacent.Contains(end1.gameObject)) {
            end2.adjacent.Add(end1.gameObject);
            end2.adjDist = null;
        }

        game.BroadcastMessage("RedrawPaths");
    }

    // Closes the bridge
    void CloseBridge() {
        text.text = "Closed";
        toggle.GetComponent<Toggle>().isOn = false;
        open = false;
        // Disconnect end1 and end2
        if (end1.adjacent.Contains(end2.gameObject)) {
            end1.adjacent.Remove(end2.gameObject);
            end1.adjDist = null;
        }

        if (end2.adjacent.Contains(end1.gameObject)) {
            end2.adjacent.Remove(end1.gameObject);
            end2.adjDist = null;
        }

        game.BroadcastMessage("RedrawPaths");
    }

    // Calls OnClick if the meu for this building is open
    void CloseMenus() {
        if (toggle.gameObject.activeInHierarchy) {
            OnClick();
        }
    }
}
