using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DistanceFlag
{
	public List<Vector2> triggerDistance = new List<Vector2>();
	
	public int count;
	
	public int triggerFlag;
	
}

public class BattleDistanceFlag : MonoBehaviour
{
	public List<Vector2> triggerDistance = new List<Vector2>();
	
	public int count;
	
	public BattleFlag triggerFlag;

}
