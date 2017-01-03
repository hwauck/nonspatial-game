using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HintManager : MonoBehaviour {
	private string hintType;

	public GameObject SAB;
	public GameObject SAT;
	public GameObject player;
	public GameObject[] boardSquares;
	private int moves;
	private const int HINT_INTERVAL = 15; //the number of moves in between hint checks
	private const int HINT_THRESHOLD = 21; //don't give hints if the player has already explored this many squares
	private int prevSquaresExplored; 

	public Sprite boardSquare;
	public Sprite victorySquare;
	public Sprite boardSquareHintNotExplored;
	public Sprite victorySquareHintNotExplored;
	public GameObject hintButton;
	public CanvasGroup hintText;
	public CanvasGroup howToPlayText;

	private bool hintOn;
	private int numHints;

	void Awake() {
		prevSquaresExplored = 1;
		moves = 0;
		numHints=0;
		hintOn = false;

		int rand = Random.Range(0,3);
		if(rand==0) {
			hintType="demand";
		} else if (rand==1) {
			hintType="auto";
		} else if (rand==2) {
			hintType="adapt";
		}

		print(rand);
		if(hintType.Equals("demand")) {
			hintButton.SetActive(true);
		}
		//player.AddComponent ("playerArrow");
	}
	// Use this for initialization
	void Start () {

	}

	public void reset() {
		numHints = 0;
		// reset data collection?

	}

	public int getNumHints() {
		return numHints;
	}

	public string getHintType() {
		return hintType;
	}

	public void hintButtonClicked() {
		print ("ON-DEMAND HINT");
		giveHint ();
	}
		
	public void giveHint() {
		hintOn = true;
		numHints++;
		if(hintType.Equals("auto") || hintType.Equals("adapt")) {
			StartCoroutine(disableWaitEnableControls());
		}
		IList<string> squaresExplored = player.GetComponent<playerArrowStatue>().getSquaresExplored();
		for(int i = 0; i < boardSquares.Length; i++) {
			GameObject currentSquare = boardSquares[i];
			string name = currentSquare.name.Substring(6); // just the number of the square
			if(currentSquare.name.Equals ("Square11") || currentSquare.name.Equals ("Square31")) {
				if(!squaresExplored.Contains (name)) {
					currentSquare.GetComponent<SpriteRenderer>().sprite = victorySquareHintNotExplored;
				}
			} else {
				if(!squaresExplored.Contains (name)) {
					currentSquare.GetComponent<SpriteRenderer>().sprite = boardSquareHintNotExplored;
				}
			}
		}
		howToPlayText.alpha = 0;
		hintText.alpha = 1;

	}

	// next move after player receives a hint
	public void removeHint() {
		hintOn = false;
		for(int i = 0; i < boardSquares.Length; i++) {
			GameObject currentSquare = boardSquares[i];
			if(currentSquare.name.Equals ("Square11") || boardSquares[i].name.Equals ("Square31")) {
				currentSquare.GetComponent<SpriteRenderer>().sprite = victorySquare;
			} else {
				currentSquare.GetComponent<SpriteRenderer>().sprite = boardSquare;
			}
		}
		hintText.alpha = 0;
		howToPlayText.alpha = 1;
	}

	IEnumerator disableWaitEnableControls() {
		player.GetComponent<playerArrowStatue>().disableControls();
		yield return new WaitForSeconds(3f);
		player.GetComponent<playerArrowStatue>().enableControls();
	}

	// Update is called once per frame
	void Update () {
		playerArrowStatue arrow = player.GetComponent<playerArrowStatue>();
		int currentMoves = arrow.getNumMoves();
		int currentSquaresExplored = arrow.getNumSquaresExplored();
		int newSquaresExplored = currentSquaresExplored - prevSquaresExplored;
		if(hintOn && currentMoves > moves) {
			removeHint ();
		}
		if(!hintOn && currentMoves > moves) {
			print("NEW SQUARES EXPLORED: " + newSquaresExplored);
			print("PREV SQUARES EXPLORED: " + prevSquaresExplored);
			print("CURRENT SQUARES EXPLORED: " + currentSquaresExplored);
			print("CURRENT MOVES: " + currentMoves);
			print("MOVES: " + moves);
		}
		if(!hintOn && currentMoves > moves && currentMoves % HINT_INTERVAL == 0 && currentSquaresExplored < HINT_THRESHOLD) {
			prevSquaresExplored = arrow.getNumSquaresExplored();

			if(hintType.Equals("adapt") && newSquaresExplored < 10) {
				//adaptive automatic hint
				print ("ADAPTIVE HINT");
				giveHint ();
			} else if (hintType.Equals("auto")) {
				//non-adaptive automatic hint
				print ("NONADAPTIVE HINT");
				giveHint ();
			}

		}
		moves = currentMoves;
	}
}
