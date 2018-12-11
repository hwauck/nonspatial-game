using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class popupText : MonoBehaviour {
    public GameObject text1;
 
    void Start()
    {
        text1.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("entered");
        if (other.CompareTag("Player")){
            text1.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("out");
        if (other.CompareTag("Player"))
        {
            text1.SetActive(false);
        }
    }
}
