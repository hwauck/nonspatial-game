using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keynumber : MonoBehaviour {
    private DataCollector dataCollector;
    private int num;
    Text keynum;
    public GameObject popmessage;
    private float mytimer;
    private bool haspoped;
	// Use this for initialization
	void Start () {
        dataCollector = GameObject.Find("DataCollector").GetComponent<DataCollector>();
        keynum = GetComponent<Text>();
        popmessage.SetActive(false);
        mytimer = 0f;
        haspoped = false;
	}
	
	// Update is called once per frame
	void Update () {
        num = dataCollector.ReportKeyNum(); 
        if(num != 0 ){
            if(num == 2){    // change "9" in Keynumber.cs and popuptText2.cs
                if(haspoped == false){
                    popmessage.SetActive(true);
                    mytimer += Time.deltaTime;
                }
                keynum.text = "Keys: 1"; 
                if(mytimer > 4.0f){    // wait three second then make the message disappear
                    popmessage.SetActive(false);
                    mytimer = 0f;
                    haspoped = true;
                }

            }else{
                keynum.text = "Key fragments: " + num;

            }
        }
	}
}
