using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class YxChooseDefcult : MonoBehaviour,SocketProcessor {

	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

	[HideInInspector]
	
	public int BigId;

	public GameObject Item;

//	public UILabel YxName;
	public UIGrid mGid;
	public YouXiaInfo mYouXia_Info;
	public List <YouXiaItem> YouXiaItemmList = new List<YouXiaItem>();
	public static YxChooseDefcult mmmYxChooseDefcult;
	public UIScrollView mScorview;
	public List<int > PassedId = new List<int>();

	public UILabel mTimes;
	public string m_Times;
	public mFixUniform mCHangeItemsPostion; //改变坐标 当前打到的关卡

	public static YxChooseDefcult Instance()
	{
		if (!mmmYxChooseDefcult)
		{
			mmmYxChooseDefcult = (YxChooseDefcult)GameObject.FindObjectOfType (typeof(YxChooseDefcult));
		}
		
		return mmmYxChooseDefcult;
	}

	void Start () {
	
	}
	
	void Awake()
	{

		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		mmmYxChooseDefcult = null;
		SocketTool.UnRegisterMessageProcessor(this);
	}

	public void Init()
	{
		SetMessegae ();
	}
	private void SetMessegae()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		YouXiaTypeInfoReq saodanginfo = new YouXiaTypeInfoReq ();
		
		MemoryStream saodangstream = new MemoryStream ();
		
		QiXiongSerializer saodangSer = new QiXiongSerializer ();
	
		saodanginfo.type = BigId;
		//Debug.Log ("saodanginfo.type = "+saodanginfo.type);
		saodangSer.Serialize (saodangstream, saodanginfo);
		
		byte[] t_protof;
		
		t_protof = saodangstream.ToArray();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_YOUXIA_TYPE_INFO_REQ,ref t_protof,p_receiving_wait_proto_index:ProtoIndexes.S_YOUXIA_TYPE_INFO_RESP);
	}
	public   bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YOUXIA_TYPE_INFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YouXiaTypeInfoResp tempInfo = new YouXiaTypeInfoResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				//Debug.Log ("以战胜关卡Id返回 tempInfo passGuanQiaId.Count = "+tempInfo.passGuanQiaId.Count);
				if(tempInfo.passGuanQiaId != null)
				{
					PassedId = tempInfo.passGuanQiaId;
				}
				InitData();

				return true;
			}

			default: return false;
			}
		}
		
		return false;
	}
	void InitData()
	{
		mTimes.text = m_Times;
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
//		if(FreshGuide.Instance().IsActive(100315)&& TaskData.Instance.m_TaskInfoDic[100315].progress >= 0)
//		{
//			//Debug.Log("进入试练二阶界面222");
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100315];
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
//			mScorview.enabled = false;
//		}
		//Debug.Log ("BigId.Count = "+BigId);
		List<YouxiaPveTemplate> mYouxiaPveTemplateList = YouxiaPveTemplate.getYouXiaPveTemplateListBy_BigId (BigId);
		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateBy_BigId (BigId);
//		YxName.text = NameIdTemplate.GetName_By_NameId (myouxia.bigName);
		MainCityUI.setGlobalTitle(TopLeftManualAnchor, NameIdTemplate.GetName_By_NameId (myouxia.bigName), 0, 0);
		foreach(YouXiaItem m_YXItem in YouXiaItemmList)
		{
			Destroy(m_YXItem.gameObject);
		}
		YouXiaItemmList.Clear ();
		//Debug.Log ("mYouxiaPveTemplateList.Count = "+mYouxiaPveTemplateList.Count);
		int CurrLevelid = 0;
		for(int i = 0 ; i < mYouxiaPveTemplateList.Count; i++)
		{
			GameObject m_UI = Instantiate(Item) as GameObject;
			
			m_UI.SetActive (true);
			
			m_UI.transform.parent = Item.transform.parent;
			
			m_UI.transform.localScale = Vector3.one;
			
			//m_UI.transform.localPosition = new Vector3(-300+Pos_Dis*i,0,0);
			
			YouXiaItem mYXItem = m_UI.GetComponent<YouXiaItem>();
			
			mYXItem.L_id = mYouxiaPveTemplateList[i].id;
			
			mYXItem.YouXiadifficulty = i+1;
			
			//mYouXiaItem.CountTime = HavatTimes;
			mYXItem.mYou_XiaInfo = mYouXia_Info;
			mYXItem.bigid = mYouxiaPveTemplateList[i].bigId;
			if(i > 0)
			{
				int curid = mYouxiaPveTemplateList[i-1].id;
				foreach(int id in YxChooseDefcult.Instance().PassedId)
				{
					if(curid == id) 
					{
						mYXItem.ISOpenlock = true;
						CurrLevelid +=1;
						break;
					}
					mYXItem.ISOpenlock = false;
				}

			}
			else
			{
				mYXItem.ISOpenlock = true;
			}
		
			YouXiaItemmList.Add(mYXItem);
			mYXItem.Init();
		}
		mGid.repositionNow = true;
		Debug.Log ("CurrLevelid ="+CurrLevelid);
		if(CurrLevelid >= 3 && CurrLevelid < 8)
		{
			mCHangeItemsPostion.offset = new Vector3(0-270*(CurrLevelid-1) ,-11,3.7f);
			mCHangeItemsPostion.enabled = true;
			StartCoroutine("SetmFixUniformFalse");
		}
		if( CurrLevelid >= 8)
		{
			mCHangeItemsPostion.offset = new Vector3(-1791 ,-11,3.7f);
			mCHangeItemsPostion.enabled = true;
			StartCoroutine("SetmFixUniformFalse");
		}
	}
	IEnumerator SetmFixUniformFalse()
	{
		Debug.Log ("CurrLevelid = aaa");
		yield return new WaitForSeconds (0.5f);
		mCHangeItemsPostion.enabled = false;
	}
	public void BuyTiLi()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
	}
	public void BuyTongBi()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
	}
	public void BuyYuanBao()
	{
		MainCityUI.ClearObjectList();
        EquipSuoData.TopUpLayerTip();
    }

	public void Help()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.SHILIANINSTRCDUTION));
	}
	public void Close()
	{
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
