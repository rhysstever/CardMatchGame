using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
	// Fields set in inspector
	public GameObject[] deck;   // size of 40, filled with cards of each type and value
	public int numOfCards;      // # of cards on the board
	public int columns;         // # of columns of the board (# of cards in each row)
	public bool standardFill;

	// Fields set at Start()
	public List<GameObject> sceneBoard;
	public GameObject selectedGameObj;
	public GameObject prevSelectedGameObj;
	public int timesDoubled;
	bool match;

	// Start is called before the first frame update
	void Start()
	{
		sceneBoard = new List<GameObject>();
		timesDoubled = 0;
		match = false;

		if(standardFill)
			AddStandardCards(numOfCards, 0);
		else
			AddRandomCards(numOfCards);
	}

	// Update is called once per frame
	void Update()
	{
		// Selects an object with the Left Mouse "Mouse1" click
		if(Input.GetMouseButtonDown(0))
			GameObjClicked();

		// Doubles the board when "R" is pressed
		if(Input.GetKeyDown(KeyCode.R))
			DoubleBoard();
	}

	/// <summary>
	/// Doubles all elements in a 2D array that are still active in the scene
	/// </summary>
	void DoubleBoard()
	{
		List<GameObject> doubledCards = new List<GameObject>();
		foreach(GameObject card in sceneBoard) {
			if(card.activeSelf) {
				doubledCards.Add(card);
				numOfCards++;
			}
		}
		sceneBoard.AddRange(doubledCards);

		timesDoubled++;
		gameObject.GetComponent<BoardDisplayManager>().parentDisplay = gameObject.GetComponent<BoardDisplayManager>().DisplayBoard(sceneBoard,"cardBoard" + timesDoubled);
	}

	/// <summary>
	/// Adds random cards to the list of cards in the scene
	/// </summary>
	/// <param name="numOfCardsToAdd">The number of random cards to add</param>
	void AddRandomCards(int numOfCardsToAdd)
	{
		for(int i = 0; i < numOfCardsToAdd; i++) {
			sceneBoard.Add(deck[Random.Range(0,deck.Length)]);
		}
	}

	/// <summary>
	/// Adds cards to the list of cards in the scene, based on the last card in the list already 
	/// </summary>
	/// <param name="numOfCardsToAdd">The number of cards being added to the scene</param>
	/// <param name="startingDeckIndex">The starting index of the deck</param>
	void AddStandardCards(int numOfCardsToAdd, int startingDeckIndex)
	{
		// if the given starting index is more than the deck length, 
		// the starting index is recalculated to be within the the deck length
		if(startingDeckIndex > deck.Length) {
			startingDeckIndex = startingDeckIndex % deck.Length;
		}

		int deckIndex = startingDeckIndex;
		for(int i = 0; i < numOfCardsToAdd; i++) {
			// Adds the next card in the deck to the list of cards in the scene
			sceneBoard.Add(deck[deckIndex]);

			// Resets the deckIndex if it hits the end of the deck
			deckIndex++;
			if(deckIndex == deck.Length)
				deckIndex = 0;
		}
	}

	/// <summary>
	/// Assigns the selected gameObj when the user left clicks
	/// </summary>
	void GameObjClicked()
	{
		// Creates a layerMask to hit all layers except the UI layer
		int layerMask = 1 << 5;
		layerMask = ~layerMask;

		// Creates ray
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit rayHit;

		// If the ray interects with something in the scene 
		if(Physics.Raycast(ray,out rayHit,Mathf.Infinity,layerMask)
			&& rayHit.transform.gameObject.name != "Board") {

			// Finds the clicked gameObj
			GameObject hitGameObj = rayHit.transform.gameObject;

			// If the clicked gameObj has a parent, then the 
			// parent becomes the selected gameObj
			if(hitGameObj.transform.parent.GetComponent<Card>() != null)
				hitGameObj = hitGameObj.transform.parent.gameObject;

			// If a selected gameObj already exists, 
			// then it is "saved" as the previous selected gameObj
			if(selectedGameObj != null)
				prevSelectedGameObj = selectedGameObj;

			// Clicked gameObj is assigned
			selectedGameObj = hitGameObj;
			CheckForMatches();
		}
		// Not clicking on anything will unselect both selected gameObjects
		else {
			prevSelectedGameObj = null;
			selectedGameObj = null;
			match = false;
		}
	}

	/// <summary>
	/// Checks if there are 2 matchable cards selected
	/// </summary>
	void CheckForMatches()
	{
		if(selectedGameObj != null
			&& prevSelectedGameObj != null
			&& !match)
			if(isMatch(selectedGameObj,prevSelectedGameObj)) {
				match = true;
				RemoveMatch(selectedGameObj,prevSelectedGameObj);
			}
			else {
				match = false;
				prevSelectedGameObj = null;
				selectedGameObj = null;
			}
	}

	/// <summary>
	/// Compares 2 cards and returns whether they are 
	/// a match based on a number of criteria
	/// </summary>
	/// <param name="card1">The first gameObj being compared</param>
	/// <param name="card2">The second gameObj being compared</param>
	/// <returns>Returns whether the 2 cards are a match</returns>
	bool isMatch(GameObject card1,GameObject card2)
	{
		// Checks that both gameObjects are cards
		if(card1.GetComponent<Card>() == null
			|| card2.GetComponent<Card>() == null) {
			Debug.Log("These are not cards");
			return false;
		}

		// Checks that both gameObjects are unique
		if(card1 == card2) { 
			Debug.Log("These are the same object");
			return false;
		}	

		// Check the type of each card
		if(card1.GetComponent<Card>().type
			!= card2.GetComponent<Card>().type) {
			Debug.Log("Wrong type");
			return false;
		}

		// Check the values each of card
		if(card1.GetComponent<Card>().value
			!= card2.GetComponent<Card>().value) {
			Debug.Log("Not the same value");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Sets a card match to be inactive
	/// </summary>
	/// <param name="card1">The first gameObejct of the match</param>
	/// <param name="card2">The second gameObject of the match</param>
	void RemoveMatch(GameObject card1,GameObject card2)
	{
		// Deactivates the matched gameObjs from the board array
		foreach(GameObject childCard in sceneBoard) {
			if(childCard == card1 || childCard == card2)
				childCard.SetActive(false);
		}

		// Deactivates and destroys the matched gameObjs from the scene
		card1.SetActive(false);
		card2.SetActive(false);

		// Reverts values
		selectedGameObj = null;
		prevSelectedGameObj = null;
		match = false;

		gameObject.GetComponent<BoardDisplayManager>().CheckEndOfGame();
	}
}