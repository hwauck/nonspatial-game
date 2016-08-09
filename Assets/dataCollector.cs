using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dataCollector : MonoBehaviour {

	// pre-game survey
	private string gender; // M, F, or O
	private int age; // up to 99
	private int gameExperience; // 1 to 6
	private string gamesPlayed; // free response up to 300 characters
	private int numRepeatedPlays; // 0 to (4 or more)

	// post-game survey
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
		string allData = "NEW_GAME__";
		allData += "Gender:" + gender + "__";
		allData += "Age:" + age + "__";
		allData += "gameExp:" + gameExperience.ToString() + "__";
		allData += "gamesPlayed:" + gamesPlayed + "__";
		allData += "numRepeatedPlays:" + numRepeatedPlays.ToString() + "__";

		allData += playerData;

		allData += "howFun:" + howFun.ToString() + "__";
		allData += "howBoring:" + howBoring.ToString() + "__";
		allData += "howEasy:" + howEasy.ToString() + "__";
		allData += "howFrustrating:" + howFrustrating.ToString() + "___";

		Debug.Log(allData);
		string sendurl = "http://spatialcs.web.engr.illinois.edu/SaveData.php?savedata=\"";
		sendurl += allData + "\"";
		WWW www = new WWW(sendurl);

		// Wait for download to complete
		yield return www;
		Debug.Log (sendurl);

	}

	private void setGender(string gender) {
		this.gender = gender;
	}

	private void setAge(int age) {
		this.age = age;
	}

	private void setGameExperience(int gameExperience) {
		this.gameExperience = gameExperience;
	}

	private void setGamesPlayed(string gamesPlayed) {
		this.gamesPlayed = gamesPlayed;
	}

	private void setNumRepeatedPlays(int numRepeatedPlays) {
		this.numRepeatedPlays = numRepeatedPlays;
	}

	private void setHowFun(int howFun) {
		this.howFun = howFun;
	}

	private void setHowBoring(int howBoring) {
		this.howBoring = howBoring;
	}

	private void setHowEasy(int howEasy) {
		this.howEasy = howEasy;
	}

	private void setHowFrustrating(int howFrustrating) {
		this.howFrustrating = howFrustrating;
	}
}
