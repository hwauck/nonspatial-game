using UnityEngine;
using System.Collections;

public class iceBlock : MonoBehaviour {

	public playerController player;
	public iceBlock[] otherBlocks;
	private Vector3 direction;
	private Vector3 right;
	private Vector3 left;
	private Vector3 up;
	private Vector3 down;
	public Vector2 square;
	private Vector2 predictedSquare;
	public Vector3 startingPos;
	public Vector2 startingSquare;
	private bool stoppedByIce;
	private bool stoppedByOffscreen;

	// Use this for initialization
	void Start () {
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		stoppedByIce = false;
		stoppedByOffscreen = false;
	}

	private string coordinatesToSquare(Vector2 coordinates) {
		return coordinates.x.ToString () + coordinates.y.ToString ();
	}


	public bool canMove() {
		//TODO
		return !offScreen () && !blockedByIce();
		return false;
	}

	private bool offScreen() {
		if(GameObject.Find ("Block" + predictedSquare.x + "" + predictedSquare.y) == null) {
			return true;
		}
		return false;
	}

	private bool blockedByIce() {
		if(predictedSquare == otherBlocks[0].square || predictedSquare == otherBlocks[1].square) {
			return true;
		} else {
			return false;
		}
	}

	//checked right after player moves
	private bool isStoppedByOffscreen() {
		return stoppedByOffscreen;
	}

	private bool isStoppedByIce() {
		return stoppedByIce;
	}

	// index 0 is 1 if blocked by offScreen
	// index 1 is 1 if blocked by another ice block
	// index 2 is 1 if slides and then stops because of offscreen
	// index 3 is 1 if slides and then stops because of another ice block
	// called right after player moves
	public bool[] getErrorType() {
		bool[] errors = new bool[4];
		for(int i = 0; i < 4; i++) {
			errors[i] = false;
		}
		if(offScreen ()) {
			errors[0] = true;
		} else if (blockedByIce()) {
			errors[1] = true;
		} else if (isStoppedByOffscreen()) {
			errors[2] = true;
		} else if (isStoppedByIce()) {
			errors[3] = true;
		}
		return errors;
	}

	public void getPushed() {
		if(!offScreen() && !blockedByIce()) {
			//no blockages - ice block can be pushed
			move();
		} 
	}

	public string moveOneSquare() {
		direction = player.getDirection();
		if(approximately(direction, up)) {
			predictedSquare.x = square.x;
			predictedSquare.y = square.y + 1;
		} else if (approximately(direction, down)) {
			predictedSquare.x = square.x;
			predictedSquare.y = square.y - 1;
		} else if (approximately(direction, left)) {
			predictedSquare.y = square.y;
			predictedSquare.x = square.x - 1;
		} else if (approximately(direction, right)) {
			predictedSquare.y = square.y;
			predictedSquare.x = square.x + 1;
		} 
		transform.Translate(direction * 2f, Space.World);
		string newLoc = coordinatesToSquare(predictedSquare);
		Vector3 oldSquare = square; 
		square = predictedSquare; 
		predictedSquare.x = 2f * square.x - oldSquare.x; 
		predictedSquare.y = 2f * square.y - oldSquare.y; 

		// keeps track of stoppedByIce and stoppedByOffscreen events
		if(offScreen()) {
			return "OFFSCREEN_" + newLoc;
		} else if (blockedByIce()) {
			return "BLOCKED_" + newLoc;
		} else {
			Debug.Log("ERROR: Slide stopped by neither ice nor offscreen");
			return "ERROR_" + newLoc;
		}
	}

	private bool approximately(Vector3 first, Vector3 second) {
		if(Mathf.Approximately(first.x, second.x) && Mathf.Approximately(first.y, second.y) && Mathf.Approximately(first.z, second.z)) {
			return true;
		}
		return false;
	}

	// when this is called by playerController, need to parse returned string:
	// newLoc.Substring(newLoc.IndexOf("_")); (to get just the location information)
	public string move() {
		//TODO
		string newLoc = "-1";
		while(canMove()) {
			newLoc = moveOneSquare();
		}
		return newLoc;
	}

	public void reset() {
		transform.position = startingPos;
		square = startingSquare;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
