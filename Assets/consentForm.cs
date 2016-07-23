using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class consentForm : MonoBehaviour {

	public Toggle iAgree;
	public Text errorMustConsent;

	// Use this for initialization
	void Start () {
	
	}

	public void loadPregameSurvey() {
		if(iAgree.isOn) {
			Application.LoadLevel(1);
		} else {
			errorMustConsent.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
