using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class NewSelectRole : MonoBehaviour,SocketProcessor {

	public static NewSelectRole selectRole;

	public List<UIInput> nameInputList = new List<UIInput>();//名字输入框

	public List<EventHandler> roleHanderList = new List<EventHandler> ();//角色人物
	public GameObject roleSelectBox;

	public UILabel roleDes;
	private int roleId;//人物角色id
	public int RoleId
	{
		set{roleId = value;}
	}
	
	/// <summary>
	/// 国家信息
	/// </summary>
	public GameObject nationBtnObj;//选择国家按钮
	private List<GameObject> nationBtnsList = new List<GameObject> ();
	public UILabel nationDes;//国家描述
	private int currentNationId;//当前国家id
	
	public EventHandler zheZhaoHandler;

	public GameObject tipWinObj;
	
	void Awake () 
	{	
		selectRole = this;

		SocketTool.RegisterMessageProcessor (this);
	}
	
	void Start ()
	{
		// report when battle field is ready
		{
			OperationSupport.ReportClientAction( OperationSupport.ClientAction.CREATE_ROLE );
		}

		nameInputList.ForEach(item => item.GetComponent<EventHandler> ().m_handler += InputHandlerCallBack);

		zheZhaoHandler.m_handler += ZheZhaoHandlerCallBack;
		
		currentNationId = CityGlobalData.countryId;

		CreateNationBtn ();
	}

	/// <summary>
	/// 创建选择国家按钮
	/// </summary>
	void CreateNationBtn ()
	{
		for (int i = 0;i < 7;i ++)
		{
			GameObject nationBtn = (GameObject)Instantiate (nationBtnObj);
			nationBtn.SetActive (true);

			nationBtn.transform.parent = nationBtnObj.transform.parent;
			if (i < 3)
			{
				nationBtn.transform.localPosition = new Vector3((i - 1) * 80,100,0);
			}
			else if (i < 6)
			{
				nationBtn.transform.localPosition = new Vector3((i - 4) * 80,35,0);
			}
			else
			{
				nationBtn.transform.localPosition = new Vector3(0,-35,0);
			}
			nationBtn.transform.localScale = Vector3.one;

			nationBtnsList.Add (nationBtn);

			SelectNationBtn nation = nationBtn.GetComponent<SelectNationBtn> ();
			nation.GetSelelctNationInfo (i + 1,currentNationId);
		}

		ShowCountryDes (currentNationId);
	}

	//选择国家
	public void SelectNation (int nationId)
	{
		Debug.Log ("currentNationId:" + currentNationId);
		Debug.Log ("nationId:" + nationId);

		currentNationId = nationId;
		
		for (int i = 0;i < nationBtnsList.Count;i ++)
		{
			SelectNationBtn nation = nationBtnsList[i].GetComponent<SelectNationBtn> ();
			
			if (i == (currentNationId - 1))
			{
				nation.ActiveSelectBox (true);
			}
			else
			{
				nation.ActiveSelectBox (false);
			}
		}

		ShowCountryDes (nationId);
	}

	/// <summary>
	/// Shows the country DES.齐楚燕韩赵魏秦
	/// </summary>
	/// <param name="countryId">Country identifier.</param>
	void ShowCountryDes (int countryId)
	{
		switch (countryId)
		{
		case 1:
			
			nationDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.COUNTRY_6);
			
			break;
			
		case 2:
			
			nationDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.COUNTRY_7);
			
			break;
			
		case 3:
			
			nationDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.COUNTRY_2);
			
			break;
			
		case 4:
			
			nationDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.COUNTRY_5);
			
			break;
			
		case 5:
			
			nationDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.COUNTRY_3);
			
			break;
			
		case 6:
			
			nationDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.COUNTRY_4);
			
			break;
			
		case 7:
			
			nationDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.COUNTRY_1);
			
			break;
		}
	}

	//初始男女名字各随一个
	public void NameInIt ()
	{	
		int randomXin = 0;
		int randomNan = 0;
		int randomNv = 0;

		for (int i = 0;i < nameInputList.Count;i ++)
		{
			string xin = "";
			if (i < 2)
			{
				randomXin = Random.Range (0,NameKuTemplate.nameList1.Count - 1);
				randomNan = Random.Range (0,NameKuTemplate.nameList2.Count - 1);
				xin = NameKuTemplate.nameList1[randomXin];
				string nan = NameKuTemplate.nameList2[randomNan];
				nameInputList[i].value = xin + nan;
			}
			else 
			{
				randomXin = Random.Range (0,NameKuTemplate.nameList1.Count - 1);
				randomNv = Random.Range (0,NameKuTemplate.nameList3.Count - 1);
				xin = NameKuTemplate.nameList1[randomXin];
				string nv = NameKuTemplate.nameList3[randomNv];
				nameInputList[i].value = xin + nv;
			}
		}
	}
	
	//随机姓名
	public void RandomNameBtn ()
	{
		int randomXin = Random.Range (0,NameKuTemplate.nameList1.Count);
		
		int randomNan = Random.Range (0,NameKuTemplate.nameList2.Count);
		
		int randomNv = Random.Range (0,NameKuTemplate.nameList3.Count);
		
		string xin = NameKuTemplate.nameList1[randomXin];

		string nan = "";
		string nv = "";

		if (roleId == 0 || roleId == 1)
		{
			nan = NameKuTemplate.nameList2[randomNan];
			nameInputList[roleId].value = xin + nan;
		}
		else
		{
			nv = NameKuTemplate.nameList3[randomNv];
			nameInputList[roleId].value = xin + nv;
		}
	}

	public void ShowNameLabel (int index)
	{
		for (int i = 0;i < nameInputList.Count;i ++)
		{
			if (i == index)
			{
				nameInputList [i].gameObject.SetActive (true);
			}
			else
			{
				nameInputList [i].gameObject.SetActive (false);
			}
		}
	}

	public void RoleDes (int role)
	{
		int desId = 0;
		switch (role)
		{
		case 0:
			desId = 11;
			break;
		case 1:
			desId = 12;
			break;
		case 2:
			desId = 21;
			break;
		case 3:
			desId = 22;
			break;
		default:
			break;
		}

		roleDes.text = DescIdTemplate.GetDescriptionById (desId);
	}

	/// <summary>
	/// CreatRole.
	/// </summary>
	public void CreatRole () 
	{	
		if (SysparaTemplate.CompareSyeParaWord (nameInputList[roleId].text))
		{
			string des = LanguageTemplate.GetText (LanguageTemplate.Text.Name_2);
			TipWindow (des);
		}
		else
		{
			CreateRoleRequest creatReq = new CreateRoleRequest();
			
			creatReq.roleName = nameInputList[roleId].text; //角色名字
			
			creatReq.roleId = roleId + 1;  //角色id
			
			creatReq.guoJiaId = currentNationId; //国家id
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer tempSer = new QiXiongSerializer();
			
			tempSer.Serialize(tempStream, creatReq);
			
			byte[] t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.CREATE_ROLE_REQUEST , ref t_protof,"23106");
		}
	}
	
	public bool OnProcessSocketMessage( QXBuffer p_message )
	{	
		if (p_message != null) 
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.CREATE_ROLE_RESPONSE1:
				
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);
				
				CreateRoleResponse creatRes = new CreateRoleResponse ();
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				t_qx.Deserialize(t_stream, creatRes, creatRes.GetType());
				
				if (creatRes != null)
				{
					switch (creatRes.isSucceed)
					{
					case true:
						
						//CityGlobalData.m_nextSceneName = ConstInGame.CONST_SCENE_NAME_MAIN_CITY;    //创建成功跳转
						//Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE );
						
						CityGlobalData.m_CreateRoleCurrent = true;
						
						//EnterBattleField.EnterBattlePve( 1, 1, LevelType.LEVEL_NORMAL );
                        if (TimeHelper.Instance.IsTimeCalcKeyExist("RoleAnimate"))
						{
                            TimeHelper.Instance.RemoveFromTimeCalc("RoleAnimate");
						}

//						Debug.Log("EnterMainCityEnterMainCityEnterMainCityEnterMainCityEnterMainCityEnterMainCityEnterMainCityEnterMainCity");

						SceneManager.EnterMainCity();
						
						break;
						
					case false:
						
						//创建失败
						TipWindow (creatRes.msg);
						Debug.Log("creatRes.msg:" + creatRes.msg);
						
						break;
					}
				}
				
				return true;
			}
		}
		return false;
	}
	
	public void OnNameSubmit ()
	{
		zheZhaoHandler.gameObject.SetActive (false);

		nameInputList[roleId].value = TextLengthLimit (StrLimitType.CREATE_ROLE_NAME,nameInputList[roleId].value);
	}
	
	public enum StrLimitType
	{
		USER_NAME,
		CREATE_ROLE_NAME,
	}
	private StrLimitType limitType;
	
	/// <summary>
	/// 字符串长度限制
	/// </summary>
	/// <returns>The refresh.</returns>
	/// <param name="limitType">Limit type.</param>
	/// <param name="str">String.</param>
	public static string TextLengthLimit (StrLimitType limitType,string str)
	{
		int limitCount = 0;
		
		string lastStr = "";
		
		List<char> textStrList = new List<char> ();
		
		for (int i = 0;i < str.Length;i ++)
		{
			switch (limitType)
			{
			case StrLimitType.CREATE_ROLE_NAME:
				
				limitCount = 7;
				
				if ((str[i] >= 'A' && str[i] <= 'Z') || (str[i] >= '0' && str[i] <= '9') ||
				    (str[i] >= 'a' && str[i] <= 'z') || (str[i] >= 0x4e00 && str[i] <= 0x9fa5))
				{
					textStrList.Add (str[i]);
				}
				break;
				
			case StrLimitType.USER_NAME:
				
				limitCount = 8;
				
				if ((str[i] >= 'A' && str[i] <= 'Z') || (str[i] >= '0' && str[i] <= '9') ||
				    (str[i] >= 'a' && str[i] <= 'z'))
				{
					textStrList.Add (str[i]);
				}
				break;
			default:
				break;
			}
			
		}
		
		//		Debug.Log ("LimitCount:" + limitCount);
		
		for (int i = 0;i < textStrList.Count;i ++)
		{
			if (i < limitCount)
			{
				lastStr += textStrList[i];
			}
		}
		
		return lastStr;
	}
	
	void InputHandlerCallBack (GameObject obj)
	{
		#if UNITY_EDITOR
		zheZhaoHandler.gameObject.SetActive (false);
		#else
		zheZhaoHandler.gameObject.SetActive (true);
		#endif
		
		zheZhaoHandler.GetComponent<UISprite> ().alpha = 0.1f;
	}
	
	void ZheZhaoHandlerCallBack (GameObject obj)
	{
		OnNameSubmit ();
	}

	void TipWindow (string msg)
	{
		GameObject win = (GameObject)Instantiate (tipWinObj);
		
		win.SetActive (true);
		win.transform.parent = this.transform;
		win.transform.localPosition = new Vector3 (0,0,-500);
		win.transform.localScale = Vector3.one;
		
		CreateRoleTips tip = win.GetComponent<CreateRoleTips> ();
		tip.ScaleAnim ();
		tip.GetDesLabelType (msg);
	}

	void OnDestroy () 
	{	
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
