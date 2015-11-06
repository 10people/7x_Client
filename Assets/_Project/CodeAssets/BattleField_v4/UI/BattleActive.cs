using UnityEngine;
using System.Collections;

//createRole --TOB Del
public class BattleActive : MonoBehaviour
{
	public GameObject target;


	public void setActive(bool active)
	{
		target.SetActive (active);
	}

}
