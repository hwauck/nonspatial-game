using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popupParent : MonoBehaviour {
    public GameObject levelNameParent;
    private DataCollector dataCollector;

	// Use this for initialization
	void Start () {
        levelNameParent.SetActive(true);
        dataCollector = GameObject.Find("DataCollector").GetComponent<DataCollector>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            levelNameParent.SetActive(false);
            Debug.Log("set level texts to false !!!!!!!!!!!!!!!!!");
        }
	}
}
