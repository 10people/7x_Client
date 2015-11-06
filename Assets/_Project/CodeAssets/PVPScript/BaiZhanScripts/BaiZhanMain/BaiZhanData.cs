using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanData : MonoBehaviour,SocketProcessor {

	private static BaiZhanData baiZhanData;

	public static BaiZhanData Instance ()
	{
		if (!baiZhanData)
		{
			baiZhanData = (BaiZhanData)GameObject.FindObjectOfType (typeof(BaiZhanData));
		}

		return baiZhanData;
	}

	public BaiZhanInfoResp m_baiZhanInfo;//百战info
	public ChallengeResp m_tiaoZhanResp;//挑战resp

	public GameObject baiZhanObj;

	void Awake () 
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		Global.m_isOpenBaiZhan = true;
		BaiZhanReq ();
	}

	//百战首页信息请求
	public void BaiZhanReq ()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.BAIZHAN_INFO_REQ,"27002|27012|27019");
		Debug.Log ("baiZhanReq:" + ProtoIndexes.BAIZHAN_INFO_REQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message) {
		
		if (p_message != null) {
		
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.BAIZHAN_INFO_RESP:
			{
				Debug.Log ("baiZhanRes:" + ProtoIndexes.BAIZHAN_INFO_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				BaiZhanInfoResp baiZhanInfo = new BaiZhanInfoResp();
				
				t_qx.Deserialize(t_stream, baiZhanInfo, baiZhanInfo.GetType());

				if (baiZhanInfo != null)
				{
					if (baiZhanInfo.oppoList == null)
					{
						baiZhanInfo.oppoList = new List<OpponentInfo>();
					}

					m_baiZhanInfo = baiZhanInfo;
//					Debug.Log ("yindao:" + FreshGuide.Instance().IsActive(100180) + TaskData.Instance.m_TaskInfoDic[100180].progress);
					GameObject baiZhanMain = GameObject.Find ("BaiZhanMain");
					if (baiZhanMain == null)
					{
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.BAIZHAN_MAIN_PAGE ),
						                        BaiZhanMainLoadCallback );
					}

					else
					{
						BaiZhanMainPage mainPage = baiZhanMain.GetComponent<BaiZhanMainPage> ();
						mainPage.baiZhanResp = m_baiZhanInfo;
						mainPage.InItMyRank ();
						mainPage.OpponentsInfo ();
						mainPage.DefensiveSetUp ();
						mainPage.ChallangeRules ();
					}
				}

				return true;
			}

			case ProtoIndexes.CHALLENGE_RESP://请求挑战返回
			{
				Debug.Log ("TiaoZhanRes:" + ProtoIndexes.CHALLENGE_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ChallengeResp tiaoZhanInfo = new ChallengeResp();
				
				t_qx.Deserialize(t_stream, tiaoZhanInfo, tiaoZhanInfo.GetType());
				
				if (tiaoZhanInfo != null)
				{
					m_tiaoZhanResp = tiaoZhanInfo;
					Debug.Log ("组合id：" + tiaoZhanInfo.myZuheId);
					GameObject challengeObj = GameObject.Find ("ChallengePage");
					if (challengeObj != null)
					{
						GeneralTiaoZhan tiaoZhan = challengeObj.GetComponent<GeneralTiaoZhan> ();
						
						tiaoZhan.InItPvpChallengePage (GeneralTiaoZhan.ZhenRongType.PVP,tiaoZhanInfo);
					}
					else
					{
						CloneGeneralTiaoZhan ();
					}
				}

				return true;
			}

			default:return false;
			}
		}

		return false;
	}

	public void BaiZhanMainLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject baiZhanMain = Instantiate (p_object) as GameObject;

		baiZhanMain.transform.parent = baiZhanObj.transform;
		baiZhanMain.name = "BaiZhanMain";
		
		baiZhanMain.transform.localPosition = Vector3.zero;
		baiZhanMain.transform.localScale = Vector3.one;

		BaiZhanMainPage mainPage = baiZhanMain.GetComponent<BaiZhanMainPage> ();
		mainPage.baiZhanResp = m_baiZhanInfo;
	}

	//实例化挑战页面
	void CloneGeneralTiaoZhan ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_CHALLENGE_PAGE ),
		                        ChallengeLoadCallback );
	}
	
	void ChallengeLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject challenge = Instantiate (p_object) as GameObject;
		challenge.name = "GeneralChallengePage";
		challenge.transform.parent = this.transform;
		challenge.transform.localPosition = Vector3.zero;
		challenge.transform.localScale = Vector3.one;
		
		GeneralTiaoZhan tiaoZhan = challenge.GetComponent<GeneralTiaoZhan> ();

		if (m_tiaoZhanResp.myZuheId > 0)
		{
			tiaoZhan.SetYinDaoState = 2;
		}
		else
		{
			List<MibaoGroup> mibaoGroup = MiBaoGlobleData.Instance ().G_MiBaoInfo.mibaoGroup;
			
			for(int i = 0;i < mibaoGroup.Count;i ++)
			{
				if (mibaoGroup[i].hasActive == 1)
				{
					tiaoZhan.SetYinDaoState = 1;
					break;
				}
				else
				{
					tiaoZhan.SetYinDaoState = 2;
				}
			}
		}

		tiaoZhan.InItPvpChallengePage (GeneralTiaoZhan.ZhenRongType.PVP,m_tiaoZhanResp);
	}

	//关闭百战
	public void CloseBaiZhan ()
	{
		Global.m_isOpenBaiZhan = false;
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Destroy (GameObject.Find ("BaiZhan"));
	}

	void OnDestroy () 
	{	
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
