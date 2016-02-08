using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;

public class playerArrow : MonoBehaviour {
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
	private bool victorious;

	//UI for playing a new game
	private Button yes;
	private Button no;
	private Image victoryPanel;
	private Text victoryText;

	// Data to be collected
	private const string FILENAME = "DATA_FILE.txt";
	private int moves;
	private float startTime;
	private float prevMoveEndTime;
	private int turnCount;
	private float game_time;
	//ratio: # moves on left side of board/# moves of right side
	private int left_squares;
	private int right_squares;
	private int num_repeated_squares;
	private int num_traversed_squares;
	private float avg_repeats_per_square;
	private float avg_time_per_move;
	private float avg_turns_per_move;
	private IList<string> squares_explored_list;
	private IList<string> left_squares_list;
	private IList<string> right_squares_list;

	// number of times all statues and player move
	private int all_move;
	// number of times the player and only one of the statues move
	private int two_move;
	// number of times only the player moves
	private int player_only_moves;

	StreamWriter sr;

	void Awake() {
		victorious = false;
		startTime = Time.time;
		prevMoveEndTime = startTime;
	}

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
		turnCount = 0;
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;
		squares_explored_list = new List<string>();
		squares_explored_list.Add ("23");
		left_squares = 0;
		right_squares = 0;
		num_repeated_squares = 0;
		num_traversed_squares = 0;

		left_squares_list = new List<string>();
		left_squares_list.Add ("00");
		left_squares_list.Add ("10");
		left_squares_list.Add ("01");
		left_squares_list.Add ("11");
		left_squares_list.Add ("02");
		left_squares_list.Add ("12");
		left_squares_list.Add ("13");
		left_squares_list.Add ("14");

		right_squares_list = new List<string>();
		right_squares_list.Add ("30");
		right_squares_list.Add ("40");
		right_squares_list.Add ("31");
		right_squares_list.Add ("41");
		right_squares_list.Add ("32");
		right_squares_list.Add ("42");
		right_squares_list.Add ("33");
		right_squares_list.Add ("34");

		all_move = 0;
		two_move = 0;
		player_only_moves = 0;

		//Victory UI variables
		yes = GameObject.Find ("Yes").GetComponent<Button>();
		no = GameObject.Find ("No").GetComponent<Button>();
		victoryPanel = GameObject.Find ("Victory").GetComponent<Image>();
		victoryText = GameObject.Find ("Congratulations").GetComponent<Text>();

		//start file writing
		if(File.Exists(FILENAME)) {
			print ("File already exists: " + FILENAME);
		}
		sr = File.CreateText (FILENAME);
		sr.WriteLine ("NEW GAME");
		sr.WriteLine ("Gender: Female");
		sr.WriteLine ("Age: 41");

	}

	public IList<string> getSquaresExplored() {
		return squares_explored_list;
	}

	public int getNumSquaresExplored() {
		return squares_explored_list.Count;
	}

	public int getNumMoves() {
		return moves;
	}

	private void displayOptions() {
		victoryPanel.enabled = true;
		victoryText.enabled = true;

		yes.GetComponent<Image>().enabled = true;
		yes.interactable = true;
		yes.transform.FindChild("YesText").GetComponent<Text>().enabled = true;

		no.GetComponent<Image>().enabled = true;
		no.interactable = true;
		no.transform.FindChild("NoText").GetComponent<Text>().enabled = true;

	}

	private void unDisplayOptions() {
		victoryPanel.enabled = false;
		victoryText.enabled = false;

		yes.GetComponent<Image>().enabled = false;
		yes.interactable = false;
		yes.transform.FindChild("YesText").GetComponent<Text>().enabled = false;

		no.GetComponent<Image>().enabled = false;
		no.interactable = false;
		no.transform.FindChild("NoText").GetComponent<Text>().enabled = false;

	}
	
	private string coordinatesToSquare(Vector2 coordinates) {
		return coordinates.x.ToString () + coordinates.y.ToString ();
	}

	public Vector3 findHalfwayPoint(Vector3 object1Pos, Vector3 object2Pos) {
		float x1 = object1Pos.x;
		float y1 = object1Pos.y;
		float z1 = object1Pos.z;
		float x2 = object2Pos.x;
		float y2 = object2Pos.y;
		float z2 = object2Pos.z;
		Vector3 halfwayPoint = new Vector3((x1 + x2)/2f, (y1 + y2)/2f, (z1 + z2)/2f);
		return halfwayPoint;

	}

	public IEnumerator collisionHelper(GameObject collider1, GameObject collider2, Vector3 origCollider1pos, Vector3 origCollider2pos, float step) {
		Vector3 collider1pos = collider1.transform.position;
		Vector3 collider2pos = collider2.transform.position;
		Vector3 halfwayPoint = findHalfwayPoint(collider1pos, collider2pos);

		collider1.transform.position = Vector3.MoveTowards(collider1.transform.position, halfwayPoint, step);
		collider2.transform.position = Vector3.MoveTowards (collider2.transform.position, halfwayPoint, step);
		yield return new WaitForSeconds(0.01f);

		if(!approximately(collider1.transform.position, halfwayPoint) || !approximately(collider2.transform.position, halfwayPoint)) {
			StartCoroutine (collisionHelper (collider1, collider2, origCollider1pos, origCollider2pos, step));
		} else {
			collider1.transform.position = origCollider1pos;
			collider2.transform.position = origCollider2pos;
		}
	}

	public IEnumerator collisionHelperOneSided(GameObject collider1, GameObject collider2, Vector3 origCollider1pos, float step) {
		//collider1 will move towards collider2; collider2 remains stationary
		Vector3 collider1pos = collider1.transform.position;
		Vector3 collider2pos = collider2.transform.position;
		Vector3 halfwayPoint = findHalfwayPoint(origCollider1pos, collider2pos);
		
		collider1.transform.position = Vector3.MoveTowards(collider1.transform.position, halfwayPoint, step);
		yield return new WaitForSeconds(0.01f);
		
		if(!approximately(collider1.transform.position, halfwayPoint)) {
			StartCoroutine (collisionHelperOneSided (collider1, collider2, origCollider1pos, step));
		} else {
			collider1.transform.position = origCollider1pos;
		}
	}

	private bool approximately(Vector3 first, Vector3 second) {
		if(Mathf.Approximately(first.x, second.x) && Mathf.Approximately(first.y, second.y) && Mathf.Approximately(first.z, second.z)) {
			return true;
		}
		return false;
	}

	private bool playerStatueAreColliding() {
		if(predictedSquare == statueArrowBottom.predictedSquare) {
			return true;
		} else if (predictedSquare == statueArrowTop.predictedSquare) {
			return true;
		}
		return false;
	}

