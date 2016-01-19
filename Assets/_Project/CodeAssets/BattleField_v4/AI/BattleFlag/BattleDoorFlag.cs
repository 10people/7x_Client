using UnityEngine;
using System.Collections;

public class BattleDoorFlag : MonoBehaviour 
{

	public int flagId;

	public int modelId;

	public int triggerCount;


	[HideInInspector] public DoorAI door;

	void Awake(){

	}

	void OnDestroy(){
		door = null;
	}

	public void trigger()
	{
		if(triggerCount > 0)
		{
			triggerCount --;
			
			if(triggerCount > 0) return;
		}

		if (door == null) door = GetComponent<DoorAI>();

		door.CloseDoor ();
	}

}
