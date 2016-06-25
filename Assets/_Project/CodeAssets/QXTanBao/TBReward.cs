using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TBReward : MonoBehaviour {

	public static TBReward tbReward;

	private TanBaoData.TanBaoType tbType;
	private ExploreResp tbRewardResp;

	private enum TBRewardType
	{
		KUANGDONG,
		KUANGJING,
	}
	private TBRewardType tbRewardType;

	public GameObject cardItemObj;
	private List<GameObject> cardItemList = new List<GameObject> ();

	private List<Vector3> cardPosList = new List<Vector3> ();

	public UILabel m_rewardDes;

	public EventHandler blockHandler;
	public UILabel desLabel;
	private string desStr = "[dbba8f]点击任意位置继续[-]";
	private float time = 1;

	private bool isShowMiBao;
	public bool IsShowMiBao {set{isShowMiBao = value;} get{return isShowMiBao;}}

	void Awake ()
	{
		tbReward = this;
	}

	void OnDestroy ()
	{
		tbReward = null;
	}

	void Start ()
	{
		for (int i = 0;i < 10;i ++)
		{
			GameObject card = (GameObject)Instantiate (cardItemObj);
			
			card.transform.parent = cardItemObj.transform.parent;
			card.transform.localPosition = Vector3.zero;
			card.transform.localScale = Vector3.zero;
			
			cardItemList.Add (card);
		}
	
		for (int i = 0;i < 10;i ++)
		{
			cardPosList.Add (new Vector3(-300f + 150f * (i < 5 ? i : i - 5),
			                             i < 5 ? 140f : -120f,
			                             0f));
		}
	}

	/// <summary>
	/// Gets the TB reward.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempRewardResp">Temp reward resp.</param>
	public void GetTBReward (TanBaoData.TanBaoType tempType,ExploreResp tempRewardResp)
	{
		tbType = tempType;
		tbRewardResp = tempRewardResp;

		tbRewardType = tempType == TanBaoData.TanBaoType.TONGBI_SINGLE || tempType == TanBaoData.TanBaoType.TONGBI_SPEND ?
			TBRewardType.KUANGDONG : TBRewardType.KUANGJING;

		//reset info
		{
			IsShowMiBao = false;
			UnTurnIndex = 0;
			TurnIndex = 0;
			turnList.Clear ();
			totleTurnList.Clear ();
			blockHandler.m_click_handler -= BlockHandlerClickBack;
			desLabel.text = "";
			m_rewardDes.text = "";
			time = 1;
			StopCoroutine ("ShowDesLabel1");
			StopCoroutine ("ShowDesLabel2");
		}

		StopCoroutine ("BlockAlpha");
		StartCoroutine ("BlockAlpha");
	}
	IEnumerator BlockAlpha ()
	{
		while (blockHandler.GetComponent<UISprite> ().alpha < 1)
		{
			blockHandler.GetComponent<UISprite> ().alpha += 0.05f;
			yield return new WaitForSeconds (0.01f);

			bool higher = false;
			int num = 0;
			float posY = 0;
			if (blockHandler.GetComponent<UISprite> ().alpha >= 1)
			{
				switch (tbType)
				{
				case TanBaoData.TanBaoType.TONGBI_SINGLE:
					higher = false;
					num = 1;
					posY = 150;
					break;
				case TanBaoData.TanBaoType.TONGBI_SPEND:
					higher = false;
					num = 10;
					posY = 265;
					break;
				case TanBaoData.TanBaoType.YUANBAO_SINGLE:
					higher = true;
					num = 1;
					posY = 150;
					break;
				case TanBaoData.TanBaoType.YUANBAO_SPEND:
					higher = true;
					num = 10;
					posY = 265;
					break;
				default:
					break;
				}
				m_rewardDes.transform.localPosition = new Vector3(0,posY,0);
				m_rewardDes.text = "获得[cd02d8]" + (higher ? "中级" : "初级") + "武器强化石[-]"
					+ QXComData.yellow + "x" + num + "[-]，并赠送";

				CreateTBReward ();
			}
		}
	}

	void CreateTBReward ()
	{
		if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE) 
		{
			InstanceCardObj (0,new Vector3(0,30,0));
		} 
		else
		{
			StopCoroutine ("InstanceSpendCard");
			StartCoroutine ("InstanceSpendCard");
		}
	}
	IEnumerator InstanceSpendCard ()
	{
		for (int i = 0;i < tbRewardResp.awardsList.Count;i ++)
		{
			yield return new WaitForSeconds (0.1f);
			InstanceCardObj (i,cardPosList[i]);
		}
	}

	void InstanceCardObj (int tempIndex,Vector3 tempTargetPos)
	{
		cardItemList[tempIndex].SetActive (true);
		cardItemList[tempIndex].transform.localPosition = new Vector3 (tbRewardType == TBRewardType.KUANGDONG ? -225f : 225f,0f,0f);
		cardItemList[tempIndex].transform.localScale = Vector3.zero;

		TBRewardCard tbReward = cardItemList[tempIndex].GetComponent<TBRewardCard>();
		tbReward.GetCardInfo (tbRewardResp.awardsList[tempIndex],tempTargetPos,tempIndex);

		CardInfoListControl (true,tbReward);
	}

	#region CardTurnControl
	private List<TBRewardCard> turnList = new List<TBRewardCard> ();
	private List<TBRewardCard> totleTurnList = new List<TBRewardCard> ();

	public void CardInfoListControl (bool isAdd,TBRewardCard tempInfo)
	{
		if (isAdd)
		{
			totleTurnList.Add (tempInfo);
		}
		else
		{
			turnList.Add (tempInfo);
		}
	}
	#endregion

	#region CheckCardRotate
	private int unTurnIndex = 0;
	public int UnTurnIndex {set{unTurnIndex = value;} get{return unTurnIndex;}}

	private int turnIndex = 0;
	public int TurnIndex {set{turnIndex = value;} get{return turnIndex;}}

	public void CheckUnTurnRotate ()
	{
		if (UnTurnIndex < totleTurnList.Count)
		{
			totleTurnList[UnTurnIndex].CardBgRotate ();
		}
	}

	public void CheckTurnRotate ()
	{
		if (TurnIndex >= totleTurnList.Count)
		{
			UIWidget widget = desLabel.GetComponent<UIWidget> ();
			widget.alpha = 0;
			StartCoroutine ("ShowDesLabel2");
			blockHandler.m_click_handler += BlockHandlerClickBack;
		}
	}

	IEnumerator ShowDesLabel1 ()
	{
		desLabel.text = desStr;
		UIWidget widget = desLabel.GetComponent<UIWidget> ();
		while (widget.alpha > 0.2f)
		{
			yield return new WaitForSeconds (time);
			widget.alpha -= 0.1f;
			if (widget.alpha <= 0.2f)
			{
				StopCoroutine ("ShowDesLabel1");
				StartCoroutine ("ShowDesLabel2");
			}
		}
	}
	
	IEnumerator ShowDesLabel2 ()
	{
		desLabel.text = desStr;
		UIWidget widget = desLabel.GetComponent<UIWidget> ();
		while (widget.alpha < 1)
		{
			yield return new WaitForSeconds (time);
			time = 0.07f;
			widget.alpha += 0.1f;
			if (widget.alpha >= 1)
			{
				StopCoroutine ("ShowDesLabel2");
				StartCoroutine ("ShowDesLabel1");
			}
		}
	}

	#endregion

	void BlockHandlerClickBack (GameObject obj)
	{
		foreach (TBRewardCard card in turnList)
		{
			card.ClearEffect ();
			card.gameObject.SetActive (false);
		}

		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.FINISHED_TASK_YINDAO,100160,4);

		UIShoujiManager.m_UIShoujiManager.m_isPlayShouji = true;

		BlockController (false);
	}

	public void BlockController (bool isActive,float alpha = 0)
	{
		blockHandler.gameObject.SetActive (isActive);
		blockHandler.GetComponent<UISprite> ().alpha = alpha;
		m_rewardDes.text = "";
	}

	public void TurnListEffectControl (bool isClear)
	{
		foreach (TBRewardCard tbRewardCard in turnList)
		{
			if (isClear)
			{
				tbRewardCard.ClearEffect ();
			}
			else
			{
				tbRewardCard.InstanceEffect ();
			}
		}
	}

	#region ShowMiBaoInfo
	private Award rewardInfo;
	public GameObject m_mainCamera;
	public void ShowMiBaoCard (Award tempInfo)
	{
		rewardInfo = tempInfo;
//		Debug.Log ("tempInfo.itemId:" + tempInfo.itemId);
//		Debug.Log ("tempInfo.itemNumber:" + tempInfo.itemNumber);
		RewardData data = new RewardData (tempInfo.itemId,tempInfo.itemNumber,tempInfo.miBaoStar,CloseShowMiBao);
		data.m_isNew = tempInfo.pieceNumber > 0 ? false : true;
		data.m_cameraObj = m_mainCamera;
//		if (tempInfo.pieceNumber > 0)
//		{
//			data = new RewardData (tempInfo.itemId,tempInfo.itemNumber,tempInfo.miBaoStar,ShowSuiPianInfo);
//		}
//		else
//		{
//			data = new RewardData (tempInfo.itemId,tempInfo.itemNumber,tempInfo.miBaoStar,CloseShowMiBao);
//		}
		GeneralRewardManager.Instance().CreateSpecialReward (data);
	}

	void ShowSuiPianInfo ()
	{
//		TBMiBaoSuiPian.m_instance.GetMiBaoSuiPianInfo (rewardInfo);
	}

	void CloseShowMiBao ()
	{
		IsShowMiBao = false;

		//向后台请求获得秘宝广播
		int miBaoCount = 0;
		List<MibaoInfo> miBaoList = MiBaoGlobleData.Instance().G_MiBaoInfo.miBaoList;
		foreach (MibaoInfo miBao in miBaoList)
		{
			miBaoCount += (miBao.level > 0 ? 1 : 0);
		}
//		Debug.Log ("miBaoCount:" + miBaoCount);
//		Debug.Log ("rewardInfo.miBaoStar:" + rewardInfo.miBaoStar);
//		Debug.Log ("rewardInfo.pieceNumber:" + rewardInfo.pieceNumber);
		if (rewardInfo.miBaoStar >= 3 || (AnnounceTemplate.IsSendAnnounce (3,miBaoCount)) && rewardInfo.pieceNumber == 0)
		{
			QXComData.SendQxProtoMessage (ProtoIndexes.C_CLOSE_TAN_BAO_UI);
//			Debug.Log ("ProtoIndexes.C_CLOSE_TAN_BAO_UI:" + ProtoIndexes.C_CLOSE_TAN_BAO_UI);
		}

		//check card
		UnTurnIndex ++;
		if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE)
		{
			BlockHandlerClickBack (gameObject);
		}
		else
		{
			TurnListEffectControl (false);
			CheckUnTurnRotate ();
			CheckTurnRotate ();
		}
	}
	#endregion

	/// <summary>
	/// Itweens the scale.
	/// </summary>
	/// <param name="tempState">Temp state.</param>
	/// <param name="tempMethod">Temp method.</param>
	/// <param name="tempObj">Temp object.</param>
	public void ItweenScale (int tempState,string tempMethod,GameObject tempObj)
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("scale",tempState == 1 ? new Vector3 (0,1,1) : Vector3.one);
		scale.Add ("time",0.15f);
		scale.Add ("easetype",iTween.EaseType.linear);
		scale.Add ("islocal",true);
		scale.Add ("oncomplete",tempMethod);
		scale.Add ("oncompletetarget",tempObj);
		iTween.ScaleTo (tempObj,scale);
	}
}
