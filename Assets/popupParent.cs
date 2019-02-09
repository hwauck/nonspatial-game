using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popupParent : MonoBehaviour {
    /* this script is for hiding all the level names and key fragments counter text when the player presses P */
    public GameObject levelNameParent; /* parent of all level names and key fragments counter */

	// Use this for initialization
	void Start () {
        levelNameParent.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            levelNameParent.SetActive(false);
        }
	}
}
