using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour {
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

	private int victories;
	private int resets;
	private string lastOutcome;
	private int moves; // for one attempt (until reset)
	private float startTime;
	private float prevMoveEndTime;
	private int turns; // for one attempt (until reset)
	private float game_time; // across entire game

	/* use these  to figure out which quadrant player tries first */
	private int left_squares; // one attempt
	private int right_squares; // one attempt
	private int top_squares; // one attempt
	private int bottom_squares; // one attempt

	private int num_squares_explored; // one attempt
	private float avg_time_per_move;
	private float avg_turns_per_move;
	private int[,] squares_explored;
	private float left_right_symmetry;
	private float top_bottom_symmetry;
	private IList<string> left_squares_list;
	private IList<string> right_squares_list;
	private IList<string> top_squares_list;
	private IList<string> bottom_squares_list;


	// any others specific to this game??


	void Awake() {
		victorious = false;
		startTime = Time.time;
		prevMoveEndTime = startTime;

	}

	
	// Use this for initialization
	void Start () {
		direction = new Vector3(0,0,0);
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		square = new Vector2(1,0);
		predictedSquare = new Vector2(1,1);

		victories = 0;
		resets = 0;
		lastOutcome = "QUIT";
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

		squares_explored = new int[NUM_ROWS,NUM_COLS];
		squares_explored[1,0] = 1;

		left_squares_list = new List<string>();
		right_squares_list = new List<string>();
		top_squares_list = new List<string>();
		bottom_squares_list = new List<string>();

		// blockColRow
		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				squares_explored[col,row] = 0;
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
		//TODO
		return false;
	}

	public bool canMove() {
		return !offScreen() && !blockedByObstacle();
	}

	public string move() {
		transform.Translate(direction * 1.25f, Space.World);
		string predictedSquareName = coordinatesToSquare(predictedSquare);
		//resultStr += predictedSquareName;
		Vector3 oldSquare = square; 
		square = predictedSquare; 
		string newLoc = coordinatesToSquare(predictedSquare);
		int col = Convert.ToInt32(newLoc.Substring(0,1));
		int row = Convert.ToInt32(newLoc.Substring(1,1));
		squares_explored[col,row]++;
		predictedSquare.x = 1.25f * square.x - oldSquare.x; 
		predictedSquare.y = 1.25f * square.y - oldSquare.y; 
		return predictedSquareName;
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

		//Data collection variables
		victories = 0;
		resets = 0;
		lastOutcome = "QUIT";
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
		//TODO
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

	public void saveAndReset() {
		logEndGameData();
		//resultStr += "RESET__";
		resets++;
		SendSaveResult();
		reset();
	}

	public void saveAndQuit() {
		logEndGameData();
		resultStr +="QUIT__";
		SendSaveResult();
		SceneManager.LoadScene("postgame_survey");
	}

	//player clicks "No" on the "Do you want to play again?" question
	public void quit() {
		SceneManager.LoadScene("postgame_survey");
	}

	private void SendSaveResult()
	{
		GameObject.Find("DataCollector").GetComponent<dataCollector>().setPlayerData(resultStr);

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
			} else if(Input.GetMouseButtonDown(0)) {
				logMoveData ();
				if(!canMove()) {
					// player moves physically in the direction they are turned
					logMoveData();
					string newLoc = move();
					moves++;
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
