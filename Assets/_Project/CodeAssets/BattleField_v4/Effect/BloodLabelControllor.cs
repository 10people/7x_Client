using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class BloodLabelControllor : MonoBehaviour 
{
	[HideInInspector] public List<Vector3> randomList = new List<Vector3>();


	private Dictionary<int, List<BloodLabel>> dict = new Dictionary<int, List<BloodLabel>>();

	private Dictionary<int, List<BloodLabel>> dictDroppen = new Dictionary<int, List<BloodLabel>>();

	private Dictionary<int, List<BloodLabel>> dictGenaral = new Dictionary<int, List<BloodLabel>>();

	private Dictionary<int, Dictionary<int, GameObject>> objectList = new Dictionary<int, Dictionary<int, GameObject>>();

	private Dictionary<int, int> listIndexList = new Dictionary<int, int> ();

	private Dictionary<int, GameObject> labelTempleList = new Dictionary<int, GameObject> ();


	private static BloodLabelControllor m_instance;

	public static BloodLabelControllor Instance() { return m_instance; }
	
	void Awake() { m_instance = this; }
	
	void OnDestroy() { 
		m_instance = null;
	}
	

	public void initStart ()
	{
		objectList.Clear ();

		labelTempleList.Clear ();

		for(int i = 0; i <= 5; i++ )
		{
			Dictionary<int, GameObject> list = new Dictionary<int, GameObject>();

			GameObject labelTemple = BattleControlor.Instance().getLabelTemplate (i);

			for(int j = 0; j < 20; j++)
			{
				GameObject labelObject = (GameObject)Instantiate (labelTemple.gameObject);

				labelObject.name = labelTemple.name + "_" + j;

				labelObject.transform.parent = BattleControlor.Instance().transform;

				labelObject.transform.localScale = labelTemple.transform.localScale;

				list.Add(j, labelObject);
			}

			objectList.Add(i, list);

			labelTempleList.Add(i, labelTemple);
		}

		listIndexList.Clear ();

		for(int i = 0; i <= 5; i++)
		{
			listIndexList.Add (i, 0);
		}

		randomList.Clear ();

		for(float x = 0; x < 1; x += .1f)
		{
			for(float z = 0; z < 1; z += .1f)
			{
				randomList.Add(new Vector3(x, 0, z));
			}
		}

		foreach(Dictionary<int, GameObject> list in objectList.Values)
		{
			foreach(GameObject labelObject in list.Values)
			{
				BloodLabel label = labelObject.GetComponent<BloodLabel>();
				
				labelObject.transform.localScale = labelTempleList[0].transform.localScale * 1.1f;
				
				label.text = "";
				
				label.showBloodEx (1, -50, null, null);
			}
		}

		dict.Clear ();

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			dict.Add(node.nodeId, new List<BloodLabel>());
		}

		dictDroppen.Clear ();

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			dictDroppen.Add(node.nodeId, new List<BloodLabel>());
		}
	}

	public void initStartGenarl (List<GameObject> _labelTempleList)
	{
		objectList.Clear ();
		
		labelTempleList.Clear ();
		
		for(int i = 0; i < _labelTempleList.Count; i++ )
		{
			Dictionary<int, GameObject> list = new Dictionary<int, GameObject>();
			
			GameObject labelTemple = _labelTempleList[i];
			
			for(int j = 0; j < 20; j++)
			{
				GameObject labelObject = (GameObject)Instantiate (labelTemple.gameObject);
				
				labelObject.name = labelTemple.name + "_" + j;
				
				labelObject.transform.parent = transform;
				
				labelObject.transform.localScale = labelTemple.transform.localScale;
				
				list.Add(j, labelObject);
			}
			
			objectList.Add(i, list);
			
			labelTempleList.Add(i, labelTemple);
		}

		listIndexList.Clear ();
		
		for(int i = 0; i <= 5; i++)
		{
			listIndexList.Add (i, 0);
		}
		
		randomList.Clear ();
		
		for(float x = 0; x < 1; x += .1f)
		{
			for(float z = 0; z < 1; z += .1f)
			{
				randomList.Add(new Vector3(x, 0, z));
			}
		}
		
		foreach(Dictionary<int, GameObject> list in objectList.Values)
		{
			foreach(GameObject labelObject in list.Values)
			{
				BloodLabel label = labelObject.GetComponent<BloodLabel>();
				
				labelObject.transform.localScale = labelTempleList[0].transform.localScale * 1.1f;
				
				label.text = "";
				
				label.showBloodEx (1, -50, null, null);
			}
		}
	}

	public void showBloodEx(BaseAI defender, int hpValue, bool cri, BattleControlor.AttackType _type)
	{
		List<BloodLabel> list = null;

		dict.TryGetValue (defender.nodeId, out list);

		if(list == null)
		{
			list = new List<BloodLabel>();

			dict.Add(defender.nodeId, list);
		}

		string text = "";

		if(_type == BattleControlor.AttackType.ADD_HP)
		{
			text = "+" + hpValue;
		}
		else
		{
			text = "  " + hpValue;
		}

		BloodLabel label = createBloodLabel (defender, text, cri, _type, list);
		
		foreach(BloodLabel bl in list)
		{
			bl.state = 1;

			label.strong = true;
		}

		list.Add (label);
	}

	public void showDroppenAwardEx(BaseAI defender, DroppenItem _item)
	{
		List<BloodLabel> list = null;
		
		dictDroppen.TryGetValue (defender.nodeId, out list);
		
		if(list == null)
		{
			list = new List<BloodLabel>();
			
			dictDroppen.Add(defender.nodeId, list);
		}

		CommonItemTemplate commonItemTemplate = CommonItemTemplate.getCommonItemTemplateById(_item.commonItemId);

		string text = NameIdTemplate.GetName_By_NameId (commonItemTemplate.nameId) + "x" + _item.num;

		BloodLabel label = createBloodLabel (defender, text, false, BattleControlor.AttackType.DEFAULT, list);
		
		foreach(BloodLabel bl in list)
		{
			bl.state = 1;
			
			label.strong = true;
		}
		
		list.Add (label);
	}

	public void showText(BaseAI defender, string text, EventDelegate.Callback p_callback = null)
	{
		if (text == null || text.Equals ("")) return;

		List<BloodLabel> list = null;
		
		dictDroppen.TryGetValue (defender.nodeId, out list);
		
		if(list == null)
		{
			list = new List<BloodLabel>();
			
			dictDroppen.Add(defender.nodeId, list);
		}
		
		BloodLabel label = createBloodLabel (defender, text, false, BattleControlor.AttackType.DEFAULT, list, p_callback);
		
		foreach(BloodLabel bl in list)
		{
			bl.state = 1;
			
			label.strong = true;
		}
		
		list.Add (label);
	}

	private BloodLabel createBloodLabel(BaseAI defender, string text, bool cri, BattleControlor.AttackType _type, List<BloodLabel> list, EventDelegate.Callback p_callback = null)
	{
		int labelType = BattleControlor.Instance().getLabelType (defender, _type);

		GameObject labelObject = objectList[labelType][listIndexList[labelType]];

		listIndexList [labelType] ++;

		listIndexList [labelType] = listIndexList [labelType] >= 20 ? 0 : listIndexList [labelType];

		if(labelObject == null)
		{
			GameObject labelTemple = BattleControlor.Instance().getLabelTemplate (defender, _type);
		
			labelObject = (GameObject)Instantiate (labelTemple.gameObject);

			labelObject.transform.parent = BattleControlor.Instance().transform;

			labelObject.transform.localScale = labelTemple.transform.localScale;
		}

		labelObject.SetActive (Console_SetBattleFieldFx.IsEnableBloodLabel());

		labelObject.transform.localPosition = defender.transform.localPosition + new Vector3 (0, 1.5f, 0);
		
		if(Camera.main != null && Camera.main.gameObject.activeSelf == true)
		{
			labelObject.transform.forward = Camera.main.transform.position - labelObject.transform.position;
		}
		
		float time = 0.5f;
		
		float ty = defender.getHeight();
		
		BloodLabel label = (BloodLabel)labelObject.GetComponent ("BloodLabel");
		
		if(cri == true)
		{
			labelObject.transform.localScale = labelTempleList[labelType].transform.localScale * 1.1f;

			label.text = LanguageTemplate.GetText((LanguageTemplate.Text)1232) + text;

			time = 1f;
		}
		else 
		{
			labelObject.transform.localScale = labelTempleList[labelType].transform.localScale * .7f;

			if(_type == BattleControlor.AttackType.BASE_REFLEX)
			{
				text = LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_BASE_REFLEX) + text;
			}
			else if(_type == BattleControlor.AttackType.SKILL_REFLEX)
			{
				text = LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_SKILL_REFLEX) + text;
			}

			label.text = text;
		}

		if(Console_SetBattleFieldFx.IsEnableBloodLabel()) label.showBloodEx (time, ty, list, p_callback);

		return label;
	}

	public BloodLabel createBloodLabelGenarl(GameObject host, string text, float height, bool cri, int labelType, EventDelegate.Callback p_callback = null)
	{
		GameObject labelObject = objectList[(int)labelType][listIndexList[(int)labelType]];
		
		listIndexList [(int)labelType] ++;
		
		listIndexList [(int)labelType] = listIndexList [(int)labelType] >= 20 ? 0 : listIndexList [(int)labelType];
		
		if(labelObject == null)
		{
			GameObject labelTemple = labelTempleList[labelType];
			
			labelObject = (GameObject)Instantiate (labelTemple.gameObject);
			
			labelObject.transform.parent = transform;
			
			labelObject.transform.localScale = labelTemple.transform.localScale;
		}
		
		labelObject.SetActive (Console_SetBattleFieldFx.IsEnableBloodLabel());
		
		labelObject.transform.localPosition = host.transform.localPosition + new Vector3 (0, 1.5f, 0);
		
		if(Camera.main != null && Camera.main.gameObject.activeSelf == true)
		{
			labelObject.transform.forward = Camera.main.transform.position - labelObject.transform.position;
		}
		
		float time = 0.5f;
		
		BloodLabel label = (BloodLabel)labelObject.GetComponent ("BloodLabel");
		
		if(cri == true)
		{
			labelObject.transform.localScale = labelTempleList[(int)labelType].transform.localScale * 1.1f;
			
			label.text = LanguageTemplate.GetText((LanguageTemplate.Text)1232) + text;
			
			time = 1f;
		}
		else 
		{
			labelObject.transform.localScale = labelTempleList[(int)labelType].transform.localScale * .7f;
			
			label.text = text;
		}

		List<BloodLabel> list = null;
		
		dictGenaral.TryGetValue (host.GetInstanceID(), out list);
		
		if(list == null)
		{
			list = new List<BloodLabel>();

			dictGenaral.Add(host.GetInstanceID(), list);
		}

		if(Console_SetBattleFieldFx.IsEnableBloodLabel()) label.showBloodEx (time, height, list, p_callback);
		
		return label;
	}

}
