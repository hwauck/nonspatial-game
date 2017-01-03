using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dataCollector : MonoBehaviour {

	// pre-game survey
	private string gender; // M, F, or O
	private string age; // up to 99
	private int gameExperience; // 1 to 6
	private string gamesPlayed; // free response up to 300 characters

	//post-game hints survey
	private string sawHints;
	private int howHelpful;
	private string why;

	// post-game survey
	private string playedBefore; // Yes, No, or blank
	private int howFun;
	private int howBoring;
	private int howEasy;
	private int howFrustrating;

	//all in-game player data, retrieved from playerArrow
	private string playerData;

	void Awake () {
		//makes the GameObject with this script attached to it persist across game levels
		DontDestroyOnLoad(transform.gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setPlayerData(string playerData) {
		this.playerData = playerData;
	}

	// attach this to "Submit" button on post-game survey"
	// But what if player closes window between scenes?
	public void save() {
		StartCoroutine(saveAllData());
	}

	IEnumerator saveAllData() {
		//TODO
		// What if the player leaves these blank? Do they have a "default" value?
		//SANITIZE INPUT
		string allData = "";
		allData += "GENDER," + gender + ",";
		allData += "AGE," + age + ",";
		allData += "GAME_EXP," + gameExperience + ",";

		// the ' character is used to indicate beginning and end of player data section
		allData += "GAMES_PLAYED,'" + gamesPlayed + "',";
		string gameType;
		if(playerData.Contains("statue")) {
			gameType = "statue";
		} else if (playerData.Contains("ice")) {
			gameType = "ice";
		} else if (playerData.Contains("tile")) {
			gameType = "tile";
		} else {
			gameType = "???";
		}
		allData += "GAME_TYPE," + gameType + ",";
		//Debug.Log("Copy of playerData that dataCollector has: " + playerData);
		int sessionDataPos = playerData.LastIndexOf("END_SESSION");
		string sessionData = playerData.Substring(sessionDataPos);
		playerData = playerData.Remove(sessionDataPos);

		allData += "'" + playerData + "',";

		allData += sessionData + ",";
		allData += "PLAYED_THIS_GAME_BEFORE," + playedBefore + ",";
		allData += "HOW_FUN," + howFun + ",";
		allData += "HOW_BORING," + howBoring + ",";
		allData += "HOW_EASY," + howEasy + ",";
		allData += "HOW_FRUSTRATING," + howFrustrating + ",";

		allData += "SAW_HINTS," + sawHints + ",";
		allData += "HOW_HELPFUL," + howHelpful + ",";
		allData += "HINTS_WHY,'" + why + "'";

		//Debug.Log(allData);
		string sendurl = "http://puzzleweb.web.engr.illinois.edu/SaveData.php?savedata=\"";
		sendurl += allData + "\"";
		WWW www = new WWW(sendurl);

		// Wait for download to complete
		yield return www;
		Debug.Log (sendurl);

	}

	public void setGender(int gender) {
		if (gender == 1) {
			this.gender = "F";
		} else if (gender == 2) {
			this.gender = "M";
		} else if (gender == 3) {
			this.gender = "O";
		} else {
			this.gender = "";
		}
	}

	public void setAge(string age) {
		this.age = age;
	}

	public void setGameExperience(int gameExperience) {
		this.gameExperience = gameExperience;
	}

	public void setGamesPlayed(string gamesPlayed) {
		this.gamesPlayed = gamesPlayed;
	}

	public void setPlayedBefore(int playedBefore) {
		if (playedBefore == 1){
			this.playedBefore = "Y";
		} else if (playedBefore == 2) {
			this.playedBefore = "N";
		} else {
			this.playedBefore = "";
		}
	}

	public void setSawHints(int sawHints) {
		if (sawHints == 1){
			this.sawHints = "Y";
		} else if (sawHints == 2) {
			this.sawHints = "N";
		} else {
			this.sawHints = "";
		}
	}

	public void setHowFun(int howFun) {
		this.howFun = howFun;
	}

	public void setHowBoring(int howBoring) {
		this.howBoring = howBoring;
	}

	public void setHowEasy(int howEasy) {
		this.howEasy = howEasy;
	}

	public void setHowFrustrating(int howFrustrating) {
		this.howFrustrating = howFrustrating;
	}

	public void setHowHelpful(int howHelpful) {
		this.howHelpful = howHelpful;
	}

	public void setWhy(string why) {
		this.why = why;
	}
}