//	private bool playerStatueCollision() {
//		if(predictedSquare == statueArrowBottom.predictedSquare) {
//			print ("Player collides with bottom statue!");
//			StartCoroutine (collisionHelper (this.gameObject, SAB, transform.position, SAB.transform.position, 0.1f));
//			return true;
//		} else if (predictedSquare == statueArrowTop.predictedSquare) {
//			print ("Player collides with top statue!");
//			StartCoroutine (collisionHelper (this.gameObject, SAT, transform.position, SAT.transform.position, 0.1f));
//			return true;
//		}
//		return false;
//	}

	private bool playerStatueBlockEachOther() {
		if(predictedSquare == statueArrowBottom.square || predictedSquare == statueArrowTop.square) {
			//Print error message
			print ("Player blocked by statue!");
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
		} else if (playerStatueAreColliding()) {
			errors[1] = true;
		} else if (playerStatueBlockEachOther()) {
			errors[2] = true;
		}
		return errors;
	}


	public bool canMove() {
		return !offScreen() && !playerStatueAreColliding() && !playerStatueBlockEachOther();
	}

	public string move() {
		transform.Translate(direction * 2f, Space.World);
		num_traversed_squares++;
		string predictedSquareName = coordinatesToSquare(predictedSquare);
		sr.Write (predictedSquareName);
		Vector3 oldSquare = square; 
		square = predictedSquare; 
		if(!squares_explored_list.Contains(predictedSquareName)) {
			squares_explored_list.Add(predictedSquareName);
		} else {
			num_repeated_squares++;
		}
		predictedSquare.x = 2f * square.x - oldSquare.x; 
		predictedSquare.y = 2f * square.y - oldSquare.y; 
		return predictedSquareName;
	}

	public void reset() {
		sr.WriteLine ("RESET\n");
		sr.Flush ();
		if(victorious) {
			victorious = false;
		}
		transform.position = new Vector3(0,-3,-1);
		square = new Vector2(2,3);
		predictedSquare = new Vector2(2,2);
		direction = Vector3.up;
		transform.rotation = Quaternion.Euler(up);

		//Data collection variables
		moves = 0;
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;
		squares_explored_list = new List<string>();
		squares_explored_list.Add ("23");
		all_move = 0;
		two_move = 0;
		player_only_moves = 0;

		statueArrowBottom.reset ();
		statueArrowTop.reset ();

		unDisplayOptions();
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
		//When a player moves, record the current time
		// when the player next moves, record the current time
		// subtract the previous move time from the current move time
		float currentTime = Time.time;
		float currentMoveTime = currentTime - prevMoveEndTime;
		avg_time_per_move += currentMoveTime;
		prevMoveEndTime = currentTime;
		avg_turns_per_move += turnCount;
		turnCount = 0;
		if(canMove() && statueArrowBottom.canMove() && statueArrowTop.canMove ()) {
			Debug.Log ("ALL_MOVE");
			all_move++;
		} else if (canMove() && (statueArrowBottom.canMove() || statueArrowTop.canMove())) {
			Debug.Log ("TWO_MOVE");
			two_move++;
		} else if (canMove()) {
			Debug.Log ("ONE_MOVE");
			player_only_moves++;
		} 
		print ("MOVE_TIME: " + currentMoveTime);
	}

	private bool victory() {
		string statueBottomSquare = coordinatesToSquare(statueArrowBottom.square);
		string statueTopSquare = coordinatesToSquare(statueArrowTop.square);
		if(statueBottomSquare.Equals ("11") && statueTopSquare.Equals("31")) {
			victorious = true;
			return true;
		} else if (statueTopSquare.Equals ("11") && statueBottomSquare.Equals("31")) {
			victorious = true;
			return true;
		} else {
			return false;
		}
	}

	private void logEndGameData(){
		avg_time_per_move = avg_time_per_move/moves;
		avg_repeats_per_square = num_repeated_squares/(num_traversed_squares * 1.0f);
		avg_turns_per_move = avg_turns_per_move/(moves * 1.0f);
		sr.WriteLine ("TOTAL_TIME, " + (Time.time - startTime).ToString ());
		sr.WriteLine ("TOTAL_MOVES, " + moves);
		sr.WriteLine ("AVG_TIME_PER_MOVE, " + avg_time_per_move.ToString());
		sr.WriteLine ("AVG_TURNS_PER_MOVE, " + avg_turns_per_move.ToString());
		sr.WriteLine ("SQUARES_EXPLORED, " + squares_explored_list.Count);
		sr.WriteLine ("NUM_REPEATED_SQUARES, " + num_repeated_squares);
		sr.WriteLine ("AVG_REPEATS_PER_SQUARE, " + avg_repeats_per_square);
		sr.WriteLine ("ALL_MOVE, " + all_move.ToString());
		sr.WriteLine ("TWO_MOVE, " + two_move.ToString());
		sr.WriteLine ("ONE_MOVE, " + player_only_moves.ToString());
		sr.WriteLine ("LEFT_RIGHT_SYMMETRY, " + (left_squares / (right_squares * 1.0f)));
	}

	private void countLeftRightSymmetry(string newLoc) {
		if(left_squares_list.Contains (newLoc)) {
			left_squares++;
		} else if (right_squares_list.Contains (newLoc)) {
			right_squares++;
		}
	}

	// Update is called once per frame
	void Update () {
		if(!victorious) {
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				turnCount++;
				turnDown ();
				statueArrowBottom.turnDown ();
				statueArrowTop.turnUp ();
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				turnCount++;
				turnUp ();
				statueArrowBottom.turnUp ();
				statueArrowTop.turnDown ();
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				turnCount++;
				turnRight ();
				statueArrowBottom.turnRight ();
				statueArrowTop.turnLeft ();
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				turnCount++;
				turnLeft ();
				statueArrowBottom.turnLeft ();
				statueArrowTop.turnRight ();
			} else if(Input.GetMouseButtonDown(0)) {
				logMoveData ();
				bool[] errorsPlayer = getErrorType ();
				if(!errorsPlayer[0] && !errorsPlayer[1] && !errorsPlayer[2]) {
					string newLoc = move ();
					moves++;
					countLeftRightSymmetry(newLoc);
					bool[] errorsBottom = statueArrowBottom.getErrorType ();
					
					if(errorsBottom[1]) {
						//if there is a statue collision
						sr.WriteLine (",-1,-1");
						StartCoroutine (collisionHelper (SAB, SAT, SAB.transform.position, SAT.transform.position, 0.1f));
					} else if (errorsBottom[2]) {
						//if there is a statue blocking
						sr.WriteLine (",-1,-1");
						StartCoroutine (collisionHelper (SAB, SAT, SAB.transform.position, SAT.transform.position, 0.05f));
					}else {
						bool[] errorsTop = statueArrowTop.getErrorType();
						if(!errorsBottom[0]) {
							//not offscreen
							string newLocStatue = statueArrowBottom.move();
							sr.Write ("," + newLocStatue);
						} else {
							sr.Write (",-1");
						}
						if(!errorsTop[0]) {
							// not offscreen
							string newLocStatue = statueArrowTop.move();
							sr.WriteLine ("," + newLocStatue);
						} else {
							sr.WriteLine (",-1");
						}
					}
				} else if(errorsPlayer[1]) {
					//if player collides with statue (has to be top statue)
					StartCoroutine (collisionHelper(this.gameObject, SAT, transform.position, SAT.transform.position, 0.1f));
				} else if (errorsPlayer[2]) {
					//if player is blocked by a statue (could be either one)
					if(predictedSquare == statueArrowBottom.square) {
						StartCoroutine (collisionHelperOneSided(this.gameObject, SAB, transform.position, 0.05f));
					} else if(predictedSquare == statueArrowTop.square) {
						StartCoroutine (collisionHelperOneSided(this.gameObject, SAT, transform.position, 0.05f));
					}
				}
			} else if (Input.GetKeyDown (KeyCode.R)) {
				logEndGameData();
				reset();
			} else if (Input.GetKeyDown(KeyCode.Escape)) {
				logEndGameData();
				quit ();
			} else if(victory()) {
				logEndGameData ();
				sr.WriteLine ("VICTORY\n");
				sr.Flush ();
				//change later to allow player to play a new game
				displayOptions();
			}
		}

	}

	public void quit() {
		sr.WriteLine ("QUIT\n");
		sr.Flush ();
		sr.Close ();
		Application.Quit();
	}


		
}	

