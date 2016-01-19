using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PveStarAward : MonoBehaviour, SocketProcessor {

	[HideInInspector] public Level M_Level;

	[HideInInspector] public List<PveStarItem> PveStarItemList = new List<PveStarItem> ();

	public GameObject StaItemp;

	private float Dis = 140;

	public GameObject Success_obj;
	 
	public int  Opentype; // 1 在地图打开 2在UI打开

	public GameObject UIgrid;

	public UIPanel mPanle;
	void Start () {
	
	}

	void Update () {
	
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}

	public void Init()
	{
		PveStarItemList.Clear ();

		for(int i = 0 ; i < M_Level.starInfo.Count; i ++)
		{
			GameObject mStar = Instantiate( StaItemp )as GameObject;

			mStar.SetActive(true);

			mStar.transform.parent = StaItemp.transform.parent;

			mStar.transform.localScale = Vector3.one;

			mStar.transform.localPosition = new Vector3(0,180-Dis*i,0);

			PveStarItem mPveStarItem = mStar.GetComponent<PveStarItem>();

			mPveStarItem.mStarInfo = M_Level.starInfo[i];

			mPveStarItem.m_Level = M_Level;

			mPveStarItem.Init();

			PveStarItemList.Add(mPveStarItem);
		}
		mFixUniform mmFixUniform = UIgrid.GetComponent<mFixUniform>();

		mmFixUniform.offset = new Vector3(0,-30,0);

		if(M_Level.starInfo.Count > 4){

			mmFixUniform.enabled = false;
		}
		UIScrollView mscollview  = mPanle.GetComponent<UIScrollView>();
		mscollview.ResetPosition ();
	}
	public   bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
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
						if(m_item.Star_Id == tempInfo.s_starNum)
						{
							m_item.mStarInfo.getRewardState = true;
							
							m_item.Init();

							RewardData data = new RewardData ( m_item.Award_id, m_item.Awardsnum); 

							GeneralRewardManager.Instance ().CreateReward (data); 

							break;
						}
					}
					foreach(Pve_Level_Info m_Lv in MapData.mapinstance.Pve_Level_InfoList )
					{
						if(m_Lv.litter_Lv.guanQiaId == tempInfo.guanQiaId)
						{
							for(int j = 0 ; j < m_Lv.litter_Lv.starInfo.Count; j++)
							{
								if(m_Lv.litter_Lv.starInfo[j].starId == tempInfo.s_starNum)
								{
									m_Lv.litter_Lv.starInfo[j].getRewardState = true;
									
									m_Lv.ShowBox();
									
									break;
								}
							}
							break;
						}
					}

					if (tempInfo.s_result)
					{
						
//						GameObject Success = Instantiate( Success_obj )as GameObject;
//						
//						Success.SetActive(true);
//						
//						Success.transform.parent = Success_obj.transform.parent;
//						
//						Success.transform.localScale = Vector3.one;
//						
//						Success.transform.localPosition = Vector3.zero;

					}
					
					else
					{
						Debug.Log ("已领取过星级奖励");
						
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        GetXingJiRewardLoadBack );
					}
				}
				return true;
			}
			default: return false;
			}
		}
		
		return false;
	}
	UIBox ui_box;
	void GetXingJiRewardLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		if(ui_box == null)
		{
		    ui_box = (Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();
			
			string titleStr = "提示";
			
			string str = "\n\n您已领取过该星级奖励！";
			
			string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
			ui_box.setBox(titleStr, MyColorData.getColorString (1,str), null, 
			              null,confirmStr,null,null);
		}
	}

	public void CloseUI()
	{
		if(Opentype == 1)
		{
			MapData.mapinstance.OpenEffect ();
			PassLevelBtn.Instance ().OPenEffect ();
			MapData.mapinstance.ShowPVEGuid ();
		}

		Destroy (this.gameObject);
	}
}
