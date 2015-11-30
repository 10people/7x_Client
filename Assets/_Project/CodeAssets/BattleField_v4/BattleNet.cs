using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleNet : MonoBehaviour, SocketProcessor
{
	public DramaStoryReador storyReador;

	public GameObject flagRoot;

	public GameObject cameraFlagRoot;


	[HideInInspector] public List<int> m_listWantLoadEffID = new List<int>();
	
	[HideInInspector] public List<int> m_listWantLoadSoundID = new List<int>();

	[HideInInspector] public Dictionary<string, Object> modelList = new Dictionary<string, Object>();


	private bool sendingSecret;

	private static BattleNet _instance;
	
	private int modelCount;

	private int modelCountMax;

	private List<int> guangQiangLoadList = new List<int>();

	private int guangQiangCount;

	private int guangQiangTempCount;

	private InitProc curSecretResp;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );

		_instance = this;
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public static BattleNet Instance() { return _instance; }

	void Start()
	{

	}

	public void getData()
	{
		m_data_loaded = 0;

		CityGlobalData.m_showLevelupEnable = false;

		StartCoroutine ( LoadingBattleFlags() );

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan)
		{
			storyReador.SetLoadDoneCallback(LoadFlags);

			storyReador.checkin ();
		}
		else
		{
			LoadFlags();
		}
	}

	public void LoadFlags()
	{
		//BattleUIControlor.Instance ().LoadResultRes ();
		
		LoadBattleGroup ();

		LoadCameraFlag ();
		
		LoadDramaFlag ();
		
		LoadTemplateWin ();
	}

	IEnumerator LoadingBattleFlags()
	{
		while( m_data_loaded < BATTLE_DATA_TO_LOAD_COUNT )
		{
			yield return new WaitForEndOfFrame();
		}

//		EnterNextScene.LogTimeSinceLoading( "LoadingBattleFlags.Done" );

		LoadFlagsDone();
	}

	private static int m_data_loaded = 0;

	private const int BATTLE_DATA_TO_LOAD_COUNT	= 4;

	private void LoadFlagsDone()
	{
		BattleControlor.Instance().loadFlags();

		BattleControlor.Instance ().loadBuffFlags ();

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan && CityGlobalData.m_tempSection == 1 && CityGlobalData.m_tempLevel == 1)
		{
			OnSendPve();
	
			OnSendEnterBattle();
		}
		else
		{
			//CityGlobalData.t_resp.selfTroop.nodes[0].modleId = 1;

			loadModelEff (CityGlobalData.t_resp);
		}
	}

	private void OnSendPve()
	{
		PveZhanDouInitReq req = new PveZhanDouInitReq();
		
		req.chapterId = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;
		
		req.levelType = CityGlobalData.m_levelType;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ZhanDou_Init_Pve_Req, ref t_protof);
	}

	private void OnSendEnterBattle()
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_LOADING_NETWORK, "SendEnterBattle" );
		
		PlayerState t_state = new PlayerState();
		
		t_state.s_state = State.State_PVEOFBATTLE;
		
		SocketHelper.SendQXMessage( t_state, ProtoIndexes.PLAYER_STATE_REPORT );
	}

	private void LoadBattleGroup()
	{
		BattleFlagGroupTemplate.SetLoadDoneCallback( LoadBattleBuffFlag );
		
		BattleFlagGroupTemplate.LoadTemplates(CityGlobalData.m_configId);
	}

	private void LoadBattleBuffFlag()
	{
		BattleBuffTemplate.SetLoadDoneCallback (LoadBattleFlag);

		BattleBuffTemplate.LoadTemplates (CityGlobalData.m_configId);
	}

	private void LoadBattleFlag()
	{
		BattleFlagTemplate.SetLoadDoneCallback( LoadGuangQiang );

		BattleFlagTemplate.LoadTemplates(CityGlobalData.m_configId);
	}

	public void LoadGuangQiang()
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_LOADING_DATA, "" );

		guangQiangLoadList.Clear ();

		guangQiangCount = 0;

		guangQiangTempCount = 0;

		for(int i = 0; i < 5; i++)
		{
			guangQiangLoadList.Add(0);

			BattleControlor.Instance().GuangQiangList.Add(null);
		}

		foreach(BattleFlagTemplate bf in BattleFlagTemplate.templates)
		{
			if(bf.flagId > 1000 && bf.flagId < 2000)
			{
				float totalWidth = bf.cx;

				for(int i = 0; totalWidth > 2; i++)
				{
					if(totalWidth > 50)
					{
						guangQiangLoadList[4] ++;
						
						totalWidth -= 50;
					}
					else if(totalWidth > 20)
					{
						guangQiangLoadList[3] ++;
						
						totalWidth -= 20;
					}
					else if(totalWidth > 10)
					{
						guangQiangLoadList[2] ++;
						
						totalWidth -= 10;
					}
					else if(totalWidth > 5)
					{
						guangQiangLoadList[1] ++;
						
						totalWidth -= 5;
					}
					else
					{
						guangQiangLoadList[0] ++;
						
						totalWidth -= 2;
					}
				}
			}
		}

		for(int i = 0; i < guangQiangLoadList.Count; i++)
		{
			if(guangQiangLoadList[i] > 0)
			{
				guangQiangCount ++;

				Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId( 77 + i ).path, 
			                        LoadGuangQiangDone);
			}
		}

		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId( 76 ).path, 
		                        LoadGuangQiangDone_2);
	}

	public void LoadGuangQiangDone( ref WWW p_www, string p_path, Object p_object )
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_LOADING_FX, p_path );

		for(int i = 0; i < 5; i++)
		{
			if(p_path.Equals(EffectTemplate.getEffectTemplateByEffectId( 77 + i ).path))
			{
				BattleControlor.Instance().GuangQiangList[i] = (GameObject)p_object;
			}
		}

		guangQiangTempCount ++;

		if(BattleControlor.Instance ().GuangQiang_Forever != null && guangQiangTempCount >= guangQiangCount)
		{
			LoadBattleFlagDone();
		}
	}

	public void LoadGuangQiangDone_2(ref WWW p_www, string p_path, Object p_object)
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_LOADING_FX, p_path );

		BattleControlor.Instance ().GuangQiang_Forever = (GameObject)p_object;

		if(guangQiangTempCount >= guangQiangCount)
		{
			LoadBattleFlagDone();
		}
	}

	public void LoadBattleFlagDone()
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_CREATE_FLAGS, "BattleFlag" );

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

		foreach(BattleBuffTemplate template in BattleBuffTemplate.templates)
		{
			GameObject gc = new GameObject();

			gc.transform.parent = flagRoot.transform;

			gc.name = "Buff_" + template.flagId;

			gc.transform.position = new Vector3(template.x, template.y, template.z);
			
			gc.transform.eulerAngles = new Vector3(template.rx, template.ry, template.rz);
			
			gc.transform.localScale = new Vector3(template.cx, template.cy, template.cz);

			BattleBuffFlag buffBf = gc.AddComponent<BattleBuffFlag>();

			buffBf.flagId = template.flagId;

			buffBf.refreshTime = template.refreshTime;
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
			
			if(bf.flagId > 1000 && bf.flagId < 2000)
			{
				BattleControlor.Instance().createWall(bf);
			}
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

	private void LoadCameraFlag()
	{
		BattleCameraFlagTemplate.SetLoadDoneCallback( LoadCameraFlagDone );

		BattleCameraFlagTemplate.LoadTemplates(CityGlobalData.m_configId);
	}

	private void LoadCameraFlagDone()
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_CREATE_FLAGS, "CameraFlag" );
		
		List<BattleCameraFlag> flags = new List<BattleCameraFlag>();
		
		foreach( BattleCameraFlagTemplate template in BattleCameraFlagTemplate.templates )
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

		BattleDramaFlagTemplate.LoadTemplates(CityGlobalData.m_configId);
	}

	public void LoadDramaFlagDone()
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_CREATE_FLAGS, "DramaFlag" );

		List<BattleDramaFlag> flags = new List<BattleDramaFlag>();
		
		foreach( BattleDramaFlagTemplate template in BattleDramaFlagTemplate.templates )
		{
			GameObject gc = new GameObject();
			
			gc.transform.parent = cameraFlagRoot.transform;
			
			gc.name = "DramaFlag_" + template.flagId;
			
			gc.transform.localScale = new Vector3(1, 1, 1);
			
			gc.transform.position = new Vector3(template.x, template.y, template.z);

			gc.transform.localEulerAngles = new Vector3(template.rx, template.ry, template.rz);

			BattleDramaFlag bf = (BattleDramaFlag)gc.AddComponent<BattleDramaFlag>();
			
			bf.flagId = template.flagId;
			
			bf.eventId = template.eventId;
			
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

	private void LoadTemplateWin()
	{
		BattleWinTemplate.SetLoadDoneCallback (LoadBattleWinDone);

		BattleWinTemplate.LoadTemplates(CityGlobalData.m_configId);
	}

	public void LoadBattleWinDone()
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_CREATE_FLAGS, "DramaFlag" );
		
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

	private void OnSendSecret()
	{
		sendingSecret = true;

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_InitProc);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;

		switch( p_message.m_protocol_index )
		{
			case ProtoIndexes.ZhanDou_Init_Resp:
			{
				StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
			                         PrepareForBattleField.CONST_BATTLE_LOADING_NETWORK, "ZhanDou_Init_Resp" );

				MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );

				QiXiongSerializer t_qx = new QiXiongSerializer();

				ZhanDouInitResp resp = new ZhanDouInitResp();

				t_qx.Deserialize(t_stream, resp, resp.GetType());
			
				CityGlobalData.t_resp = resp;
				
				loadModelEff(resp);

				return true;
			}
