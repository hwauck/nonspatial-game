using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public GameObject SAB;
	public GameObject SAT;
	public GameObject player;
	public bool squaresExploredHints;
	public GameObject[] boardSquares;
	private int moves;
	private const int HINT_INTERVAL = 5; //the number of moves in between hint checks
	private const int HINT_THRESHOLD = 3; //don't give hints if the player has already explored this many squares
	private int squaresExploredSinceLastHint;
	private int squaresExplored; 

	public Sprite boardSquare;
	public Sprite victorySquare;
	public Sprite boardSquareHintNotExplored;
	public Sprite victorySquareHintNotExplored;

	private bool hintOn;

	void Awake() {
		squaresExplored = 1;
		squaresExploredSinceLastHint = 1;
		moves = 0;
		hintOn = false;
		//player.AddComponent ("playerArrow");
	}
	// Use this for initialization
	void Start () {

	}

	public void reset() {

		// reset data collection?

	}


	// every 30 moves:
	// if player has not explored at least 18 distinct squares
	// and player has not explored 1 new square since the last hint
	public void giveHint() {
		hintOn = true;
		IList<string> squaresExplored = player.GetComponent<playerArrow>().getSquaresExplored();
		for(int i = 0; i < boardSquares.Length; i++) {
			GameObject currentSquare = boardSquares[i];
			string name = currentSquare.name.Substring(6);
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
	}

	// Update is called once per frame
	void Update () {
		playerArrow arrow = player.GetComponent<playerArrow>();
		int currentMoves = arrow.getNumMoves();
		int currentSquaresExplored = arrow.getNumSquaresExplored();
		squaresExploredSinceLastHint = squaresExplored - currentSquaresExplored;
		if(currentMoves > moves && hintOn) {
			removeHint ();
		}
		if(!hintOn && currentMoves > 0 && currentMoves % HINT_INTERVAL == 0 && currentSquaresExplored < 18 && squaresExploredSinceLastHint < 2) {
			print ("Squares explored: " + currentSquaresExplored);
			giveHint ();
		}
		moves = currentMoves;
		squaresExplored = currentSquaresExplored;
	}
}
