using UnityEngine;
using System.Collections;

public class pregameSurvey : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void loadGame() {
		// save gender, age, game experience, repeated plays to resultStr/database
		Application.LoadLevel(2);

	}

	// Update is called once per frame
	void Update () {
	
	}
}
