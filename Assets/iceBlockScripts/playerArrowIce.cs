using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerArrowIce : MonoBehaviour {

    public float boardScalingFactor = 1;
	public GameObject timer;
	public int NUM_ROWS;
	public int NUM_COLS;
	public iceBlock[] ices;
	public List<string> victorySquare;
	private List<string> victorySquare_tmp;
	public Vector3 startPosition;
	private bool timedOut = false;

	private Vector3 direction;
	private Vector3 prevMoveDir;

	private Vector3 right;
	private Vector3 left;
	private Vector3 up;
	private Vector3 down;
	public Vector2 square;
	private Vector2 square_store;
	public Vector2 predictedSquare;
	private Vector2 predictedSquare_store;
	private bool victorious;

	//UI for playing a new game
	private Button yes;
	private Button no;
	private Image victoryPanel;
	private Text victoryText;

	/* DATA COLLECTION */
	private int plays;
	private int victories;
	private int resets;
    private DataCollector dataCollector; 

	/* MOVEMENT DATA */
	private int actions; //number of times player either moves or pushes
	private int moves; 	//number of times player moves from one square to another
	private int pushes; 	//number of times player attempts to push an ice block
	private int successfulPushes; // number of times player pushes ice block and it moves
	private int turns;	//number of times player changes direction
	private int pathTurns; //number of times player's path actually turns (pathTurns <= turns)
	private int longest_straight_path;
	private float avg_turns_per_move;
	private float avg_turns_per_action;
	private float avg_turns_per_push;
	private float avg_path_turns_per_move;

	/* ICE BLOCK "ERROR" DATA */
	private int iceCantMove; 	// number of times player tries to push ice block but something is in the way
	private int iceBlockedByIce; // ice block doesn't move due to presence of other ice block
	private int iceBlockedByOffscreen; // ice block doesn't move due to it being on edge of screen
	private int iceStoppedByIce; // ice block slides and then hits another ice block
	private int iceStoppedByOffscreen; // ice block slides and then hits the edge of the area

	/* TIME DATA */
	private float startTime;
	private float prevActionEndTime;
	private float prevPushEndTime;
	private float prevMoveEndTime;
	private float avg_time_per_action;
	private float avg_time_per_push;
	private float avg_time_per_move;
	private float game_time;
	private float session_time; // time spent in all games combined
	private float sessionStart_time;

    /* PLAYER LOCATION DATA */
    /*
	private int left_squares_player;
	private int right_squares_player;
	private int top_squares_player;
	private int bottom_squares_player;
	private int num_repeated_squares_player; // number of times player steps on square they've already been to
	private int num_traversed_squares_player; // total displacement of player
	private int squares_explored_player; // squares player has moved onto
	private float left_right_symmetry_player;
	private float top_bottom_symmetry_player;
	public List<string> squares_explored_player_list;
	private List<string> squares_explored_player_list_store;
	*/

    private Sprite upSprite;
    private Sprite downSprite;
    private Sprite leftSprite;
    private Sprite rightSprite;
    private SpriteRenderer spriteRenderer;

    private string pathTrace;

    public AudioClip musicClip;
    public AudioSource vicJingle;
    //Scene currentScene;
    public static string sceneName;
	//write to database
	string resultStr;

	void Awake() {
		victorious = false;
		startTime = Time.time;
		sessionStart_time = startTime;
		prevActionEndTime = startTime;
		prevMoveEndTime = startTime;
		prevPushEndTime = startTime;
	}

	// Use this for initialization
	void Start () {
        vicJingle.clip = musicClip;

        //resultStr = "NEW_GAME,ice,";
        GameObject collectorObj = GameObject.Find("DataCollector");
        if (collectorObj != null)
        {
            dataCollector = collectorObj.GetComponent<DataCollector>();
        }

        upSprite = Resources.Load<Sprite>("player_astronaut/player-up");
        downSprite = Resources.Load<Sprite>("player_astronaut/player-down");
        leftSprite = Resources.Load<Sprite>("player_astronaut/player-left");
        rightSprite = Resources.Load<Sprite>("player_astronaut/player-right");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        direction = Vector3.right;
		prevMoveDir = direction;
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		//square = new Vector2(5,2);
		//predictedSquare = new Vector2(5,3); //player faces right to start

		square_store = new Vector2(square.x, square.y);
		predictedSquare_store = new Vector2(predictedSquare.x, predictedSquare.y);

		victorySquare_tmp = new List<string>();

		foreach(string s in victorySquare){
			victorySquare_tmp.Add(s);
            print("Added " + s + " to victorySquare_tmp");
		}

		//Data collection variables
		plays = 1;
		victories = 0;
		resets = 0;
		actions = 0;
		moves = 0;
		turns = 0;
		pathTurns = 0;
		longest_straight_path = 0;
		pushes = 0;
		successfulPushes = 0;
		avg_time_per_move = -1f;
		avg_turns_per_move = -1f;
		avg_turns_per_action = -1f;
		avg_turns_per_push = -1f;
		avg_path_turns_per_move = -1f;

		iceCantMove = 0;
		iceBlockedByIce = 0;
		iceBlockedByOffscreen = 0;
		iceStoppedByIce = 0;
		iceStoppedByOffscreen = 0;

		/*
		squares_explored_player_list = new List<string>();
		squares_explored_player_list.Add ("52");

		squares_explored_ice_list = new List<string>();
		squares_explored_ice_list.Add ("22");
		squares_explored_ice_list.Add ("42");
		squares_explored_ice_list.Add ("26");
		*/

		/*
		squares_explored_player_list_store = new List<string>();
		foreach(string s in squares_explored_player_list){
			squares_explored_player_list_store.Add(s);
		}

		squares_explored_ice_list_store = new List<string>();
		foreach(string s in squares_explored_ice_list){
			squares_explored_ice_list_store.Add(s);
		}

		left_squares_player = 1;
		right_squares_player = 0;
		top_squares_player = 0;
		bottom_squares_player = 1;
		squares_explored_player = 1;
		num_repeated_squares_player = 0;
		num_traversed_squares_player = 1;

		left_squares_ice = 0; // because it will be calculated at end
		right_squares_ice = 0; // because it will be calculated at end
		top_squares_ice = 0; // because it will be calculated at end
		bottom_squares_ice = 0; // because it will be calculated at end
		squares_explored_ice = 0; // because it will be calculated at end
		num_repeated_squares_ice = 0; // because it will be calculated at end
		num_traversed_squares_ice = 0; // because it will be calculated at end
		*/

		pathTrace = coordinatesToSquare(square); //starting square

		/*
		left_squares_list = new List<string>();
		left_squares_list.Add ("11");
		left_squares_list.Add ("12");
		left_squares_list.Add ("13");
		left_squares_list.Add ("21");
		left_squares_list.Add ("22");
		left_squares_list.Add ("23");
		left_squares_list.Add ("31");
		left_squares_list.Add ("32");
		left_squares_list.Add ("33");
		left_squares_list.Add ("41");
		left_squares_list.Add ("42");
		left_squares_list.Add ("43");
		left_squares_list.Add ("51");
		left_squares_list.Add ("52");
		left_squares_list.Add ("53");

		right_squares_list = new List<string>();
		right_squares_list.Add ("15");
		right_squares_list.Add ("16");
		right_squares_list.Add ("17");
		right_squares_list.Add ("25");
		right_squares_list.Add ("26");
		right_squares_list.Add ("27");
		right_squares_list.Add ("35");
		right_squares_list.Add ("36");
		right_squares_list.Add ("37");
		right_squares_list.Add ("45");
		right_squares_list.Add ("46");
		right_squares_list.Add ("47");
		right_squares_list.Add ("55");
		right_squares_list.Add ("56");
		right_squares_list.Add ("57");

		bottom_squares_list = new List<string>();
		bottom_squares_list.Add ("41");
		bottom_squares_list.Add ("42");
		bottom_squares_list.Add ("43");
		bottom_squares_list.Add ("44");
		bottom_squares_list.Add ("45");
		bottom_squares_list.Add ("46");
		bottom_squares_list.Add ("47");
		bottom_squares_list.Add ("51");
		bottom_squares_list.Add ("52");
		bottom_squares_list.Add ("53");
		bottom_squares_list.Add ("54");
		bottom_squares_list.Add ("55");
		bottom_squares_list.Add ("56");
		bottom_squares_list.Add ("57");

		top_squares_list = new List<string>();
		top_squares_list.Add ("11");
		top_squares_list.Add ("12");
		top_squares_list.Add ("13");
		top_squares_list.Add ("14");
		top_squares_list.Add ("15");
		top_squares_list.Add ("16");
		top_squares_list.Add ("17");
		top_squares_list.Add ("21");
		top_squares_list.Add ("22");
		top_squares_list.Add ("23");
		top_squares_list.Add ("24");
		top_squares_list.Add ("25");
		top_squares_list.Add ("26");
		top_squares_list.Add ("27");


		left_right_symmetry_player = -1f;
		top_bottom_symmetry_player = -1f;
		left_right_symmetry_ice = -1f;
		top_bottom_symmetry_ice = -1f;
		*/

		//Victory UI variables
		//yes = GameObject.Find ("Yes").GetComponent<Button>();
		no = GameObject.Find ("No").GetComponent<Button>();
		victoryPanel = GameObject.Find ("Victory").GetComponent<Image>();
		victoryText = GameObject.Find ("Congratulations").GetComponent<Text>();

	}

	public int getNumMoves() {
		return moves;
	}

	public Vector3 getDirection() {
		return direction;
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

		//yes.GetComponent<Image>().enabled = true;
		//yes.interactable = true;
		//yes.transform.Find("YesText").GetComponent<Text>().enabled = true;

		no.GetComponent<Image>().enabled = true;
		no.interactable = true;
		no.transform.Find("NoText").GetComponent<Text>().enabled = true;

		if(timer != null){
			timer.GetComponent<Timer>().SetVictory();
		}
	}

	private void unDisplayOptions() {
		victoryPanel.enabled = false;
		victoryText.enabled = false;

		//yes.GetComponent<Image>().enabled = false;
		//yes.interactable = false;
		//yes.transform.Find("YesText").GetComponent<Text>().enabled = false;

		no.GetComponent<Image>().enabled = false;
		no.interactable = false;
		no.transform.Find("NoText").GetComponent<Text>().enabled = false;

	}

	private string coordinatesToSquare(Vector2 coordinates) {
		return coordinates.x.ToString () + coordinates.y.ToString ();
	}

	private bool approximately(Vector3 first, Vector3 second) {
		if(Mathf.Approximately(first.x, second.x) && Mathf.Approximately(first.y, second.y) && Mathf.Approximately(first.z, second.z)) {
			return true;
		}
		return false;
	}

	private bool offScreen() {
		if(GameObject.Find ("Block" + coordinatesToSquare(predictedSquare)) == null) {
			// can't move - offscreen
			return true;
		}
		return false;
	}

	// index 0 is 1 if player blocked by offscreen
	// index 1 is 1 if player blocked by ice
	public bool[] getErrorType() {
		bool[] errors = new bool[2];
		for(int i = 0; i < 2; i++) {
			errors[i] = false;
		}
		if(offScreen ()) {
			errors[0] = true;

		} else if (blockedByIce()) {
			errors[1] = true;
		}
		return errors;
	}

	public string move() {
        if (dataCollector != null)
        {
            dataCollector.AddMove();
        } else
        {
            Debug.Log("Warning: DataCollector not found in scene.");
        }

        transform.Translate(direction * 2f * boardScalingFactor, Space.World);
		//num_traversed_squares_player++;
		string predictedSquareName = coordinatesToSquare(predictedSquare);
        Debug.Log("MOVED TO " + predictedSquareName);

		pathTrace += "-" + predictedSquareName;

		Vector3 oldSquare = square;
		square = predictedSquare;
		/*
		if(!squares_explored_player_list.Contains(predictedSquareName)) {
			squares_explored_player_list.Add(predictedSquareName);
		} else {
			num_repeated_squares_player++;
		}
		*/
		predictedSquare.x = 2f * square.x - oldSquare.x;
		predictedSquare.y = 2f * square.y - oldSquare.y;
		return predictedSquareName;

	}

    // TODO: change all these to change the sprite rather than rotate it
	public void turnDown(){
		direction = Vector3.down;
		//transform.rotation = Quaternion.Euler(down); 
		predictedSquare.x = square.x + 1;
		predictedSquare.y = square.y;
        spriteRenderer.sprite = downSprite; 

    }

    public void turnUp(){
		direction = Vector3.up;
		// transform.rotation = Quaternion.Euler(up);
		predictedSquare.x = square.x - 1;
		predictedSquare.y = square.y;
        spriteRenderer.sprite = upSprite;

    }

    public void turnLeft(){
		direction = Vector3.left;
		//transform.rotation = Quaternion.Euler(left);
		predictedSquare.x = square.x;
		predictedSquare.y = square.y - 1;
        spriteRenderer.sprite = leftSprite;

    }

    public void turnRight(){
		direction = Vector3.right;
		//transform.rotation = Quaternion.Euler(right);
		predictedSquare.x = square.x;
		predictedSquare.y = square.y + 1;
        spriteRenderer.sprite = rightSprite;

    }

    // logs end game data, increments resets, and saves results to database
    // only when "Play Again? Yes" button is clicked
    // TODO - get rid of this. Replace with victory message saying you completed the puzzle and got the key fragment, load start room
    public void newGame() {
		plays++;
		reset();
		if(timer != null){
			timer.GetComponent<Timer>().ResetTimer();
		}
		//resultStr += "NEW_GAME,ice,";
	}

	// when the "Reset" button is clicked
	public void buttonReset() {
        if (dataCollector != null)
        {
            dataCollector.setOutcome("reset");
            dataCollector.AddNewAttempt(SceneManager.GetActiveScene().name);
        } else
        {
            Debug.Log("Warning: DataCollector not found in scene.");
        }
        resets++;
		plays++;
		logEndGameData();
		reset();
		//resultStr += "OUTCOME,RESET,NEW_ATTEMPT,ice,";
	}

	// only when "Back to Main room" button is clicked
	// (end game data has not yet been logged)
    // this ends the whole game and takes player to next part of pilot study
    // TODO - this should only happen when player presses C key - get rid of this button in each level
	public void buttonQuit() {
		logEndGameData();

        // For testing purposes - TODO will need to make these happen when player presses C key instead in future
        if (dataCollector != null)
        {
            dataCollector.saveAllData();
        } else
        {
            Debug.Log("Warning: DataCollector not found in scene.");
        }
        SceneManager.LoadScene("start room");
       }

    public void buttonWin()
    {
        logEndGameData();

        if (dataCollector != null)
        {
            dataCollector.saveAllData();
        }
        else
        {
            Debug.Log("Warning: DataCollector not found in scene.");
        }
        sceneName = SceneManager.GetActiveScene().name; // get the current scene's name so that 
        // we can use it in the start room for closing the door to this scene
        SceneManager.LoadScene("start room");

    }

    //called by trigger event when player steps on the stairs/door tile
    public void leaveRoom()
    {
        if (dataCollector != null)
        {
            dataCollector.setOutcome("left");
        } else
        {
            Debug.Log("Warning: DataCollector not found in scene.");
        }
        SceneManager.LoadScene("start room");

    }

	public void reset() {
		transform.position = startPosition;
		square = new Vector2(square_store.x, square_store.y);
		predictedSquare = new Vector2(predictedSquare_store.x, predictedSquare_store.y);
        /**direction = Vector3.right;
		prevMoveDir = direction;
		transform.rotation = Quaternion.Euler(right);**/ 
        direction = Vector3.up;
        transform.rotation = Quaternion.Euler(up);

        //Data collection variables
        actions = 0;
		moves = 0;
		turns = 0;
		pathTurns = 0;
		longest_straight_path = 0;
		pushes = 0;
		successfulPushes = 0;
		avg_time_per_move = -1f;
		startTime = Time.time;
		avg_turns_per_move = -1f;
		avg_turns_per_action = -1f;
		avg_turns_per_push = -1f;
		avg_path_turns_per_move = -1f;

		iceCantMove = 0;
		iceBlockedByIce = 0;
		iceBlockedByOffscreen = 0;
		iceStoppedByIce = 0;
		iceStoppedByOffscreen = 0;

		pathTrace = coordinatesToSquare(square);

		foreach(iceBlock i in ices) {
			i.reset();
		}

		unDisplayOptions();

	}

    // this should only be checked after an ice block's location changes, not during every update
	private bool victory() {

		foreach(string s in victorySquare_tmp){
			foreach(iceBlock i in ices){
				if(coordinatesToSquare(i.square).Equals (s)){
					victorySquare.Remove(s);
				}
			}
		}

		if(victorySquare.Count == 0){
			victorious = true;
			victorySquare.Clear();
			foreach(string s in victorySquare_tmp){
				victorySquare.Add(s);
			}
			return true;
		}else{
			victorySquare.Clear();
			foreach(string s in victorySquare_tmp){
				victorySquare.Add(s);
			}
			return false;
		}

	}

	// add square to count only once
	// create new array that is union of all 3 ice arrays
	private int iceNumSquaresExplored() {

		//create the union array to count each square only once
		/*
		int[,] ice1Squares = ices[0].getSquaresExplored();
		int[,] ice2Squares = ices[1].getSquaresExplored();
		int[,] ice3Squares = ices[2].getSquaresExplored();
		*/

		int[][,] iceSquares = new int[ices.Length][,];
		for(int i = 0; i < ices.Length; i++){
			iceSquares[i] = ices[i].getSquaresExplored();
		}

		int[,] union = new int[NUM_ROWS,NUM_COLS];
		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				//union[row,col] = ice1Squares[row,col] + ice2Squares[row,col] + ice3Squares[row,col];

				for(int i = 0; i < iceSquares.Length; i++){
					union[row, col] += iceSquares[i][row,col];
				}

				if(union[row,col] > 0) {
					union[row,col] = 1;
				}
			}
		}

		//actually count the number of unique squares explored
		int count = 0;
		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				count += union[row,col];
			}
		}
		return count;
	}

	private void logActionData() {
		actions++;
		float currentTime = Time.time;
		float currentActionTime = currentTime - prevActionEndTime;
		avg_time_per_action += currentActionTime;
		prevActionEndTime = currentTime;
	}

	private void logMoveData() {
		moves++;
		float currentTime = Time.time;
		float currentMoveTime = currentTime - prevMoveEndTime;
		avg_time_per_move += currentMoveTime;
		prevMoveEndTime = currentTime;
	}

	private void logPushData() {
		pushes++;
		float currentTime = Time.time;
		float currentPushTime = currentTime - prevPushEndTime;
		avg_time_per_move += currentPushTime;
		prevPushEndTime = currentTime;
	}

	private void logEndGameData(){
		longest_straight_path = getLongestStraightPath(); //also calculates pathTurns value

		if(turns == 0) {
			avg_turns_per_action = -1f;
			avg_turns_per_move = -1f;
			avg_turns_per_push = -1f;
		} else {
			avg_turns_per_action = actions / (1.0f * turns);
			avg_turns_per_move = moves / (1.0f * turns);
			avg_turns_per_push = pushes / (1.0f * turns);
		}

		foreach(iceBlock i in ices) {
			iceCantMove += i.getIceCantMove();
			iceBlockedByIce += i.getIceBlockedByIce();
			iceBlockedByOffscreen += i.getIceBlockedByOffscreen();

			iceStoppedByOffscreen += i.getStoppedByOffscreen();
			iceStoppedByIce += i.getStoppedByIce();
		}
		if(actions == 0) {
			avg_time_per_action = -1f;
		} else {
			avg_time_per_action = avg_time_per_action/actions;
		}
		if(moves == 0) {
			avg_time_per_move = -1f;
			avg_path_turns_per_move = -1f;
		} else {
			avg_time_per_move = avg_time_per_move/moves;
			avg_path_turns_per_move = pathTurns/(1.0f * moves);
		}
		float avg_push_success_rate;
		if(pushes == 0) {
			avg_time_per_push = -1f;
			avg_push_success_rate = -1f;
		} else {
			avg_time_per_push = avg_time_per_push/pushes;
			avg_push_success_rate = (1.0f * successfulPushes)/pushes;
		}
		game_time = (Time.time - startTime);

		/*
		squares_explored_player = getNumSquaresPlayerExplored(); // squares player has moved onto

		if(right_squares_player == 0) {
			left_right_symmetry_player = -1f;
		} else {
			left_right_symmetry_player = left_squares_player / (1.0f * right_squares_player);
		}

		if(bottom_squares_player == 0) {
			top_bottom_symmetry_player = -1f;
		} else {
			top_bottom_symmetry_player = top_squares_player / (1.0f * bottom_squares_player);
		}

		countIceSymmetry();
		squares_explored_ice += iceNumSquaresExplored();

		foreach(iceBlock i in ices) {
			num_traversed_squares_ice += i.getTraversedSquares();
			num_repeated_squares_ice += i.getRepeatedSquares();
			successfulPushes += i.getSuccessfulPushes();
		}

		if(right_squares_ice == 0) {
			left_right_symmetry_ice = -1f;
		} else {
			left_right_symmetry_ice = left_squares_ice / (1.0f * right_squares_ice);
		}

		if(bottom_squares_ice == 0) {
			top_bottom_symmetry_ice = -1f;
		} else {
			top_bottom_symmetry_ice = top_squares_ice / (1.0f * bottom_squares_ice);
		}
		*/

		/* MOVEMENT DATA */
		//resultStr += "ACTIONS," + actions +","; //should be equal to moves + pushes
		resultStr += "MOVES," + moves +",";
		resultStr += "TURNS," + turns +",";
		//resultStr += "PATH_TURNS," + pathTurns + ",";
		//resultStr +="LONGEST_STRAIGHT_PATH," + longest_straight_path + ",";

		resultStr += "PUSHES," + pushes +",";
		//resultStr += "SUCCESSFUL_PUSHES," + successfulPushes +",";
		//resultStr += "AVG_PUSH_SUCCESS_RATE," + avg_push_success_rate +",";
		//resultStr += "AVG_TIME_PER_ACTION," + avg_time_per_action + ",";
		//resultStr += "AVG_TIME_PER_MOVE," + avg_time_per_move + ",";
		//resultStr += "AVG_TIME_PER_PUSH," + avg_time_per_push + ",";
		//resultStr +="AVG_TURNS_PER_ACTION," + avg_turns_per_action+",";
		//resultStr +="AVG_TURNS_PER_MOVE," + avg_turns_per_move+",";
		//resultStr +="AVG_TURNS_PER_PUSH," + avg_turns_per_push+",";
		//resultStr +="AVG_PATH_TURNS_PER_MOVE," + avg_path_turns_per_move + ",";

		/* ICE BLOCK DATA */
		//resultStr += "ICE_CANT_MOVE," + iceCantMove + ","; // total "errors"
		//resultStr += "ICE_BLOCKED_BY_ICE," + iceBlockedByIce + ",";
		//resultStr += "ICE_BLOCKED_BY_OFFSCREEN," + iceBlockedByOffscreen + ",";
		resultStr += "ICE_STOPPED_BY_ICE," + iceStoppedByIce + ",";
		resultStr += "ICE_STOPPED_BY_OFFSCREEN," + iceStoppedByOffscreen + ",";

		/* PLAYER LOCATION DATA */
		/*
		resultStr += "PLAYER_SQUARES_TRAVERSED," + num_traversed_squares_player + ",";
		resultStr += "PLAYER_SQUARES_EXPLORED," + squares_explored_player + ",";
		resultStr += "PLAYER_SQUARES_REPEATED," + num_repeated_squares_player + ",";

		resultStr += "PLAYER_LEFT_SQUARES," + left_squares_player + ",";
		resultStr += "PLAYER_RIGHT_SQUARES," + right_squares_player + ",";
		resultStr += "PLAYER_TOP_SQUARES," + top_squares_player + ",";
		resultStr += "PLAYER_BOTTOM_SQUARES," + bottom_squares_player + ",";
		resultStr += "PLAYER_LEFT_RIGHT_SYMMETRY," + left_right_symmetry_player + ",";
		resultStr += "PLAYER_TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry_player + ",";
		*/

		/* ICE LOCATION DATA */
		/*
		resultStr += "ICE_SQUARES_TRAVERSED," + num_traversed_squares_ice + ",";
		resultStr += "ICE_SQUARES_EXPLORED," + squares_explored_ice + ",";
		resultStr += "ICE_SQUARES_REPEATED," + num_repeated_squares_ice + ",";

		resultStr += "ICE_LEFT_SQUARES," + left_squares_ice + ",";
		resultStr += "ICE_RIGHT_SQUARES," + right_squares_ice + ",";
		resultStr += "ICE_TOP_SQUARES," + top_squares_ice + ",";
		resultStr += "ICE_BOTTOM_SQUARES," + bottom_squares_ice + ",";
		resultStr += "ICE_LEFT_RIGHT_SYMMETRY," + left_right_symmetry_ice + ",";
		resultStr += "ICE_TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry_ice + ",";
		*/

		//resultStr +="TOTAL_TIME," + game_time +",";
		//resultStr +="PATH_TRACE," + pathTrace + ",";

	}

	/*
	private void countLeftRightSymmetry(string newLoc) {
		if(left_squares_list.Contains (newLoc)) {
			left_squares_player++;
		} else if (right_squares_list.Contains (newLoc)) {
			right_squares_player++;
		}
	}

	private void countTopBottomSymmetry(string newLoc) {
		if(bottom_squares_list.Contains (newLoc)) {
			bottom_squares_player++;
		} else if (top_squares_list.Contains (newLoc)) {
			top_squares_player++;
		}
	}

	private void countIceSymmetry() {
		foreach(iceBlock i in ices) {
			int[,] iceSquaresExplored = i.getSquaresExplored();
			for(int row = 0; row < NUM_ROWS; row++) {
				for(int col = 0; col < NUM_COLS; col++) {
					string squareVisited = "" + (row + 1) + "" + (col + 1);
					if(left_squares_list.Contains(squareVisited)) {
						left_squares_ice += iceSquaresExplored[row,col];
					} else if(right_squares_list.Contains(squareVisited)) {
						right_squares_ice += iceSquaresExplored[row,col];
					}

					if(top_squares_list.Contains(squareVisited)) {
						top_squares_ice += iceSquaresExplored[row,col];
					} else if(bottom_squares_list.Contains(squareVisited)) {
						bottom_squares_ice += iceSquaresExplored[row,col];
					}

				}
			}
		}

	}
	*/



	// Update is called once per frame
	void Update () {
        foreach(string s in victorySquare)
        {
            //print("Victory Square: " + s);
        }
		if(!victorious) {
			if(victory()) {
                if (dataCollector != null)
                {
                    dataCollector.setOutcome("victory");
                } else
                {
                    Debug.Log("Warning: DataCollector not found in scene.");
                }
                logEndGameData ();
				//resultStr +="OUTCOME,VICTORY,";
				victories++;
                vicJingle.Play();
                displayOptions();
			}else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if(!timedOut) {
                    // turn down
                    turnDown();
                    // move down
                    tryMove();

				}
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if(!timedOut){
                    // turn up
                    turnUp();
                    // move up
                    tryMove();

				}
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if(!timedOut){
                    // turn right
                    turnRight();
                    // move right
                    tryMove();

				}
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if(!timedOut){
                    // turn left
                    turnLeft();
                    // move left
                    tryMove();

				}
			}
		}

	}

	private void push(iceBlock ice) {
		string newLoc = ice.move();
	}

	private bool blockedByIce() {
		foreach(iceBlock i in ices) {
			if(predictedSquare == i.square) {
				return true;
			}
		}
		return false;
	}

	private void tryMove() {
		logActionData ();
		bool[] errorsPlayer = getErrorType ();
		if(!errorsPlayer[0] && !errorsPlayer[1]) {
            // player moves physically in the direction they are turned
            Debug.Log("PLAYER MOVED");
			logMoveData();
			if(!approximately(prevMoveDir, direction)) {
				pathTurns++;
				prevMoveDir = direction;
			}
			string newLoc = move();

		} else if(errorsPlayer[1]) {
            //Player blocked by ice, move ice if possible
            Debug.Log("BLOCKED BY ICE");
			logPushData();
			foreach(iceBlock i in ices) {
				if(predictedSquare == i.square) {
					push(i);
				}

			}

		}
	}

	public void SetTimedOut (bool timedOut) {
        if (dataCollector != null)
        {
            dataCollector.setOutcome("time");
        }
        this.timedOut = timedOut;
	}
}
