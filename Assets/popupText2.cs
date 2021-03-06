﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class popupText2 : MonoBehaviour {
    public GameObject text1;
    public GameObject text2;
    private DataCollector dataCollector;
    public static bool isUnlocked = false;
    public bool debugMode; // can make the timed ice level unlocked to start with for testing purposes

    void Start()
    {
        text1.SetActive(false);
        text2.SetActive(false);
        dataCollector = GameObject.Find("DataCollector").GetComponent<DataCollector>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("entered");
        if (other.CompareTag("Player") && !DataCollector.finishedIceTimed)
        {
            if(dataCollector.ReportKeyNum() != 9 && !debugMode){
                text1.SetActive(true);
            }
            else{
                isUnlocked = true;   // unlock IceTimed when the player gain all 9 other key fragments
                text2.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("out");
        if (other.CompareTag("Player"))
        {
            text1.SetActive(false);
            text2.SetActive(false);
        }
    }}
