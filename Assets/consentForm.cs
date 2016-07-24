using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class consentForm : MonoBehaviour {

	public Toggle iAgree;
	public Text errorMustConsent;

	// Use this for initialization
	void Start () {
	
	}

	public void loadPregameSurvey() {
		if(iAgree.isOn) {
			SceneManager.LoadScene("pregame_survey");
		} else {
			errorMustConsent.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
