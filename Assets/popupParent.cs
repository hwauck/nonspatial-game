using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popupParent : MonoBehaviour {
    public GameObject levelNameParent;

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
