using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class PveStarAwardpanel : MonoBehaviour  ,SocketProcessor {

	[HideInInspector] public List<PveStarItem> PveStarItemList = new List<PveStarItem> ();

	[HideInInspector] public Level M_Level;

	public GameObject StaItemp;

	private float Dis = 82;

	public GameObject UIgrid;

	public UIPanel mPanle;
	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
	    SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
	}
	

	void Update () {
	
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	public void Init()
	{
		PveStarItemList.Clear ();

		for (int i = 0; i < M_Level.starInfo.Count; i ++) {
		
			GameObject mStar = Instantiate( StaItemp )as GameObject;
			mStar.SetActive(true);
			mStar.transform.parent = StaItemp.transform.parent;
			mStar.transform.localScale = new Vector3 (0.6f,0.6f,1);
			mStar.transform.localPosition = new Vector3(0,70-Dis*i,0);
			PveStarItem mPveStarItem = mStar.GetComponent<PveStarItem>();
			if(CityGlobalData.PT_Or_CQ)
			{
				mPveStarItem.mStarInfo = M_Level.starInfo[i];
			}
			else
			{
				mPveStarItem.mStarInfo = M_Level.cqStarInfo[i];
			}
			mPveStarItem.m_Level = M_Level;
			mPveStarItem.Init();
			PveStarItemList.Add(mPveStarItem);
		}
		mFixUniform mmFixUniform = UIgrid.GetComponent<mFixUniform>();
		mmFixUniform.offset = new Vector3(0,0,0);

		if (M_Level.starInfo.Count > 4) {
			mmFixUniform.enabled = false;
			UIScrollView mscollview  = mPanle.GetComponent<UIScrollView>();
			mscollview.ResetPosition ();
		}
		UIYindao.m_UIYindao.CloseUI ();
	}
	public   bool OnProcessSocketMessage(QXBuffer p_message){
	
		if (p_message != null) {
			switch (p_message.m_protocol_index)
			{
				case ProtoIndexes.PVE_STAR_REWARD_GET_RET: 
				{
					MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
					QiXiongSerializer t_qx = new QiXiongSerializer();
					PveStarGetSuccess tempInfo = new PveStarGetSuccess();
					t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
					
					if (tempInfo != null)
					{
						foreach(PveStarItem m_item in PveStarItemList )
						{
//							UIButton mBtn = m_item.gameObject.GetComponent<UIButton>();
//							
//							mBtn.enabled = true;

							if(m_item.Star_Id == tempInfo.s_starNum)
							{
								m_item.mStarInfo.getRewardState = true;
								
								m_item.Init();
								
								RewardData data = new RewardData ( m_item.Award_id, m_item.Awardsnum); 
								
								GeneralRewardManager.Instance().CreateReward (data); 
								
							}
						}
					   
						foreach(Pve_Level_Info m_Lv in MapData.mapinstance.Pve_Level_InfoList )
						{

							if(m_Lv.litter_Lv.guanQiaId == tempInfo.guanQiaId)
							{
							List <StarInfo> starList = new List<StarInfo>();
							if(CityGlobalData.PT_Or_CQ)
							{
								starList = m_Lv.litter_Lv.starInfo;
							}
							else
							{
								starList = m_Lv.litter_Lv.cqStarInfo;
							}
							for(int j = 0 ; j < starList.Count; j++)
								{
								    if(starList[j].starId == tempInfo.s_starNum)
									{
									starList[j].getRewardState = true;
										
									bool jingying = false ;
									if(CityGlobalData.PT_Or_CQ)
									{
										jingying = true;
									}
									m_Lv.ShowBox(jingying);
										
										break;
									}
								}
								break;
							}
						}
						
					}
					return true;
				}

			 default: return false;
			}
		}
		return false;	
	}
	public void CloseUI()
	{			
		MapData.mapinstance.ShowYinDao = true;

		Destroy (this.gameObject);
	}
}
