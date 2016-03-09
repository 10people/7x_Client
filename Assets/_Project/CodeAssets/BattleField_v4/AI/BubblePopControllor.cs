using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubblePopControllor : MonoBehaviour 
{
	public GameObject gcTemple;


	private Dictionary<int, BubblePopNode> bubblePopNodeDict = new Dictionary<int, BubblePopNode> ();


	private static BubblePopControllor _instance;


	void Awake()
	{
		_instance = this; 
	}
	
	public static BubblePopControllor Instance() 
	{
		return _instance; 
	}

	public void init()
	{
		bubblePopNodeDict.Clear ();
	}

	public BubblePopNode createBubblePopNode(BaseAI node)
	{
		int levelId = CityGlobalData.m_configId;
		
		int nodeId = node.nodeId;
		
		bool have = BubblePopTemplate.haveBubblePopTemplateByLevelIdAndNodeId (levelId, nodeId);
		
		if (have == false) return null;
		
		BubblePopNode bpn = node.gameObject.GetComponent<BubblePopNode>();

		if(bpn == null) bpn = node.gameObject.AddComponent<BubblePopNode>();

		bpn.dict = BubblePopTemplate.getBubblePopListByLevelIdAndNodeId (levelId, nodeId);
		
		bpn.node = node;

		bpn.gcTemple = gcTemple;

		bpn.node.bubblePopNode = bpn;

		bpn.init ();

		bubblePopNodeDict.Add (node.nodeId, bpn);

		return bpn;
	}

	public void triggerFuncOpenEye(int nodeId)
	{
		BubblePopNode bpn = null;
		
		bubblePopNodeDict.TryGetValue (nodeId, out bpn);
		
		if (bpn == null) return;
		
		BubblePopTemplate temp = null;
		
		bpn.dict.TryGetValue (0, out temp);
		
		if (temp == null) return;
		
		bpn.triggerFunc (temp);
	}

	public void triggerFuncSkill(int nodeId, int skillId)
	{
		BubblePopNode bpn = null;
		
		bubblePopNodeDict.TryGetValue (nodeId, out bpn);
		
		if (bpn == null) return;
		
		BubblePopTemplate temp = null;
		
		bpn.dict.TryGetValue (1, out temp);
		
		if (temp == null) return;

		if (temp.tp1 != skillId) return;

		bpn.triggerFunc (temp);
	}

	public void triggerFuncDie(int nodeId)
	{
		BubblePopNode bpn = null;
		
		bubblePopNodeDict.TryGetValue (nodeId, out bpn);
		
		if (bpn == null) return;
		
		BubblePopTemplate temp = null;
		
		bpn.dict.TryGetValue (2, out temp);
		
		if (temp == null) return;
		
		bpn.triggerFunc (temp);
	}

	public void triggerFuncBATC(int nodeId)
	{
		BubblePopNode bpn = null;
		
		bubblePopNodeDict.TryGetValue (nodeId, out bpn);
		
		if (bpn == null) return;

		BubblePopTemplate temp = null;
		
		bpn.dict.TryGetValue (5, out temp);
		
		if (temp == null) return;

		bpn.triggerFunc (temp);
	}

	public void closeAllBubble()
	{
		foreach(BubblePopNode bubble in bubblePopNodeDict.Values)
		{
			bubble.close();
		}

		BattleUIControlor.Instance().barAPC.closeBubble ();
	}

}
