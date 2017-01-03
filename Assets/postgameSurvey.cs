using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class postgameSurvey : MonoBehaviour {

	private dataCollector data;
	public Dropdown playedBefore;
	public Slider howFun;
	public Slider howBoring;
	public Slider howEasy;
	public Slider howFrustrating;

	// Use this for initialization
	void Start () {
		data = GameObject.Find("DataCollector").GetComponent<dataCollector>();
	
	}

	public void loadHintsSurvey() {
		// save how fun/easy/boring/frustrating answers to resultStr/database
		updatePlayedBefore();
		updateHowFun();
		updateHowBoring();
		updateHowEasy();
		updateHowFrustrating();
		SceneManager.LoadScene("postgame_survey_hints");

	}

	public void updatePlayedBefore() {
		data.setPlayedBefore(playedBefore.value);
	}

	public void updateHowFun() {
		data.setHowFun(Convert.ToInt32(howFun.value));
	}

	public void updateHowBoring() {
		data.setHowBoring(Convert.ToInt32(howBoring.value));
	}

	public void updateHowEasy() {
		data.setHowEasy(Convert.ToInt32(howEasy.value));
	}

	public void updateHowFrustrating() {
		data.setHowFrustrating(Convert.ToInt32(howFrustrating.value));
	}

	// Update is called once per frame
	void Update () {
	
	}
}
