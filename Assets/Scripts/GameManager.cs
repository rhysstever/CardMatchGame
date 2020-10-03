using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
