using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;


public class DataCollector : MonoBehaviour {
    private List<string> completedScenes;
    private static DataCollector instance;

    [DllImport("__Internal")]
    private static extern void sendToDB(string playerData);

    // overall variables
    private float sessionTime; // how long the player spent with the entire game
    private string levelOrder; // in which order the player attempted levels
    private int totalMoves; // total number of moves across the entire game
    private int numLevelsCompleted; // total number of levels finished

    // total number of attempts across all levels - counted every time a player 
    // 1) wins OR 2) runs out of time OR 3) leaves room without winning OR 4) clicks reset button
    // on timed levels, clicking reset also resets the timer
    private int totalAttempts;
    private int numResets;
    private int numLeftRoom;
    private int numRanOutOfTime;
    private Dictionary<string,bool> levelsCompleted; //ice,ice2,ice3,ice4,ice5,iceTimed, tile, tile2, tile3, tileHard

    //level-specific variables
    private List<Attempt> attempts;

    private int keysobtained = 0; // keep track of how many key fragments the player has won 

    private static bool pressedP = false;

    public FadeScreen screenFader;

    public Text demoFinishedText;

    public Text demoFinishedAltText;

    // public varaibles for hiding the level name texts after the player has completed the scene, default to false
    public static bool finishedIce, finishedIce2, finishedIce3, finishedIce4, finishedIce5, finishedIceTimed,
    finishedTile, finishedTile2, finishedTile3, finishedTileHard;

    private class Attempt
    {
        public string level; // the name of the current level
        public float attemptTime; //time spent on this attempt
        public int moves; // moves in this attempt
        public int pushes; // for ice block levels only - number of pushes of ice blocks
        public string outcome; // victory or left room or ran out of time or clicked reset

        public Attempt(string levelName)
        {
            level = levelName;
            attemptTime = 0;
            moves = 0;
            pushes = 0;
            outcome = "";
        }

        public string toString()
        {
            return "Level:" + level + ";Time:" + attemptTime + ";Moves:" + moves + ";Pushes:" + pushes + ";Outcome:" + outcome + "\n";
        }
    }

	protected void Awake () {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
            return;
        }

		//makes the GameObject with this script attached to it persist across game levels
		DontDestroyOnLoad(transform.gameObject);
        sessionTime = 0;
        levelOrder = "";
        totalMoves = 0;
        numLevelsCompleted = 0;

        totalAttempts = -1;
        numResets = 0;
        numLeftRoom = 0;
        numRanOutOfTime = 0;

        levelsCompleted = new Dictionary<string, bool>();
        levelsCompleted.Add("ice", false);
        levelsCompleted.Add("ice_2", false);
        levelsCompleted.Add("ice_3", false);
        levelsCompleted.Add("ice_4", false);
        levelsCompleted.Add("ice_5", false);
        levelsCompleted.Add("ice_timed", false);
        levelsCompleted.Add("tile", false);
        levelsCompleted.Add("tile2", false);
        levelsCompleted.Add("tile3", false);
        levelsCompleted.Add("tileHard", false);

        attempts = new List<Attempt>();

