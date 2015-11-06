using UnityEngine;
using System.Collections;

public class ReplayCameraFocus : MonoBehaviour
{
	public float moveSpeed;


	private CharacterController character;


	void Start()
	{
		character = gameObject.GetComponent<CharacterController>();

		transform.position = BattleReplayControlor.Instance().enemyPositionFlag[0].transform.position;
	}

	void LateUpdate()
	{
		//updateCamera();
	}

	private void updateCamera()
	{
		Camera.main.transform.position = transform.position + new Vector3(0, 9.6f, 17.25f);
	}

	public void move(Vector3 offset)
	{
		character.Move(offset * moveSpeed * Time.deltaTime);
	}

}
