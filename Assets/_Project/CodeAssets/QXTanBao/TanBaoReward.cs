using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TanBaoReward : MonoBehaviour {

	public static TanBaoReward tbReward;

	private ExploreResp tbRewardResp;

	private TanBaoData.TanBaoType tbType;

	private TanBaoPage.RewardType rewardType;

	private List<GameObject> cardList = new List<GameObject> ();
	public GameObject cardObj;

	private List<Vector3> cardPosList = new List<Vector3> ();

	public EventHandler blockHandler;
	public EventHandler sureHandler;

	private int cardTurnEndNum;

	private List<Award> miBaoList = new List<Award>();
	private int miBaoIndex;
	public int MiBaoIndex
	{
		set{miBaoIndex = value;}
		get{return miBaoIndex;}
	}

	void Awake ()
	{
		tbReward = this;
	}

	void Start ()
	{
		if (cardList.Count == 0)
		{
			for (int i = 0;i < 10;i ++)
			{
				GameObject card = (GameObject)Instantiate (cardObj);
				
				card.transform.parent = cardObj.transform.parent;
				card.transform.localPosition = Vector3.zero;
				card.transform.localScale = Vector3.zero;
				
				cardList.Add (card);
			}
		}
		if (cardPosList.Count == 0)
		{
			for (int i = 0;i < 10;i ++)
			{
				cardPosList.Add (new Vector3(-340f + 170f * (i < 5 ? i : i - 5),
				                             i < 5 ? 120f : -120f,
				                             0f));
			}
		}
	}

	/// <summary>
	/// Blocks the controller.
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> is active.</param>
	public void BlockController (bool isActive,float alpha)
	{
		blockHandler.gameObject.SetActive (isActive);
		blockHandler.GetComponent<UISprite> ().alpha = alpha;
	}

	/// <summary>
	/// Gets the TB reward info.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempRewardResp">Temp reward resp.</param>
	public void GetTBRewardInfo (TanBaoData.TanBaoType tempType,ExploreResp tempRewardResp)
	{
		tbRewardResp = tempRewardResp;
		tbType = tempType;

		cardTurnEndNum = 0;
		MiBaoListClear ();

		rewardType = tempType == TanBaoData.TanBaoType.TONGBI_SINGLE || tempType == TanBaoData.TanBaoType.TONGBI_SPEND ?
			TanBaoPage.RewardType.KUANGDONG : TanBaoPage.RewardType.KUANGJING;

		StopCoroutine ("BlockAlpha");
		StartCoroutine ("BlockAlpha");
	}
	IEnumerator BlockAlpha ()
	{
		while (blockHandler.GetComponent<UISprite> ().alpha < 1)
		{
			blockHandler.GetComponent<UISprite> ().alpha += 0.05f;
			yield return new WaitForSeconds (0.01f);
			
			if (blockHandler.GetComponent<UISprite> ().alpha >= 1)
			{
				Debug.Log ("CardTrans");
				CreateTBReward ();
			}
		}
	}

	/// <summary>
	/// Creates the TB reward.
	/// </summary>
	void CreateTBReward ()
	{
		if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE) 
		{
			InstanceCardObj (0,Vector3.zero);
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

	/// <summary>
	/// Instances the card object.
	/// </summary>
	/// <param name="tempIndex">Temp index.</param>
	/// <param name="tempTargetPos">Temp target position.</param>
	void InstanceCardObj (int tempIndex,Vector3 tempTargetPos)
	{
		cardList[tempIndex].SetActive (true);
		cardList[tempIndex].transform.localPosition = new Vector3(rewardType == TanBaoPage.RewardType.KUANGDONG ? -225f : 225f,0f,0f);
		cardList[tempIndex].transform.localScale = Vector3.zero;
		
		TBCardInfo cardInfo = cardList[tempIndex].GetComponent<TBCardInfo> ();
		cardInfo.GetTBCardInfo (tbType,tbRewardResp.awardsList[tempIndex],tempTargetPos);
	}

	/// <summary>
	/// Gets the card turn end number.
	/// </summary>
	public void GetCardTurnEndNum ()
	{
		cardTurnEndNum ++;
		if (cardTurnEndNum >= cardList.Count)
		{
			sureHandler.gameObject.SetActive (true);
			sureHandler.GetComponent<UISprite> ().alpha = 0;
			StopCoroutine ("SureBtnAlpha");
			StartCoroutine ("SureBtnAlpha");
		}
	}
	IEnumerator SureBtnAlpha ()
	{
		while (sureHandler.GetComponent<UISprite> ().alpha < 1)
		{
			sureHandler.GetComponent<UISprite> ().alpha += 0.1f;
			yield return new WaitForSeconds (0.01f);
			if (sureHandler.GetComponent<UISprite> ().alpha >= 1)
			{
				sureHandler.m_handler += SureBtnHandlerBack;
			}
		}
	}

	void SureBtnHandlerBack (GameObject obj)
	{
		foreach (GameObject card in cardList)
		{
			TBCardInfo cardInfo = card.GetComponent<TBCardInfo> ();
			cardInfo.ClearAllEffect ();
		}
		sureHandler.m_handler -= SureBtnHandlerBack;
		sureHandler.gameObject.SetActive (false);
		BlockController (false,0);
	}

	/// <summary>
	/// Itweens the scale.
	/// </summary>
	/// <param name="tempScale">Temp scale.</param>
	/// <param name="tempTime">Temp time.</param>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempMethod">Temp method.</param>
	/// <param name="tempObj">Temp object.</param>
	public void ItweenScale (Vector3 tempScale,float tempTime,iTween.EaseType tempType,string tempMethod,GameObject tempObj,GameObject tempTargetObj)
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("scale",tempScale);
		scale.Add ("time",tempTime);
		scale.Add ("easetype",tempType);
		scale.Add ("islocal",true);
		scale.Add ("oncomplete",tempMethod);
		scale.Add ("oncompletetarget",tempTargetObj);
		iTween.ScaleTo (tempObj,scale);
	}

	/// <summary>
	/// Adds the award in to mi bao list.
	/// </summary>
	/// <param name="tempAward">Temp award.</param>
	public void AddAwardInToMiBaoList (Award tempAward)
	{
		miBaoList.Add (tempAward);
	}

	/// <summary>
	/// Shows the mibao card.
	/// </summary>
	public void ShowMibaoCard ()
	{
		if (MiBaoIndex < miBaoList.Count)
		{
			TBMiBaoReward.tbMibaoReward.ShowMibaoReward (miBaoList[MiBaoIndex],tbType);
		}
		else
		{
			//关闭显示秘宝
			TBMiBaoReward.tbMibaoReward.CloseMiBaoReward ();
		}
	}

	/// <summary>
	/// Checks the mi bao card.
	/// </summary>
	public void CheckMiBaoCard ()
	{
		MiBaoIndex ++;
		ShowMibaoCard ();
	}

	/// <summary>
	/// Mis the bao list clear.
	/// </summary>
	public void MiBaoListClear ()
	{
		MiBaoIndex = 0;
		miBaoList.Clear ();
	}
}
