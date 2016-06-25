using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MiBaoStarAwardUI : MonoBehaviour,SocketProcessor {

	public UILabel Instruction;

	public int mSum;

	[HideInInspector]
	public GameObject IconSamplePrefab;

	public GameObject AwardUitroot;

	public bool IsGetaward;
	public GameObject GetawardBtn;

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
		
	}

	int Awardid;
	int AwardidNum;
	public void Init()
	{
		if(IsGetaward)
		{
			GetawardBtn.SetActive(true);
		}
		else
		{
			GetawardBtn.SetActive(false);
		}

		Instruction.text = "集星奖励说明："+"\r\n"+"秘宝总星数达到要求（含已获得但未解锁的秘宝），即可领取奖励。";

		MiBaoJiXingTemplate mMiBaoJiXingTemplate = MiBaoJiXingTemplate.getMiBaoJiXingTemplateBysumId (mSum);

		string mAward = mMiBaoJiXingTemplate.award;


		string []s = mAward.Split(':');

		for(int i = 0 ; i < s.Length; i ++)
		{

		}
		 Awardid = int.Parse( s [1]);
		AwardidNum = int.Parse( s [2]);
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{

		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
				
		GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;

		iconSampleObject.SetActive(true);
		
		iconSampleObject.transform.parent = AwardUitroot.transform;
		
		//iconSampleObject.transform.localPosition = new Vector3(i * 20 - (stars - 1) * 10, 0, 0);
		iconSampleObject.transform.localScale = Vector3.one;

		iconSampleObject.transform.localPosition = new Vector3(0, 0, 0);
		//FirstAwardPos = iconSampleObject.transform.localPosition;
		
		var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
		
		var iconSpriteName = "";
		
       CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(Awardid);
		
		iconSpriteName = mItemTemp.icon.ToString();
		
		iconSampleManager.SetIconType(IconSampleManager.IconType.item);
		
		NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
		
		string mdesc = DescIdTemplate.GetDescriptionById(mItemTemp.descId);
		
		var popTitle = mNameIdTemplate.Name;
		
		var popDesc = mdesc;

        iconSampleManager.SetIconByID(mItemTemp.id, "", 10);
        iconSampleManager.SetIconPopText(mItemTemp.id, popTitle, popDesc, 1);
		iconSampleManager.SetAwardNumber(AwardidNum);
	}

	public bool OnProcessSocketMessage(QXBuffer p_message){
		//Debug.Log("jieshouxinxi" );
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			
			case ProtoIndexes.GET_FULL_STAR_AWARD_RESP: // 领取秘宝星级奖励返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
//				GetFullStarAwardresp mGetFullStarAwardresp = new GetFullStarAwardresp();
//				
//				t_qx.Deserialize(t_stream, mGetFullStarAwardresp, mGetFullStarAwardresp.GetType());
//				
//				Debug.Log("mGetFullStarAwardresp = " +mGetFullStarAwardresp.success);
//				
//				if(mGetFullStarAwardresp.success == 1)
//				{
//					MiBaoScrollView.m_MiBaoScrollView.StarNum -= MiBaoScrollView.m_MiBaoScrollView.my_MiBaoInfo.needAllStar;
//					
//					MiBaoScrollView.m_MiBaoScrollView.my_MiBaoInfo.needAllStar = mGetFullStarAwardresp.nexNeedAllStar;
//					
//					MiBaoScrollView.m_MiBaoScrollView.ShowScrollBar();
//				}
//				else
//				{
//					Debug.Log("领取失败了 后台数据");
//				}
//				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
//				Close();
				return true;
			}
			default: return false;
			}
			
		}else
		{
			//Debug.Log ("p_message == null");
		}
		
		return false;
	}
	public void GetBtn()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.GET_FULL_STAR_AWARD_REQ);
	}
	public void Close()
	{
		Destroy (this.gameObject);
	}
}
