using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDisplayManager : MonoBehaviour
{
    // Fields set in inspector
    public float rowGap;
    public float columnGap;

    // Materials
    public Material brownMat;

    // Fields set in Start()
    GameObject boardBG;                 // the background gameObj
    public GameObject parentDisplay;    // the empty gameObj that holds the cards on the board
    int columns;                        // # of columns of the board
    int rows;                           // # of rows of the board
    float cardWidth;
    float cardHeight;

    // Start is called before the first frame update
    void Start()
    {
        // Background for the cards
        boardBG = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boardBG.name = "Board";
        boardBG.GetComponent<MeshRenderer>().material = brownMat;

        columns = gameObject.GetComponent<CardManager>().columns;
        rows = gameObject.GetComponent<CardManager>().numOfCards / columns;
        if(gameObject.GetComponent<CardManager>().numOfCards % columns > 0)
            rows++;

        cardWidth = gameObject.GetComponent<CardManager>().deck[0].GetComponent<BoxCollider>().size.x;
        cardHeight = gameObject.GetComponent<CardManager>().deck[0].GetComponent<BoxCollider>().size.y;

        if(gameObject.GetComponent<CardManager>().sceneBoard != null)
            parentDisplay = DisplayBoard(gameObject.GetComponent<CardManager>().sceneBoard,
                "cardBoard" + gameObject.GetComponent<CardManager>().timesDoubled);
    }

    // Update is called once per frame
    void Update()
    {
		
	}

    /// <summary>
    /// Instantiates card gameObjs and adds them to a board parent object
    /// </summary>
    /// <param name="board">The 2D array of GameObjs that are being created in the scene</param>
    /// <param name="emptyParentObjectName">The name of the new parent object that will 
    /// be created to hold all created GameObjs</param>
    /// <returns>Returns the empty gameObj that is the parent of all the displayed cards</returns>
    public GameObject DisplayBoard(List<GameObject> board,string emptyParentObjectName)
    {
		// New empty parent gameObj to hold all of the child card gameObjs
        GameObject newDisplay = new GameObject(emptyParentObjectName);

		// Loops thr the board and instantiates each card object
        int c = 0;
        int r = 0;
        foreach(GameObject card in board) {
            GameObject newCard = Instantiate(
                card,
                new Vector3(
                    c + c * columnGap + cardWidth / 2,
                    -r - r * rowGap - cardHeight / 2),
                Quaternion.identity,newDisplay.transform);
            newCard.name = card.GetComponent<Card>().ToString();
            newCard.SetActive(true);

            c++;
            if(c == columns) {
                c = 0;
                r++;
			}
        }
		// Sets the new row value
		rows = r;

		// If the board is being doubled, then old boards are deleted
		if(gameObject.GetComponent<CardManager>().timesDoubled > 0) {
            for(int i = gameObject.GetComponent<CardManager>().timesDoubled - 1; i >= 0; i--) {
                GameObject oldBoard = GameObject.Find("cardBoard" + i);
                if(oldBoard != null)
                    Destroy(oldBoard);
            }
		}

		CenterView();

		return newDisplay;
    }

    /// <summary>
    /// Centers the board and cam and resizes to be around all of the cards 
    /// </summary>
    public void CenterView()
    {
        // Calculates x and y offsets
        float xOffset = ((columns - 1) * (1 + columnGap) + cardWidth) / 2;
        float yOffset = ((rows - 1) * (1 + rowGap) + cardHeight) / 2;

		// Repositions the camera based on offsets if this is the first time the board is being displayed
		if(gameObject.GetComponent<CardManager>().timesDoubled == 0)
			gameObject.GetComponent<GameManager>().ShiftCamera(Camera.main,new Vector3(xOffset,-yOffset));

        // Resizes and repositions background object
        boardBG.transform.position = new Vector3(xOffset,-yOffset,3.0f);
        boardBG.transform.localScale = new Vector3(xOffset * 2 + cardWidth,yOffset * 2 + cardHeight,1);
    }

    /// <summary>
    /// Changes the gameState to the end state if all cards in the scene are deactivated
    /// </summary>
    public void CheckEndOfGame()
	{
        bool allCardsMatched = true;

        // Loops thr list of cards and checks if any are still active in the scene
        for(int i = 0; i < parentDisplay.transform.childCount; i++)
            if(parentDisplay.transform.GetChild(i).gameObject.activeSelf)
                allCardsMatched = false;

        // If the board is empty, the gameState is changed to the end state
        if(allCardsMatched)
            gameObject.GetComponent<GameManager>().ChangeGameState(GameState.End);
    }
}
