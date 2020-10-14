using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
	// Fields set in inspector
	public int rows;            // # of rows of the board
	public int columns;         // # of columns of the board
	public GameObject[] deck;   // size of 40, filled with cards of each type and value

	// Fields set at Start()
	public GameObject[,] sceneBoard;
	public GameObject selectedGameObj;
	public GameObject prevSelectedGameObj;
	public bool match;
	public int timesDoubled;

	// Start is called before the first frame update
	void Start()
	{
		sceneBoard = new GameObject[rows,columns];
		match = false;
		timesDoubled = 0;

		// if the array has capacity, it is filled and the board is displayed
		if(rows + columns > 0) 
		{
			sceneBoard = PopulateCardArray(CreateRandomCardArray(rows,columns), rows, columns);
			// sceneBoard = PopulateCardArray(CreateStandardCardArray(rows,columns), rows, columns); 

			if(sceneBoard != null) {
				gameObject.GetComponent<BoardDisplayManager>().DisplayBoard(sceneBoard,"card board");
				gameObject.GetComponent<BoardDisplayManager>().CenterView();
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Selects an object with the Left Mouse "Mouse1" click
		if(Input.GetMouseButtonDown(0))
			GameObjClicked();

		// Doubles the board when "R" is pressed
		if(Input.GetKeyDown(KeyCode.R)) {
			sceneBoard = DoubleBoard(sceneBoard);
		}
	}

	/// <summary>
	/// Doubles all elements in a 2D array that are still active in the scene
	/// </summary>
	/// <param name="oldBoard">The old 2D array that will be doubled</param>
	/// <returns>Returns the new 2D array</returns>
	GameObject[,] DoubleBoard(GameObject[,] oldBoard)
	{
		// Creates a list to hold all active gameObjs from the 2D array "board" 
		List<GameObject> remainingGameObjs = new List<GameObject>();

		for(int r = 0; r < oldBoard.GetLength(0); r++) {
			for(int c = 0; c < oldBoard.GetLength(1); c++) {
				if(oldBoard[r,c] != null)
					remainingGameObjs.Add(oldBoard[r,c]);
			}
		}

		// Creates a new 2D array using the old 2D array and the newly created list
		GameObject[,] newBoard = new GameObject[oldBoard.GetLength(0) + remainingGameObjs.Count,columns];

		// Adds all elements from the old 2D array
		for(int r = 0; r < oldBoard.GetLength(0); r++) {
			for(int c = 0; c < oldBoard.GetLength(1); c++) {
				newBoard[r,c] = oldBoard[r,c];
			}
		}

		// Adds all elements from the list "doubling" all active elements
		int index = 0;
		for(int r = oldBoard.GetLength(0); r < newBoard.GetLength(0); r++) {
			for(int c = oldBoard.GetLength(1); c < newBoard.GetLength(1); c++) {
				if(newBoard[r,c] == null
					&& index < remainingGameObjs.Count)
					newBoard[r,c] = remainingGameObjs[index];
			}
		}

		// Sets new row and column amount and displays the new board
		rows = newBoard.GetLength(0);
		columns = newBoard.GetLength(1);
		gameObject.GetComponent<BoardDisplayManager>().DisplayBoard(newBoard,"Doubled Board");
		timesDoubled++;
		return newBoard;
	}

	/// <summary>
	/// Fills a new 2D array with GameObjects from another array
	/// </summary>
	/// <param name="oldArray">The old 2D array</param>
	/// <param name="newRows">The number of rows of the new 2D array</param>
	/// <param name="newColumns">The number of columns of the new 2D array</param>
	/// <returns></returns>
	GameObject[,] PopulateCardArray(GameObject[,] oldArray, int newRows, int newColumns)
	{
		// Checks if the new row and column size is too small
		if(oldArray.GetLength(0) > newRows
			|| oldArray.GetLength(1) > newColumns) {
			Debug.Log("Error! New sizes are too small. The old array won't fit!");
			return null;
		}
		else {
			GameObject[,] newBoard = new GameObject[newRows,newColumns];
			for(int r = 0; r < oldArray.GetLength(0); r++) {
				for(int c = 0; c < oldArray.GetLength(1); c++) {
					newBoard[r,c] = oldArray[r,c];
					newBoard[r,c].GetComponent<Card>().row = r;
					newBoard[r,c].GetComponent<Card>().column = c;
				}
			}
			return newBoard;
		}
	}

	/// <summary>
	/// Creates a 2D array of random card gameObjects
	/// </summary>
	/// <param name="rowsAmount">The number of rows in the 2D array</param>
	/// <param name="columnsAmount">The number of columns in the 2D array</param>
	/// <returns>Returns a 2D array of gameObjects</returns>
	GameObject[,] CreateRandomCardArray(int rowsAmount,int columnsAmount)
	{
		// Create empty array
		GameObject[,] cardArray = new GameObject[rowsAmount,columnsAmount];

		for(int r = 0; r < rowsAmount; r++) {
			for(int c = 0; c < columnsAmount; c++) {
				cardArray[r,c] = deck[Random.Range(0,deck.Length)];
				//Debug.Log(cardArray[r,c].GetComponent<Card>().ToString());
			}
		}

		return cardArray;
	}

	/// <summary>
	/// Creates a 2D array of card gameObjects, in order of a card deck
	/// </summary>
	/// <param name="rowsAmount">The number of rows in the 2D array</param>
	/// <param name="columnsAmount">The number of columns in the 2D array</param>
	/// <returns>Returns a 2D array of gameObjects</returns>
	GameObject[,] CreateStandardCardArray(int rowsAmount,int columnsAmount)
	{
		GameObject[,] cardArray = new GameObject[rowsAmount,columnsAmount];
		int cardCount = 0;

		for(int r = 0; r < rowsAmount; r++) {
			for(int c = 0; c < columnsAmount; c++) {
				cardArray[r,c] = deck[cardCount];
				//Debug.Log(cardArray[r,c].GetComponent<Card>().ToString());

				cardCount++;
				if(cardCount == deck.Length)
					cardCount = 0;
			}
		}

		return cardArray;
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
		sceneBoard[card1.GetComponent<Card>().row,card1.GetComponent<Card>().column].SetActive(false);
		sceneBoard[card2.GetComponent<Card>().row,card2.GetComponent<Card>().column].SetActive(false);

		// Deactivates and destroys the matched gameObjs from the scene
		card1.SetActive(false);
		card2.SetActive(false);
		Destroy(card1);
		Destroy(card2);

		// Reverts values
		selectedGameObj = null;
		prevSelectedGameObj = null;
		match = false;

		// Check for end of game
		CheckVictory();
	}

	/// <summary>
	/// If the board has been cleared, the game state to the end state
	/// </summary>
	void CheckVictory()
	{
		// If the board is empty, it sets isWon to true and
		// to changes the gameState to the end state
		if(sceneBoard.GetLength(0) == 0
			&& sceneBoard.GetLength(1) == 0) {
			gameObject.GetComponent<GameManager>().ChangeGameState(GameState.End);
			gameObject.GetComponent<GameManager>().isWon = true;
		}
	}
}