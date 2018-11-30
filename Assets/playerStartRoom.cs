using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		//This part is for moving
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if(this.transform.position.y==-6){
				if(this.transform.position.x==-2.5||this.transform.position.x==1.5||this.transform.position.x==5.5) transform.position = new Vector3(transform.position.x, transform.position.y-2, transform.position.z);
			}
			else if(this.transform.position.y>-6&&this.transform.position.x!=-6.5&&this.transform.position.x!=11.5) transform.position = new Vector3(transform.position.x, transform.position.y-2, transform.position.z);
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if(this.transform.position.y==4){
				if(this.transform.position.x==-0.5||this.transform.position.x==5.5) transform.position = new Vector3(transform.position.x, transform.position.y+2, transform.position.z);
			}
			else if(this.transform.position.y<4&&this.transform.position.x!=-6.5&&this.transform.position.x!=11.5) transform.position = new Vector3(transform.position.x, transform.position.y+2, transform.position.z);
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if(this.transform.position.x==9.5){
				if(this.transform.position.y==2||this.transform.position.y==-4) transform.position = new Vector3(transform.position.x+2, transform.position.y, transform.position.z);
			}
			else if(this.transform.position.x<9.5&&this.transform.position.y!=-8&&this.transform.position.y!=6) transform.position = new Vector3(transform.position.x+2, transform.position.y, transform.position.z);
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if(this.transform.position.x==-4.5){
				if(this.transform.position.y==2||this.transform.position.y==-4) transform.position = new Vector3(transform.position.x-2, transform.position.y, transform.position.z);
			}
			else if(this.transform.position.x>-4.5&&this.transform.position.y!=-8&&this.transform.position.y!=6) transform.position = new Vector3(transform.position.x-2, transform.position.y, transform.position.z);
		}
		//This part is for going to other scenes
		if(this.transform.position.x==-2.5&&this.transform.position.y==-8)  SceneManager.LoadScene("ice");
		else if(this.transform.position.x==1.5&&this.transform.position.y==-8) SceneManager.LoadScene("ice_2");
		else if(this.transform.position.x==5.5&&this.transform.position.y==-8) SceneManager.LoadScene("ice_3");
		else if(this.transform.position.x==11.5&&this.transform.position.y==-4) SceneManager.LoadScene("ice_4");
		else if(this.transform.position.x==11.5&&this.transform.position.y==2) SceneManager.LoadScene("ice_5");
		// else if()
		// else if()
		// else if()
		// else if()

	}
}
