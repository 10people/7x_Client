using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MiBaoScrollView : MonoBehaviour,SocketProcessor {

	public GameObject mMiBaoGroupTemp;

	public MibaoInfoResp my_MiBaoInfo;

	public MibaoActivateResp m_iBaoActiveInfo;

	private float Dis = 150;

	public List<MibaoZuheTemp> mMibaoZuheTempList = new List<MibaoZuheTemp>();

	public UIScrollView mScrollViewPanle;
	public UISlider mUISlider;

	public int StarNum;

	public UILabel Star_Num;

	//public GameObject Btnbg;
	public static MiBaoScrollView m_MiBaoScrollView;

	public GameObject GetMiBaoAwardBtn;
	public GameObject EffectTempt;

	public static bool IsOPenPath;

	public static int OpenMiBaoId;

	public static bool FirstOPenPath;
	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
		m_MiBaoScrollView = this;
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
		
	}

	void Start () {
	
	}

//	void OnEnable()
//	{
//		mMiBaoGroupTemp = MiBaoManager.Instance ().G_MiBaoInfo;
//
//		Init ();
//	}

	void Update () {
	
	}

	public void Init()
	{
		foreach(MBTemp mMBTemp in MiBaoManager.Instance().mMBTempList)
		{
			Destroy(mMBTemp.gameObject);
		}
		MiBaoManager.Instance().mMBTempList.Clear ();

		foreach(MibaoZuheTemp m in mMibaoZuheTempList)
		{
			Destroy( m.gameObject );
		}

		mMibaoZuheTempList.Clear();

		StarNum = 0;

		for( int i = 0 ; i < my_MiBaoInfo.mibaoGroup.Count; i ++)
		{
			GameObject m_MiBaoGroupTemp = Instantiate(mMiBaoGroupTemp) as GameObject;

			m_MiBaoGroupTemp.SetActive(true);

			m_MiBaoGroupTemp.transform.parent = mMiBaoGroupTemp.transform.parent;

			m_MiBaoGroupTemp.transform.localPosition = new Vector3(0,150-i*Dis,0);

			m_MiBaoGroupTemp.transform.localScale = Vector3.one;

			MibaoZuheTemp mMibaoZuheTemp = m_MiBaoGroupTemp.GetComponent<MibaoZuheTemp>();

			mMibaoZuheTemp.mMiBaoGroup = my_MiBaoInfo.mibaoGroup[i];

			mMibaoZuheTemp.Skill_id = my_MiBaoInfo.mibaoGroup[i].skillId;

			mMibaoZuheTemp.DragonNum = i+1;

			mMibaoZuheTemp.Zuhe = i+1;

			mMibaoZuheTempList.Add(mMibaoZuheTemp);

			mMibaoZuheTemp.Init();

			for(int j = 0 ; j < my_MiBaoInfo.mibaoGroup[i].mibaoInfo.Count; j++)
			{
				if( my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level > 0)
				{
					StarNum += my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].star;
				}

			}
		}
		ShowScrollBar ();

		mScrollViewPanle.GetComponent<UIScrollView> ().UpdateScrollbars (true);

		if(FreshGuide.Instance().IsActive(100050)&& TaskData.Instance.m_TaskInfoDic[100050].progress>= 0)
		{
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100050];

//			Debug.Log("秘宝升级引导1");

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]); //选择秘宝
			ClosemScrollViewPanlemove();
			return;
		}

		if(FreshGuide.Instance().IsActive(100210)&& TaskData.Instance.m_TaskInfoDic[100210].progress>= 0)
		{
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100210];

//			Debug.Log("秘宝合成引导1");

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]); //选择秘宝
			ClosemScrollViewPanlemove();
			return;
		}
		if(FreshGuide.Instance().IsActive(100220)&& TaskData.Instance.m_TaskInfoDic[100220].progress>= 0)
		{
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100220];
			
//			Debug.Log("秘宝技能激活1");
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]); //选择秘宝
			ClosemScrollViewPanlemove();
			return;
		}
		if(FreshGuide.Instance().IsActive(100350)&& TaskData.Instance.m_TaskInfoDic[100350].progress>= 0)
		{
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100350];
			
