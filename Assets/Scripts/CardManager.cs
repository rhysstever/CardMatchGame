using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
	// Fields set in inspector
	public int rows;            // # of rows of the board
	public int columns;         // # of columns of the board
	public float rowGap;
	public float columnGap;
	public GameObject[] deck;   // size of 40, filled with cards of each type and value

	// Materials
	public Material brownMat;

	// Fields set at Start()
	public GameObject[,] board;
	public GameObject selectedGameObj;
	public GameObject prevSelectedGameObj;
	public bool match;
	GameObject boardBG;
	GameObject cardBoard;
	float cardWidth;
	float cardHeight;

	// Start is called before the first frame update
	void Start()
	{
		board = new GameObject[rows,columns];
		match = false;

		boardBG = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cardBoard = new GameObject("cards");
		cardWidth = deck[0].GetComponent<BoxCollider>().size.x;
		cardHeight = deck[0].GetComponent<BoxCollider>().size.y;

		// if the array has capacity, it is filled and the board is displayed
		if(rows + columns > 0) 
		{
			PopulateCardArray(CreateRandomCardArray(rows,columns));
			// PopulateCardArray(CreateStandardCardArray(rows, columns)); 
		
			DisplayBoard();
			boardBG.name = "Board";
			boardBG.GetComponent<MeshRenderer>().material = brownMat;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetMouseButtonDown(0)) {
			GameObjClicked();
		}
	}

	/// <summary>
	/// Instantiates card gameObjs and adds them to a board parent object
	/// </summary>
	void DisplayBoard()
	{
		// Loop through board, instantiating each gameObj with a gap
		for(int r = 0; r < board.GetLength(0); r++) {
			for(int c = 0; c < board.GetLength(1); c++) {
				GameObject newGO = Instantiate(
					board[r,c],
					new Vector3(
						c + c * columnGap + cardWidth / 2,
						r + r * rowGap + cardHeight / 2),
					Quaternion.identity);
				newGO.transform.parent = cardBoard.transform;
			}
		}

		// Center the cam and board 
		CenterView();
	}

	/// <summary>
	/// Centers the board and cam and resizes to be around all of the cards 
	/// </summary>
	void CenterView()
	{
		float xOffset = ((columns - 1) * (1 + columnGap) + cardWidth) / 2;
		float yOffset = ((rows - 1) * (1 + rowGap) + cardHeight) / 2;

		gameObject.GetComponent<GameManager>().ShiftCamera(Camera.main,new Vector3(xOffset,yOffset));
		boardBG.transform.position = new Vector3(xOffset,yOffset,3);
		boardBG.transform.localScale = new Vector3(xOffset * 2 + cardWidth,yOffset * 2 + cardHeight,1);
	}

	/// <summary>
	/// Fills the card array with strings from another array
	/// </summary>
	void PopulateCardArray(GameObject[,] givenArray)
	{
		// Checks if the given array is the same size as the card array
		if(givenArray.GetLength(0) != rows
			|| givenArray.GetLength(1) != columns) {
			Debug.Log("Error! Array is the wrong size");
			return;
		}
		else {
			for(int r = 0; r < rows; r++) {
				for(int c = 0; c < columns; c++) {
					board[r,c] = givenArray[r,c];
					board[r,c].GetComponent<Card>().row = r;
					board[r,c].GetComponent<Card>().column = c;
				}
			}
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
			// If a selected gameObj already exists, 
			// then it is "saved" as the previous selected gameObj
			if(selectedGameObj != null)
				prevSelectedGameObj = selectedGameObj;

			// Clicked gameObj is assigned
			selectedGameObj = rayHit.transform.gameObject;
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
		// Deactivates the matched gameObjs
		card1.SetActive(false);
		card2.SetActive(false);

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
		if(board.GetLongLength(0) == 0
			&& board.GetLongLength(1) == 0) {
			gameObject.GetComponent<GameManager>().ChangeGameState(GameState.End);
			gameObject.GetComponent<GameManager>().isWon = true;
		}
	}
}