using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class pregameSurvey : MonoBehaviour {

	private dataCollector data;
	public Dropdown genderDropdown;
	public InputField ageField;
	public Dropdown howOftenDropdown;
	public InputField whatGamesField;

	// Use this for initialization
	void Start () {
		data = GameObject.Find("DataCollector").GetComponent<dataCollector>();
	}

	public void loadGame() {
		// save gender, age, game experience, repeated plays to resultStr/database
		// pick a random puzzle to load - tile, statue, or ice
		updateGender();
		updateAge();
		updateGameExp();
		updateGamesPlayed();
		//SceneManager.LoadScene("puzzleHints");
		//SceneManager.LoadScene("ice_2");
		SceneManager.LoadScene("tile");

	}

	public void updateGender() {
		data.setGender(genderDropdown.value);
	}

	public void updateAge() {
		data.setAge(ageField.text);
	}

	public void updateGameExp() {
		data.setGameExperience(howOftenDropdown.value);
	}

	//TODO: sanitize!!!
	public void updateGamesPlayed() {
		data.setGamesPlayed(whatGamesField.text);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
