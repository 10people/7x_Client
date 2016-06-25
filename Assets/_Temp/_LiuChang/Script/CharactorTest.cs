using UnityEngine;
using System.Collections;

public class CharactorTest : MonoBehaviour
{
	public CharacterController actor_1;

	public CharacterController actor_2;



	void Start () 
	{
		StartCoroutine (action());
	}
	
	IEnumerator action () 
	{
		yield return new WaitForSeconds(1f);

		Vector3 midPo = (actor_1.transform.position + actor_2.transform.position) / 2;

		for(;;)
		{
			yield return new WaitForEndOfFrame();

			step(actor_1, midPo);

			step(actor_2, midPo);
		}
	}

	private void step(CharacterController actor, Vector3 targetP)
	{
		if(Vector3.Distance(actor.transform.position, targetP) < Time.deltaTime * 7)
		{
			return;
		}

		actor.Move (targetP - actor.transform.position);

//		actor.Move ((targetP - actor.transform.position).normalized * Time.deltaTime * 7);
	}

}