//			case ProtoIndexes.S_ZHANDOU_INIT_ERROR:
//			{
//				MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
//				
//				QiXiongSerializer t_qx = new QiXiongSerializer();
//				
//				ZhanDouInitError resp = new ZhanDouInitError();
//				
//				t_qx.Deserialize(t_stream, resp, resp.GetType());
//			
//				initError(resp);
//
//				return true;
//			}
			case ProtoIndexes.S_InitProc:
			{
				MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				InitProc resp = new InitProc();

				t_qx.Deserialize(t_stream, resp, resp.GetType());
			
				initSecret(resp);

				return true;
			}
		}

		return false;
	}

	private void initError(ZhanDouInitError resp)
	{
		Debug.Log ("Battle Init Error Net:  " + resp.result);

		CityGlobalData.t_respError = resp;

		//Global.CreateBox ("", t_respError.result, null, null, "OK", null, clickErrorCallback);

		StartCoroutine(clickErrorCallback (0));
	}

	private void initSecret(InitProc resp)
	{
		curSecretResp = resp;

		StartCoroutine (initSecretAction());
	}

	IEnumerator initSecretAction()
	{
		float startTime = Time.realtimeSinceStartup;

		float targetTime = startTime + curSecretResp.c * .001f;

		for(;;)
		{
			float nowTime = Time.realtimeSinceStartup;

			if(nowTime > targetTime)
			{
				break;
			}

			yield return new WaitForEndOfFrame();
		}

		if(sendingSecret == true)
		{
			InProgress req = new InProgress ();
			
			req.a = curSecretResp.b;

			req.b = "#";

			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();

			SocketTool.Instance().SendSocketMessage((short)curSecretResp.a, ref t_protof);
		}
	}

	public void sendEndSecret()
	{
		sendingSecret = false;
		
		InProgress req = new InProgress ();
		
		req.a = curSecretResp.b;
		
		req.b = "#";

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_zlgdlc, ref t_protof);
	}

	public IEnumerator clickErrorCallback(int index)
	{
		EnterNextScene.Instance().DestroyUI();

		yield return new WaitForEndOfFrame ();

		GameObject root3d = GameObject.Find("BattleField_V4_3D");

		GameObject root2d = GameObject.Find("BattleField_V4_2D");

		//Destroy(root3d);

		//Destroy(root2d);
		
		//  SceneManager.EnterMainCity();
		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao)
		{
			CarriageSceneManager.Instance.ReturnCarriage();
		}
		else //if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
		{
			SceneManager.EnterMainCity();
		}
		//else
		//{
		//	SceneManager.EnterAllianceCity();
		//}
		
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("JunZhu")))
		{
			CityGlobalData.m_JunZhuCreate = true;
		}
	}

	private void loadModelEff( ZhanDouInitResp resp )
	{
		//CityGlobalData.t_resp = resp;

		CityGlobalData.m_pve_max_level = resp.selfTroop.maxLevel;

		BattleControlor.Instance ().HYK = resp.HYK;

		CityGlobalData.setDramable (resp.selfTroop.maxLevel);

		modelList.Clear ();

		modelCount = 0;

		{
			PreCountModels( resp );
		}

		//////////////////    preLoadEff   //////////////////

		for(int i = 0; i < CityGlobalData.t_resp.selfTroop.nodes.Count; i++)
		{
			Node nodeData = CityGlobalData.t_resp.selfTroop.nodes[i];
			
			for(int j = 0; nodeData.skills != null && j < nodeData.skills.Count; j++)
			{
				NodeSkill ns = nodeData.skills[j];

				wantAddEffID(SkillTemplate.setFirstLoadEffID(ns.value7));
			}

			if(nodeData.droppenItems != null)
			{
				m_listWantLoadEffID.Add(200000);

				m_listWantLoadEffID.Add(200001);
			}

			int model = 0;

			if(nodeData.nodeType == NodeType.PLAYER)
			{
				model = nodeData.modleId + 1001;
			}
			else
			{
				model = nodeData.modleId;
			}

			wantAddEffID(BattleEffectControllor.Instance(). getEffectIdByModelId(model));

			wantAddSoundID(BattleEffectControllor.Instance().getSoundIdByModelId(model));
		}
		
		for(int i = 0; i < CityGlobalData.t_resp.enemyTroop.nodes.Count; i++)
		{
			Node nodeData = CityGlobalData.t_resp.enemyTroop.nodes[i];
			
			for(int j = 0; nodeData.skills != null && j < nodeData.skills.Count; j++)
			{
				NodeSkill ns = nodeData.skills[j];
				
				wantAddEffID(SkillTemplate.setFirstLoadEffID(ns.value7));
			}

			if(nodeData.droppenItems != null)
			{
				m_listWantLoadEffID.Add(200000);

				m_listWantLoadEffID.Add(200001);
			}

			int model = 0;

			if(nodeData.nodeType == NodeType.PLAYER)
			{
				model = nodeData.modleId + 1001;
			}
			else
			{
				model = nodeData.modleId;
			}

			wantAddEffID(BattleEffectControllor.Instance(). getEffectIdByModelId(model));

			wantAddSoundID(BattleEffectControllor.Instance().getSoundIdByModelId(model));
		}

		effCount = 0;

		foreach(int effid in m_listWantLoadEffID)
		{
			//BattleEffectControllor.Instance ().LoadEffectByEffectId( effid, BattleControlor.Instance().LoadEffectCallback );

			BattleEffectControllor.Instance ().LoadEffectByEffectId( effid, loadModel );
		}

		//////////////////    preLoadEff  end  //////////////////
	}

	public void loadComplete()
	{
		BattleControlor.Instance ().nodeCreateComplete();

		OnSendSecret ();
	}

	private int effCount;

	public void loadModel()
	{
		//BattleControlor.Instance ().LoadEffectCallback ();

		effCount ++;

		if(effCount < m_listWantLoadEffID.Count)
		{
			return;
		}

		modelCountMax = CityGlobalData.t_resp.selfTroop.nodes.Count + CityGlobalData.t_resp.enemyTroop.nodes.Count;

		for(int i = 0; i < CityGlobalData.t_resp.selfTroop.nodes.Count; i++)
		{
			Node nodeData = CityGlobalData.t_resp.selfTroop.nodes[i];
			
			//nodeData.modleId = 3;//Debug

			for(int j = 0; j < nodeData.flagIds.Count; j++)
			{
				loadModel(nodeData, nodeData.flagIds[j]);
			}

			if(nodeData.weaponLight != null)
			{
				loadWeaponModel((int)nodeData.weaponLight.weaponId);

				modelCountMax ++;
			}

			if(nodeData.weaponHeavy != null)
			{
				loadWeaponModel((int)nodeData.weaponHeavy.weaponId);
				
				modelCountMax ++;
			}

			if(nodeData.weaponRanged != null)
			{
				loadWeaponModel((int)nodeData.weaponRanged.weaponId);
				
				modelCountMax ++;
			}
		}
		
		for(int i = 0; i < CityGlobalData.t_resp.enemyTroop.nodes.Count; i++)
		{
			Node nodeData = CityGlobalData.t_resp.enemyTroop.nodes[i];
			
			for(int j = 0; j < nodeData.flagIds.Count; j++)
			{
				loadModel(nodeData, nodeData.flagIds[j]);
			}

			if(nodeData.weaponLight != null)
			{
				loadWeaponModel((int)nodeData.weaponLight.weaponId);
				
				modelCountMax ++;
			}

			if(nodeData.weaponHeavy != null)
			{
				loadWeaponModel((int)nodeData.weaponHeavy.weaponId);
				
				modelCountMax ++;
			}

			if(nodeData.weaponRanged != null)
			{
				loadWeaponModel((int)nodeData.weaponRanged.weaponId);
				
				modelCountMax ++;
			}
		}

		loadWeaponModel (6001);//掉落物品金币

		loadWeaponModel (6002);//掉落物品钱袋

		modelCountMax += 2;

//		for(int i = 0; t_resp.selfTroop.mibaoIcons != null && i < t_resp.selfTroop.mibaoIcons.Count; i++)
//		{
//			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.EQUIP_ICON_PREFIX ) + t_resp.selfTroop.mibaoIcons[i], ResourceLoadCallback );
//		}
	}
	
	public void wantAddEffID(List<int> data)
	{
		for(int i = 0; i < data.Count; i ++)
		{
			if(data[i] == 0)
			{
				data.RemoveAt(i);

				i --;

				continue;
			}
		}

		EffectIdTemplate tempEffectTemp;

		for(int i = 0; i < data.Count; i ++)
		{
			if(!m_listWantLoadEffID.Contains(data[i]))
			{
				m_listWantLoadEffID.Add(data[i]);

				tempEffectTemp = EffectTemplate.getEffectTemplateByEffectId(data[i]);

				wantAddSoundID(tempEffectTemp.sound);
			}
		}
	}

	public void wantAddSoundID(string data)
	{
		List<int> tempLoadSoundID = new List<int>();

		string[] tempPath = data.Split('|');

		for(int q = 0; q < tempPath.Length; q ++)
		{
			string tp = tempPath[q];

			tempLoadSoundID.Add(int.Parse(tp));
		}

		for(int q = 0; q < tempLoadSoundID.Count; q ++)
		{
			if(tempLoadSoundID[q] == -1)
			{
				tempLoadSoundID.RemoveAt(0);
				q --;
				continue;
			}
		}
		for(int q = 0; q < tempLoadSoundID.Count; q ++)
		{
			if(!m_listWantLoadSoundID.Contains(tempLoadSoundID[q]))
			{
				m_listWantLoadSoundID.Add(tempLoadSoundID[q]);
			}
		}
	}

	private void PreCountModels( ZhanDouInitResp resp )
	{
		int t_count = 0;

		//t_count += resp.selfTroop.nodes.Count;

		for( int i = 0; i < CityGlobalData.t_resp.selfTroop.nodes.Count; i++ )
		{
			Node nodeData = CityGlobalData.t_resp.selfTroop.nodes[i];

			t_count += nodeData.flagIds.Count;
		}

		for( int i = 0; i < CityGlobalData.t_resp.enemyTroop.nodes.Count; i++ )
		{
			Node nodeData = CityGlobalData.t_resp.enemyTroop.nodes[i];

			t_count += nodeData.flagIds.Count;
		}

		{
			StaticLoading.LoadingSection t_loading = StaticLoading.GetSection( StaticLoading.m_loading_sections,
			                                                                  PrepareForBattleField.CONST_BATTLE_LOADING_3D );

			if( t_loading != null )
			{
				t_loading.SetTotalCount( t_count );
			}
			else
			{
				Debug.LogError( "Error In Finding." );
			}
		}
	}

	private string getNodePath( Node nodeData, int position ){
		int model = nodeData.modleId;

		model = nodeData.nodeType == NodeType.PLAYER ? model + 1001 : model;

		return ModelTemplate.getModelTemplateByModelId (model).path;
	}

	private void loadWeaponModel(int weaponModelId)
	{
		Global.ResourcesDotLoad (ModelTemplate.getModelTemplateByModelId(weaponModelId).path, ResourceLoadCallback);
	}

	private void loadModel( Node nodeData, int position )
	{
		Global.ResourcesDotLoad( getNodePath( nodeData, position ), ResourceLoadCallback );
	}

	public void ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object )
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_LOADING_3D, p_path );

		if(modelList.ContainsKey(p_path) == false)
		{
			modelList.Add (p_path, p_object);
		}

		modelCount ++;

		if( modelCount >= modelCountMax )
		{
			//initData();

			BattleControlor.Instance ().PreLoadEffect();
		}
