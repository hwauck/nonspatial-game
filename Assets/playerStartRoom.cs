using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStartRoom : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// right = new Vector3(0,0,270);
		// left = new Vector3(0,0,90);
		// up = new Vector3(0,0,0);
		// down = new Vector3(0,0,180);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if(this.transform.position.y>-6) transform.position = new Vector3(transform.position.x, transform.position.y-2, transform.position.z);
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if(this.transform.position.y<4) transform.position = new Vector3(transform.position.x, transform.position.y+2, transform.position.z);
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if(this.transform.position.x<9.5) transform.position = new Vector3(transform.position.x+2, transform.position.y, transform.position.z);
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if(this.transform.position.x>-4.5) transform.position = new Vector3(transform.position.x-2, transform.position.y, transform.position.z);
		}


	}
}