        completedScenes = new List<string>(); 
	}

	// Use this for initialization
	void Start () {
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    //finishedIce, finishedIce2, finishedIce3, finishedIce4, finishedIce5, finishedIceTimed,
    //finishedTile, finishedTile2, finishedTile3, finishedTileHard;

    // delegate function to detect when a new scene is loaded.
    // a new Attempt is created every time a new scene is loaded, even if it's the main room
    // this is because it's easier to partition everything into attempts
    // TODO: Don't use completedScenes and playerArrowIce.sceneName public variable
    // Instead, just use the levelsCompleted dictionary
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Finished Loading Scene " + scene.name);
        AddNewAttempt(scene.name);
        GameObject hideHint;
        // detect if the player has finished any games, if so, close the door 
        if(!completedScenes.Contains(playerArrowIce.sceneName))
            completedScenes.Add(playerArrowIce.sceneName);

        if (completedScenes.Contains("ice")){
            GameObject go = GameObject.Find("ToIce");
            if(go != null)
                go.SetActive(false);  

            hideHint = GameObject.Find("ice1_level_name");   // hide level name text if the player has completed this level
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedIce = true;
            }
        }
        if (completedScenes.Contains("ice_2")){
            GameObject go = GameObject.Find("ToIce2");
            if(go != null)
                go.SetActive(false);
            
            hideHint = GameObject.Find("ice2_level_name");   
            if (hideHint != null){
                hideHint.SetActive(false);
                finishedIce2 = true;
            }
        }
        if (completedScenes.Contains("ice_3")){
            GameObject go = GameObject.Find("ToIce3");
            if(go != null)
                go.SetActive(false);

            hideHint = GameObject.Find("ice3_level_name");   
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedIce3 = true;
            }
        }
        if (completedScenes.Contains("ice_4")){
            GameObject go = GameObject.Find("ToIce4");
            if(go != null)
                go.SetActive(false);

            hideHint = GameObject.Find("ice4_level_name");
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedIce4 = true;
            }
        }
        if (completedScenes.Contains("ice_5")){
            GameObject go = GameObject.Find("ToIce5");
            if(go != null)
                go.SetActive(false);

            hideHint = GameObject.Find("ice5_level_name");
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedIce5 = true;
            }
        }
        if (completedScenes.Contains("ice_timed")){
            GameObject go = GameObject.Find("ToTimedIce");
            if(go != null)
                go.SetActive(false);

            hideHint = GameObject.Find("ice_timed_level_name");
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedIceTimed = true;
            }
        }
        if (completedScenes.Contains("tile")){
            GameObject go = GameObject.Find("ToTile1");
            if(go != null)
                go.SetActive(false);

            hideHint = GameObject.Find("tile1_level_name");
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedTile = true;
            }
        }
        if (completedScenes.Contains("tile2")){
            GameObject go = GameObject.Find("ToTile2");
            if(go != null)
                go.SetActive(false);

            hideHint = GameObject.Find("tile2_level_name");
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedTile2 = true;
            }
        }
        if (completedScenes.Contains("tile3")){
            GameObject go = GameObject.Find("ToTile3");
            if(go != null)
                go.SetActive(false);

            hideHint = GameObject.Find("tile3_level_name");
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedTile3 = true;
            }
        }
        if (completedScenes.Contains("tileHard")){
            GameObject go = GameObject.Find("ToTileHard");
            if(go != null)
                go.SetActive(false);

            hideHint = GameObject.Find("tile_hard_level_name");
            if (hideHint != null)
            {
                hideHint.SetActive(false);
                finishedTileHard = true;
            }
        }
    }

    // needs to be called every time player resets or runs of out of time. Is automatically called each time a new level is entered
    // by OnLevelFinishedLoading
    public void AddNewAttempt(string levelName)
    {
        Attempt newAttempt = new Attempt(levelName);
        attempts.Add(newAttempt);
        totalAttempts++;
    }

    private Attempt getCurrentAttempt()
    {
        return attempts[totalAttempts];
    }

    //public methods for incrementing/changing attempt-level data, to be called from other scripts

    public void AddMove()
    {
        getCurrentAttempt().moves++;
    }

    public void AddPush()
    {
        getCurrentAttempt().pushes++;
    }

    public void setOutcome(string outcome)
    {
        getCurrentAttempt().outcome = outcome;
        if(outcome.Equals("victory"))
        {
            levelsCompleted[getCurrentAttempt().level] = true;
        }
    }


    // Update is called once per frame
    void Update () {
        getCurrentAttempt().attemptTime += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.P)){
            Debug.Log("End Gameplay Session");
            pressedP = true;

            doFadeToDemoFinished(3f, true);
        }

    }

    public void doFadeToDemoFinished(float seconds, bool playerInitiated)
    {
        /*
        if(playerInitiated)
        {
            expDataManager.setOutcome("quit");
        } else
        {
            expDataManager.setOutcome("finishedDemo");
        }
        
        gameQuit.Invoke(); // sends out broadcast that game is over; any other scripts can perform actions based on this
        disablePlayerControl();
        StopAllCoroutines();
        timer.stopTimer();
        timer.stopMusic();
        ConversationController.Disable();
        errorPanel.alpha = 0;
        countdownPanel.alpha = 0;
        expDataManager.setPauseGameplay(true);
        */
        StartCoroutine(fadeToDemoFinished(seconds, playerInitiated));
    }

    private IEnumerator fadeToDemoFinished(float seconds, bool playerInitiated)
    {
        screenFader.fadeOut(seconds);
        yield return new WaitForSeconds(seconds);

        string allData;
        if (playerInitiated)
        {
            demoFinishedAltText.enabled = true;
            yield return new WaitForSeconds(3f);

            allData = saveAllData();
            // This is where we send data to the server - use Javascript library added to project
            sendToDB(allData);

        } else
        {
            demoFinishedText.enabled = true;
            yield return new WaitForSeconds(3f);

            allData = saveAllData();
            // This is where we send data to the server - use Javascript library added to project
            sendToDB(allData);
        }
    }

    public void AddKey(){
        keysobtained += 1;
    }

    public int ReportKeyNum(){
        return keysobtained;
    }

    public Dictionary<string, bool> ListCompletedLevels(){
        return levelsCompleted;
    }

	public string saveAllData() {
        // setting these to zero again so this can recalculate and save totals multiple times during a game
        sessionTime = 0;
        levelOrder = "";
        totalMoves = 0;
        numLevelsCompleted = 0;
        numResets = 0;
        numLeftRoom = 0;
        numRanOutOfTime = 0;

        // calculate totals across levels/attempts
        for(int i = 0; i < attempts.Count; i++)
        {
            sessionTime += attempts[i].attemptTime;
            levelOrder += attempts[i].level + ":";
            totalMoves += attempts[i].moves;
            if(attempts[i].outcome.Equals("victory"))
            {
                numLevelsCompleted++;
            } else if(attempts[i].outcome.Equals("reset"))
            {
                numResets++;
            } else if(attempts[i].outcome.Equals("left"))
            {
                numLeftRoom++;
            } else if(attempts[i].outcome.Equals("time"))
            {
                numRanOutOfTime++;
            } else
            {
                Debug.Log("ERROR: invalid outcome code in attempt " + i + " in level " + attempts[i].level + ": " + attempts[i].outcome);
            }

        }
        levelOrder = levelOrder.Substring(0, levelOrder.Length - 1); //get rid of extra colon at end

        //header
        string allData = "totalTime,levelOrder,totalMoves,levelsCompleted,totalAttempts,numResets,numLeftRoom,numRanOutOfTime,";
        allData = allData + "iceCompleted,ice2Completed,ice3Completed,ice4Completed,ice5Completed,iceTimedCompleted,tileCompleted,";
        allData = allData + "tile2Completed,tile3Completed,tileHardCompleted\n";

        allData += sessionTime + "," + levelOrder + "," + totalMoves + "," + numLevelsCompleted + "," + totalAttempts + "," + numResets + ",";
        allData += numLeftRoom + "," + numRanOutOfTime + ",";

        allData += levelsCompleted["ice"] + ",";
        allData += levelsCompleted["ice_2"] + ",";
        allData += levelsCompleted["ice_3"] + ",";
        allData += levelsCompleted["ice_4"] + ",";
        allData += levelsCompleted["ice_5"] + ",";
        allData += levelsCompleted["ice_timed"] + ",";
        allData += levelsCompleted["tile"] + ",";
        allData += levelsCompleted["tile2"] + ",";
        allData += levelsCompleted["tile3"] + ",";
        allData += levelsCompleted["tileHard"];

        allData += "\n";

        //attempts data
        for (int i = 0; i < attempts.Count; i++)
        {
            allData += attempts[i].toString();
        }

		Debug.Log(allData);
        return allData;

	}


}
