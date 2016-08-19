using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerArrowTile : MonoBehaviour {
	public int NUM_ROWS;
	public int NUM_COLS;
	public int START_COL;
	public int START_ROW;
	public int PLAYER_START_INDEX;
	public Vector3 PLAYER_START_POS;
	public Vector2 PLAYER_START_SQUARE;
	public Vector2 PLAYER_START_PREDICTED;
	private Vector3 direction;
	private Vector3 right;
	private Vector3 left;
	private Vector3 up;
	private Vector3 down;
	public Vector2 square;
	public Vector2 predictedSquare;
	private bool victorious;
	private IList<string> obstacles; // all rock positions plus places player has already explored
	public string[] initialObstacles;

	// UI for playing a new game
	private Button yes;
	private Button no;
	private Image victoryPanel;
	private Text victoryText;

	// Data collection
	string resultStr;

	private int plays;
	private int victories; // don't reset
	private int resets; // don't reset
	private int moves; 
	private float startTime;
	private float prevMoveEndTime;
	private float game_time; 
	private float session_time; // time spent in all games combined
	private float sessionStart_time;

	/* use these  to figure out which quadrant player tries first */
	public int left_squares; 
	public int right_squares; 
	public int top_squares; 
	public int bottom_squares; 

	private int num_squares_explored; 
	private float avg_time_per_move; // how long player takes to make one move, on average
	private bool[,] squares_explored;
	private float left_right_symmetry;
	private float top_bottom_symmetry;

	private string pathTrace;
	private int pathTurns;
	private int longest_straight_path;
	private float avg_turns_per_displacement; // actual turns in path made divided by length of path

	private IList<string> left_squares_list;
	private IList<string> right_squares_list;
	private IList<string> top_squares_list;
	private IList<string> bottom_squares_list;
	public GameObject[] boardSquares;

	void Awake() {
		victorious = false;
		startTime = Time.time;
		sessionStart_time = startTime;
		prevMoveEndTime = startTime;

	}

	
	// Use this for initialization
	void Start () {
		resultStr = "NEW_GAME,tile,";
		boardSquares[PLAYER_START_INDEX].GetComponent<SpriteRenderer>().color = Color.yellow;
		direction = Vector3.up;
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		plays = 1;
		victories = 0;
		resets = 0;
		moves = 0; // for one attempt (until reset)
		game_time = 0f; // for one attempt (until reset)

		num_squares_explored = 1; // one attempt
		avg_time_per_move = 0f;
		left_right_symmetry = -1f;
		top_bottom_symmetry = -1f;
		pathTrace = coordinatesToSquare(square); //starting square
		pathTurns = 0;
		longest_straight_path = 0;
		avg_turns_per_displacement = 0f;

		squares_explored = new bool[NUM_COLS,NUM_ROWS];
		squares_explored[START_COL,START_ROW] = true;

		left_squares_list = new List<string>();
		right_squares_list = new List<string>();
		top_squares_list = new List<string>();
		bottom_squares_list = new List<string>();
		obstacles = new List<string>();
		for(int i = 0; i < initialObstacles.Length; i++) {
			obstacles.Add(initialObstacles[i]);

		}
		obstacles.Add(coordinatesToSquare(square)); // player start loc

		// blockColRow
		int halfCol = NUM_COLS/2;
		int halfRow = NUM_ROWS/2;
		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				squares_explored[col,row] = false;
				if(col < halfCol) {
					left_squares_list.Add("" + col + "" + row);
				} else if (col > halfCol) {
					right_squares_list.Add("" + col + "" + row);
				}

				if(row < halfRow) {
					bottom_squares_list.Add("" + col + "" + row);
				} else if (row > halfRow) {
					top_squares_list.Add("" + col + "" + row);
				}
			}
		}

		//Victory UI variables
		yes = GameObject.Find ("Yes").GetComponent<Button>();
		no = GameObject.Find ("No").GetComponent<Button>();
		victoryPanel = GameObject.Find ("Victory").GetComponent<Image>();
		victoryText = GameObject.Find ("Congratulations").GetComponent<Text>();

	
	}

	// calculates the longest subsequence of this attempt's path trace without a turn
	// and the avg straight path length for the entire path
	private int getLongestStraightPath() {
		Debug.Log(pathTrace);
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

	private bool offScreen() {
		if(GameObject.Find ("block" + predictedSquare.x + "" + predictedSquare.y) == null) {
			//Print error message
			return true;
		}
		return false;
	}

	// returns true if player is blocked by either a rock or a space they've already visited
	private bool blockedByObstacle() {
		string squareToVisit = coordinatesToSquare(predictedSquare);
		if(obstacles.Contains(squareToVisit)) {
			return true;
		} else {
			return false;
		}
	}

	public bool canMove() {
		if(offScreen()) {
			Debug.Log("Can't Move: Offscreen! Tried to move to " + coordinatesToSquare(predictedSquare));

		} else if (blockedByObstacle()){
			Debug.Log("Can't Move: Blocked by Obstacle at " + coordinatesToSquare(predictedSquare));

		} 
		return !offScreen() && !blockedByObstacle();
	}

	public string move() {
		transform.Translate(direction * 1.25f, Space.World);
		string predictedSquareName = coordinatesToSquare(predictedSquare);
		GameObject.Find("block" + predictedSquareName).GetComponent<SpriteRenderer>().color = Color.yellow;
		//resultStr += predictedSquareName;
		Vector3 oldSquare = square; 
		square = predictedSquare; 
		Debug.Log("Moved to " + coordinatesToSquare(square));
		pathTrace += "-" + predictedSquareName;
		int col,row;
		if(predictedSquareName.Length > 2) {
			col = Convert.ToInt32(predictedSquareName.Substring(0,2));
			row = Convert.ToInt32(predictedSquareName.Substring(2,1));
		} else {
			col = Convert.ToInt32(predictedSquareName.Substring(0,1));
			row = Convert.ToInt32(predictedSquareName.Substring(1,1));
		}


		squares_explored[col,row] = true;
		obstacles.Add(predictedSquareName);

		predictedSquare.x = 2f * square.x - oldSquare.x; 
		predictedSquare.y = 2f * square.y - oldSquare.y; 
		return predictedSquareName;
	}

	// logs end game data, increments resets, and saves results to database
	// only when "Play Again? Yes" button is clicked
	public void newGame() {
		plays++;
		reset();
		resultStr += "NEW_GAME,tile,";
	}

	// when the "Reset" button is clicked
	public void buttonReset() {
		plays++;
		resets++;
		logEndGameData();
		reset();
		resultStr += "OUTCOME,RESET,NEW_ATTEMPT,tile,";
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
		Debug.Log(resultStr);

		GameObject.Find("DataCollector").GetComponent<dataCollector>().setPlayerData(resultStr);

	}

	// when the "Reset" button is clicked
	public void reset() {
		if(victorious) {
			victorious = false;
		}
		transform.position = PLAYER_START_POS;
		square = PLAYER_START_SQUARE;
		predictedSquare = PLAYER_START_PREDICTED;
		direction = Vector3.up;
		transform.rotation = Quaternion.Euler(up);

		//change all board squares back to white (except starting square)
		for(int i = 0; i < boardSquares.Length; i++) {
				boardSquares[i].GetComponent<SpriteRenderer>().color = Color.white;
		}
		boardSquares[PLAYER_START_INDEX].GetComponent<SpriteRenderer>().color = Color.yellow;

		// reset obstacles to just the rocks
		obstacles = new List<string>();
		for(int i = 0; i < initialObstacles.Length; i++) {
			obstacles.Add(initialObstacles[i]);
		}
		obstacles.Add(coordinatesToSquare(square)); // player start loc

		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				squares_explored[col,row] = false;
			}
		}

		//Data collection variables
		moves = 0; 
		startTime = Time.time;
		game_time = 0f; 

		/* use these  to figure out which quadrant player tries first */
		left_squares = 1; 
		right_squares = 0; 
		top_squares = 0; 
		bottom_squares = 1; 

		num_squares_explored = 1; 
		avg_time_per_move = 0f;
		left_right_symmetry = -1f;
		top_bottom_symmetry = -1f;
		pathTrace = coordinatesToSquare(PLAYER_START_SQUARE);
		longest_straight_path = 0;
		pathTurns = 0;

		squares_explored = new bool[NUM_COLS,NUM_ROWS];


		unDisplayOptions();
	}

	private bool approximately(Vector3 first, Vector3 second) {
		if(Mathf.Approximately(first.x, second.x) && Mathf.Approximately(first.y, second.y) && Mathf.Approximately(first.z, second.z)) {
			return true;
		}
		return false;
	}

	public bool turnDown(){
		bool turned;
		if(approximately(direction, Vector3.down)) {
			turned = false;
		} else {
			direction = Vector3.down;
			turned = true;
		}
		predictedSquare.x = square.x;
		predictedSquare.y = square.y - 1;
		return turned;
	}

	public bool turnUp(){
		bool turned;
		if(approximately(direction, Vector3.up)) {
			turned = false;
		} else {
			direction = Vector3.up;
			turned = true;
		}
		predictedSquare.x = square.x;
		predictedSquare.y = square.y + 1;
		return turned;

	}

	public bool turnLeft(){
		bool turned;
		if(approximately(direction, Vector3.left)) {
			turned = false;
		} else {
			direction = Vector3.left;
			turned = true;
		}
		predictedSquare.x = square.x - 1;
		predictedSquare.y = square.y;
		return turned;
	}

	public bool turnRight(){
		bool turned;
		if(approximately(direction, Vector3.right)) {
			turned = false;
		} else {
			direction = Vector3.right;
			turned = true;
		}
		predictedSquare.x = square.x + 1;
		predictedSquare.y = square.y;
		return turned;
	}

	private void logMoveData() {
		moves++;
		float currentTime = Time.time;
		float currentMoveTime = currentTime - prevMoveEndTime;
		avg_time_per_move += currentMoveTime;
		prevMoveEndTime = currentTime;
	}

	private bool victory() {
		if(obstacles.Count == NUM_ROWS * NUM_COLS) {
			victorious = true;
			return true;
		} else {
			return false;
		}
	}

	private void logEndGameData(){
		longest_straight_path = getLongestStraightPath(); //also calculates pathTurns value
		game_time = (Time.time - startTime);

		if(moves == 0) {
			avg_time_per_move = -1f;
			avg_turns_per_displacement = -1f;

		} else {
			avg_time_per_move = avg_time_per_move/moves;
			avg_turns_per_displacement = pathTurns / (1.0f * moves);

		}

		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				if(squares_explored[col,row]) {
					num_squares_explored++;
				}
			}
		}

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


		resultStr +="TOTAL_MOVES," + moves+",";
		resultStr +="AVG_TIME_PER_MOVE," + avg_time_per_move.ToString()+",";

		resultStr +="SQUARES_EXPLORED," + num_squares_explored+",";
		resultStr += "LEFT_SQUARES," + left_squares + ",";
		resultStr += "RIGHT_SQUARES," + right_squares + ",";
		resultStr += "TOP_SQUARES," + top_squares + ",";
		resultStr += "BOTTOM_SQUARES," + bottom_squares + ",";
		resultStr +="LEFT_RIGHT_SYMMETRY," + left_right_symmetry +",";
		resultStr +="TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry +",";

		resultStr +="PATH_TRACE," + pathTrace + ",";
		resultStr +="LONGEST_STRAIGHT_PATH," + longest_straight_path + ",";
		resultStr +="NUM_TURNS_IN_PATH," + pathTurns + ",";
		resultStr +="AVG_TURNS_PER_DISPLACEMENT," + avg_turns_per_displacement + ",";

		resultStr +="TOTAL_TIME," + game_time + ",";

	}

	private void countLeftRightSymmetry(string newLoc) {
		if(left_squares_list.Contains (newLoc)) {
			left_squares++;
		} else if (right_squares_list.Contains (newLoc)) {
			right_squares++;
		}
	}

	private void countTopBottomSymmetry(string newLoc) {
		if(bottom_squares_list.Contains (newLoc)) {
			bottom_squares++;
		} else if (top_squares_list.Contains (newLoc)) {
			top_squares++;
		}
	}


	// Update is called once per frame
	void Update () {
		if(!victorious) {
			if(victory()) {
				victories++;
				logEndGameData ();
				resultStr +="OUTCOME,VICTORY,";
				displayOptions();
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				bool turned = turnDown ();
				tryMove(turned);
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				bool turned = turnUp ();
				tryMove(turned);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				bool turned = turnRight ();
				tryMove(turned);
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				bool turned = turnLeft ();
				tryMove(turned);
			}
		}
	}

	public void tryMove(bool turned) {
		if(canMove()) {
			// player moves physically in the direction they are turned
			if(turned) {
				pathTurns++;
			}
			logMoveData();
			string newLoc = move();
			countLeftRightSymmetry(newLoc); 
			countTopBottomSymmetry(newLoc); 

		} 
	}
}
