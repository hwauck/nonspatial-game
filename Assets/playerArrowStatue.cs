using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class playerArrowStatue : MonoBehaviour {
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
	private int plays; // increment each time player begins again (reset, victory-yes, start of session)
	private int victories;
	private int resets;
	private int moves;
	private float startTime;
	private float prevMoveEndTime;
	private int turns;
	private float game_time;
	private float session_time; // time spent in all games combined
	private float sessionStart_time;
	//ratio: # moves on left side of board/# moves of right side
	private int left_squares;
	private int right_squares;
	private int top_squares;
	private int bottom_squares;
	private int num_repeated_squares;
	private int num_traversed_squares; // total displacement, including repeated squares
	private int squares_explored;
	private float avg_time_per_move;
	private float avg_turns_per_move;
	private IList<string> squares_explored_list;
	private IList<string> left_squares_list;
	private IList<string> right_squares_list;
	private IList<string> top_squares_list;
	private IList<string> bottom_squares_list;
	private float left_right_symmetry;
	private float top_bottom_symmetry;

	// number of times all statues and player move
	private int all_move;
	// number of times the player and only one of the statues move
	private int two_move;
	// number of times only the player moves
	private int player_only_moves;
	private int playerStatueCollide;
	private int playerBlockedByStatue;
	private int statuesBlockEachOther; // facing each other, can't move
	private int statuesCollide; // facing each other, try to move onto same square
	private int statueBlockedByOffscreen;

	/* STATUE ARROW DATA */
	private int num_traversed_squares_statues;
	private int num_repeated_squares_statues;
	private int squares_explored_statues;
	private int left_squares_statues;
	private int right_squares_statues;
	private int bottom_squares_statues;
	private int top_squares_statues;
	private float left_right_symmetry_statues;
	private float top_bottom_symmetry_statues;

	string resultStr;

	void Awake() {
		victorious = false;
		startTime = Time.time;
		sessionStart_time = startTime;
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
		plays = 1;
		victories = 0;
		resets = 0;
		resultStr = "NEW_GAME,statue__";
		moves = 0;
		turns = 0;
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;
		squares_explored_list = new List<string>();
		squares_explored_list.Add ("23");
		left_squares = 0;
		right_squares = 0;
		top_squares = 0;
		bottom_squares = 1;
		squares_explored = 0;
		num_repeated_squares = 0;
		num_traversed_squares = 1;
		left_right_symmetry = -1f;
		top_bottom_symmetry = -1f;
		all_move = 0;
		two_move = 0;
		player_only_moves = 0;
		playerStatueCollide = 0;
		playerBlockedByStatue = 0;
		statuesBlockEachOther = 0;
		statuesCollide = 0;
		statueBlockedByOffscreen = 0;

		/* STATUE ARROW DATA */
		num_traversed_squares_statues = 0; // statue Arrow will take care of this
		num_repeated_squares_statues = 0;
		squares_explored_statues = 2;
		left_squares_statues = 0;
		right_squares_statues = 0;
		bottom_squares_statues = 1;
		top_squares_statues = 1;
		left_right_symmetry_statues = -1f;
		top_bottom_symmetry_statues = -1f;

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

		bottom_squares_list = new List<string>();
		bottom_squares_list.Add ("13");
		bottom_squares_list.Add ("23");
		bottom_squares_list.Add ("33");
		bottom_squares_list.Add ("14");
		bottom_squares_list.Add ("24");
		bottom_squares_list.Add ("34");
		bottom_squares_list.Add ("25");

		top_squares_list = new List<string>();
		top_squares_list.Add ("00");
		top_squares_list.Add ("10");
		top_squares_list.Add ("20");
		top_squares_list.Add ("30");
		top_squares_list.Add ("40");
		top_squares_list.Add ("01");
		top_squares_list.Add ("11");
		top_squares_list.Add ("21");
		top_squares_list.Add ("31");
		top_squares_list.Add ("41");
		top_squares_list.Add ("02");
		top_squares_list.Add ("12");
		top_squares_list.Add ("22");
		top_squares_list.Add ("32");
		top_squares_list.Add ("42");

		//Victory UI variables
		yes = GameObject.Find ("Yes").GetComponent<Button>();
		no = GameObject.Find ("No").GetComponent<Button>();
		victoryPanel = GameObject.Find ("Victory").GetComponent<Image>();
		victoryText = GameObject.Find ("Congratulations").GetComponent<Text>();

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
		//resultStr += predictedSquareName;
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

	// logs end game data, increments resets, and saves results to database
	// only when "Play Again? Yes" button is clicked
	public void newGame() {
		//resultStr += "RESET__";
		plays++;
		reset();
		resultStr += "\nNEW_GAME,statue__";
	}
		
	// when the "Reset" button is clicked
	public void buttonReset() {
		resets++;
		plays++;
		logEndGameData();
		reset();
		resultStr += "RESET__\nNEW_ATTEMPT,statue__";

	}

	// only when "I'm done playing" button is clicked
	// (end game data has not yet been logged)
	public void buttonQuit() {
		logEndGameData();
		resultStr += "DONE__\nEND_SESSION,done__";

		SendSaveResult();
		SceneManager.LoadScene("postgame_survey");
	}

	// only when "Play Again? No" button is clicked
	// (end game data has already been logged)
	public void saveAndQuit() {
		//resultStr +="QUIT__";
		resultStr += "NO__\nEND_SESSION,no__";

		SendSaveResult();
		SceneManager.LoadScene("postgame_survey");
	}

	private void SendSaveResult()
	{	
		session_time = Time.time - sessionStart_time;
		resultStr += "ATTEMPTS," + plays + "__";
		resultStr += "RESETS," + resets + "__";
		resultStr += "VICTORIES," + victories + "__";
		resultStr += "SESSION_TIME," + session_time + "__";
		GameObject.Find("DataCollector").GetComponent<dataCollector>().setPlayerData(resultStr);
		Debug.Log(resultStr);

	}

	// resets data after 
	public void reset() {
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
		turns = 0;
		startTime = Time.time;
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;
		squares_explored_list = new List<string>();
		squares_explored_list.Add ("23");
		left_squares = 0;
		right_squares = 0;
		top_squares = 0;
		bottom_squares = 1;
		squares_explored = 0;
		num_repeated_squares = 0;
		num_traversed_squares = 1;
		left_right_symmetry = -1f;
		top_bottom_symmetry = -1f;
		all_move = 0;
		two_move = 0;
		player_only_moves = 0;
		playerStatueCollide = 0;
		playerBlockedByStatue = 0;
		statuesBlockEachOther = 0;
		statuesCollide = 0;
		statueBlockedByOffscreen = 0;

		/* STATUE ARROW DATA */
		num_traversed_squares_statues = 0;
		num_repeated_squares_statues = 0;
		squares_explored_statues = 2;
		left_squares_statues = 0;
		right_squares_statues = 0;
		bottom_squares_statues = 1;
		top_squares_statues = 1;
		left_right_symmetry_statues = -1f;
		top_bottom_symmetry_statues = -1f;

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
		if(canMove() && statueArrowBottom.canMove() && statueArrowTop.canMove ()) {
			all_move++;
		} else if (canMove() && (statueArrowBottom.canMove() || statueArrowTop.canMove())) {
			two_move++;
		} else if (canMove()) {
			player_only_moves++;
		} 
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
		if(moves == 0) {
			avg_time_per_move = -1f;
			avg_turns_per_move = -1f;
		} else {
			avg_time_per_move = avg_time_per_move/moves;
			avg_turns_per_move = turns/(moves * 1.0f);
		}

		squares_explored = squares_explored_list.Count;

		if(right_squares == 0) {
			left_right_symmetry = -1f;
		} else {
			left_right_symmetry = (left_squares / (right_squares * 1.0f));
		}
		if(bottom_squares == 0) {
			top_bottom_symmetry = -1f;
		} else {
			top_bottom_symmetry = (top_squares / (bottom_squares * 1.0f));
		}

		game_time = (Time.time - startTime);
		num_traversed_squares_statues = statueArrowBottom.getNumTraversedSquares() 
			+ statueArrowTop.getNumTraversedSquares();

		if(right_squares_statues == 0) {
			left_right_symmetry_statues = -1f;
		} else {
			left_right_symmetry_statues = (left_squares_statues / (right_squares_statues * 1.0f));
		}
		if(bottom_squares_statues == 0) {
			top_bottom_symmetry_statues = -1f;
		} else {
			top_bottom_symmetry_statues = (top_squares_statues / (bottom_squares_statues * 1.0f));
		}

		/* PLAYER MOVEMENT DATA */
		resultStr +="TOTAL_MOVES," + moves+"__";
		resultStr += "TURNS," + turns +"__";
		resultStr +="AVG_TIME_PER_MOVE," + avg_time_per_move.ToString()+"__";
		resultStr +="AVG_TURNS_PER_MOVE," + avg_turns_per_move.ToString()+"__";
		resultStr +="NUM_SQUARES_TRAVERSED," + num_traversed_squares+"__";
		resultStr +="SQUARES_EXPLORED," + squares_explored_list.Count+"__";
		resultStr +="NUM_REPEATED_SQUARES," + num_repeated_squares+"__";

		/* PLAYER LOCATION DATA */
		resultStr += "LEFT_SQUARES," + left_squares + "__";
		resultStr += "RIGHT_SQUARES," + right_squares + "__";
		resultStr += "TOP_SQUARES," + top_squares + "__";
		resultStr += "BOTTOM_SQUARES," + bottom_squares + "__";
		resultStr +="LEFT_RIGHT_SYMMETRY," + left_right_symmetry +"__";
		resultStr +="TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry +"__";

		/* COLLISION DATA */
		resultStr +="PLAYER_STATUE_COLLIDE," + playerStatueCollide + "__";
		resultStr +="PLAYER_BLOCKED_BY_STATUE," + playerBlockedByStatue + "__";
		resultStr +="STATUES_BLOCK_EACH_OTHER," + statuesBlockEachOther + "__";
		resultStr +="STATUES_COLLIDE," + statuesCollide + "__";
		resultStr +="STATUE_BLOCKED_BY_OFFSCREEN," + statueBlockedByOffscreen + "__";

		/* STATUE ARROW MOVEMENT DATA */
		resultStr += "STATUE_SQUARES_TRAVERSED," + num_traversed_squares_statues + "__";
		resultStr += "STATUE_SQUARES_EXPLORED," + squares_explored_statues + "__";
		resultStr += "STATUE_SQUARES_REPEATED," + num_repeated_squares_statues + "__";

		resultStr += "STATUE_LEFT_SQUARES," + left_squares_statues + "__";
		resultStr += "STATUE_RIGHT_SQUARES," + right_squares_statues + "__";
		resultStr += "STATUE_TOP_SQUARES," + top_squares_statues + "__";
		resultStr += "STATUE_BOTTOM_SQUARES," + bottom_squares_statues + "__";
		resultStr += "STATUE_LEFT_RIGHT_SYMMETRY," + left_right_symmetry_statues + "__";
		resultStr += "STATUE_TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry_statues + "__";	

		/* COMBINED MOVEMENT DATA */
		resultStr +="ALL_MOVE," + all_move.ToString()+"__"; //redundant with collision vars?
		resultStr +="TWO_MOVE," + two_move.ToString()+"__"; //redundant with collision vars?
		resultStr +="ONE_MOVE," + player_only_moves.ToString()+"__"; //redundant with collision vars?

		resultStr +="TOTAL_TIME," + game_time+"__";

	}

	private void countLeftRightSymmetry(string newLoc) {
		if(left_squares_list.Contains (newLoc)) {
			left_squares++;
		} else if (right_squares_list.Contains (newLoc)) {
			right_squares++;
		}
	}

	private void countTopBottomSymmetry(string newLoc) {
		if(top_squares_list.Contains (newLoc)) {
			top_squares++;
		} else if (bottom_squares_list.Contains (newLoc)) {
			bottom_squares++;
		}
	}

	private void countStatueSymmetry(string newLoc) {
		if(left_squares_list.Contains (newLoc)) {
			left_squares_statues++;
		} else if (right_squares_list.Contains (newLoc)) {
			right_squares_statues++;
		}
		if(top_squares_list.Contains (newLoc)) {
			top_squares_statues++;
		} else if (bottom_squares_list.Contains (newLoc)) {
			bottom_squares_statues++;
		}
	}

	// Update is called once per frame
	void Update () {
		if(!victorious) {
			if(victory()) {
				victories++;
				logEndGameData ();
				resultStr +="VICTORY__";
				displayOptions();
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				turns++;

				turnDown ();
				statueArrowBottom.turnDown ();
				statueArrowTop.turnUp ();
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				turns++;

				turnUp ();
				statueArrowBottom.turnUp ();
				statueArrowTop.turnDown ();
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				turns++;

				turnRight ();
				statueArrowBottom.turnRight ();
				statueArrowTop.turnLeft ();
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				turns++;

				turnLeft ();
				statueArrowBottom.turnLeft ();
				statueArrowTop.turnRight ();
			} else if(Input.GetKeyDown(KeyCode.F)) {
				logMoveData ();
				bool[] errorsPlayer = getErrorType ();
				if(!errorsPlayer[0] && !errorsPlayer[1] && !errorsPlayer[2]) {
					string newLoc = move ();
					moves++;
					countLeftRightSymmetry(newLoc);
					countTopBottomSymmetry(newLoc);
					bool[] errorsBottom = statueArrowBottom.getErrorType ();
					
					if(errorsBottom[1]) {
						//if there is a statue collision
						//resultStr +=",-1,-1__";
						statuesCollide++;
						StartCoroutine (collisionHelper (SAB, SAT, SAB.transform.position, SAT.transform.position, 0.1f));
					} else if (errorsBottom[2]) {
						//if there is a statue blocking
						//resultStr +=",-1,-1__";
						statuesBlockEachOther++;
						StartCoroutine (collisionHelper (SAB, SAT, SAB.transform.position, SAT.transform.position, 0.05f));
					}else {
						bool[] errorsTop = statueArrowTop.getErrorType();
						if(!errorsBottom[0]) {
							//not offscreen
							string bottomStatuePredicted = statueArrowBottom.getPredictedSquare();
							IList<string> bottomStatue_squares_explored = statueArrowBottom.getSquaresExplored();
							IList<string> topStatue_squares_explored = statueArrowTop.getSquaresExplored();
							if(bottomStatue_squares_explored.Contains(bottomStatuePredicted) || topStatue_squares_explored.Contains(bottomStatuePredicted)) {
								num_repeated_squares_statues++;
							} else {
								squares_explored_statues++;
							}
							countStatueSymmetry(bottomStatuePredicted);
							string newLocStatue = statueArrowBottom.move();
		
							//resultStr +="," + newLocStatue+ "__";
						} else {
							// tried to move offscreen
							statueBlockedByOffscreen++;
							//resultStr +=",-1__";
						}
						if(!errorsTop[0]) {
							// not offscreen
							string topStatuePredicted = statueArrowTop.getPredictedSquare();
							IList<string> bottomStatue_squares_explored = statueArrowBottom.getSquaresExplored();
							IList<string> topStatue_squares_explored = statueArrowTop.getSquaresExplored();
							if(bottomStatue_squares_explored.Contains(topStatuePredicted) || topStatue_squares_explored.Contains(topStatuePredicted)) {
								num_repeated_squares_statues++;
							} else {
								squares_explored_statues++;
							}
							countStatueSymmetry(topStatuePredicted);
							string newLocStatue = statueArrowTop.move();
							//resultStr +="," + newLocStatue+"__";
						} else {
							// tried to move offscreen
							statueBlockedByOffscreen++;
							//resultStr +=",-1"+"__";
						}
					}
				} else if(errorsPlayer[1]) {
					//if player collides with statue (has to be top statue)
					playerStatueCollide++;
					StartCoroutine (collisionHelper(this.gameObject, SAT, transform.position, SAT.transform.position, 0.1f));
				} else if (errorsPlayer[2]) {
					//if player is blocked by a statue (could be either one)
					playerBlockedByStatue++;
					if(predictedSquare == statueArrowBottom.square) {
						StartCoroutine (collisionHelperOneSided(this.gameObject, SAB, transform.position, 0.05f));
					} else if(predictedSquare == statueArrowTop.square) {
						StartCoroutine (collisionHelperOneSided(this.gameObject, SAT, transform.position, 0.05f));
					}
				}
			} 
		}

	}
		



		
}	

