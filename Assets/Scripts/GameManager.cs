using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	MainMenu,
	Instructions,
	Credits,
	Game,
	Pause,
	End
}

public class GameManager : MonoBehaviour
{
	public GameState currentGameState;
	
	// Start is called before the first frame update
    void Start()
    {
		currentGameState = GameState.Game;
	}

    // Update is called once per frame
    void Update()
    {
		// Moves the Camera up (W) and down (S)
		if(Input.GetKey(KeyCode.W))
			ShiftCamera(Camera.main,new Vector3(0.0f,0.01f,0.0f));
		else if(Input.GetKey(KeyCode.S))
			ShiftCamera(Camera.main,new Vector3(0.0f,-0.01f,0.0f));

		switch(currentGameState) {
			case GameState.MainMenu:
				break;
			case GameState.Instructions:
				break;
			case GameState.Credits:
				break;
			case GameState.Game:
				break;
			case GameState.Pause:
				break;
			case GameState.End:
				break;
		}
	}

	public void ChangeGameState(GameState newGameState)
	{
		switch(newGameState) {
			case GameState.MainMenu:
				currentGameState = GameState.MainMenu;
				break;
			case GameState.Instructions:
				currentGameState = GameState.Instructions;
				break;
			case GameState.Credits:
				currentGameState = GameState.Credits;
				break;
			case GameState.Game:
				currentGameState = GameState.Game;
				break;
			case GameState.Pause:
				currentGameState = GameState.Pause;
				break;
			case GameState.End:
				currentGameState = GameState.End;
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
