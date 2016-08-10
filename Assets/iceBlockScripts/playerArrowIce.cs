using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerArrowIce : MonoBehaviour {
	private const int NUM_ROWS = 5;
	private const int NUM_COLS = 7;
	public iceBlock[] ices;

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

	/* DATA COLLECTION */
	private int plays;
	private int victories;
	private int resets;

	/* MOVEMENT DATA */
	private int actions; //number of times player either moves or pushes
	private int moves; 	//number of times player moves from one square to another
	private int pushes; 	//number of times player attempts to push an ice block
	private int successfulPushes; // number of times player pushes ice block and it moves
	private int turns;	//number of times player changes direction
	private float avg_turns_per_move;
	private float avg_turns_per_action;
	private float avg_turns_per_push;

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

	/* PLAYER LOCATION DATA */
	private int left_squares_player;
	private int right_squares_player;
	private int top_squares_player;
	private int bottom_squares_player;
	private int num_repeated_squares_player; // number of times player steps on square they've already been to
	private int num_traversed_squares_player; // total displacement of player
	private int squares_explored_player; // squares player has moved onto
	private float left_right_symmetry_player;
	private float top_bottom_symmetry_player;
	private IList<string> squares_explored_player_list;

	/* ICE LOCATION DATA */
	private int left_squares_ice; //including repetitions
	private int right_squares_ice; //including repetitions
	private int top_squares_ice; //including repetitions
	private int bottom_squares_ice; //including repetitions
	private int num_repeated_squares_ice;
	private int num_traversed_squares_ice;
	private int squares_explored_ice; // squares ice blocks have moved onto/across
	private float left_right_symmetry_ice;
	private float top_bottom_symmetry_ice;
	private IList<string> squares_explored_ice_list;

	/* Keep track of which squares are in which area of board */
	private IList<string> left_squares_list;
	private IList<string> right_squares_list;
	private IList<string> top_squares_list;
	private IList<string> bottom_squares_list;

	//write to database
	string resultStr;

	void Awake() {
		victorious = false;
		startTime = Time.time;
		prevActionEndTime = startTime;
		prevMoveEndTime = startTime;
		prevPushEndTime = startTime;
	}

	// Use this for initialization
	void Start () {
		resultStr = "NEW_GAME,ice__";
		direction = Vector3.right;
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		square = new Vector2(5,2);
		predictedSquare = new Vector2(5,3); //player faces right to start

		//Data collection variables
		plays = 1;
		victories = 0;
		resets = 0;
		actions = 0;
		moves = 0;
		turns = 0;
		pushes = 0;
		successfulPushes = 0;
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;
		avg_turns_per_action = 0f;
		avg_turns_per_push = 0f;

		iceCantMove = 0; 	
		iceBlockedByIce = 0; 
		iceBlockedByOffscreen = 0; 
		iceStoppedByIce = 0; 
		iceStoppedByOffscreen = 0; 

		squares_explored_player_list = new List<string>();
		squares_explored_player_list.Add ("52");

		squares_explored_ice_list = new List<string>();
		squares_explored_ice_list.Add ("22");
		squares_explored_ice_list.Add ("42");
		squares_explored_ice_list.Add ("26");

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
	
		//Victory UI variables
		yes = GameObject.Find ("Yes").GetComponent<Button>();
		no = GameObject.Find ("No").GetComponent<Button>();
		victoryPanel = GameObject.Find ("Victory").GetComponent<Image>();
		victoryText = GameObject.Find ("Congratulations").GetComponent<Text>();

	}

	public IList<string> getSquaresPlayerExplored() {
		return squares_explored_player_list;
	}

	public int getNumSquaresPlayerExplored() {
		return squares_explored_player_list.Count;
	}

	public int getNumMoves() {
		return moves;
	}

	public Vector3 getDirection() {
		return direction;
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
		transform.Translate(direction * 2f, Space.World);
		num_traversed_squares_player++;
		string predictedSquareName = coordinatesToSquare(predictedSquare);
		//resultStr += predictedSquareName;
		Vector3 oldSquare = square; 
		square = predictedSquare; 
		if(!squares_explored_player_list.Contains(predictedSquareName)) {
			squares_explored_player_list.Add(predictedSquareName);
		} else {
			num_repeated_squares_player++;
		}
		predictedSquare.x = 2f * square.x - oldSquare.x; 
		predictedSquare.y = 2f * square.y - oldSquare.y; 
		return predictedSquareName;

	}

	public void turnDown(){
		direction = Vector3.down;
		transform.rotation = Quaternion.Euler(down);
		predictedSquare.x = square.x + 1;
		predictedSquare.y = square.y;

	}

	public void turnUp(){
		direction = Vector3.up;
		transform.rotation = Quaternion.Euler(up);
		predictedSquare.x = square.x - 1;
		predictedSquare.y = square.y;

	}

	public void turnLeft(){
		direction = Vector3.left;
		transform.rotation = Quaternion.Euler(left);
		predictedSquare.x = square.x;
		predictedSquare.y = square.y - 1;

	}

	public void turnRight(){
		direction = Vector3.right;
		transform.rotation = Quaternion.Euler(right);
		predictedSquare.x = square.x;
		predictedSquare.y = square.y + 1;

	}

	// logs end game data, increments resets, and saves results to database
	// only when "Play Again? Yes" button is clicked
	public void newGame() {
		//resultStr += "RESET__";
		plays++;
		reset();
		resultStr += "\nNEW_GAME,ice__";
	}

	// when the "Reset" button is clicked
	public void buttonReset() {
		resets++;
		plays++;
		logEndGameData();
		reset();
		resultStr += "RESET__\nNEW_ATTEMPT,ice__";
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
		logEndGameData();
		resultStr += "NO__\nEND_SESSION,no__";
		SendSaveResult();
		SceneManager.LoadScene("postgame_survey");
	}

	private void SendSaveResult()
	{
		resultStr += "ATTEMPTS," + plays + "__";
		resultStr += "RESETS," + resets + "__";
		resultStr += "VICTORIES," + victories + "__";
		//GameObject.Find("DataCollector").GetComponent<dataCollector>().setPlayerData(resultStr);
		Debug.Log(resultStr);

	}

	public void reset() {
		if(victorious) {
			victorious = false;
		}
		transform.position = new Vector3(-4.5f,-4,0);
		square = new Vector2(5,2);
		predictedSquare = new Vector2(5,3);
		direction = Vector3.right;
		transform.rotation = Quaternion.Euler(right);

		//Data collection variables
		actions = 0;
		moves = 0;
		turns = 0;
		pushes = 0;
		successfulPushes = 0;
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;
		avg_turns_per_action = 0f;
		avg_turns_per_push = 0f;

		iceCantMove = 0; 	
		iceBlockedByIce = 0; 
		iceBlockedByOffscreen = 0; 
		iceStoppedByIce = 0; 
		iceStoppedByOffscreen = 0; 

		squares_explored_player_list = new List<string>();
		squares_explored_player_list.Add ("52");

		squares_explored_ice_list = new List<string>();
		squares_explored_ice_list.Add ("22");
		squares_explored_ice_list.Add ("42");
		squares_explored_ice_list.Add ("26");

		left_squares_player = 1;
		right_squares_player = 0;
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

		left_right_symmetry_player = -1f;
		top_bottom_symmetry_player = -1f;
		left_right_symmetry_ice = -1f;
		top_bottom_symmetry_ice = -1f;

		foreach(iceBlock i in ices) {
			i.reset(); 
		}

		unDisplayOptions();

	}

	private bool victory() {
		foreach(iceBlock i in ices) {
			if(coordinatesToSquare(i.square).Equals ("34")) {
				victorious = true;
				return true;
			} 
		}
		return false;
	}

	// add square to count only once
	// create new array that is union of all 3 ice arrays
	private int iceNumSquaresExplored() {

		//create the union array to count each square only once
		int[,] ice1Squares = ices[0].getSquaresExplored();
		int[,] ice2Squares = ices[1].getSquaresExplored();
		int[,] ice3Squares = ices[2].getSquaresExplored();
		int[,] union = new int[NUM_ROWS,NUM_COLS];
		for(int row = 0; row < NUM_ROWS; row++) {
			for(int col = 0; col < NUM_COLS; col++) {
				union[row,col] = ice1Squares[row,col] + ice2Squares[row,col] + ice3Squares[row,col];
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
		} else {
			avg_time_per_move = avg_time_per_move/moves;
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

		/* MOVEMENT DATA */
		resultStr += "ACTIONS," + actions +"__"; //should be equal to moves + pushes
		resultStr += "MOVES," + moves +"__";
		resultStr += "TURNS," + turns +"__";
		resultStr += "PUSHES," + pushes +"__";
		resultStr += "SUCCESSFUL_PUSHES," + successfulPushes +"__";
		resultStr += "AVG_PUSH_SUCCESS_RATE," + avg_push_success_rate +"__";
		resultStr +="AVG_TURNS_PER_ACTION," + avg_turns_per_action+"__"; 
		resultStr +="AVG_TURNS_PER_MOVE," + avg_turns_per_move+"__";
		resultStr +="AVG_TURNS_PER_PUSH," + avg_turns_per_push+"__";

		/* ICE BLOCK DATA */
		resultStr += "ICE_CANT_MOVE," + iceCantMove + "__";
		resultStr += "ICE_BLOCKED_BY_ICE," + iceBlockedByIce + "__";
		resultStr += "ICE_BLOCKED_BY_OFFSCREEN," + iceBlockedByOffscreen + "__";
		resultStr += "ICE_STOPPED_BY_ICE," + iceStoppedByIce + "__";
		resultStr += "ICE_STOPPED_BY_OFFSCREEN," + iceStoppedByOffscreen + "__";

		/* TIME DATA */
		resultStr += "AVG_TIME_PER_ACTION," + avg_time_per_action + "__";
		resultStr += "AVG_TIME_PER_MOVE," + avg_time_per_move + "__";
		resultStr += "AVG_TIME_PER_PUSH," + avg_time_per_push + "__";
		resultStr +="TOTAL_TIME," + game_time +"__";

		/* PLAYER LOCATION DATA */
		resultStr += "PLAYER_SQUARES_TRAVERSED," + num_traversed_squares_player + "__";
		resultStr += "PLAYER_SQUARES_EXPLORED," + squares_explored_player + "__";
		resultStr += "PLAYER_SQUARES_REPEATED," + num_repeated_squares_player + "__";

		resultStr += "PLAYER_LEFT_SQUARES," + left_squares_player + "__";
		resultStr += "PLAYER_RIGHT_SQUARES," + right_squares_player + "__";
		resultStr += "PLAYER_TOP_SQUARES," + top_squares_player + "__";
		resultStr += "PLAYER_BOTTOM_SQUARES," + bottom_squares_player + "__";
		resultStr += "PLAYER_LEFT_RIGHT_SYMMETRY," + left_right_symmetry_player + "__";
		resultStr += "PLAYER_TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry_player + "__";

		/* ICE LOCATION DATA */
		resultStr += "ICE_SQUARES_TRAVERSED," + num_traversed_squares_ice + "__";
		resultStr += "ICE_SQUARES_EXPLORED," + squares_explored_ice + "__";
		resultStr += "ICE_SQUARES_REPEATED," + num_repeated_squares_ice + "__";

		resultStr += "ICE_LEFT_SQUARES," + left_squares_ice + "__";
		resultStr += "ICE_RIGHT_SQUARES," + right_squares_ice + "__";
		resultStr += "ICE_TOP_SQUARES," + top_squares_ice + "__";
		resultStr += "ICE_BOTTOM_SQUARES," + bottom_squares_ice + "__";
		resultStr += "ICE_LEFT_RIGHT_SYMMETRY," + left_right_symmetry_ice + "__";
		resultStr += "ICE_TOP_BOTTOM_SYMMETRY," + top_bottom_symmetry_ice + "__";			
	}

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


		
	// Update is called once per frame
	void Update () {
		if(!victorious) {
			if(victory()) {
				logEndGameData ();
				resultStr +="VICTORY__";
				victories++;
				displayOptions();
			}else if (Input.GetKeyDown (KeyCode.DownArrow)) {
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
				logActionData ();
				bool[] errorsPlayer = getErrorType ();
				if(!errorsPlayer[0] && !errorsPlayer[1]) {
					// player moves physically in the direction they are turned
					logMoveData();
					string newLoc = move();
					countLeftRightSymmetry(newLoc); // includes repetitions
					countTopBottomSymmetry(newLoc); //includes repetitions

				} else if(errorsPlayer[1]) {
					//Player blocked by ice, move ice if possible
					logPushData();
					foreach(iceBlock i in ices) {
						if(predictedSquare == i.square) {
							push(i);
						}

					}

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
}