//		else{
//			Debug.LogError( modelCount + " - " + 
//			               ( t_resp.enemys.Count + t_resp.selfs.Count + miBaoCount ) );
//		}
	}

	#region Load Init Data

	public void initData(){
//		Debug.Log( "initData()" );

		BattleControlor.Instance ().BattleStart();

		{
			PreCountEffects();
		}

		{
			LoadNodes();
		}
	}

	public void LoadOtherData()
	{
		EnterNextScene.Instance ().AddLoadingDoneCallback (loadComplete);

		BattleControlor.Instance ().battleId = CityGlobalData.t_resp.zhandouId;
		
		BattleControlor.Instance ().PreLoadSoundFx();

		BattleControlor.Instance ().setMiBaoId (CityGlobalData.t_resp.selfTroop.mibaoIcons);

		BattleControlor.Instance ().setStarTemp (CityGlobalData.t_resp.starTemp);
		
		BattleControlor.Instance ().setTimeLimit (CityGlobalData.t_resp.limitTime);

		BattleControlor.Instance ().setAchivmentId (CityGlobalData.t_resp.starTemp);
	}

	private void PreCountEffects()
	{
		int t_count = 0;

		for( int i = 0; i < CityGlobalData.t_resp.selfTroop.nodes.Count; i++ )
		{
			Node node = CityGlobalData.t_resp.selfTroop.nodes[i];

			int model = 0;
			
			if( node.nodeType == NodeType.PLAYER ){
				model = node.modleId + 1001;
			}
			else{
				model = node.modleId;
			}

			t_count = t_count + BattleEffectControllor.GetEffectCount( model );
		}
		
		for(int i = 0; i < CityGlobalData.t_resp.enemyTroop.nodes.Count; i++)
		{
			Node node = CityGlobalData.t_resp.enemyTroop.nodes[i];

			int model = 0;

			for( int j = 0; j < node.flagIds.Count; j++ ){
				if( node.nodeType == NodeType.PLAYER ){
					model = node.modleId + 1001;
				}
				else{
					model = node.modleId;
				}

				t_count = t_count + BattleEffectControllor.GetEffectCount( model );
			}
		}

		{
			StaticLoading.LoadingSection t_loading = StaticLoading.GetSection( StaticLoading.m_loading_sections,
			                                                                  PrepareForBattleField.CONST_BATTLE_LOADING_FX );
			
			if( t_loading != null ){
				t_loading.SetTotalCount( m_listWantLoadEffID.Count );
			}
			else{
				Debug.LogError( "Error In Finding." );
			}
		}
	}

	#endregion



	#region Load Nodes

	public class NodeToLoad
	{
		private Node m_node_data;
		
		private BaseAI.Stance m_stance;
		
		private int m_flag;
		
		private GameObject m_node_temple;

		private bool m_node_loaded = false;

		public NodeToLoad( Node p_node, BaseAI.Stance p_stance, int p_flag, GameObject p_node_temple ){
			m_node_data = p_node;

			m_stance = p_stance;

			m_flag = p_flag;

			m_node_temple = p_node_temple;
		}

		public void Exec(BattleNet net){
//			bool t_bool = (m_node_data == null);
//
//			Debug.Log( "BattleNet.Exec m_node_data: " + ( t_bool ? "m_node_data=null" : "m_node_data!=null" ) );
//
//			Debug.Log( "BattleNet.Exec m_node_temple: " + m_node_temple );

			BaseAI node = BattleControlor.Instance().CreateNode(
				m_node_data, 
				m_stance, 
				m_flag, 
				m_node_temple);

			if (m_stance == BaseAI.Stance.STANCE_SELF && m_node_data.nodeType == NodeType.PLAYER)
			{
				((KingControllor)node).maxLevel = CityGlobalData.t_resp.selfTroop.maxLevel;
			}
			else if(m_stance == BaseAI.Stance.STANCE_ENEMY && m_node_data.nodeType == NodeType.PLAYER)
			{
				if(CityGlobalData.t_resp.enemyTroop.maxLevel == 999)
				{
					((KingControllor)node).maxLevel = 200000;
				}
				else
				{
					((KingControllor)node).maxLevel = CityGlobalData.t_resp.enemyTroop.maxLevel;
				}
			}

			NodeLoaded ();
		}

		public void NodeLoaded()
		{
			m_node_loaded = true;
		}

		public bool IsLoaded()
		{
			return m_node_loaded;
		}
	}

	private List<NodeToLoad> m_load_nodes_list = new List<NodeToLoad>();
	
	private void LoadNodes()
	{
		{
//			EnterNextScene.LogTimeSinceLoading( "LoadNodes" );

			SetIsLogNodeLoading( true );
		}

		{
			m_load_nodes_list.Clear();
		}

		for( int i = 0; i < CityGlobalData.t_resp.selfTroop.nodes.Count; i++ ){
			Node node = CityGlobalData.t_resp.selfTroop.nodes[i];
			
			string nodePath = getNodePath(node, i);
			
			Object model;
			
			modelList.TryGetValue( nodePath, out model );

			{
				NodeToLoad t_to_load = new NodeToLoad(
					node, 
					BaseAI.Stance.STANCE_SELF, 
					i + 1, 
					model as GameObject );

				m_load_nodes_list.Add( t_to_load );
			}
		}
		
		for( int i = 0; i < CityGlobalData.t_resp.enemyTroop.nodes.Count; i++ ){
			Node node = CityGlobalData.t_resp.enemyTroop.nodes[i];
			
			for( int j = 0; j < node.flagIds.Count; j++ ){
				string nodePath = getNodePath(node, node.flagIds[j]);
				
				Object model;
				
				modelList.TryGetValue(nodePath, out model);

				{
					NodeToLoad t_to_load = new NodeToLoad(
						node, 
						BaseAI.Stance.STANCE_ENEMY, 
						node.flagIds[j], 
						model as GameObject);
					
					m_load_nodes_list.Add( t_to_load );
				}
			}
		}

//		Debug.Log( "Node Count: " + m_load_nodes_list.Count );

		StartCoroutine( LoadingNodes() );
	}

	IEnumerator LoadingNodes()
	{
		int t_loaded_count = 0;

		NodeToLoad t_cur_load = LoadNextNode();

		while( t_cur_load != null )
		{
			if( t_cur_load.IsLoaded() )
			{
				{
					t_loaded_count++;

					SetIsLogNodeLoading( false );
				}

				m_load_nodes_list.Remove( t_cur_load );

				t_cur_load = LoadNextNode();
			}

			yield return new WaitForFixedUpdate();
		}
	}

	public NodeToLoad LoadNextNode()
	{
		if( m_load_nodes_list.Count > 0 )
		{
			NodeToLoad t_to_load = m_load_nodes_list[ 0 ];

			t_to_load.Exec(this);

			return t_to_load;
		}
		else
		{
//			Debug.Log( "All Nodes Loaded." );

			{
//				LogTimeInfoItem( "Not.Found.In.Bundle", 
//				                UtilityTool.CONST_TIME_INFO_NOT_FOUND_IN_BUNDLE );

//				LogTimeInfoItem( "Create.Node", 
//				                UtilityTool.CONST_TIME_INFO_CREATE_NODE );

//				LogTimeInfoItem( "Create.Effect", 
//				                UtilityTool.CONST_TIME_INFO_CREATE_EFFECT );
			}

			LoadOtherData();

			return null;
		}
	}

	private static bool m_log_node_loading = false;

	public static bool IsLogNodeLoading(){
		return m_log_node_loading;
	}

	public static void SetIsLogNodeLoading( bool p_log_node_loading ){
		m_log_node_loading = p_log_node_loading;
	}

	#endregion

}