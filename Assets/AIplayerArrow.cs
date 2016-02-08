using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class AIplayerArrow : MonoBehaviour {
	private GameObject SAB;
	private GameObject SAT;
	private statueArrow statueArrowBottom; 
	private statueArrow statueArrowTop; 
	private Vector3 direction;
	private Vector3 right;
	private Vector3 left;
	private Vector3 up;
	private Vector3 down;
	private Vector2 square;
	private Vector2 predictedSquare;
	private int players = 0;
	private bool finished = false;
	private const int NUM_PLAYERS = 20;
	private const int MAX_MOVES = 200; //based on maximum # of moves players had in pilot study

	// Data to be collected
	private int moves;
	private List<string> squares_explored_list;
	//records complete sequence in order with repeats
	private List<string> states_explored_list;
	private int unique_states;
	// number of times all statues and player move
	private int all_move;
	// number of times the player and only one of the statues move
	private int two_move;
	// number of times only the player moves
	private int player_only_moves;

	//file to collect data
	private string dataFile;
	private  StreamWriter outputStream;
	
	// Use this for initialization
	void Start () {
		SAB = GameObject.Find ("statueArrowBottom");
		SAT = GameObject.Find ("statueArrowTop");
		statueArrowBottom = (statueArrow)SAB.GetComponent("statueArrow");
		statueArrowTop = (statueArrow)SAT.GetComponent("statueArrow");
		direction = new Vector3(0,0,0);
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		square = new Vector2(2,3);
		predictedSquare = new Vector2(2,2);
		
		//Data collection variables
		moves = 0;
		squares_explored_list = new List<string>();
		squares_explored_list.Add ("23");
		states_explored_list = new List<string>();
		states_explored_list.Add ("23:25:21");
		unique_states = 1;
		all_move = 0;
		two_move = 0;
		player_only_moves = 0;

		//Data collection file writing
		dataFile = "AI_data.txt";
		outputStream = new StreamWriter( dataFile, true );
		
	}

	public bool isStuck() {
		if(coordinatesToSquare(square).Equals ("25") && coordinatesToSquare(statueArrowBottom.square).Equals ("24")) {
			//bottom
			return true;
		} else if (coordinatesToSquare(square).Equals("00") && coordinatesToSquare(statueArrowBottom.square).Equals ("10")){
			//upper left horizontal
			return true;
		} else if (coordinatesToSquare(square).Equals("40") && coordinatesToSquare(statueArrowBottom.square).Equals ("30")){
			//upper right horizontal
			return true;
		} else if (coordinatesToSquare(square).Equals("40") && coordinatesToSquare(statueArrowBottom.square).Equals ("41")){
			//upper right vertical
			return true;
		} else if (coordinatesToSquare(square).Equals("00") && coordinatesToSquare(statueArrowBottom.square).Equals ("01")){
			//upper left vertical
			return true;
		} else {
			return false;
		}
	}

	public bool victory() {
		bool cond1 = coordinatesToSquare(statueArrowBottom.square).Equals ("11") && coordinatesToSquare(statueArrowTop.square).Equals ("31");
		bool cond2 = coordinatesToSquare(statueArrowBottom.square).Equals ("31") && coordinatesToSquare(statueArrowTop.square).Equals ("11");
		return cond1 || cond2;
	}

	public void act() {
		if (players < NUM_PLAYERS) {
			if (moves > MAX_MOVES || isStuck() || victory()) {
				logEndGameData();
				resetForNewPlayer();

				players++;
			}
			int randomTurn = (int)UnityEngine.Random.Range (0.0f, 4.0f);
			if(randomTurn == 0) {
				turnUp();
				statueArrowBottom.turnUp ();
				statueArrowTop.turnDown ();
			} else if (randomTurn == 1){
				turnRight();
				statueArrowBottom.turnRight ();
				statueArrowTop.turnLeft ();
			} else if (randomTurn == 2){
				turnDown();
				statueArrowBottom.turnDown ();
				statueArrowTop.turnUp ();
			} else if (randomTurn == 3){
				turnLeft();
				statueArrowBottom.turnLeft ();
				statueArrowTop.turnRight ();
			}
			logMoveData();
			if(canMove ()) {
				move ();
				moves++;
				if(statueArrowBottom.canMove()) {
					statueArrowBottom.move();
				}
				if(statueArrowTop.canMove()) {
					statueArrowTop.move();
				}
				Debug.Log ("ADDED new state " + stateToString());
				if(!states_explored_list.Contains(stateToString ())) {
					unique_states++;
				}
				states_explored_list.Add (stateToString ());
			}
		} else {
			finished = true;
			Application.Quit ();
		}

	}

	private string stateToString() {
		string state = "";
		state += coordinatesToSquare(square);
		state += ":" + coordinatesToSquare(statueArrowBottom.square);
		state += ":" + coordinatesToSquare(statueArrowTop.square);
		return state;
	}
	
	private string coordinatesToSquare(Vector2 coordinates) {
		return coordinates.x.ToString () + coordinates.y.ToString ();
	}
	
	public Vector3[] findHalfwayPoint(Vector3 object1Pos, Vector3 object2Pos) {
		float x1 = object1Pos.x;
		float y1 = object1Pos.y;
		float z1 = object1Pos.z;
		float x2 = object2Pos.x;
		float y2 = object2Pos.y;
		float z2 = object2Pos.z;
		Vector3 halfwayPoint = new Vector3((x1 + x2)/2f, (y1 + y2)/2f, (z1 + z2)/2f);
		print("HalfwayPoint: " + halfwayPoint);
		float xHalf = halfwayPoint.x;
		float yHalf = halfwayPoint.y;
		float zHalf = halfwayPoint.z;
		Vector3 amountToMoveObject1 = new Vector3((xHalf - x1),(yHalf - y1),(zHalf - z1));
		Vector3 amountToMoveObject2 = new Vector3((xHalf - x2),(yHalf - y2),(zHalf - z2));
		Vector3[] moveAmounts = new Vector3[2];
		moveAmounts[0] = amountToMoveObject1;
		moveAmounts[1] = amountToMoveObject2;
		return moveAmounts;
		
	}
	
	
	private bool playerStatueCollision() {
		if(predictedSquare == statueArrowBottom.predictedSquare) {
			return true;
		} else if (predictedSquare == statueArrowTop.predictedSquare) {
			return true;
		}
		return false;
	}
	
	private bool playerStatueBlockEachOther() {
		if(predictedSquare == statueArrowBottom.square || predictedSquare == statueArrowTop.square) {
			//Print error message
			
			return true;
		}
		return false;
	}
	
	private bool offScreen() {
		if(GameObject.Find ("Square" + predictedSquare.x + "" + predictedSquare.y) == null) {
			//Print error message
			return true;
		}
		return false;
	}
	
	// index 0 is 1 if offScreen
	// index 1 is 1 if playerStatueCollision
	// index 2 is 1 if playerStatueBlockEachOther
	public bool[] getErrorType() {
		bool[] errors = new bool[3];
		for(int i = 0; i < 3; i++) {
			errors[i] = false;
		}
		if(offScreen ()) {
			errors[0] = true;
		} else if (playerStatueCollision()) {
			errors[1] = true;
		} else if (playerStatueBlockEachOther()) {
			errors[2] = true;
		}
		return errors;
	}
	
	
	public bool canMove() {
		return !offScreen() && !playerStatueCollision() && !playerStatueBlockEachOther();
	}
	
	public void move() {
		transform.Translate(direction * 2f, Space.World);
		string predictedSquareName = coordinatesToSquare(predictedSquare);
		Debug.Log ("PLAYER_MOVE_TO," + predictedSquareName);
		Vector3 oldSquare = square; 
		square = predictedSquare; 
		if(!squares_explored_list.Contains(predictedSquareName)) {
			squares_explored_list.Add(predictedSquareName);
		}
		predictedSquare.x = 2f * square.x - oldSquare.x; 
		predictedSquare.y = 2f * square.y - oldSquare.y; 
		
	}

	public void resetForNewPlayer() {
		transform.position = new Vector3(0,-3,-1);
		square = new Vector2(2,3);
		predictedSquare = new Vector2(2,2);
		direction = Vector3.up;
		transform.rotation = Quaternion.Euler(up);

		statueArrowBottom.reset();
		statueArrowTop.reset();

		// reset data collection
		moves = 0;
		squares_explored_list = new List<string>();
		squares_explored_list.Add ("23");
		states_explored_list = new List<string>();
		states_explored_list.Add ("23:25:21");
		unique_states = 1;
		all_move = 0;
		two_move = 0;
		player_only_moves = 0;
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
	
	private void logMoveData() {
		if(canMove() && statueArrowBottom.canMove() && statueArrowTop.canMove ()) {
			//outputStream.WriteLine("ALL_MOVE");
			//Debug.Log ("ALL_MOVE");
			all_move++;
		} else if (canMove() && (statueArrowBottom.canMove() || statueArrowTop.canMove())) {
			//outputStream.WriteLine("TWO_MOVE");
			//Debug.Log ("TWO_MOVE");
			two_move++;
		} else if (canMove()) {
			//outputStream.WriteLine("ONE_MOVE");
			//Debug.Log ("ONE_MOVE");
			player_only_moves++;
		} 
	}

	private string listToString(List<string> listToConvert) {
		string listString = "";
		char[] charsToTrim = {'-', ' '};
		foreach (string item in listToConvert) {
			listString += item;
			listString += "-";
		}
		listString = listString.TrimEnd(charsToTrim);
		return listString;
	}
	
	private void logEndGameData(){
		string squaresExplored = listToString (squares_explored_list);
		string statesExplored = listToString (states_explored_list);
		// did the player win, get stuck, or run out of moves?
		string endState = "LOSE";
		if (isStuck()) {
			endState += ",STUCK";
		}
		if (victory ()) {
			endState = "WIN";
		}

		StringBuilder sb = new StringBuilder();

		sb.AppendLine ("PLAYER " + players);
		sb.AppendLine ("END_STATE," + endState);
		sb.AppendLine ("TOTAL_MOVES," + moves);
		sb.AppendLine ("SQUARES_EXPLORED," + squares_explored_list.Count);
		sb.AppendLine ("LIST_OF_SQUARES_EXPLORED," + squaresExplored);
		sb.AppendLine ("LIST_OF_STATES_EXPLORED," + statesExplored);
		sb.AppendLine ("STATES_EXPLORED," + unique_states);
		sb.AppendLine ("ALL_MOVE," + all_move.ToString());
		sb.AppendLine ("TWO_MOVE," + two_move.ToString());
		sb.AppendLine ("ONE_MOVE," + player_only_moves.ToString() + "\n");

		outputStream.Write(sb);

		Debug.Log ("PLAYER " + players);
		Debug.Log ("END_STATE," + endState);
		Debug.Log ("TOTAL_MOVES," + moves);
		Debug.Log ("SQUARES_EXPLORED," + squares_explored_list.Count);
		Debug.Log ("LIST_OF_SQUARES_EXPLORED, " + squaresExplored);
		Debug.Log ("LIST_OF_STATES_EXPLORED," + statesExplored);
		Debug.Log ("STATES_EXPLORED," + unique_states);
		Debug.Log ("ALL_MOVE," + all_move.ToString());
		Debug.Log ("TWO_MOVE," + two_move.ToString());
		Debug.Log ("ONE_MOVE," + player_only_moves.ToString() + "\n");
		
	}
	
	// Update is called once per frame
	void Update () {
		if(finished) {
			CancelInvoke();
			outputStream.Close ();
		}

	}

	public void runGame() {

		InvokeRepeating("act", 0.01f, 0.2f);

	
	}
	
	
	
}	


