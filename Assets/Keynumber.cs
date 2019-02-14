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
    private Image keyCounterPanel;
    private Image messagePanel;
	// Use this for initialization
	void Start () {
        dataCollector = GameObject.Find("DataCollector").GetComponent<DataCollector>();
        keynum = GetComponent<Text>();
        popmessage.SetActive(false);
        mytimer = 0f;
        haspoped = false;

        keyCounterPanel = GameObject.Find("keycounterBox").GetComponent<Image>();
        messagePanel = GameObject.Find("messageBox").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        num = dataCollector.ReportKeyNum(); 
        if(num != 0 ){
            keyCounterPanel.enabled = true;
            if(num == 9){    // change "9" in Keynumber.cs and popuptText2.cs
                if(haspoped == false){
                    messagePanel.enabled = true;
                    popmessage.SetActive(true);
                    mytimer += Time.deltaTime;
                }
                keynum.text = "Keys: 1"; 
                if(mytimer > 4.0f){    // wait four second then make the message disappear
                    popmessage.SetActive(false);
                    messagePanel.enabled = false;
                    mytimer = 0f;
                    haspoped = true;
                }

            }else{
                keynum.text = "Key fragments: " + num;

            }
        }
	}
}

