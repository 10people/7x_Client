using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeXmlToJson : MonoBehaviour
{
	public List<int> levels;

	public GameObject flagRoot;
	
	public GameObject cameraFlagRoot;

	public GameObject dramaFlagRoot;
	

	private int level;

	private int index;

	private Dictionary<string, Object> modelList = new Dictionary<string, Object>();
	
	private int modelCount;

	private static int m_data_loaded = 0;
	
	private const int BATTLE_DATA_TO_LOAD_COUNT	= 5;

	
	void Start()
	{
		index = 0;

		for(int i = 0; i < 4; i++) levels.Add (1 + i);
		
		levels.Add (900);

		for(int i = 0; i < 14; i++) levels.Add (100001 + i);

		levels.Add (100101);

		for(int i = 0; i < 7; i++) levels.Add (100201 + i);

		for(int i = 0; i < 7; i++) levels.Add (100301 + i);

		for(int i = 0; i < 7; i++) levels.Add (100401 + i);

		for(int i = 0; i < 9; i++) levels.Add (100501 + i);

		for(int i = 0; i < 9; i++) levels.Add (100601 + i);

		for(int i = 0; i < 9; i++) levels.Add (100701 + i);

		for(int i = 0; i < 9; i++) levels.Add (100801 + i);

		for(int i = 0; i < 9; i++) levels.Add (100901 + i);

		for(int i = 0; i < 9; i++) levels.Add (101001 + i);

		for(int i = 0; i < 3; i++) levels.Add (300101 + i);

		for(int i = 0; i < 3; i++) levels.Add (300201 + i);

		for(int i = 0; i < 3; i++) levels.Add (300301 + i);

		writeNext ();
	}

	public void writeNext()
	{
		if(index >= levels.Count)
		{
			Debug.Log("ALL FLAG WRITE DONE !");

//			GameObject writorObject = GameObject.Find ("Writor");
//			
//			BattleFlagWritor writor = (BattleFlagWritor)writorObject.GetComponent ("BattleFlagWritor");
//			
//			writor.writeWall();

			return;
		}

		level = levels [index];

		clearRoot (flagRoot);

		clearRoot (cameraFlagRoot);

		clearRoot (dramaFlagRoot);

		clearWinFlag ();

		StartCoroutine (getData());

		index ++;
	}

	private void clearRoot(GameObject root)
	{
		for(int i = 0; i < root.transform.childCount; i++)
		{
			GameObject gc = root.transform.GetChild (i).gameObject;
			
			Destroy(gc);
		}
	}

	private void clearWinFlag()
	{
		BattleWinFlag[] flags = transform.parent.GetComponents<BattleWinFlag>();

		foreach(BattleWinFlag flag in flags)
		{
			Destroy(flag);
		}
	}

	private IEnumerator getData()
	{
		yield return new WaitForSeconds (0.5f);

		m_data_loaded = 0;
		
		StartCoroutine ( LoadingBattleFlags() );
		
		LoadBattleGroup ();

		LoadBattleBuffFlag ();

		LoadCameraFlag ();
		
		LoadDramaFlag ();

		LoadWinFlag ();
	}
	
	IEnumerator LoadingBattleFlags()
	{
		while( m_data_loaded < BATTLE_DATA_TO_LOAD_COUNT )
		{
			yield return new WaitForEndOfFrame();
		}
		
		LoadFlagsDone();
	}
	
	private void LoadFlagsDone()
	{
		GameObject writorObject = GameObject.Find ("Writor");

		BattleFlagWritor writor = (BattleFlagWritor)writorObject.GetComponent ("BattleFlagWritor");
	
		writor.chapterId = level;

		writor.checkout ();
	}

	private void LoadBattleGroup()
	{
		BattleFlagGroupTemplate.SetLoadDoneCallback( LoadBattleFlag );

		BattleFlagGroupTemplate.LoadTemplates (level);
	}
	
	private void LoadBattleFlag()
	{
		BattleFlagTemplate.SetLoadDoneCallback( LoadBattleFlagDone );
		
		BattleFlagTemplate.LoadTemplates(level);
	}

	public void LoadBattleFlagDone()
	{
		Dictionary<int, BattleFlagGroup> groupDict = new Dictionary<int, BattleFlagGroup> ();

		foreach(BattleFlagGroupTemplate template in BattleFlagGroupTemplate.templates)
		{
			GameObject gc = new GameObject();

			gc.transform.parent = flagRoot.transform;

			gc.name = "Group_" + template.groupId;

			gc.transform.localPosition = Vector3.zero;

			gc.transform.localEulerAngles = Vector3.zero;

			gc.transform.localScale = new Vector3(1, 1, 1);

			BattleFlagGroup group = gc.AddComponent<BattleFlagGroup>();

			group.groupId = template.groupId;

			group.maxActive = template.maxActive;

			group.delay = template.delay;

			groupDict.Add(group.groupId, group);
		}

		List<BattleFlag> flags = new List<BattleFlag>();
		
		foreach( BattleFlagTemplate template in BattleFlagTemplate.templates )
		{
			GameObject gc = new GameObject();
			
			gc.transform.parent = flagRoot.transform;
			
			gc.name = "Flag_" + template.flagId;
			
			gc.transform.position = new Vector3(template.x, template.y, template.z);
			
			gc.transform.eulerAngles = new Vector3(template.rx, template.ry, template.rz);
			
			gc.transform.localScale = new Vector3(template.cx, template.cy, template.cz);
			
			BattleFlag bf = (BattleFlag)gc.AddComponent<BattleFlag>();

			bf.flagId = template.flagId;

			bf.forwardFlagId = template.forwardFlagId;

			bf.triggerCount = template.triggerCount;
			
			bf.triggerFunc = (BattleFlag.TriggerFunc)template.triggerFunc;
			
			bf.willRelive = (template.willRelive != 0);
			
			bf.dieable = (template.dieable != 0);
			
			bf.accountable = (template.accountable != 0);

			bf.hideInDrama = (template.hideInDrama != 0);

			bf.hideWithDramable = (template.hideWithDramable != 0);

			bf.guideId = template.guideId;
			
			bf.hintLabelId = template.hintLabelId;

			{
				if(template.groupId == 0)
				{
					bf.flagGroup = null;
				}
				else
				{
					BattleFlagGroup group;

					groupDict.TryGetValue(template.groupId, out group);

					if(group != null)
					{
						gc.transform.parent = group.transform;

						bf.flagGroup = group;

						group.listFlags.Add(bf);
					}
				}
			}

			bf.nodeSkillAble.Clear();
			
			foreach(int skillId in template.nodeSkillAble)
			{
				bf.nodeSkillAble.Add(skillId);
			}
			
			string[] strAp = template.alarmPosition.Split(',');
			
			bf.alarmPosition = new Vector3(float.Parse(strAp[0]), float.Parse(strAp[1]), float.Parse(strAp[2]));
			
			string[] strsP = template.path.Split('|');
			
			foreach(string strP in strsP)
			{
				if(strP.Length == 0) continue;
				
				string[] strPos = strP.Split(',');
				
				Vector3 pos = new Vector3(float.Parse(strPos[0]), float.Parse(strPos[1]), float.Parse(strPos[2]));
				
				bf.hoverPath.Add(pos);
			}
			
			foreach(int eyei in template.triggerFlagEye2eye)
			{
				bf.triggerFlagEye2eyeInteger.Add(eyei);
			}
			
			foreach(int enteri in template.triggerFlagEnter)
			{
				bf.triggerFlagEnterInteger.Add(enteri);
			}
			
			foreach(int attacki in template.triggerFlagAttack)
			{
				bf.triggerFlagAttackInteger.Add(attacki);
			}
			
			foreach(int killi in template.triggerFlagKill)
			{
				bf.triggerFlagKillInteger.Add(killi);
			}
			
			foreach(Vector2 bloodVec in template.triggerFlagBlood)
			{
				bf.triggerFlagBloodInteger.Add(bloodVec);
			}
			
			foreach(DistanceFlag df in template.triggerFlagDistance)
			{
				bf.triggerFlagDistance.Add(df);
			}
			
			flags.Add(bf);
		}
		
		foreach( BattleFlag bf in flags )
		{
			bf.triggerFlagEye2eye.Clear();
			
			foreach(int eyei in bf.triggerFlagEye2eyeInteger)
			{
				BattleFlag temp = getFlagById(flags, eyei);
				
				if(temp == null) continue;
				
				bf.triggerFlagEye2eye.Add(temp);
			}
			
			bf.triggerFlagEnter.Clear();
			
			foreach(int enteri in bf.triggerFlagEnterInteger)
			{
				BattleFlag temp = getFlagById(flags, enteri);
				
				if(temp == null) continue;
				
				bf.triggerFlagEnter.Add(temp);
			}
			
			bf.triggerFlagAttack.Clear();
			
			foreach(int attacki in bf.triggerFlagAttackInteger)
			{
				BattleFlag temp = getFlagById(flags, attacki);
				
				if(temp == null) continue;
				
				bf.triggerFlagAttack.Add(temp);
			}
			
			bf.triggerFlagKill.Clear();
			
			foreach(int killi in bf.triggerFlagKillInteger)
			{
				BattleFlag temp = getFlagById(flags, killi);
				
				if(temp == null) continue;
				
				bf.triggerFlagKill.Add(temp);
			}
		}
		
		{
			m_data_loaded++;
		}
	}

	private void LoadBattleBuffFlag()
	{
		BattleBuffTemplate.SetLoadDoneCallback (LoadBattleBuffFlagDone);
		
		BattleBuffTemplate.LoadTemplates (level);
	}

	private void LoadBattleBuffFlagDone()
	{
		List<BattleBuffFlag> buffFlags = new List<BattleBuffFlag> ();

		foreach(BattleBuffTemplate template in BattleBuffTemplate.templates)
		{
			GameObject gc = new GameObject();

			gc.transform.parent = flagRoot.transform;

			gc.name = "BuffFlag_" + template.flagId;

			gc.transform.localScale = new Vector3(1, 1, 1);

			gc.transform.position = new Vector3(template.x, template.y, template.z);

			BattleBuffFlag bf = gc.AddComponent<BattleBuffFlag>();

			bf.flagId = template.flagId;

			bf.refreshTime = template.refreshTime;

			buffFlags.Add(bf);
		}

		{
			m_data_loaded++;
		}
	}

	private void LoadCameraFlag()
	{
		BattleCameraFlagTemplate.SetLoadDoneCallback( LoadCameraFlagDone );
		
		BattleCameraFlagTemplate.LoadTemplates(level);
	}
	
	private void LoadCameraFlagDone()
	{
		List<BattleCameraFlag> flags = new List<BattleCameraFlag>();
		
		foreach(BattleCameraFlagTemplate template in BattleCameraFlagTemplate.templates)
		{
			GameObject gc = new GameObject();
			
			gc.transform.parent = cameraFlagRoot.transform;
			
			gc.name = "CameraFlag_" + template.flagId;
			
			gc.transform.localScale = new Vector3(1, 1, 1);
			
			gc.transform.position = new Vector3(template.x, template.y, template.z);
			
			BattleCameraFlag bf =  (BattleCameraFlag)gc.AddComponent<BattleCameraFlag>();
			
			bf.flagId = template.flagId;
			
			bf.radius = template.radius;
			
			bf.cameraPosition = new Vector3(template.px, template.py, template.pz);
			
			bf.cameraRotation = new Vector3(template.rx, template.ry, template.rz);

			bf.camera4Param = new Vector4(template.ex, template.ey, template.ez, template.ew);

			bf.killMin = template.killMin;

			bf.killMax = template.killMax;

			flags.Add(bf);
		}
		
		{
			m_data_loaded++;
		}
	}
	
	private void LoadDramaFlag()
	{
		BattleDramaFlagTemplate.SetLoadDoneCallback( LoadDramaFlagDone );

		BattleDramaFlagTemplate.LoadTemplates(level);
	}

	public void LoadDramaFlagDone()
	{
		List<BattleDramaFlag> flags = new List<BattleDramaFlag>();
		
		foreach(BattleDramaFlagTemplate template in BattleDramaFlagTemplate.templates)
		{
			GameObject gc = new GameObject();
			
			gc.transform.parent = dramaFlagRoot.transform;
			
			gc.name = "DramaFlag_" + template.flagId;
			
			gc.transform.localScale = new Vector3(1, 1, 1);
			
			gc.transform.position = new Vector3(template.x, template.y, template.z);

			gc.transform.localEulerAngles = new Vector3(template.rx, template.ry, template.rz);

			BattleDramaFlag bf = (BattleDramaFlag)gc.AddComponent<BattleDramaFlag>();
			
			bf.flagId = template.flagId;

			bf.nodeId = template.nodeId;

			//triggerFlagList
			foreach(int triggerI in template.triggerFlag)
			{
				bf.triggerFlagListInteger.Add(triggerI);
			}
			
			BoxCollider bc = (BoxCollider)gc.AddComponent<BoxCollider>();
			
			bc.size = new Vector3(template.cx, template.cy, template.cz);
			
			bc.center = Vector3.zero;
			
			bc.isTrigger = true;
			
			flags.Add(bf);
		}
		
		{
			m_data_loaded++;
		}
	}

	private void LoadWinFlag()
	{
		BattleWinTemplate.SetLoadDoneCallback( LoadWinFlagDone );
		
		BattleWinTemplate.LoadTemplates(level);
	}

	public void LoadWinFlagDone()
	{
		foreach(BattleWinTemplate template in BattleWinTemplate.templates)
		{
			BattleWinFlag flag = transform.parent.gameObject.AddComponent<BattleWinFlag>();

			flag.winId = 1;

			flag.winType = template.winType;

			flag.killNum = template.killNum;

			if(Vector3.Distance(template.destination, Vector3.zero) > .2f)
			{
				flag.destinationObject = new GameObject();

				flag.destinationObject.transform.position = template.destination;
			}
			else
			{
				flag.destinationObject = null;
			}

			flag.showOnUI = template.showOnUI != 0;

			flag.destinationRadius = template.destinationRadius;

			flag.protectNodeId = template.protectNodeId;
		}

		{
			m_data_loaded++;
		}
	}

	private BattleFlag getFlagById(List<BattleFlag> flags, int flagId)
	{
		foreach(BattleFlag bf in flags)
		{
			if(bf.flagId == flagId)
			{
				return bf;
			}
		}
		
		return null;
	}

}
