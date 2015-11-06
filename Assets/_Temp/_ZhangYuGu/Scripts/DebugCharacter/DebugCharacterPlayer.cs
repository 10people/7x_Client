using UnityEngine;
using System.Collections;

public class DebugCharacterPlayer : MonoBehaviour
{
	private CharacterController character;


	void Start ()
	{
		character = this.GetComponent<CharacterController>();
	}
	
	void Update ()
	{
	
	}

	void LateUpdate()
	{
		updateCamera();
	}

	private void updateCamera()
	{
		Camera.main.transform.position = transform.position + new Vector3(0, 8.7f,  -10f);
	}

	public void move(Vector3 offset)
	{
		character.Move(offset * 6.0f * Time.deltaTime);
	}

}
