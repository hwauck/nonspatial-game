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
        if (other.CompareTag("Player"))
        {
            // display the level names only if this level is not yet completed
            if ((!DataCollector.finishedIce && other.CompareTag("Player") && this.gameObject.name == "text1") ||
            (!DataCollector.finishedIce2 && other.CompareTag("Player") && this.gameObject.name == "text2") ||
            (!DataCollector.finishedIce3 && other.CompareTag("Player") && this.gameObject.name == "text3") ||
            (!DataCollector.finishedIce4 && other.CompareTag("Player") && this.gameObject.name == "text4") ||
            (!DataCollector.finishedIce5 && other.CompareTag("Player") && this.gameObject.name == "text5") ||
            (!DataCollector.finishedTile && other.CompareTag("Player") && this.gameObject.name == "text tl1") ||
            (!DataCollector.finishedTile2 && other.CompareTag("Player") && this.gameObject.name == "text tl2") ||
            (!DataCollector.finishedTile3 && other.CompareTag("Player") && this.gameObject.name == "text tl3") ||
            (!DataCollector.finishedTileHard && other.CompareTag("Player") && this.gameObject.name == "text tl hard"))
            {
                text1.SetActive(true);
            }
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
