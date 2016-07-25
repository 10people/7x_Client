using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleArrowControllor : MonoBehaviour 
{
	public GameObject arrowTemple;

	public GameObject screenFlag;


	private Dictionary<int, GameObject> arrowList = new Dictionary<int, GameObject>();

	private List<GameObject> arrowTemplateList = new List<GameObject>();

	private List<int> tempRemove = new List<int>();

	private float width;

	private float height;


	void Start () 
	{
		arrowList.Clear ();

		arrowTemplateList.Clear ();

		for(int i = 0; i < 20; i ++)
		{
			GameObject arrowObject = Instantiate(arrowTemple) as GameObject;

			arrowObject.SetActive(false);

			arrowObject.transform.parent = transform;

			arrowObject.transform.localPosition = Vector3.zero;

			arrowObject.transform.localEulerAngles = Vector3.zero;

			arrowObject.transform.localScale = Vector3.one;

			arrowTemplateList.Add(arrowObject);
		}

		screenFlag.transform.parent = transform;

		width = screenFlag.transform.localPosition.x;

		height = screenFlag.transform.localPosition.y;
	}

	void Update () 
	{
		if (!BattleControlor.Instance ().completed) return;

		UpdateList ();

		updatePosition ();
	}

	private void UpdateList() 
	{
		tempRemove.Clear ();
		
		foreach(int id in arrowList.Keys)
		{
			BaseAI node = BattleControlor.Instance().getNodebyId(id);

			if(node == null || !node.gameObject.activeSelf || !node.isAlive || node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0)
			{
				arrowList[id].SetActive(false);

				tempRemove.Add(id);
			}
		}

		foreach(int id in tempRemove)
		{
			arrowList.Remove(id);
		}

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			if(node == null || !node.gameObject.activeSelf) continue;

			if(!node.flag.accountable) continue;

			if(node.nodeData.nodeType == qxmobile.protobuf.NodeType.GOD || node.nodeData.nodeType == qxmobile.protobuf.NodeType.NPC) continue;

			if(!node.isAlive || node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0) continue;

			if(!arrowList.ContainsKey(node.nodeId))
			{
				GameObject arrowObject = getNextArrow();

				arrowObject.SetActive(true);

				arrowList.Add(node.nodeId, arrowObject);
			}
		}
	}
	private void updatePosition()
	{
		foreach(int id in arrowList.Keys)
		{
			GameObject gc = arrowList[id];

			BaseAI node = BattleControlor.Instance().getNodebyId(id);

			BaseAI hero = BattleControlor.Instance().getNodebyId(1);

			Vector3 curPos = node.gameObject.transform.position;

			GameObject temp;
			if(Vector3.Distance(hero.gameObject.transform.position, node.gameObject.transform.position) >= 20)
			{
				Vector3 tempMove = node.gameObject.transform.position - hero.gameObject.transform.position;
				tempMove = tempMove.normalized * 20;
				node.gameObject.transform.position = hero.gameObject.transform.position + tempMove;
			}

			Vector3 screenPo = Camera.main.WorldToScreenPoint(node.transform.position);

//			Debug.Log();
			float angle = Vector3.Angle(Camera.main.transform.forward, node.transform.position - Camera.main.transform.position);

			float x = screenPo.x * width / Screen.width;

			float y = screenPo.y * height / Screen.height;

			screenPo = new Vector3(x, y, 0);

			//以左下角为0,0

			//怪在屏幕外
			if(screenPo.x < 0 || screenPo.y < 0 || screenPo.x > width || screenPo.y > height)
			{
				gc.SetActive(true);

				gc.transform.localPosition = screenPo;
				
				if(gc.transform.localPosition.x < width * .25f)
				{
					gc.transform.localPosition += new Vector3(width * .25f - gc.transform.localPosition.x, 0, 0);
				}
				
				if(gc.transform.localPosition.x > width * .75f)
				{
					gc.transform.localPosition += new Vector3(width * .75f - gc.transform.localPosition.x, 0, 0);
				}

				if(gc.transform.localPosition.y < height * .25f || angle > 90)
				{
					gc.transform.localPosition += new Vector3(0, height * .25f - gc.transform.localPosition.y, 0);
				}
				
				if(gc.transform.localPosition.y > height * .75f)
				{
					gc.transform.localPosition += new Vector3(0, height * .75f - gc.transform.localPosition.y, 0);
				}

				float z = Vector3.Angle(new Vector3(width / 2, height * .75f, 0) - new Vector3(width / 2, height / 2, 0), gc.transform.localPosition - new Vector3(width / 2, height / 2, 0));

				if(gc.transform.localPosition.x > width / 2) z = -z;

				gc.transform.localEulerAngles = new Vector3(0, 0, z);
			}
			else
			{
				gc.SetActive(false);

				gc.transform.localPosition = new Vector3(-width, -height, 0);
			}
			node.gameObject.transform.position = curPos;
//			Camera.main.gameObject.transform.position = temp1.position;
//			Camera.main.gameObject.transform.rotation = temp1.rotation;
//			Camera.main.gameObject.transform.position = temp1.position;
		}
	}

	private GameObject getNextArrow()
	{
		foreach(GameObject gc in arrowTemplateList)
		{
			if(!gc.activeSelf)
			{
				return gc;
			}
		}

		GameObject arrowObject = Instantiate(arrowTemple) as GameObject;
		
		arrowObject.SetActive(false);
		
		arrowObject.transform.parent = transform;
		
		arrowObject.transform.localPosition = Vector3.zero;
		
		arrowObject.transform.localEulerAngles = Vector3.zero;
		
		arrowObject.transform.localScale = Vector3.one;
		
		arrowTemplateList.Add(arrowObject);

		return arrowObject;
	}

}
