using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	// Use this for initialization

	public float waitTime;

	public InputField timerLabel;

	public GameObject TimeOut;

	public GameObject player;

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
				timerLabel.text = "00:00";
				TimeOut.SetActive(true);
				GameObject timeOutLabel = TimeOut.transform.Find("TimeOutLabel").gameObject;
				timeOutLabel.GetComponent<InputField>().text = "You Are Out Of Time";

				player.GetComponent<playerArrowIce>().SetTimedOut(true);
			}else{
				timerLabel.text = "" + minutes.ToString("00") + ":" + seconds.ToString("00");
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
		player.GetComponent<playerArrowIce>().SetTimedOut(false);
		player.GetComponent<playerArrowIce>().newGame();
		TimeOut.SetActive(false);
	}
}
