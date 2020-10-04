using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	MainMenu,
	Instructions,
	Game,
	Pause,
	End
}

public class GameManager : MonoBehaviour
{
	public GameState currentGameState;
	public bool isWon;
	
	// Start is called before the first frame update
    void Start()
    {
		currentGameState = GameState.Game;
		isWon = false;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ChangeGameState(GameState newGameState)
	{
		switch(newGameState) {
			case GameState.MainMenu:
				break;
			case GameState.Instructions:
				break;
			case GameState.Game:
				break;
			case GameState.Pause:
				break;
			case GameState.End:
				break;
		}
	}

	/// <summary>
	/// Shifts the given camera by a given vec3
	/// </summary>
	/// <param name="cam">The camera being shifted</param>
	/// <param name="shift">The amount the camrea is being moved</param>
	public void ShiftCamera(Camera cam,Vector3 shift)
	{
		Vector3 newPos = new Vector3();
		newPos.x = cam.transform.position.x + shift.x;
		newPos.y = cam.transform.position.y + shift.y;
		newPos.z = cam.transform.position.z + shift.z;

		cam.transform.position = newPos;
	}
}
