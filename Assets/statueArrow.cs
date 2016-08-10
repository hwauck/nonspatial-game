using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class statueArrow : MonoBehaviour {
	public GameObject OS;
	private statueArrow otherStatue; 
 	public Vector3 direction;
	private Vector3 right;
	private Vector3 left;
	private Vector3 up;
	private Vector3 down;
	public Vector2 square;
	public Vector2 predictedSquare;
	public Vector3 startingPos;
	public Vector2 startingSquare;
	public Vector2 startingPredicted;
	public Vector3 startingRotation;
	private IList<string> squares_explored_list;

	private int num_traversed_squares;


	// For data collection: top or bottom
	public string NAME;
	
	// Use this for initialization
	void Start () {
		otherStatue = (statueArrow)OS.GetComponent("statueArrow");
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		squares_explored_list = new List<string>();
		squares_explored_list.Add (coordinatesToSquare(startingSquare));
		num_traversed_squares = 1;
	}

	private string coordinatesToSquare(Vector2 coordinates) {
		return coordinates.x.ToString () + coordinates.y.ToString ();
	}

	private bool statueStatueCollision() {
		if(predictedSquare == otherStatue.predictedSquare) {
			return true;
		}
		return false;
	}

	private bool statuesBlockEachOther() {
		if(predictedSquare == otherStatue.square){
			return true;
		}
		return false;
	}

	private bool offScreen() {
		if(GameObject.Find ("Square" + predictedSquare.x + "" + predictedSquare.y) == null) {
			return true;
		}
		return false;
	}

	// index 0 is 1 if offScreen
	// index 1 is 1 if statueStatueCollision
	// index 2 is 1 if statuesBlockEachOther
	public bool[] getErrorType() {
		bool[] errors = new bool[3];
		for(int i = 0; i < 3; i++) {
			errors[i] = false;
		}
		if(offScreen ()) {
			errors[0] = true;
		} else if (statueStatueCollision()) {
			errors[1] = true;
		} else if (statuesBlockEachOther()) {
			errors[2] = true;
		}
		return errors;
	}

	public bool canMove() {
		return !offScreen () && !statueStatueCollision() && !statuesBlockEachOther();
	}
	
	public string move() {
		transform.Translate(direction * 2f, Space.World);
		num_traversed_squares++;
		string newLoc = coordinatesToSquare(predictedSquare);
		IList<string> otherStatue_squares_explored = otherStatue.getSquaresExplored();
		if(!squares_explored_list.Contains(newLoc) && !otherStatue_squares_explored.Contains(newLoc)) {
			squares_explored_list.Add(newLoc);
		} 
		Vector3 oldSquare = square; 
		square = predictedSquare; 
		predictedSquare.x = 2f * square.x - oldSquare.x; 
		predictedSquare.y = 2f * square.y - oldSquare.y; 
		return newLoc;
	}

	public void reset() {
		transform.position = startingPos;
		square = startingSquare;
		predictedSquare = startingPredicted;
		direction = startingRotation;
		transform.rotation = Quaternion.Euler(startingRotation);
	}
	
	public void turnDown(){
		direction = Vector3.down;
		transform.rotation = Quaternion.Euler(down);
		predictedSquare.x = square.x;
		predictedSquare.y = square.y + 1;
		
	}
	
	public void turnUp(){
		direction = Vector3.up;
		transform.rotation = Quaternion.Euler(up);
		predictedSquare.x = square.x;
		predictedSquare.y = square.y - 1;
	}
	
	public void turnLeft(){
		direction = Vector3.left;
		transform.rotation = Quaternion.Euler(left);
		predictedSquare.x = square.x - 1;
		predictedSquare.y = square.y;
	}
	
	public void turnRight(){
		direction = Vector3.right;
		transform.rotation = Quaternion.Euler(right);
		predictedSquare.x = square.x + 1;
		predictedSquare.y = square.y;
	}

	public string getPredictedSquare() {
		return coordinatesToSquare(predictedSquare);
	}

	public IList<string> getSquaresExplored() {
		return squares_explored_list;
	}

	public int getNumTraversedSquares() {
		return num_traversed_squares++;
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	
	
}	