//			Debug.Log("秘宝第二次合成1");
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]); //选择秘宝
			ClosemScrollViewPanlemove();
			return;
		}
	}

	public void ShowScrollBar()
	{
//		Debug.Log ("StarNum =" +StarNum);
//		Debug.Log ("my_MiBaoInfo.needAllStar =" +my_MiBaoInfo.needAllStar);

		if(my_MiBaoInfo.needAllStar == -1)
		{
			mUISlider.gameObject.SetActive(false);
			return;
		}
		if(StarNum < my_MiBaoInfo.needAllStar)
		{
			GetMiBaoAwardBtn.SetActive(false);
		}
		else
		{
			GetMiBaoAwardBtn.SetActive(true);
			ShowEffect();
		}

		if(StarNum <= 0)
		{
			Star_Num.text = MyColorData.getColorString(5, StarNum.ToString())+"/"+my_MiBaoInfo.needAllStar.ToString ();
		}
		 else if(StarNum >= my_MiBaoInfo.needAllStar)
		{
			Star_Num.text = MyColorData.getColorString(6, StarNum.ToString())+"/"+my_MiBaoInfo.needAllStar.ToString ();
		}
		else
		{
			Star_Num.text = StarNum.ToString () + "/" + my_MiBaoInfo.needAllStar.ToString ();
		}


		mUISlider.value = (float)StarNum/(float)my_MiBaoInfo.needAllStar;
		if(this.gameObject.activeInHierarchy)
		{
			StopCoroutine ("BtnShake");
			StartCoroutine ("BtnShake");
		}

	}

	IEnumerator BtnShake()
	{
		while(StarNum >= my_MiBaoInfo.needAllStar)
		{
			//iTween.ShakePosition(this.gameObject,new Vector3(0.02f,0.001f,0),1);
			iTween.ShakeRotation(GetMiBaoAwardBtn,new Vector3(0,0,20f),0.8f);
			yield return new WaitForSeconds (2.5f);
		}
	}
	public void ShowEffect()
	{
		int effectid = 600154;

		UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,EffectTempt,EffectIdTemplate.GetPathByeffectId(effectid));

	}
	public void CloseEffect()
	{
		UI3DEffectTool.Instance ().ClearUIFx (EffectTempt);
	}
	public void GetStarAward()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAO_DANG_DONE ),OpenLockLoadBack);
	}

	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempObject = ( GameObject )Instantiate( p_object );

		tempObject.transform.parent = this.transform.parent;

		tempObject.transform.localPosition = Vector3.zero;

		tempObject.transform.localScale  = Vector3.one;

		MiBaoStarAwardUI mMiBaoStarAwardUI = tempObject.GetComponent<MiBaoStarAwardUI>();

		mMiBaoStarAwardUI.mSum = my_MiBaoInfo.needAllStar;

		if(StarNum >= my_MiBaoInfo.needAllStar)
		{
			mMiBaoStarAwardUI.IsGetaward = true;
		}
		else
		{
			mMiBaoStarAwardUI.IsGetaward = false;
		}
		mMiBaoStarAwardUI.Init ();
	}
	public void ClosemScrollViewPanlemove()
	{
		mScrollViewPanle.GetComponent<UIScrollView>().enabled = false;
	}
	public void ActiveScrollViewPanlemove()
	{
		mScrollViewPanle.GetComponent<UIScrollView>().enabled = true;
	}
	public bool OnProcessSocketMessage(QXBuffer p_message){
		//Debug.Log("jieshouxinxi" );
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MIBAO_ACTIVATE_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoActivateResp MiBaoActiveInfo = new MibaoActivateResp();
				
				t_qx.Deserialize(t_stream, MiBaoActiveInfo, MiBaoActiveInfo.GetType());
				
				if(MiBaoActiveInfo.mibaoInfo != null)
				{

					m_iBaoActiveInfo = MiBaoActiveInfo;

					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_CARD_TEMP ),LoadBck_2);
					
					if(UIYindao.m_UIYindao.m_isOpenYindao)
					{
						UIYindao.m_UIYindao.CloseUI();
					}

				}
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
				
				MiBaoManager.Instance().ShowZhanLiAnmition();

				return true;
			}
		
			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}
	void LoadBck_2(ref WWW p_www,string p_path, Object p_object) // 合成秘宝时候弹出的框 大秘宝
	{
		GameObject cardtemp = Instantiate(p_object) as GameObject;
		
		cardtemp.transform.parent = this.transform.parent;
		
		cardtemp.transform.localPosition = new Vector3(0,-46,0);
		
		cardtemp.transform.localScale = new Vector3(0.9f,0.9f,0.9f);
		
		mbCardTemp mmbCardTemp = cardtemp.GetComponent<mbCardTemp>();
		
		mmbCardTemp.mibaoTemp =  m_iBaoActiveInfo.mibaoInfo;
		
		mmbCardTemp.init();	
	}
}
