using UnityEngine;
using System;
using System.Collections;

public class iceBlock : MonoBehaviour {

	public playerArrowIce player;
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
	private int iceCantMove; 	//should equal iceBlockedByIce + iceBlockedbyOffscreen
	private int iceBlockedByIce; 
	private int iceBlockedByOffscreen; 
	private int stoppedByIce;
	private int stoppedByOffscreen;
	private int num_traversed_squares;
	private int num_repeated_squares;
	private int[,] squaresExplored; // each entry indicates how many times a square has been explored
	private string[] movementSquares; // ice can only move on white squares

	// Use this for initialization
	void Start () {
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		iceCantMove = 0; 	
		iceBlockedByIce = 0; 
		iceBlockedByOffscreen = 0; 
		stoppedByIce = 0;
		stoppedByOffscreen = 0;
		num_traversed_squares = 1;
		num_repeated_squares = 0;

		squaresExplored = new int[5,7];
		for(int row = 0; row < 5; row++) {
			for(int col = 0; col < 7; col++) {
				squaresExplored[row,col] = 0;
			}
		}
		movementSquares = new string[15];
		int i = 0;
		for(int x = 2; x < 5; x++) {
			for(int y = 2; y < 7; y++) {
				movementSquares[i] = "" + x + "" + y;
				i++;
			}
		}
	}

	public int getIceCantMove() {
		return iceCantMove;
	}

	public int getIceBlockedByIce() {
		return iceBlockedByIce;
	}

	public int getIceBlockedByOffscreen() {
		return iceBlockedByOffscreen;
	}

	public int getStoppedByOffscreen() {
		return stoppedByOffscreen;
	}

	public int getStoppedByIce() {
		return stoppedByIce;
	}

	public int getTraversedSquares() {
		return num_traversed_squares;
	}

	public int getRepeatedSquares() {
		return num_repeated_squares;
	}

	public int getNumSquaresExplored() {
		int count = 0;
		for(int row = 0; row < 5; row++) {
			for(int col = 0; col < 7; col++) {
				count+=squaresExplored[row,col];
			}
		}
		return count;
	}

	//this should be equal to traversed squares + repeated squares
	public int[,] getSquaresExplored() {
		return squaresExplored;
	}


	private string coordinatesToSquare(Vector2 coordinates) {
		return coordinates.x.ToString () + coordinates.y.ToString ();
	}


	public bool canMove() {
		updatePredictedSquare();
		return !offScreen () && !blockedByIce();
	}

	private bool offScreen() {
		for(int i = 0; i < movementSquares.Length; i++) {
			if(movementSquares[i].Equals(coordinatesToSquare(predictedSquare))) {
				return false;
			}
		}
		iceBlockedByOffscreen++;
		iceCantMove++;
		return true;
	}

	private bool blockedByIce() {
		if(predictedSquare == otherBlocks[0].square || predictedSquare == otherBlocks[1].square) {
			iceBlockedByIce++;
			iceCantMove++;
			return true;
		} else {
			return false;
		}
	}

	private void updatePredictedSquare() {
		direction = player.getDirection();
		if(approximately(direction, Vector3.up)) {
			predictedSquare.x = square.x - 1;
			predictedSquare.y = square.y;
		} else if (approximately(direction, Vector3.down)) {
			predictedSquare.x = square.x + 1;
			predictedSquare.y = square.y;
		} else if (approximately(direction, Vector3.left)) {
			predictedSquare.y = square.y - 1;
			predictedSquare.x = square.x;
		} else if (approximately(direction, Vector3.right)) {
			predictedSquare.y = square.y + 1;
			predictedSquare.x = square.x;
		} 
	}

	public string moveOneSquare() {
		transform.Translate(direction * 2f, Space.World);
		string newLoc = coordinatesToSquare(predictedSquare);
		int x = Convert.ToInt32(newLoc.Substring(0,1));
		int y = Convert.ToInt32(newLoc.Substring(1,1));
		num_traversed_squares++;
		if(squaresExplored[x-1,y-1] > 0) {
			num_repeated_squares++;
		} 
		squaresExplored[x-1,y-1]++;

		Vector3 oldSquare = square; 
		square = predictedSquare; 
		predictedSquare.x = 2f * square.x - oldSquare.x; 
		predictedSquare.y = 2f * square.y - oldSquare.y; 

		// keeps track of stoppedByIce and stoppedByOffscreen events
		if(offScreen()) {
			stoppedByOffscreen++;
		} else if (blockedByIce()) {
			stoppedByIce++;
		} 
		return newLoc;
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
