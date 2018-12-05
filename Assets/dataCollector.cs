using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dataCollector : MonoBehaviour {

    // overall variables
    public float sessionTime; // how long the player spent with the entire game
    public string levelOrder; // in which order the player attempted levels
    public int moves; // total number of moves across the entire game
    public int levelsCompleted; // total number of levels finished
    public bool[] icesCompleted; //ice,ice2,ice3,ice4,ice5,iceTimed
    public bool[] tilesCompleted; //tile, tile2, tile3, tileHard

    //level-specific variables

	void Awake () {
		//makes the GameObject with this script attached to it persist across game levels
		DontDestroyOnLoad(transform.gameObject);
        sessionTime = 0;
        levelOrder = "";
        moves = 0;
        levelsCompleted = 0;
        icesCompleted = new bool[6];
        tilesCompleted = new bool[4];
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void saveAllData() {

        //header
		string allData = "totalTime,levelOrder,totalMoves,levelsCompleted,iceCompleted,ice2Completed,ice3Completed,ice4Completed,ice5Completed,iceTimedCompleted,";
        allData = allData + "tileCompleted,tile2Completed,tile3Completed,tileHardCompleted\n";

        allData += sessionTime + "," + levelOrder + "," + moves + "," + levelsCompleted + ",";
        for(int i = 0; i < icesCompleted.Length; i++)
        {
            allData = allData + icesCompleted[i] + ",";
        }
        for(int i = 0; i < tilesCompleted.Length; i++)
        {
            allData = allData + tilesCompleted[i] + ",";
        }
	

		//Debug.Log(allData);
        // This is where we send data to the server - use Javascript library added to project

	}


}
