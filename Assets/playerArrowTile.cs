using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerArrowTile : MonoBehaviour {
	private const int NUM_ROWS = 7;
	private const int NUM_COLS = 9;
	private Vector3 direction;
	private Vector3 right;
	private Vector3 left;
	private Vector3 up;
	private Vector3 down;
	private Vector2 square;
	private Vector2 predictedSquare;
	private bool victorious;

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
	private int turns; 
	private float game_time; 

	/* use these  to figure out which quadrant player tries first */
	private int left_squares; 
	private int right_squares; 
	private int top_squares; 
	private int bottom_squares; 

	private int num_squares_explored; 
	private float avg_time_per_move; // how long player takes to make one move, on average
	private float avg_turns_per_move; // how many times player tries different turns before committing to a move
	private int[,] squares_explored;
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
		prevMoveEndTime = startTime;

	}

	
	// Use this for initialization
	void Start () {
		boardSquares[7].GetComponent<SpriteRenderer>().color = Color.yellow;
		direction = new Vector3(0,0,0);
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		square = new Vector2(1,0);
		predictedSquare = new Vector2(1,1);

		plays = 1;
		victories = 0;
		resets = 0;
		moves = 0; // for one attempt (until reset)
		turns = 0; // for one attempt (until reset)
		game_time = 0f; // for one attempt (until reset)

		/* use these  to figure out which quadrant player tries first */
		left_squares = 1; // one attempt
		right_squares = 0; // one attempt
		top_squares = 0; // one attempt
		bottom_squares = 1; // one attempt

		num_squares_explored = 1; // one attempt
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;
		left_right_symmetry = -1f;
		top_bottom_symmetry = -1f;
		pathTrace = "23";
		pathTurns = 0;
		longest_straight_path = 0;
		avg_turns_per_displacement = 0f;

		squares_explored = new int[NUM_ROWS,NUM_COLS];
		squares_explored[1,0] = 1;

		left_squares_list = new List<string>();
		right_squares_list = new List<string>();
		top_squares_list = new List<string>();
		bottom_squares_list = new List<string>();

		// blockColRow
		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				squares_explored[row,col] = 0;
				if(col < 4) {
					left_squares_list.Add("" + col + "" + row);
				} else if (col > 4) {
					right_squares_list.Add("" + col + "" + row);
				}

				if(row < 3) {
					bottom_squares_list.Add("" + col + "" + row);
				} else if (row > 3) {
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
		int curCol = 0;
		int curRow = 0;
		int nextCol = 0;
		int nextRow = 0;
		int longestCol = 1;
		int longestRow = 1;
		int currentColLength = 1;
		int currentRowLength = 1;
		for(int i = 0; i <= pathTrace.Length-4; i+=3) {
			curCol = pathTrace[i];
			curRow = pathTrace[i+1];
			nextCol = pathTrace[i+3];
			nextRow = pathTrace[i+4];

			// update longest column so far
			if(nextCol == curCol) {
				currentColLength++;
				if(currentColLength > longestCol) {
					longestCol = currentColLength;
				}
			} else {
				currentColLength = 1;
				pathTurns++;
			}

			// update longest row so far
			if(nextRow == curRow) {
				currentRowLength++;
				if(currentRowLength > longestRow) {
					longestRow = currentRowLength;
				}
			} else {
				currentRowLength = 1;
				pathTurns++;
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

	private bool blockedByObstacle() {
		string squareToVisit = coordinatesToSquare(predictedSquare);
		if(squareToVisit.Equals("26") || squareToVisit.Equals("25") || squareToVisit.Equals("75")) {
			return true;
		} else if(squareToVisit.Equals("13") || squareToVisit.Equals("83") || squareToVisit.Equals("32")) {
			return true;
		} else if (squareToVisit.Equals("71") || squareToVisit.Equals("20")){
			return true;
		}
		return false;
	}

	public bool canMove() {
		return !offScreen() && !blockedByObstacle();
	}

	public string move() {
		transform.Translate(direction * 1.25f, Space.World);
		string predictedSquareName = coordinatesToSquare(predictedSquare);
		GameObject.Find("block" + predictedSquareName).GetComponent<SpriteRenderer>().color = Color.yellow;
		//resultStr += predictedSquareName;
		Vector3 oldSquare = square; 
		square = predictedSquare; 
		string newLoc = coordinatesToSquare(predictedSquare);
		pathTrace += "-" + newLoc;
		int col = Convert.ToInt32(newLoc.Substring(0,1));
		int row = Convert.ToInt32(newLoc.Substring(1,1));
		squares_explored[col,row]++;
		predictedSquare.x = 1.25f * square.x - oldSquare.x; 
		predictedSquare.y = 1.25f * square.y - oldSquare.y; 
		return predictedSquareName;
	}

	// logs end game data, increments resets, and saves results to database
	// only when "Play Again? Yes" button is clicked
	public void newGame() {
		//resultStr += "RESET__";
		plays++;
		logEndGameData();
		reset();
		resultStr += "\nNEW_GAME,statue__";
	}

	// when the "Reset" button is clicked
	public void buttonReset() {
		plays++;
		resets++;
		logEndGameData();
		reset();
		resultStr += "\nNEW_ATTEMPT,statue__";
	}

	// only when "I'm done playing" button is clicked
	// (end game data has not yet been logged)
	public void buttonQuit() {
		logEndGameData();
		SendSaveResult();
		SceneManager.LoadScene("postgame_survey");
	}

	// only when "Play Again? No" button is clicked
	// (end game data has already been logged)
	public void saveAndQuit() {
		//resultStr +="QUIT__";
		SendSaveResult();
		SceneManager.LoadScene("postgame_survey");
	}

	private void SendSaveResult()
	{
		resultStr += "ATTEMPTS," + plays + "__";
		resultStr += "RESETS," + resets + "__";
		resultStr += "VICTORIES," + victories + "__";
		GameObject.Find("DataCollector").GetComponent<dataCollector>().setPlayerData(resultStr);
		Debug.Log(resultStr);

	}

	// when the "Reset" button is clicked
	public void reset() {
		if(victorious) {
			victorious = false;
		}
		transform.position = new Vector3(-4.75f,-4,0);
		square = new Vector2(1,0);
		predictedSquare = new Vector2(1,1);
		direction = Vector3.up;
		transform.rotation = Quaternion.Euler(up);
		boardSquares[7].GetComponent<SpriteRenderer>().color = Color.yellow;

		//change all board squares back to white

		for(int i = 0; i < boardSquares.Length; i++) {
				boardSquares[i].GetComponent<SpriteRenderer>().color = Color.white;
		}

		//Data collection variables
		moves = 0; 
		turns = 0; 
		game_time = 0f; 

		/* use these  to figure out which quadrant player tries first */
		left_squares = 1; 
		right_squares = 0; 
		top_squares = 0; 
		bottom_squares = 1; 

		num_squares_explored = 1; 
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;
		left_right_symmetry = -1f;
		top_bottom_symmetry = -1f;
		pathTrace = "23";
		longest_straight_path = 0;
		pathTurns = 0;

		squares_explored = new int[NUM_ROWS,NUM_COLS];


		unDisplayOptions();
	}

	public void turnDown(){
		direction = Vector3.down;
		transform.rotation = Quaternion.Euler(down);
		predictedSquare.x = square.x;
		predictedSquare.y = square.y - 1;
	}

	public void turnUp(){
		direction = Vector3.up;
		transform.rotation = Quaternion.Euler(up);
		predictedSquare.x = square.x;
		predictedSquare.y = square.y + 1;

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
		moves++;
		float currentTime = Time.time;
		float currentMoveTime = currentTime - prevMoveEndTime;
		avg_time_per_move += currentMoveTime;
		prevMoveEndTime = currentTime;
	}

	private bool victory() {
		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				if(squares_explored[col,row] == 0) {
					return false;
				}
			}
		}
		return true;
	}

	private void logEndGameData(){
		avg_time_per_move = avg_time_per_move/moves;
		avg_turns_per_move = turns/(moves * 1.0f);
		resultStr +="TOTAL_MOVES," + moves+"__";
		resultStr += "TURNS," + turns +"__";
		resultStr +="AVG_TIME_PER_MOVE," + avg_time_per_move.ToString()+"__";
		resultStr +="AVG_TURNS_PER_MOVE," + avg_turns_per_move.ToString()+"__";

		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				num_squares_explored += squares_explored[row,col];
			}
		}
		resultStr +="SQUARES_EXPLORED," + num_squares_explored+"__";
		resultStr += "LEFT_SQUARES," + left_squares + "__";
		resultStr += "RIGHT_SQUARES," + right_squares + "__";
		resultStr += "TOP_SQUARES," + top_squares + "__";
		resultStr += "BOTTOM_SQUARES," + bottom_squares + "__";
		left_right_symmetry = (left_squares / (right_squares * 1.0f));
		resultStr +="LEFT_RIGHT_SYMMETRY," + left_right_symmetry +"__";
		top_bottom_symmetry = (top_squares / (bottom_squares * 1.0f));
		resultStr +="TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry +"__";
		resultStr +="PATH_TRACE," + pathTrace + "__";
		longest_straight_path = getLongestStraightPath();
		resultStr +="LONGEST_STRAIGHT_PATH," + longest_straight_path + "__";
		avg_turns_per_displacement = pathTurns / (1.0f * moves);
		resultStr +="AVG_TURNS_PER_DISPLACEMENT," + pathTurns + "__";

		game_time = (Time.time - startTime);
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
		if(bottom_squares_list.Contains (newLoc)) {
			bottom_squares++;
		} else if (top_squares_list.Contains (newLoc)) {
			top_squares++;
		}
	}


	// Update is called once per frame
	void Update () {
		if(!victorious) {
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				turns++;
				turnDown ();
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				turns++;
				turnUp ();
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				turns++;				
				turnRight ();
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				turns++;
				turnLeft ();
			} else if(Input.GetKeyDown(KeyCode.F)) {
				if(canMove()) {
					// player moves physically in the direction they are turned
					logMoveData();
					string newLoc = move();
					countLeftRightSymmetry(newLoc); 
					countTopBottomSymmetry(newLoc); 

				} 
			} else if(victory()) {
				logEndGameData ();
				//resultStr +="VICTORY__";
				victories++;
				SendSaveResult();
				//change later to allow player to play a new game
				displayOptions();
			}
		}
	}
}
