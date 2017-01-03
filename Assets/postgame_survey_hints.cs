using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class postgame_survey_hints : MonoBehaviour {

	private dataCollector data;
	public Dropdown sawHints;
	public InputField why;
	public Slider howHelpful;

	// Use this for initialization
	void Start () {
		data = GameObject.Find("DataCollector").GetComponent<dataCollector>();

	}

	public void loadThankYouSurvey() {
		// save how fun/easy/boring/frustrating answers to resultStr/database
		updateSawHints();
		updateHowHelpful();
		updateWhy();
		//Debug.Log("Before data.save()");
		data.save();
		//Debug.Log("After data.save()");
		SceneManager.LoadScene("thankyou_screen");

	}

	public void updateSawHints() {
		data.setSawHints(sawHints.value);
	}

	public void updateHowHelpful() {
		data.setHowHelpful(Convert.ToInt32(howHelpful.value));
	}

	public void updateWhy() {
		data.setWhy(why.text);
	}
		
	// Update is called once per frame
	void Update () {

	}
}
