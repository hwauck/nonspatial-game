using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keynumber : MonoBehaviour {
    private DataCollector dataCollector;
    Text keynum;

	// Use this for initialization
	void Start () {
        dataCollector = GameObject.Find("DataCollector").GetComponent<DataCollector>();
        keynum = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if(dataCollector.ReportKeyNum() != 0 ){
            keynum.text = "Number of key fragments obtained: " + dataCollector.ReportKeyNum();
        }
	}
}
