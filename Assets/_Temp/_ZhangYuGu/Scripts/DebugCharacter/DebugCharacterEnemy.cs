using UnityEngine;
using System.Collections;

public class DebugCharacterEnemy : MonoBehaviour
{
	public GameObject player;

	private NavMeshAgent nav;

	void Start ()
	{
		nav = (NavMeshAgent)GetComponent("NavMeshAgent");
	}
	
	void Update ()
	{
		startFindPath();

		checkPath();
	}

	public void startFindPath()
	{
		//nav.SetDestination(new Vector3(0, transform.position.y, -5.0f));

		nav.SetDestination(player.transform.position);
	}

	private void checkPath()
	{

	}

}
