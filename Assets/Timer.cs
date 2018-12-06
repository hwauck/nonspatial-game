using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	// Use this for initialization

	public float waitTime;

	public Text timeRemaining;

	public GameObject TimeOut;

	public playerArrowIce playerArrow;

	private float timer;

	private bool victory = false;

    void Start () {
		timer = waitTime;
 
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		float minutes = Mathf.Floor(timer / 60);
		float seconds = timer % 60;

		if(!victory){
			if(minutes <= 0 && seconds <= 0){
				timeRemaining.text = "Time Left:\n\n00:00";
				TimeOut.SetActive(true);

				playerArrow.SetTimedOut(true);
			}else{
				timeRemaining.text = "Time Left:\n\n" + minutes.ToString("00") + ":" + seconds.ToString("00");
			}
		}
	}

	public void SetVictory () {
		victory = true;
	}

	public void ResetTimer () {
		timer = waitTime;
		victory = false;
	}

	public void Restart () {
		playerArrow.SetTimedOut(false);
		playerArrow.newGame();
		TimeOut.SetActive(false);
	}
}
