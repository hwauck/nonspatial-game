using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class postgameSurvey : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void loadThankYouSurvey() {
		// save how fun/easy/boring/frustrating answers to resultStr/database
		SceneManager.LoadScene("thankyou_screen");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
