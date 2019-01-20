using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keynumber : MonoBehaviour {
    private DataCollector dataCollector;
    private int num;
    Text keynum;

	// Use this for initialization
	void Start () {
        dataCollector = GameObject.Find("DataCollector").GetComponent<DataCollector>();
        keynum = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        num = dataCollector.ReportKeyNum(); 
        if(num != 0 ){
            if(num == 9){    // change "9" in Keynumber.cs and popuptText2.cs
                keynum.text = "Number of Keys obtained: 1"; 

            }else{
                keynum.text = "Number of key fragments obtained: " + num;

            }
        }
	}
}
