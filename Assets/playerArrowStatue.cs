using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class playerArrowStatue : MonoBehaviour {

	private HintManager hintManager;
	private GameObject SAB;
	private GameObject SAT;
	private statueArrow statueArrowBottom; 
	private statueArrow statueArrowTop; 
	private Vector3 direction;
	private Vector3 prevMoveDir;
	private Vector3 right;
	private Vector3 left;
	private Vector3 up;
	private Vector3 down;
	private Vector2 square;
	private Vector2 predictedSquare;
	private bool victorious;
	private bool isColliding;

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
	private int pathTurns;
	private int longest_straight_path;
	private float avg_path_turns_per_move; // actual turns in path made divided by length of path
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
	private float movementTime;
	private IList<string> squares_explored_list;
	private IList<string> left_squares_list;
	private IList<string> right_squares_list;
	private IList<string> top_squares_list;
	private IList<string> bottom_squares_list;

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

	/* HINT DATA */
	private int numHints;
	private string hintType;

	private string pathTrace;

	string resultStr;

	private bool controlsEnabled;

	void Awake() {
		victorious = false;
		isColliding = false;
		startTime = Time.time;
		sessionStart_time = startTime;
		prevMoveEndTime = startTime;
		controlsEnabled = true;
	}

	// Use this for initialization
	void Start () {
		hintManager = GameObject.Find("EventSystem").GetComponent<HintManager>();
		SAB = GameObject.Find ("statueArrowBottom");
		SAT = GameObject.Find ("statueArrowTop");
		statueArrowBottom = (statueArrow)SAB.GetComponent("statueArrow");
		statueArrowTop = (statueArrow)SAT.GetComponent("statueArrow");
		direction = Vector3.up;
		prevMoveDir = direction;
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
		resultStr = "NEW_GAME,statue,";
		moves = 0;
		turns = 0;
		pathTurns = 0;
		longest_straight_path = 0;
		avg_path_turns_per_move = -1f; // actual turns in path made divided by length of path
		movementTime = 0f;
		squares_explored_list = new List<string>();
		squares_explored_list.Add ("23");
		left_squares = 0;
		right_squares = 0;
		top_squares = 0;
		bottom_squares = 1;
		squares_explored = 0;
		num_repeated_squares = 0;
		num_traversed_squares = 1;
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

		pathTrace = coordinatesToSquare(square); //starting square

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
		

	// calculates the longest subsequence of this attempt's path trace without a turn
	// and the avg straight path length for the entire path
	private int getLongestStraightPath() {
		string[] path = pathTrace.Split('-');
		int curCol, curRow, nextCol, nextRow;
		int currentColLength = 0;
		int currentRowLength = 0;
		int longestCol = 0;
		int longestRow = 0;
		for(int i = 0; i < path.Length - 1; i++) {
			if (path[i].Length > 2) { // 3 digit column number
				curCol = Convert.ToInt32(path[i].Substring(0,2));
				curRow = Convert.ToInt32(path[i].Substring(2,1));
			} else { // 2 digit column number (normal)
				curCol = Convert.ToInt32(path[i].Substring(0,1));
				curRow = Convert.ToInt32(path[i].Substring(1,1));
			}
			if (path[i+1].Length > 2) { // 3 digit column number
				nextCol = Convert.ToInt32(path[i+1].Substring(0,2));
				nextRow = Convert.ToInt32(path[i+1].Substring(2,1));
			} else { // 2 digit column number (normal)
				nextCol = Convert.ToInt32(path[i+1].Substring(0,1));
				nextRow = Convert.ToInt32(path[i+1].Substring(1,1));
			}

			// update longest column so far
			if(nextCol == curCol) {
				currentColLength++;
				if(currentColLength > longestCol) {
					longestCol = currentColLength;
				}
			} else {
				currentColLength = 1;
			}

			// update longest row so far
			if(nextRow == curRow) {
				currentRowLength++;
				if(currentRowLength > longestRow) {
					longestRow = currentRowLength;
				}
			} else {
				currentRowLength = 1;
			}
		}

		// set the longest path to the larger of the longest col and row
		if(longestCol > longestRow) {
			return longestCol;
		} else {
			return longestRow;
		}
	}

	private void displayOptions() {
		victoryPanel.enabled = true;
		victoryText.enabled = true;

		yes.GetComponent<Image>().enabled = true;
		yes.interactable = true;
		yes.transform.Find("YesText").GetComponent<Text>().enabled = true;

		no.GetComponent<Image>().enabled = true;
		no.interactable = true;
		no.transform.Find("NoText").GetComponent<Text>().enabled = true;

	}

	private void unDisplayOptions() {
		victoryPanel.enabled = false;
		victoryText.enabled = false;

		yes.GetComponent<Image>().enabled = false;
		yes.interactable = false;
		yes.transform.Find("YesText").GetComponent<Text>().enabled = false;

		no.GetComponent<Image>().enabled = false;
		no.interactable = false;
		no.transform.Find("NoText").GetComponent<Text>().enabled = false;

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
		isColliding = true;
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
			isColliding = false;
		}
	}

	public IEnumerator collisionHelperOneSided(GameObject collider1, GameObject collider2, Vector3 origCollider1pos, float step) {
		//collider1 will move towards collider2; collider2 remains stationary
		isColliding = true;
		Vector3 collider1pos = collider1.transform.position;
		Vector3 collider2pos = collider2.transform.position;
		Vector3 halfwayPoint = findHalfwayPoint(origCollider1pos, collider2pos);
		
		collider1.transform.position = Vector3.MoveTowards(collider1.transform.position, halfwayPoint, step);
		yield return new WaitForSeconds(0.01f);
		
		if(!approximately(collider1.transform.position, halfwayPoint)) {
			StartCoroutine (collisionHelperOneSided (collider1, collider2, origCollider1pos, step));
		} else {
			collider1.transform.position = origCollider1pos;
			isColliding = false;
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
		pathTrace += "-" + predictedSquareName;
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
		plays++;
		reset();
		resultStr += "NEW_GAME,statue,";
	}
		
	// when the "Reset" button is clicked
	public void buttonReset() {
		resets++;
		plays++;
		logEndGameData();
		reset();
		resultStr += "OUTCOME,RESET,NEW_ATTEMPT,statue,";

	}

	// only when "I'm done playing" button is clicked
	// (end game data has not yet been logged)
	public void buttonQuit() {
		logEndGameData();
		resultStr += "OUTCOME,QUIT,END_SESSION,done,";

		SendSaveResult();
		SceneManager.LoadScene("postgame_survey");
	}

	// only when "Play Again? No" button is clicked
	// (end game data has already been logged)
	public void saveAndQuit() {
		resultStr += "END_SESSION,no,";

		SendSaveResult();
		SceneManager.LoadScene("postgame_survey");
	}

	private void SendSaveResult()
	{	
		session_time = Time.time - sessionStart_time;
		resultStr += "ATTEMPTS," + plays + ",";
		resultStr += "RESETS," + resets + ",";
		resultStr += "VICTORIES," + victories + ",";
		resultStr += "SESSION_TIME," + session_time;
		GameObject.Find("DataCollector").GetComponent<dataCollector>().setPlayerData(resultStr);
		//Debug.Log("Sending to Data Collector: " + resultStr);

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
		prevMoveDir = direction;
		transform.rotation = Quaternion.Euler(up);

		//Data collection variables
		hintManager.reset();
		moves = 0;
		turns = 0;
		pathTurns = 0;
		avg_path_turns_per_move = -1f; // actual turns in path made divided by length of path
		startTime = Time.time;
		movementTime = 0f;
		squares_explored_list = new List<string>();
		squares_explored_list.Add ("23");
		left_squares = 0;
		right_squares = 0;
		top_squares = 0;
		bottom_squares = 1;
		squares_explored = 0;
		num_repeated_squares = 0;
		num_traversed_squares = 1;
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

		statueArrowBottom.reset ();
		statueArrowTop.reset ();

		pathTrace = coordinatesToSquare(square);

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
		movementTime += currentMoveTime;
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
		longest_straight_path = getLongestStraightPath(); 
		squares_explored = squares_explored_list.Count;
		game_time = (Time.time - startTime);
		num_traversed_squares_statues = statueArrowBottom.getNumTraversedSquares() 
			+ statueArrowTop.getNumTraversedSquares();

		float avg_time_per_move;
		float avg_turns_per_move;
		if(moves > 0) {
			avg_time_per_move = movementTime/(1.0f * moves);
			avg_turns_per_move = turns/(1.0f * moves);
			avg_path_turns_per_move = pathTurns/(1.0f * moves);

		} else {
			avg_time_per_move = -1f;
			avg_turns_per_move = -1f;
			avg_path_turns_per_move = -1f; 
		}

		float left_right_symmetry;
		float top_bottom_symmetry;
		float statue_left_right_symmetry;
		float statue_top_bottom_symmetry;
		if(right_squares > 0) {
			left_right_symmetry = left_squares/(1.0f * right_squares);
		} else {
			left_right_symmetry = -1f;
		}
		if(bottom_squares > 0) {
			top_bottom_symmetry = top_squares/(1.0f * bottom_squares);
		} else {
			top_bottom_symmetry = -1f;
		}

		if(right_squares_statues > 0) {
			statue_left_right_symmetry = left_squares_statues/(1.0f * right_squares_statues);
		} else {
			statue_left_right_symmetry = -1f;
		}
		if(bottom_squares > 0) {
			statue_top_bottom_symmetry = top_squares_statues/(1.0f * bottom_squares_statues);
		} else {
			statue_top_bottom_symmetry = -1f;
		}

		/* PLAYER MOVEMENT DATA */

		resultStr +="TOTAL_MOVES," + moves+",";
		resultStr += "TURNS," + turns +",";
		resultStr += "PATH_TURNS," + pathTurns + ",";
		resultStr +="LONGEST_STRAIGHT_PATH," + longest_straight_path + ",";
		resultStr +="AVG_PATH_TURNS_PER_MOVE," + avg_path_turns_per_move + ",";

		resultStr += "AVG_TIME_PER_MOVE," + avg_time_per_move + ",";
		resultStr += "AVG_TURNS_PER_MOVE," + avg_turns_per_move + ",";
		resultStr +="TIME_SPENT_MOVING," + movementTime.ToString()+",";
		resultStr +="NUM_SQUARES_TRAVERSED," + num_traversed_squares+",";
		resultStr +="SQUARES_EXPLORED," + squares_explored_list.Count+",";
		resultStr +="NUM_REPEATED_SQUARES," + num_repeated_squares+",";

		/* PLAYER LOCATION DATA */
		resultStr += "LEFT_SQUARES," + left_squares + ",";
		resultStr += "RIGHT_SQUARES," + right_squares + ",";
		resultStr += "TOP_SQUARES," + top_squares + ",";
		resultStr += "BOTTOM_SQUARES," + bottom_squares + ",";
		resultStr += "LEFT_RIGHT_SYMMETRY," + left_right_symmetry + ",";
		resultStr += "TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry + ",";

		/* COLLISION DATA */
		resultStr +="PLAYER_STATUE_COLLIDE," + playerStatueCollide + ","; // "error"
		resultStr +="PLAYER_BLOCKED_BY_STATUE," + playerBlockedByStatue + ","; // "error"
		resultStr +="STATUES_BLOCK_EACH_OTHER," + statuesBlockEachOther + ",";
		resultStr +="STATUES_COLLIDE," + statuesCollide + ",";
		resultStr +="STATUE_BLOCKED_BY_OFFSCREEN," + statueBlockedByOffscreen + ",";

		/* STATUE ARROW MOVEMENT DATA */
		resultStr += "STATUE_SQUARES_TRAVERSED," + num_traversed_squares_statues + ",";
		resultStr += "STATUE_SQUARES_EXPLORED," + squares_explored_statues + ",";
		resultStr += "STATUE_SQUARES_REPEATED," + num_repeated_squares_statues + ",";

		resultStr += "STATUE_LEFT_SQUARES," + left_squares_statues + ",";
		resultStr += "STATUE_RIGHT_SQUARES," + right_squares_statues + ",";
		resultStr += "STATUE_TOP_SQUARES," + top_squares_statues + ",";
		resultStr += "STATUE_BOTTOM_SQUARES," + bottom_squares_statues + ",";	
		resultStr += "STATUE_LEFT_RIGHT_SYMMETRY," + statue_left_right_symmetry + ",";
		resultStr += "STATUE_TOP_BOTTOM_SYMMETRY," + statue_top_bottom_symmetry + ",";

		/* COMBINED MOVEMENT DATA */
		resultStr +="ALL_MOVE," + all_move.ToString()+","; //redundant with collision vars?
		resultStr +="TWO_MOVE," + two_move.ToString()+","; //redundant with collision vars?
		resultStr +="ONE_MOVE," + player_only_moves.ToString()+","; //redundant with collision vars?

		resultStr+="NUM_HINTS," + hintManager.getNumHints() + ",";
		resultStr+="HINT_TYPE," + hintManager.getHintType() + ",";

		resultStr +="TOTAL_TIME," + game_time + ",";
		resultStr +="PATH_TRACE," + pathTrace + ",";

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
		if(!victorious && !isColliding) {
			if(victory()) {
				victories++;
				logEndGameData ();
				resultStr +="OUTCOME,VICTORY,";
				displayOptions();
			} else if (controlsEnabled && Input.GetKeyDown (KeyCode.DownArrow)) {
				if(approximately(direction, Vector3.down)) {
					// move down
					tryMove();
				} else {
					// turn down
					turns++;
					turnDown ();
					statueArrowBottom.turnDown ();
					statueArrowTop.turnUp ();
				}

			} else if (controlsEnabled && Input.GetKeyDown (KeyCode.UpArrow)) {
				if(approximately(direction, Vector3.up)) {
					// move up
					tryMove();
				} else {
					turns++;
					turnUp ();
					statueArrowBottom.turnUp ();
					statueArrowTop.turnDown ();
				}
			} else if (controlsEnabled && Input.GetKeyDown (KeyCode.RightArrow)) {
				if(approximately(direction, Vector3.right)) {
					// move right
					tryMove();
				} else {
					turns++;
					turnRight ();
					statueArrowBottom.turnRight ();
					statueArrowTop.turnLeft ();
				}
			} else if (controlsEnabled && Input.GetKeyDown (KeyCode.LeftArrow)) {
				if(approximately(direction, Vector3.left)) {
					// move left
					tryMove();
				} else {
					turns++;
					turnLeft ();
					statueArrowBottom.turnLeft ();
					statueArrowTop.turnRight ();
				}
			} 
		}

	}
		
	public void enableControls() {
		controlsEnabled = true;
	}

	public void disableControls() {
		controlsEnabled = false;
	}

	private void tryMove() {
		logMoveData ();
		bool[] errorsPlayer = getErrorType ();
		if(!errorsPlayer[0] && !errorsPlayer[1] && !errorsPlayer[2]) {
			if(!approximately(prevMoveDir, direction)) {
				pathTurns++;
				prevMoveDir = direction;
			}
			string newLoc = move ();
			moves++;
			countLeftRightSymmetry(newLoc);
			countTopBottomSymmetry(newLoc);
			bool[] errorsBottom = statueArrowBottom.getErrorType ();

			if(errorsBottom[1]) {
				//if there is a statue collision
				statuesCollide++;
				StartCoroutine (collisionHelper (SAB, SAT, SAB.transform.position, SAT.transform.position, 0.1f));
			} else if (errorsBottom[2]) {
				//if there is a statue blocking
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

				} else {
					// tried to move offscreen
					statueBlockedByOffscreen++;
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
				} else {
					// tried to move offscreen
					statueBlockedByOffscreen++;
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

