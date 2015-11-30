using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MultipleReward : MonoBehaviour {

//	private ExploreAwardsInfo rewardResp;
//
//	public GameObject rewardParent;
//	public GameObject rewardObj;
//	private List<GameObject> rewardObjList = new List<GameObject> ();
//	public List<Award> rewardInfoList = new List<Award> ();//奖励卡片list
//
//	public GameObject qtObj;
//	private int qtNum;//青铜个数
//
//	public GameObject zheZhao;
//
//	public GameObject scrollviewObj;
//
//	private List<int> itemNumList = new List<int> ();//物品飞入物品栏数量list
//
//	private List<int> turnEndCardList = new List<int> ();//卡片翻转完毕个数
//
//	public GameObject sureBtn;
//
//	public GameObject stopPanel;
//
//	private GameObject qt;
//
//	public Dictionary<int,Award> miBaoAwardList = new Dictionary<int, Award> ();
//
//	private float miBaoCardShowTime = 1f;
//
//	public GameObject mibaoWinObj;//显示秘宝窗口
//
//	private int mibaoIndex = 0;
//	public int MiBaoIndex 
//	{
//		set{mibaoIndex = value;}
//		get{return mibaoIndex;}
//	}
//
//	private bool isClickOver = false;//连续点击是否点的第一张秘宝卡
//	public bool SetClickOver
//	{
//		set{isClickOver = value;}
//	}
//	public bool GetClickOver
//	{
//		get{return isClickOver;}
//	}
//
//	void Start ()
//	{
//		zheZhao.SetActive (true);
//		zheZhao.GetComponent<UISprite> ().alpha = 0.05f;
//		TweenAlpha zheZhaoAlpha = zheZhao.GetComponent<TweenAlpha> ();
//		zheZhaoAlpha.from = 0.05f;
//		zheZhaoAlpha.to = 1;
//		EventDelegate.Add (zheZhaoAlpha.onFinished,CreateRewardItem);
//
//		if (TimeHelper.Instance.IsTimeCalcKeyExist ("ShowMiBaoCard"))
//		{
//			TimeHelper.Instance.RemoveFromTimeCalc ("ShowMiBaoCard");
//		}
//	}
//
//	//获得探宝十抽信息
//	public void GetMultipleRewardInfo (ExploreAwardsInfo tempInfo)
//	{
//		rewardResp = tempInfo;
//
//		for (int i = 0;i < tempInfo.awardsList.Count;i ++)
//		{
//			if (tempInfo.awardsList[i].isQuality == 1)
//			{
//				qtNum = tempInfo.awardsList[i].itemNumber;
//				StartCoroutine (CreateJt (qtNum));
//			}
//
//			else
//			{
//				rewardInfoList.Add (tempInfo.awardsList[i]);
//			}
//		}
//	}
//
//	//创建青铜
//	IEnumerator CreateJt (int tempNum)
//	{
//		for (int i = 0;i < tempNum;i ++)
//		{
//			yield return new WaitForSeconds(0.1f);
//
//			qt = (GameObject)Instantiate (qtObj);
//
//			qt.SetActive (true);
//			qt.transform.parent = qtObj.transform.parent;
//			qt.transform.localPosition = qtObj.transform.localPosition;
//			qt.transform.localScale = qtObj.transform.localScale;
//			qt.GetComponent<ItemTransform>().icon.depth = 21 - i;
//
//			Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId(100140).path, 
//			                        ResourceLoadCallbackShadowTemple );
//
//			ItemTransform item = qt.GetComponent<ItemTransform> ();
//			item.tanBaoType = rewardResp.type;
//		}
//	}
//
//	public void ResourceLoadCallbackShadowTemple( ref WWW p_www, string p_path, UnityEngine.Object p_object )
//	{
//		GameObject effectObj = GameObject.Instantiate (p_object) as GameObject;
//		effectObj.SetActive (false);
//		effectObj.name = "ItemEffectObj";
//		effectObj.transform.parent = qt.transform;
//		effectObj.transform.localPosition = Vector3.zero;
//		effectObj.transform.localScale = Vector3.one;
//	}
//
//	//创建奖励卡片
//	void CreateRewardItem ()
//	{
//		StartCoroutine (CreateReward ());
//	}
//	//创建奖励卡片
//	IEnumerator CreateReward ()
//	{
//		for (int i = 0;i < rewardInfoList.Count;i ++)
//		{
//			yield return new WaitForSeconds(0.1f);
//
//			GameObject rewardItem = (GameObject)Instantiate (rewardObj);
//			
//			rewardItem.SetActive (true);
//			rewardItem.transform.parent = rewardParent.transform;
//			rewardItem.transform.localPosition = rewardObj.transform.localPosition;
//			rewardItem.transform.localScale = Vector3.zero;
//
//			rewardObjList.Add (rewardItem);
//
//			RewardCardInfo rewardCard = rewardItem.GetComponent<RewardCardInfo> ();
//			rewardCard.cardInfo = rewardInfoList[i];
//			rewardCard.tanBaoType = rewardResp.type;
//			rewardCard.InItCardInfo ();
//
//			KmCardFly fly = rewardItem.GetComponent<KmCardFly> ();
//			fly.tanBaoType = rewardResp.type;
//			fly.CardTrans (TanBaoManager.tbManager.flyPointList[i].transform.localPosition);
//		}
//	}
//
//	//获得物品飞入物品栏数量
//	public void GetItemNum (int num)
//	{
//		itemNumList.Add (num);
//
//		if (itemNumList.Count == qtNum)
//		{
//			zheZhao.GetComponent<TweenAlpha> ().enabled = true;
////			Debug.Log ("Create Begin!");
//		}
//	}
//
//	void Update ()
//	{
//		if (rewardResp.type == 10)
//		{
//			qtObj.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3 (295,0,0);
//			rewardObj.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3 (295,0,0);
//		}
//		
//		else if (rewardResp.type == 12)
//		{
//			qtObj.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3 (885,0,0);
//			rewardObj.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3 (885,0,0);
//		}
//	}
//
//	//获得翻转完毕的卡片个数
//	public void GetTurnCardEndNum (int num)
//	{
//		turnEndCardList.Add (num);
//
//		if (turnEndCardList.Count == rewardInfoList.Count)
//		{
//			CheckExitMiBao ();
////			sureBtn.SetActive (true);
//		}
//	}
//
//	public void TurnMiBao (bool isFlag)
//	{
////		Debug.Log ("hahahah" + isFlag);
//		stopPanel.SetActive (isFlag);
//		stopPanel.GetComponent<UISprite> ().alpha = 0.05f;
//	}
//
//	/// <summary>
//	/// 获得秘宝卡信息
//	/// </summary>
//	/// <param name="cardInfo">Card info.</param>
//	public void GetMiBaoCardInfo (Award cardInfo)
//	{
////		Debug.Log ("PreAdd:" + miBaoAwardList.Count);
//		if (!miBaoAwardList.ContainsValue (cardInfo))
//		{
//			miBaoAwardList.Add (miBaoAwardList.Count,cardInfo);
//		}
////		Debug.Log ("miBaoAwardList.Count:" + miBaoAwardList.Count);
////		Debug.Log ("AfterAdd:" + miBaoAwardList.Count);
//		if (!isClickOver)
//		{
//			isClickOver = true;
//
////			Debug.Log ("isClickOver:" + isClickOver);
//
//			StopCoroutine ("ShowTimeWait");
//			StartCoroutine ("ShowTimeWait");
//		}
//	}
//
//	/// <summary>
//	/// 规定秘宝卡显示时间
//	/// </summary>
//	IEnumerator ShowTimeWait ()
//	{
//		yield return new WaitForSeconds (1);
//		CheckExitMiBao ();
//	}
//
//	/// <summary>
//	/// 检测是否还存在未显示秘宝卡
//	/// </summary>
//	public void CheckExitMiBao ()
//	{
////		Debug.Log ("mibaoIndex:" + mibaoIndex + "||" + "miBaoAwardList:" + miBaoAwardList.Count);
//		//显示大卡
//		if (mibaoIndex < miBaoAwardList.Count)
//		{
////			Debug.Log ("Haha");
////			TanBaoManager.tbManager.CameraShake ();
//			GameObject miBaoWin = GameObject.Find ("MiBaoWindow" + mibaoIndex);
////			Debug.Log ("mibaoIndex：" + mibaoIndex);
//			if (miBaoWin == null)
//			{
//				miBaoWin = (GameObject)Instantiate (mibaoWinObj);
//				
//				miBaoWin.SetActive (true);
//				miBaoWin.name = "MiBaoWindow" + mibaoIndex;
//				
//				miBaoWin.transform.parent = mibaoWinObj.transform.parent;
//				miBaoWin.transform.localPosition = mibaoWinObj.transform.localPosition;
//				miBaoWin.transform.localScale = mibaoWinObj.transform.localScale;
//				
//				ShowMiBaoReward showMiBao = miBaoWin.GetComponent<ShowMiBaoReward> ();
//				showMiBao.miBaoRewardInfo = miBaoAwardList[mibaoIndex];
//				showMiBao.tanBaoType = rewardResp.type;
//				showMiBao.ShowMiBao ();
//			}
//		}
//		else
//		{
//			TurnMiBao (false);
//			ClearMibaoAwardList ();
//			if (turnEndCardList.Count == rewardInfoList.Count)
//			{
//				sureBtn.SetActive (true);
//			}
//		}
//	}
//
//	void ClearMibaoAwardList ()
//	{
//		mibaoIndex = 0;
//		miBaoAwardList.Clear ();
//
////		Debug.Log ("miBaoAwardList.Count:" + miBaoAwardList.Count);
//	}
//
//	public void SureBtn ()
//	{
//		List<RewardData> dataList = new List<RewardData> ();
//		Dictionary <int,int> rewardDic = new Dictionary<int, int> ();
//		List<Award> awardList = rewardResp.awardsList;
//		for (int i = 0;i < awardList.Count;i ++)
//		{
//			int itemId = awardList[i].pieceNumber > 0 ? awardList[i].pieceId : awardList[i].itemId;
//			int itemNum = awardList[i].pieceNumber > 0 ? awardList[i].pieceNumber : awardList[i].itemNumber;
//
//			if (!rewardDic.ContainsKey (itemId))
//			{
//				rewardDic.Add (itemId,itemNum);
//			}
//			else
//			{
//				rewardDic[itemId] += itemNum;
//			}
//		}
//
//		foreach (KeyValuePair <int,int> pair in rewardDic)
//		{
////			Debug.Log (pair.Key + " - " + pair.Value);
//
//			RewardData data = new RewardData(pair.Key,pair.Value);
//
//			dataList.Add (data);
//		}
//
//		GeneralRewardManager.Instance ().CreateReward (dataList);
//
//		zheZhao.SetActive (false);
//		Destroy (this.gameObject);
//	}
}
