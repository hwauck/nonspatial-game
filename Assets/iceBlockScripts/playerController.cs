using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour {

	public iceBlock ice1;
	public iceBlock ice2;
	public iceBlock ice3;

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

	/* MOVEMENT DATA */
	private int actions; //number of times player either moves or pushes
	private int moves; 	//number of times player moves from one square to another
	private int pushes; 	//number of times player attempts to push an ice block
	private int turns;	//number of ties player changes direction
	private float avg_turns_per_move;

	/* "ERROR" DATA - PUT THIS IN iceBlock.cs */
	private int iceCantMove; 	// number of times player tries to push ice block but something is in the way
	private int iceBlockedByIce; // ice block doesn't move due to presence of other ice block
	private int iceBlockedByOffscreen; // ice block doesn't move due to it being on edge of screen
	private int iceStoppedByIce; // ice block slides and then hits another ice block
	private int iceStoppedByOffscreen; // ice block slides and then hits the edge of the area

	/* TIME DATA */
	private float startTime;
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
	private int num_repeated_squares_player;
	private int num_traversed_squares_player;
	private int squares_explored_player; // squares player has moved onto
	private float avg_repeats_per_square_player;
	private float left_right_symmetry_player;
	private float top_bottom_symmetry_player;
	private IList<string> squares_explored_player_list;

	/* ICE LOCATION DATA - PUT THIS IN iceBlock.cs */
	private int left_squares_ice;
	private int right_squares_ice;
	private int top_squares_ice;
	private int bottom_squares_ice;
	private int num_repeated_squares_ice;
	private int num_traversed_squares_ice;
	private int squares_explored_ice; // squares ice blocks have moved onto/across
	private float avg_repeats_per_square_ice;
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
		prevMoveEndTime = startTime;
	}

	// Use this for initialization
	void Start () {
		direction = new Vector3(0,0,0);
		right = new Vector3(0,0,270);
		left = new Vector3(0,0,90);
		up = new Vector3(0,0,0);
		down = new Vector3(0,0,180);
		square = new Vector2(5,2);
		predictedSquare = new Vector2(5,3); //player faces right to start

		//Data collection variables
		moves = 0;
		turns = 0;
		avg_time_per_move = 0f;
		avg_turns_per_move = 0f;

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
		num_traversed_squares_player = 0;

		left_squares_ice = 2;
		right_squares_ice = 1;
		squares_explored_ice = 3;
		num_repeated_squares_ice = 0;
		num_traversed_squares_ice = 0;

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

	public bool canMove() {
		//TODO
		return false;
	}

	private bool offScreen() {
		if(GameObject.Find ("Block" + predictedSquare.x + "" + predictedSquare.y) == null) {
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
//		if(offScreen ()) {
//			errors[0] = true;
//		} else if (blockedByIce()) {
//			errors[1] = true;
//		} 
		return errors;
	}

	public string move() {
		//TODO
		//if this is a push, set ice block's direction to player direction
		return "";
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

	public void reset() {
		if(victorious) {
			victorious = false;
		}
		transform.position = new Vector3(-4.5f,-4,0);
		square = new Vector2(5,2);
		predictedSquare = new Vector2(5,3);
		direction = Vector3.right;
		transform.rotation = Quaternion.Euler(right);
		//TODO: reset all data collection vars
		//TODO: reset all ice blocks

		unDisplayOptions();

	}

	private bool victory() {
//		string ice1Square = coordinatesToSquare(ice1.square);
//		string ice2Square = coordinatesToSquare(ice2.square);
//		string ice3Square = coordinatesToSquare(ice3.square);
//		if(ice1Square.Equals ("34") || ice2Square.Equals("34") || ice3Square.Equals("34")) {
//			victorious = true;
//			return true;
//		} else {
//			return false;
//		}
		return false;
	}

	private void logMoveData() {
		//TODO 
	}

	private void logEndGameData(){
		//TODO
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

	public void saveAndReset() {
		logEndGameData();
		resultStr += "RESET__";
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
				bool[] errorsPlayer = getErrorType ();
				if(!errorsPlayer[0] && !errorsPlayer[1]) {
					string newLoc = move ();
					moves++;
					countLeftRightSymmetry(newLoc);
					countTopBottomSymmetry(newLoc);
//					bool[] errorsIce1 = ice1.getErrorType ();
//					bool[] errorsIce2 = ice2.getErrorType ();
//					bool[] errorsIce3 = ice3.getErrorType ();
//
//					if(errorsIce1[1]) {
//						//TODO
//					} else if (errorsIce1[2]) {
//						//TODO
//					}else { 
//						//TODO
//					}
				} else if(errorsPlayer[1]) {
					//if player blocked by ice
					//TODO
				} 
			} else if(victory()) {
				logEndGameData ();
				resultStr +="VICTORY__";
				SendSaveResult();
				//change later to allow player to play a new game
				displayOptions();
			}
		}

	}


}
