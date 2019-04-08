using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerID : MonoBehaviour {
    string pid;
    string gid;

	// Called when player clicks on either level of the game or when playing a custom game
	public void Begin()
    {

        pid = GameObject.Find("PlayID").GetComponent<Text>().text;
        gid = GameObject.Find("GroupID").GetComponent<Text>().text;
        GameObject.Find("PlayerData").GetComponent<PlayerData>().playerID = pid;
        GameObject.Find("PlayerData").GetComponent<PlayerData>().groupID = gid;

    }

} 
