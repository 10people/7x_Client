
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class WildnessManager :  MonoBehaviour{
//	public GameObject Fog_obg;//迷雾对象
//
//	public GameObject hy_Map;//map
//
//	public GameObject City_Btn;//City_Btn
//
//	public GameObject Treasure_obg ;//宝藏
//
//	public GameObject Resource_obg ; //宝藏
//
//	int allFogNum = 0; // 假设 总的有5个迷雾
//
//	public List<Huangyetearce> mHuangYe_Huangyetearce_List = new List<Huangyetearce> ();
//
//	public List<HuangyeResource> mHuangYe_RES_List = new List<HuangyeResource> ();
//
//	public List<HuangYeFog> mHuangYeFogList = new List<HuangYeFog> ();
//
//	public List<GameObject> m_listGameobjFog = new List<GameObject>();
//
//	public OpenHuangYeResp M_OpenHuangYeResp;
//
//	public static WildnessManager  HuangYeData;
//
//	public AllianceHaveResp M_UnionInfo;
//
//	private float m_Distance = 0;
//
//	public UILabel Builds;
//
//	public UIPanel m_Panel;
//
//	public static WildnessManager Instance()
//	{
//		if (!HuangYeData)
//		{
//			HuangYeData = (WildnessManager)GameObject.FindObjectOfType (typeof(WildnessManager));
//		}
//		
//		return HuangYeData;
//	}
//
//
////	void Awake()
////	{ 
////	
////		SocketTool.RegisterMessageProcessor(this);
////		
////	}
////	void OnDestroy()
////	{
////		SocketTool.UnRegisterMessageProcessor(this);
////		
////	}
//	void Start () {
//	
////		M_UnionInfo = AllianceData.Instance.g_UnionInfo;//获取联盟的信息
////		if(M_UnionInfo != null )
////		{
////			Debug.Log("请求荒野信息 。。。。");
////			init();
////		}
//
//		m_Panel.baseClipRegion = new Vector4(0,0,960,640);
//		m_Panel.clipOffset = Vector2.zero;
//		m_Panel.gameObject.transform.localPosition = Vector3.zero;
////		m_Panel.clip
//	}
//
	public void init()
	{
////		Debug.Log("0000000000请求荒野信息 。。。。");
////		M_UnionInfo = AllianceData.Instance.g_UnionInfo;//获取联盟的信息
////
////		Builds.text = M_UnionInfo.build.ToString();
////
////		if(M_UnionInfo == null )
////		{
////			Debug.Log("M_UnionInfo为空。。。。");
////
////			return;
////		}
////
////		OpenHuangYe mOpen_Huangye = new OpenHuangYe ();
////		
////		MemoryStream mOpen_HuangyeStream = new MemoryStream ();
////		
////		QiXiongSerializer Huangye_er = new QiXiongSerializer ();
////		
////		mOpen_Huangye.lianmengId = M_UnionInfo.id;
////		
////		Huangye_er.Serialize (mOpen_HuangyeStream,mOpen_Huangye);
////		
////		byte[] t_protof;
////
////		t_protof = mOpen_HuangyeStream.ToArray();
////
////		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_OPEN_HUANGYE,ref t_protof);
//
	}
//	//public bool OnProcessSocketMessage(QXBuffer p_message){
//		
////		if (p_message != null)
////		{
////			switch (p_message.m_protocol_index)
////			{
////			case ProtoIndexes.S_OPEN_HUANGYE:
////			{
//////				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
//////				
//////				QiXiongSerializer t_qx = new QiXiongSerializer();
//////				
//////				OpenHuangYeResp Huangye_resp = new OpenHuangYeResp();
//////				
//////				t_qx.Deserialize(t_stream, Huangye_resp, Huangye_resp.GetType());
//////
//////				if(Huangye_resp != null)
//////				{
//////					M_OpenHuangYeResp = Huangye_resp;
//////					//Debug.Log(Huangye_resp.resource);
//////					//Debug.Log(Huangye_resp.resource.Count);
//////					CreateHuangyeInfo(Huangye_resp);
//////				}
////		     	
////				return true;
////			}
////			case ProtoIndexes.S_OPEN_FOG:
////			{
//////				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
//////				
//////				QiXiongSerializer t_qx = new QiXiongSerializer();
//////				
//////				//OpenFogResp HuangyeFog_resp = new OpenFogResp();
//////				
//////				t_qx.Deserialize(t_stream, HuangyeFog_resp, HuangyeFog_resp.GetType());
//////				//Debug.Log("请求开启迷雾数据返回 HuangyeFog_resp.result = "+HuangyeFog_resp.result);
//////				if(HuangyeFog_resp.result == 0 ) // 开乌成功
//////				{
//////					AllianceData.Instance.RequestData();
//////					for(int i = 0; i < mHuangYeFogList.Count; i ++)
//////					{
//////						if(mHuangYeFogList[i].IconNumber == HuangyeFog_resp.fileId)
//////						{
//////							mHuangYeFogList[i].JianSheData.gameObject.SetActive(false);
//////							mHuangYeFogList[i].setFogColor(new Color(1f, 1f, 1f, 0f));
//////						}
//////					}
//////					Debug.Log("HuangyeFog_resp.treasure。count = "+HuangyeFog_resp.treasure.Count);
//////
//////					Debug.Log("HuangyeFog_resp.resource.Count = "+HuangyeFog_resp.resource.Count);
//////					List<HuangYeTreasure > TreList = HuangyeFog_resp.treasure;
//////
//////					List<HuangYeResource > ResList = HuangyeFog_resp.resource;
//////
//////					Createtreasure (TreList);
//////
//////					Createresource (ResList);
//////				}
//////				else
//////				{
//////					Global.CreateBox(LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_1), null, LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_3), null, LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM), null, null, null);
//////				}
////				return true;
////
////			}	
////			case ProtoIndexes.S_OPEN_TREASURE:
////			{
//////				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
//////				
//////				QiXiongSerializer t_qx = new QiXiongSerializer();
//////				
//////				OpenHuangYeTreasureResp HuangyeTrea_resp = new OpenHuangYeTreasureResp();
//////				
//////				t_qx.Deserialize(t_stream, HuangyeTrea_resp, HuangyeTrea_resp.GetType());
//////
//////				Debug.Log("开启藏宝点请求返回了 = "+HuangyeTrea_resp.result);
//////				if(HuangyeTrea_resp.result == 0 ) // 开藏宝点成功
//////				{
//////					AllianceData.Instance.RequestData();
//////
//////					for(int i = 0; i < mHuangYe_Huangyetearce_List.Count; i++) 
//////					{
//////						if(mHuangYe_Huangyetearce_List[i].m_HuangYeTreasure.id == HuangyeTrea_resp.id)
//////						{
//////							mHuangYe_Huangyetearce_List[i].m_HuangYeTreasure.isOpen = 1;
//////
//////							Debug.Log("222222开启藏宝点请求返回了");
//////
//////							mHuangYe_Huangyetearce_List[i].Init();
//////						}
//////					}
//////				}
//////				return true;
////			}
////			default: return false;
////			}
////			
////		}
//	//	return false;
//	//}
//
//	void CreateHuangyeInfo(OpenHuangYeResp m_Huangye)
//	{
//		foreach(Huangyetearce mTre in mHuangYe_Huangyetearce_List)
//		{
//			Destroy(mTre.gameObject);
//		}
//		mHuangYe_Huangyetearce_List.Clear ();
//
//
//		foreach(HuangyeResource mRes in mHuangYe_RES_List)
//		{
//			Destroy(mRes.gameObject);
//		}
//		mHuangYe_RES_List.Clear ();
//
//		allFogNum = 40;
//		for(int n = 1; n <= allFogNum; n++)
//		{
//			GameObject m_fog = Instantiate(Fog_obg) as GameObject;
//
//			m_fog.name = "tile" + n;
//
//			m_fog.SetActive(true);
//
//			m_fog.transform.parent = Fog_obg.transform.parent;
//
//			m_fog.transform.localScale = Fog_obg.transform.localScale;
//
//			HuangYeFogTemplete mHuangyeTemp = HuangYeFogTemplete.getHuangYeFogTemplete(n);
//
//			float x = (float)mHuangyeTemp.positionX;
//
//			float y = (float)mHuangyeTemp.positionY;
//
//			Vector3 pos = new Vector3(x,y-m_Distance,0);
//
//			m_fog.transform.localPosition = pos;
//
////			HuangYeFog mHuangYeFog = m_fog.GetComponent<HuangYeFog>();
////
////			mHuangYeFog.IconNumber = n;
////			mHuangYeFog.Startinit(mHuangyeTemp, m_listGameobjFog[n]);
////			bool tempBool = true;
////			for(int i = 0; i < m_Huangye.fogInfo.Count; i++)   // 创建迷雾
////			{
////				if( n == m_Huangye.fogInfo[i].fileId)
////				{
////					tempBool = false;
////
////					mHuangYeFog.mFoinfo = m_Huangye.fogInfo[i];
////
////					mHuangYeFog.JianShezhi = mHuangyeTemp.openCost;
////
////					mHuangYeFog.init(m_Huangye);
////				}
////			}
////			if(tempBool)
////			{
////				m_listGameobjFog[n].SetActive(true);
////			}
////			mHuangYeFogList.Add(mHuangYeFog);
////		}
////
////		hy_Map.SetActive (true);
////
////		City_Btn.SetActive (true);
////
////		List<HuangYeTreasure> TreList = m_Huangye.treasure;
////
////		List<HuangYeResource> ResList = m_Huangye.resource;
////
////		if(m_Huangye.treasure == null)
////		{
////			Debug.Log("m_Huangye.treasure == null");
////		}
////	//	Debug.Log("m_Huangye.treasure.Count。。。。"+m_Huangye.treasure.Count);
////		//Debug.Log("m_Huangye.resource.Count---------"+m_Huangye.resource.Count);
////		
////
////
////		Createtreasure (TreList);
////
////		Createresource (ResList);
////		//return; //开关
////
////	}
//
////	void Createtreasure(List<HuangYeTreasure> m_HuangyeTrealist )   //宝藏点
////	{
////
////		for(int j = 0; j < m_HuangyeTrealist.Count; j++)   // 创建宝藏点
////		{
////			//Debug.Log("m_Huangye.treasure.[j].fileId---------"+m_Huangye.treasure[j].fileId);
////			HuangyeTemplate mHuangyeTemplate = HuangyeTemplate.getHuangyeTemplate_byid(m_HuangyeTrealist[j].fileId);
////
////			float x = (float)(mHuangyeTemplate.positionX);
////
////			float y = (float)(mHuangyeTemplate.positionY);
////			
////			GameObject m_Treasure = Instantiate(Treasure_obg) as GameObject;
////
////			m_Treasure.SetActive(true);
////
////			m_Treasure.transform.parent = Treasure_obg.transform.parent;
////
////			m_Treasure.transform.localScale = Treasure_obg.transform.localScale ;
////
////			m_Treasure.transform.localPosition = new Vector3(x,y-m_Distance,0);
////
////			Huangyetearce mHuangyetearce = m_Treasure.GetComponent<Huangyetearce>();
////
////			mHuangyetearce.m_HuangYeTreasure = m_HuangyeTrealist[j];
////
////			mHuangyetearce.Init();
////
////			mHuangYe_Huangyetearce_List.Add(mHuangyetearce);
////		}
////	}
//
////	void Createresource(List<HuangYeResource> m_HuangyeReslist)//资源点
////	{
//////		Debug.Log(m_HuangyeReslist);
//////		Debug.Log(m_HuangyeReslist.Count);
////		for(int j = 0; j < m_HuangyeReslist.Count; j++)   // 创建资源
////		{
//////			Debug.Log(j);
////			//Debug.Log("m_Huangye.resource.[j].fileId---------"+m_HuangyeReslist[j].fileId);
////			HuangyeTemplate mHuangyeTemplate = HuangyeTemplate.getHuangyeTemplate_byid(m_HuangyeReslist[j].fileId);
////
////			float x = (float)(mHuangyeTemplate.positionX);
////
////			float y = (float)(mHuangyeTemplate.positionY);
////			
////			GameObject m_Resource= Instantiate(Resource_obg) as GameObject;
////
////			m_Resource.SetActive(true);
////
////			m_Resource.transform.parent = Resource_obg.transform.parent;
////
////			m_Resource.transform.localScale = Resource_obg.transform.localScale;
////
////			m_Resource.transform.localPosition = new Vector3(x,y-m_Distance,0);
//
////			HuangyeResource m_HuangyeResource = m_Resource.GetComponent<HuangyeResource>();
////
////			m_HuangyeResource.mHuangYeResource = m_HuangyeReslist[j];
////
////			m_HuangyeResource.init();
//
////			mHuangYe_RES_List.Add(m_HuangyeResource);
//	//	}
//	}
//
////	void Update () {
////	
////		M_UnionInfo = AllianceData.Instance.g_UnionInfo;//获取联盟的信息
////
////		Builds.text = M_UnionInfo.build.ToString();
////	}
////	public void backtoAlianceCity()//返回联盟城池
////	{
//////		Global.m_isOpenHuangYe = false;
//////		MainCityUI.TryRemoveFromObjectList(this.gameObject);
//////		Destroy (this.gameObject);
////	}
//	public void OpenStock()//打开资源库
//	{
//		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HY_REWARD_LIBRARY), RewardLibraryLoadBack);
//	}
//
//	public void RewardLibraryLoadBack (ref WWW p_www,string p_path, Object p_object)
//	{
////		GameObject hy = Instantiate( p_object ) as GameObject;
////		
////		hy.name = "HYRewardLibrary";
////
////		hy.transform.localPosition = new Vector3 (0,1800,0);
////
////		hy.transform.localScale = Vector3.one; 
//	}
//
//	void StarOpenFog(int i)
//	{
//		if (i == 2)
//		{
////			OpenFog mOpen_fog = new OpenFog ();
////			
////			MemoryStream mOpen_fogStream = new MemoryStream ();
////			
////			QiXiongSerializer fog_er = new QiXiongSerializer ();
////			
////			mOpen_fog.fileId = tempHuangYeData.mFoinfo.fileId;
////			
////			Debug.Log ("mFoinfo.fileId = "+ tempHuangYeData.mFoinfo.fileId);
////			
////			fog_er.Serialize (mOpen_fogStream,mOpen_fog);
////			
////			byte[] t_protof;
////			t_protof = mOpen_fogStream.ToArray();
////			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_OPEN_FOG,ref t_protof,ProtoIndexes.S_OPEN_FOG.ToString());
//		}
//	}
//	void LoadOpemFogBack(ref WWW p_www, string p_path, Object p_object)
//	{
////		
////		//string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
////		string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.BUY_1) + LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_19);
////
////		string str2 = "\r\n"+"是否花费"+tempHuangYeData.JianShezhi.ToString()+"建设值开启此区域？";//LanguageTemplate.GetText(LanguageTemplate.Text.BAIZHAN_CONFIRM_DUIHUAN_USE_WEIWANG_ASKSTR1);
////	
////		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
////		
////		string strbtn1 = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
////
////		string strbtn2 = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
////		
////		uibox.setBox(str1, str2, null,null, strbtn1, strbtn2, StarOpenFog, null, null, null);
//
//
//	}
//
//	HuangYeFog tempHuangYeData;
//
//	public override void MYClick(GameObject ui)
//	{
////		int index = int.Parse(ui.name.Substring(4, ui.name.Length - 4));
//////		m_listGameobjFog[index].SetActive(false);
////		Debug.Log ("AllianceData.Instance.g_UnionInfo.build = "+AllianceData.Instance.g_UnionInfo.build);
////
////		 tempHuangYeData = mHuangYeFogList[0];
////		for(int i = 0; i < mHuangYeFogList.Count; i ++)
////		{
////			if(mHuangYeFogList[i].IconNumber == index)
////			{
////				tempHuangYeData = mHuangYeFogList[i];
////			}
////		}
////		if(AllianceData.Instance.g_UnionInfo.build < tempHuangYeData.JianShezhi)  // 判断建设值是否够开启迷雾
////		{
////			Global.CreateBox(LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_1), null, LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_2), null, LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM), null, null, null);
////			return;
////		}
////		Debug.Log ("AllianceData.Instance.g_UnionInfo.identity= "+AllianceData.Instance.g_UnionInfo.identity);
////		if(AllianceData.Instance.g_UnionInfo.identity == 0)
////		{
////			Global.CreateBox(LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_1), null, LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_3), null, LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM), null, null, null);
////			return;//身份不够开启资格
////		}
////
////		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadOpemFogBack);
//
//	}
//	
//	public override void MYMouseOver(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYMouseOut(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYPress(bool isPress, GameObject ui)
//	{
////		int index = int.Parse(ui.name.Substring(4, ui.name.Length - 4));
////		HuangYeFog tempHuangYeData = mHuangYeFogList[0];
////		for(int i = 0; i < mHuangYeFogList.Count; i ++)
////		{
////			if(mHuangYeFogList[i].IconNumber == index)
////			{
////				tempHuangYeData = mHuangYeFogList[i];
////			}
////		}
////
////		if(isPress)
////		{
////			tempHuangYeData.setFogColor(new Color(205f / 255f, 205f / 255f, 223f / 255f, 1f));
////		}
////		else
////		{
////			tempHuangYeData.setFogColor(Color.white);
////		}
//	}
//	
//	public override void MYelease(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYondrag(Vector2 delta)
//	{
//		
//	}
//	
//	public override void MYoubleClick(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYonInput(GameObject ui, string c)
//	{
//		
//	}
}
