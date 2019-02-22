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
            if ((!DataCollector.finishedIce && this.gameObject.name == "text_Ice") ||
            (!DataCollector.finishedIce2 && this.gameObject.name == "text_Ice2") ||
            (!DataCollector.finishedIce3 && this.gameObject.name == "text_Ice3") ||
            (!DataCollector.finishedIce4 && this.gameObject.name == "text_Ice4") ||
            (!DataCollector.finishedIce5 && this.gameObject.name == "text_Ice5") ||
            (!DataCollector.finishedIceTimed && this.gameObject.name == "text_IceTimed") ||
            (!DataCollector.finishedTile && this.gameObject.name == "text_Tile") ||
            (!DataCollector.finishedTile2 && this.gameObject.name == "text_Tile2") ||
            (!DataCollector.finishedTile3 && this.gameObject.name == "text_Tile3") ||
            (!DataCollector.finishedTileHard && this.gameObject.name == "text_TileHard"))
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
