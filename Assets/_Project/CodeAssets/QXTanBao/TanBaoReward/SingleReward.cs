using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SingleReward : MonoBehaviour {

//	private ExploreAwardsInfo rewardResp;//奖励信息
//
//	private Award cardInfo;//奖励卡片信息
//
//	public GameObject itemObj;//精铁或青铜obj
//
//	public GameObject cardItem;//奖励卡
//
//	public GameObject scrollviewObj;
//
//	public GameObject zheZhao;//遮罩
//
//	private GameObject item;
//
//	void Start ()
//	{
//		ZheZhaoControl (true);
//		zheZhao.GetComponent<UISprite> ().alpha = 0.05f;
//		TweenAlpha zheZhaoAlpha = zheZhao.GetComponent<TweenAlpha> ();
//		zheZhaoAlpha.from = 0.05f;
//		zheZhaoAlpha.to = 1;
//		EventDelegate.Add (zheZhaoAlpha.onFinished,InItCardItem);
//	}
//
//	//获得奖励信息
//	public void GetRewardInfo (ExploreAwardsInfo tempInfo)
//	{
//		rewardResp = tempInfo;
//
//		UISprite itemSprite = itemObj.GetComponent<UISprite> ();
//		if (tempInfo.type == 0)
//		{
//			itemSprite.spriteName = "920001";
//		}
//
//		else
//		{
//			itemSprite.spriteName = "920002";
//		}
//
//		for (int i = 0;i < tempInfo.awardsList.Count;i ++)
//		{
//			if (tempInfo.awardsList[i].isQuality == 1)
//			{
//				StartCoroutine (WaitForInstant ());
//			}
//			
//			else 
//			{
//				cardInfo = tempInfo.awardsList[i];
//			}
//		}
//	}
//	
//	IEnumerator WaitForInstant ()
//	{
//		yield return new WaitForSeconds (0.2f);
//		
//		CloneItem ();
//	}
//	//克隆物品
//	void CloneItem ()
//	{
//		item = (GameObject)Instantiate (itemObj);
//		
//		item.SetActive (true);
//		
//		item.transform.parent = itemObj.transform.parent;
//		item.transform.localPosition = itemObj.transform.localPosition;
//		item.transform.localScale = itemObj.transform.localScale;
//	
////		UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI,item,EffectIdTemplate.GetPathByeffectId(100140));
//
//		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId(100140).path, 
//		                        ResourceLoadCallbackShadowTemple );
//
//		ItemTransform itemTrans = item.GetComponent<ItemTransform> ();
//		itemTrans.tanBaoType = rewardResp.type;
//	}
//
//	public void ResourceLoadCallbackShadowTemple( ref WWW p_www, string p_path, UnityEngine.Object p_object )
//	{
//		GameObject effectObj = GameObject.Instantiate (p_object) as GameObject;
//		effectObj.SetActive (false);
//		effectObj.name = "ItemEffectObj";
//		effectObj.transform.parent = item.transform;
//		effectObj.transform.localPosition = Vector3.zero;
//		effectObj.transform.localScale = Vector3.one;
//	}
//
//	//物品飞到上边显示栏后背景变暗
//	public void GetItemTransState (bool isFlyEnd)
//	{
//		if (isFlyEnd)
//		{
//			zheZhao.GetComponent<TweenAlpha>().enabled = true;
//		}
//	}
//
//	//克隆cardItem
//	void InItCardItem ()
//	{
//		GameObject card = (GameObject)Instantiate (cardItem);
//		
//		card.SetActive (true);
//		
//		card.transform.parent = this.transform;
//		card.transform.localPosition = cardItem.transform.localPosition;
//		card.transform.localScale = Vector3.zero;
//		
//		RewardCardInfo rewardCard = card.GetComponent<RewardCardInfo> ();
//		rewardCard.cardInfo = cardInfo;
//		rewardCard.tanBaoType = rewardResp.type;
//		rewardCard.InItCardInfo ();
//		
//		KmCardFly fly = card.GetComponent<KmCardFly> ();
//		fly.tanBaoType = rewardResp.type;
//		fly.CardTrans (Vector3.zero);
//	}
//
//	//得到物体坐标
//	void Update ()
//	{
//		if (rewardResp.type == 0)
//		{
//			itemObj.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3 (-295,-90,0);
//			cardItem.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3 (-295,0,0);
//		}
//		else if (rewardResp.type == 1)
//		{
//			itemObj.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3 (0,-90,0);
//			cardItem.transform.localPosition = scrollviewObj.transform.localPosition;
//		}
//		
//		else if (rewardResp.type == 11)
//		{
//			itemObj.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3(590,-90,0);
//			cardItem.transform.localPosition = scrollviewObj.transform.localPosition + new Vector3(590,0,0);
//		}
//	}
//
//	public bool ZheZhaoControl (bool isActive)
//	{
//		zheZhao.gameObject.SetActive (isActive);
//		return true;
//	}
}
