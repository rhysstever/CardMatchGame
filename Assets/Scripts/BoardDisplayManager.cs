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
    GameObject boardBG;     // the background gameObj
    int rows;               // # of rows of the board
    int columns;            // # of columns of the board
    float cardWidth;
    float cardHeight;

    // Start is called before the first frame update
    void Start()
    {
        // Background for the cards
        boardBG = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boardBG.name = "Board";
        boardBG.GetComponent<MeshRenderer>().material = brownMat;

        rows = gameObject.GetComponent<CardManager>().rows;
        columns = gameObject.GetComponent<CardManager>().columns;
        cardWidth = gameObject.GetComponent<CardManager>().deck[0].GetComponent<BoxCollider>().size.x;
        cardHeight = gameObject.GetComponent<CardManager>().deck[0].GetComponent<BoxCollider>().size.y;
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
    public void DisplayBoard(GameObject[,] board,string emptyParentObjectName)
    {
        GameObject newDisplay = new GameObject(emptyParentObjectName);
        // Loop through board, instantiating each gameObj with a gap
        for(int r = 0; r < board.GetLength(0); r++) {
            for(int c = 0; c < board.GetLength(1); c++) {
                if(board[r,c] != null) {
                    GameObject newGO = Instantiate(
                        board[r,c],
                        new Vector3(
                            c + c * columnGap + cardWidth / 2,
                            r + r * rowGap + cardHeight / 2),
                        Quaternion.identity);
                    newGO.transform.parent = newDisplay.transform;
                }
            }
        }
    }

    /// <summary>
    /// Centers the board and cam and resizes to be around all of the cards 
    /// </summary>
    public void CenterView()
    {
        // Calculates x and y offsets
        float xOffset = ((columns - 1) * (1 + columnGap) + cardWidth) / 2;
        float yOffset = ((rows - 1) * (1 + rowGap) + cardHeight) / 2;

        // Repositions the camera and background object based on offsets
        gameObject.GetComponent<GameManager>().ShiftCamera(Camera.main,new Vector3(xOffset,yOffset));
        boardBG.transform.position = new Vector3(xOffset,yOffset,3);

        // Resizes background object
        boardBG.transform.localScale = new Vector3(xOffset * 2 + cardWidth,yOffset * 2 + cardHeight,1);
    }
}
