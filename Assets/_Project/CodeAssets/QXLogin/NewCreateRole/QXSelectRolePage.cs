using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXSelectRolePage : GeneralInstance<QXSelectRolePage> {

	public delegate void RoleDelegate ();
	public RoleDelegate M_RoleDelegate;

	public enum SelectType
	{
		CREATE_ROLE,
		UNLOCK_ROLE,
	}
	private SelectType m_selectType;

	new void Awake ()
	{
		base.Awake ();
	}

	#region RoleModel
	public GameObject m_roleParent;
	private List<GameObject> m_roleObjList = new List<GameObject>();
	private Vector3 loadSize = Vector3.one;
	#endregion

	#region RoleUI
	public GameObject m_roleObj;
	private List<GameObject> m_roleList = new List<GameObject> ();
	public UISprite m_nameSprite;
	public UILabel m_nameDesLabel;
	#endregion

	//0-desid 1-selectName 2-unSelectName
	public readonly Dictionary<int,string[]> M_RoleDic = new Dictionary<int, string[]>
	{
		{0,new string[]{"11","role10","role11"}},
		{1,new string[]{"12","role20","role21"}},
		{2,new string[]{"21","role30","role31"}},
		{3,new string[]{"22","role40","role41"}},
	};

	private int m_roleId = -1;

	private bool m_leftPress = false;
	private bool m_rightPress = false;

	public void InItRolePage (SelectType tempSelectType,ErrorMessage tempMsg)
	{
		m_selectType = tempSelectType;
		m_unLockMsg = tempMsg;

		StopCoroutine ("UnLockCd");
		if (tempSelectType == SelectType.UNLOCK_ROLE)
		{
			m_isShowCd = true;
			string[] activeStr = m_unLockMsg.errorDesc.Split ('#')[1].Split (',');
			m_unLockIdList.Clear ();
			foreach (string s in activeStr)
			{
				m_unLockIdList.Add (int.Parse (s) - 1);
			}

			m_cdTime = int.Parse (m_unLockMsg.errorDesc.Split ('#')[0]);

			StartCoroutine ("UnLockCd");
		}
		else
		{
			if (m_inputList.Count == 0)
			{
				for (int i = 0;i < 4;i ++)
				{
					GameObject inputObj = (GameObject)Instantiate (m_inputObj.gameObject);
					inputObj.transform.parent = m_inputObj.transform.parent;
					inputObj.name = "Input" + i;
					inputObj.transform.localPosition = m_inputObj.transform.localPosition;
					inputObj.transform.localScale = m_inputObj.transform.localScale;
					m_inputList.Add (inputObj.GetComponent<UIInput> ());
				}
			}
		}

		//Create RoleBtnList
		m_roleList = QXComData.CreateGameObjectList (m_roleObj,4,m_roleList);
		for (int i = 0;i < m_roleList.Count;i ++)
		{
			m_roleList[i].name = i.ToString ();
			m_roleList[i].transform.localPosition = new Vector3(170,180 - 140 * i,0);
		}

		//Create RoleModelList
		if (m_roleObjList.Count == 0)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.HAOJIE_CREATE_ROLE ),HaoJieLoadCallback );
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.RUYA_CREATE_ROLE ),RuYaLoadCallback );
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.YUJIE_CREATE_ROLE ),YuJieLoadCallback );
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LUOLI_CREATE_ROLE ),LuoLiLoadCallback );
		}
		else
		{
			AddLoadIndex ();
		}
	}

	void HaoJieLoadCallback( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject haojie = Instantiate( p_object ) as GameObject;
		
		haojie.name = "0";
		haojie.SetActive (false);
		haojie.transform.parent = m_roleParent.transform;
		haojie.transform.localPosition = new Vector3(0,0.04f,0);
		haojie.transform.localScale = loadSize * 0.8f;
		
		m_roleObjList.Add ( haojie );
		
		AddLoadIndex ();
	}
	
	void RuYaLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject ruya = Instantiate( p_object ) as GameObject;
		
		ruya.name = "1";
		ruya.SetActive (false);
		ruya.transform.parent = m_roleParent.transform;
		ruya.transform.localPosition = new Vector3(0,0.035f,0);
		ruya.transform.localScale = loadSize * 0.7f;
		
		m_roleObjList.Add ( ruya );
		
		AddLoadIndex ();
	}
	
	void YuJieLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject yujie = Instantiate( p_object ) as GameObject;
		
		yujie.name = "2";
		yujie.SetActive (false);
		yujie.transform.parent = m_roleParent.transform;
		yujie.transform.localPosition = new Vector3(0,0.03f,0);
		yujie.transform.localScale = loadSize * 0.7f;
		
		m_roleObjList.Add ( yujie );
		
		AddLoadIndex ();
	}
	
	void LuoLiLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject luoli = Instantiate( p_object ) as GameObject;
		
		luoli.name = "3";
		luoli.SetActive (false);
		luoli.transform.parent = m_roleParent.transform;
		luoli.transform.localPosition = new Vector3(0,0.03f,0);
		luoli.transform.localScale = loadSize * 0.7f;
		
		m_roleObjList.Add ( luoli );

		AddLoadIndex ();
	}

	void AddLoadIndex ()
	{	
		if (m_roleObjList.Count == 4)
		{
			Debug.Log ("AddLoadIndex");
			switch (m_selectType)
			{
			case SelectType.CREATE_ROLE:
				
				ClientMain.m_sound_manager.chagneBGSound (1000);
				QXComData.SendQxProtoMessage (ProtoIndexes.C_OPEN_CREATE_ROLE);
				// report when battle field is ready
				OperationSupport.ReportClientAction( OperationSupport.ClientAction.CREATE_ROLE );

				SetObjListActiveState (m_createRoleObjList,true);
				SetObjListActiveState (m_unLockRoleObjList,false);
				m_changeRoleBtn.SetActive (false);

				SelectRole (Random.Range (0,m_roleObjList.Count));
				NameInIt ();

				break;
			case SelectType.UNLOCK_ROLE:

				SetObjListActiveState (m_createRoleObjList,false);
				SetObjListActiveState (m_unLockRoleObjList,true);
				Debug.Log ("CityGlobalData.m_king_model_Id:" + CityGlobalData.m_king_model_Id);
				SelectRole (m_roleId == -1 ? CityGlobalData.m_king_model_Id - 1 : m_roleId);

				break;
			default:
				break;
			}
		}
	}

	//选择人物
	void SelectRole (int tempRoleId)
	{	
		m_roleId = tempRoleId;

		for (int i = 0;i < m_roleObjList.Count;i ++)
		{
			m_roleObjList[i].SetActive (m_roleObjList[i].name == m_roleId.ToString () ? true : false);
			if (m_roleObjList[i].name == m_roleId.ToString ())
			{	
				RoleRotate roleRotate = m_roleObjList[i].GetComponent<RoleRotate> ();
				roleRotate.rotateObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				
				Animator roleAnimat = m_roleObjList[i].GetComponent<Animator> ();
				roleAnimat.SetTrigger ("Show");
			}

			if (m_selectType == SelectType.CREATE_ROLE)
			{
				m_inputList[i].gameObject.SetActive (i == m_roleId ? true : false);
			}

			QXRoleItem roleItem = m_roleList[i].GetComponent<QXRoleItem> ();
			roleItem.InItRole (m_selectType,i,m_roleId,m_unLockIdList);
		}

		if (m_selectType == SelectType.UNLOCK_ROLE)
		{
			m_changeRoleBtn.SetActive (m_roleId == CityGlobalData.m_king_model_Id - 1 ? false : true);
			if (m_unLockIdList.Contains (m_roleId))
			{
				m_costLabel.gameObject.SetActive (false);
				m_desLabel1.text = m_roleId == CityGlobalData.m_king_model_Id - 1 ? "[10ff2b]形象使用中[-]" : "[00bfe8]已拥有该形象[-]";
				m_changeRoleBtn.GetComponent<UISprite> ().spriteName = "changerole";
				m_isShowCd = true;
				if (m_cdTime == 0)
				{
					m_cdLabel.text = "[00bfe8]每次切换形象有48小时冷却时间[-]";
				}
			}
			else
			{
				//达到几级可购买
				m_costLabel.gameObject.SetActive (true);
				m_costLabel.text = CanshuTemplate.GetValueByKey (CanshuTemplate.UNLOCK_ROLE_PIRCE).ToString ();
				m_vip.spriteName = "v" + VipFuncOpenTemplate.GetNeedLevelByKey (25);
				m_desLabel1.text = "";
				m_changeRoleBtn.GetComponent<UISprite> ().spriteName = "buyrole";
				m_isShowCd = false;
				m_cdLabel.text = MyColorData.getColorString (5,"注意：切换形象并不能增加战力");
			}
		}

		m_nameSprite.spriteName = "roleName" + (m_roleId + 1);
		m_nameDesLabel.text = DescIdTemplate.GetDescriptionById (int.Parse (M_RoleDic[m_roleId][0]));
	}

	void SetObjListActiveState (List<GameObject> tempList,bool tempActive)
	{
		foreach (GameObject obj in tempList)
		{
			obj.SetActive (tempActive);
		}
	}

	#region CreateRole

	public List<GameObject> m_createRoleObjList = new List<GameObject>();

	public UIInput m_inputObj;
	private List<UIInput> m_inputList = new List<UIInput> ();

	public UISprite m_zheZhao;

	/// <summary>
	/// Names the in it.
	/// </summary>
	void NameInIt ()
	{
		if (ThirdPlatform.IsMyAppAndroidPlatform ())
		{
			if (ThirdPlatform.Instance() != null)
			{
				if (ThirdPlatform.Instance().HavePersonInfo())
				{
					Debug.Log( "Init With Platform NickName: " + ThirdPlatform.Instance().GetNickName() );
					
					string t_nick_name = ThirdPlatform.Instance().GetNickName();
					
					string t_final_nick_name = QXComData.TextLengthLimit( QXComData.StrLimitType.CREATE_ROLE_NAME, t_nick_name );
					
					if (QXComData.GetAvailableTextLength (QXComData.StrLimitType.CREATE_ROLE_NAME, t_nick_name) > QXComData.GetCreateRoleNameLimit())
					{
						t_final_nick_name = t_final_nick_name.Substring (0, QXComData.GetCreateRoleNameLimit() - 2);
						
						t_final_nick_name = t_final_nick_name + "..";
					}
					
					foreach (UIInput input in m_inputList)
					{
						input.value = t_final_nick_name;
					}
				}
			}
		}
		else
		{
			for (int i = 0;i < m_inputList.Count;i ++)
			{
				// custom ramdom
				int randomXin = 0;
				string xin = "";
				if (i < 2)
				{
					randomXin = Random.Range (0,NameKuTemplate.nameList1.Count - 1);
					xin = NameKuTemplate.nameList1[randomXin];
					int randomNan = Random.Range (0,NameKuTemplate.nameList2.Count - 1);
					string nan = NameKuTemplate.nameList2[randomNan];
					m_inputList[i].value = xin + nan;
				}
				else
				{
					randomXin = Random.Range (0,NameKuTemplate.nameList1.Count - 1);
					xin = NameKuTemplate.nameList1[randomXin];
					int randomNv = Random.Range (0,NameKuTemplate.nameList3.Count - 1);
					string nv = NameKuTemplate.nameList3[randomNv];
					m_inputList[i].value = xin + nv;
				}
			}
		}
	}
	
	/// <summary>
	/// Randoms the name.
	/// </summary>
	public void RandomName ()
	{
		Debug.Log( "RandomNameBtn()" );
		
		int randomXin = Random.Range (0,NameKuTemplate.nameList1.Count);
		string xin = NameKuTemplate.nameList1[randomXin];
		
		if (m_roleId == 0 || m_roleId == 1)
		{
			int randomNan = Random.Range (0,NameKuTemplate.nameList2.Count);
			string nan = NameKuTemplate.nameList2[randomNan];
			m_inputList[m_roleId].value = xin + nan;
		}
		else
		{
			int randomNv = Random.Range (0,NameKuTemplate.nameList3.Count);
			string nv = NameKuTemplate.nameList3[randomNv];
			m_inputList[m_roleId].value = xin + nv;
		}
	}

	public void OnNameSubmit ()
	{
		m_zheZhao.gameObject.SetActive (false);
		m_inputList[m_roleId].value = QXComData.TextLengthLimit (QXComData.StrLimitType.CREATE_ROLE_NAME,m_inputList[m_roleId].value);
	}

	#endregion

	#region UnLockRole

	private ErrorMessage m_unLockMsg;

	public List<GameObject> m_unLockRoleObjList = new List<GameObject>();

	public GameObject m_changeRoleBtn;

	public UILabel m_desLabel1;
	public UILabel m_cdLabel;
	public UILabel m_costLabel;
	public UISprite m_vip;

	private List<int> m_unLockIdList = new List<int>();
	private int m_cdTime;

	private bool m_isShowCd = true;

	public void RefreshSelectRole (int tempRoleId)
	{
		SelectRole (tempRoleId - 1);
	}

	IEnumerator UnLockCd ()
	{
		while (m_cdTime > 0)
		{
			m_cdTime --;
			yield return new WaitForSeconds (1);

			if (m_isShowCd)
			{
				if (m_cdTime == 0)
				{
					m_cdLabel.text = "[00bfe8]每次切换形象有48小时冷却时间[-]";
				}
			}
		}
	}

	#endregion

	#region BuyRole

	public GameObject m_buyRoleObj;

	void BuyRoleCallBack ()
	{
		m_buyRoleObj.SetActive (false);
	}

	#endregion

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "RandomBtn":

			RandomName ();

			break;
		case "StartBtn":

			QXSelectRole.Instance.CreatRole (m_inputList[m_roleId].value,m_roleId);

			break;
		case "CloseBtn":

			m_roleId = -1;
			M_RoleDelegate ();

			break;
		case "ZheZhao":

			OnNameSubmit ();

			break;
		case "ChangeRoleBtn":

			if (m_unLockIdList.Contains (m_roleId))
			{
				if (m_cdTime <= 0)
				{
					QXSelectRole.Instance.UnLockRoleOperate (m_roleId,false);
				}
				else
				{
					//cd
					ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,"正处于冷却期！"));
				}
			}
			else
			{
				m_buyRoleObj.SetActive (true);
				QXBuyRole.m_instance.InItBuyRole (m_roleId);
				QXBuyRole.m_instance.M_BuyRoleDelegate = BuyRoleCallBack;
			}

			break;
		case "LeftBtn":
			RoleRotate roleRotateL = m_roleObjList[m_roleId].GetComponent<RoleRotate> ();
			roleRotateL.ClickRotate (RoleRotate.Direction.LEFT);
			break;
		case "RightBtn":
			RoleRotate roleRotateR = m_roleObjList[m_roleId].GetComponent<RoleRotate> ();
			roleRotateR.ClickRotate (RoleRotate.Direction.RIGHT);
			break;
		default:
			
			if (m_selectType == SelectType.CREATE_ROLE)
			{
				UIInput input = ui.GetComponent<UIInput> ();
				if (input != null)
				{
					#if UNITY_EDITOR
					m_zheZhao.gameObject.SetActive (false);
					#else
					m_zheZhao.gameObject.SetActive (true);
					#endif

					m_zheZhao.alpha = 0.1f;
				}
			}
			
			QXRoleItem roleItem = ui.transform.parent.GetComponent<QXRoleItem> ();
			if (roleItem != null)
			{
				if (int.Parse (ui.transform.parent.name) != m_roleId)
				{	
					SelectRole (int.Parse (ui.transform.parent.name));
				}
			}
			
			break;
		}
	}

	public void DragRole (Vector2 delta)
	{
		RoleRotate roleRotate = m_roleObjList[m_roleId].GetComponent<RoleRotate> ();
		roleRotate.DragRotate (delta);
	}

	public override void MYPress (bool isPress, GameObject ui)
	{
		switch (ui.name)
		{
		case "LeftBtn":
			m_leftPress = isPress;
			break;
		case "RightBtn":
			m_rightPress = isPress;
			break;
		default:
			break;
		}
	}

	void Update ()
	{
		if (m_selectType == SelectType.UNLOCK_ROLE)
		{
			if (m_unLockIdList.Contains (m_roleId))
			{
				if (m_isShowCd)
				{
					if (m_cdTime > 0)
					{
						m_cdLabel.text = "[10ff2b]" + TimeHelper.GetUniformedTimeString (m_cdTime) + "后可再次切换形象[-]";
					}
				}
			}
		}

		RoleRotate roleRotate = m_roleObjList[m_roleId].GetComponent<RoleRotate> ();
		if (m_leftPress)
		{
			roleRotate.ClickRotate (RoleRotate.Direction.LEFT);
		}
		if (m_rightPress)
		{
			roleRotate.ClickRotate (RoleRotate.Direction.RIGHT);
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
