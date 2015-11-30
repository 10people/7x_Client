using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RewardCardInfo : MonoBehaviour {
	
//	public Award cardInfo;
//	[HideInInspector]public int tanBaoType;
//
//	public EventHandler cardHandler;
//	private BoxCollider cardBoxCollider;
//
//	public GameObject shiningObj;
//
//	public UISprite cardBg;
//	public GameObject cardTexObj;//显示信息卡片obj
//
//	private GameObject iconSamplePrefab;
//
//	public UISprite cardBorder;
//	private int pinZhiId;
//	
//	public UILabel nameLabel;
//	
//	public UILabel num_disLabel;
//
//	public GameObject multipObj;
//	public GameObject singleObj;
//	
//	public GameObject mibaoWinObj;//显示秘宝窗口
//	
//	public GameObject moneyManagerObj;
//	
//	private float time1;
//	private float time2;
//	private Vector3 currentPos;
//
//	private int effectId;
//	private int borderId;
//
//	void Start ()
//	{
//		time1 = 0.3f;
//		time2 = 0.4f;
//		
////		currentPos = new Vector3 (-6,-6,0);
//		currentPos = Vector3.zero;
//	}
//
//	public void InItCardInfo ()
//	{	
//		cardHandler.GetComponent<BoxCollider> ().enabled = false;
//		cardBg.spriteName = "CardBg_back";
//		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (cardInfo.itemId);
//		int nameId = commonTemp.nameId;
//		pinZhiId = commonTemp.color;//0 | 1.2 | 3.4.5 | 6.7.8 | 9.10
//
//		if (cardInfo.itemType == 5)
//		{
//			List<char> nameCharList = new List<char>();
//			
//			for (int i = 0;i < NameIdTemplate.GetName_By_NameId (nameId).Length;i ++)
//			{
//				if (i < 5)
//				{
//					nameCharList.Add (NameIdTemplate.GetName_By_NameId (nameId)[i]);
//				}
//			}
//			string nameStr = "";
//			for (int i = 0;i < nameCharList.Count;i ++)
//			{
//				nameStr += nameCharList[i];
//			}
//			
//			nameLabel.text = nameStr;
//		}
//		else
//		{
//			nameLabel.text = NameIdTemplate.GetName_By_NameId (nameId);
//		}
//
//		//秘宝
//		if (cardInfo.itemType == 4)
//		{
//			nameLabel.gameObject.transform.localPosition = Vector3.zero;
//			
//			num_disLabel.text = "";
//		}
//
//		else
//		{
//			//秘宝碎片
//			if (cardInfo.itemType == 5)
//			{	
//				string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_PIECEWINDOW_DES3);
//				
//				num_disLabel.text = str + "x" + cardInfo.itemNumber;
//			}
//			
//			//进阶材料，强化材料
//			else
//			{	
//				nameLabel.gameObject.transform.localPosition = new Vector3(0,10,0);
//				
//				num_disLabel.text = "x" + cardInfo.itemNumber;
//			}
//		}
//
//		ShowBorder (pinZhiId,cardBorder);
//
//		if (iconSamplePrefab == null)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
//			                        IconSampleLoadCallBack);
//		}
//		else
//		{
//			WWW tempWww = null;
//			IconSampleLoadCallBack(ref tempWww, null, iconSamplePrefab);
//		}
//
//		cardHandler.m_handler += ClickCardBg;
//	}
//
//	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
//	{
//		if (iconSamplePrefab == null) {
//			iconSamplePrefab = p_object as GameObject;
//		}
//
//		GameObject iconSample = (GameObject)Instantiate (iconSamplePrefab);
//		
//		iconSample.SetActive (true);
//		iconSample.transform.parent = cardTexObj.transform;
//		iconSample.transform.localPosition = new Vector3 (0, 20, 0);
//
//		iconSample.GetComponent<UIDragScrollView> ().enabled = false;
//
//		//0普通道具;3当铺材料;4秘宝;5秘宝碎片;6进阶材料;7基础宝石;8高级宝石;9强化材料
//		IconSampleManager iconSampleManager = iconSample.GetComponent<IconSampleManager>();
//		string itemName = "";
//		if (cardInfo.itemType == 4)//秘宝
//		{
//			MiBaoXmlTemp mibaoTemp = MiBaoXmlTemp.getMiBaoXmlTempById (cardInfo.itemId);
//			itemName = NameIdTemplate.GetName_By_NameId (mibaoTemp.nameId);
//			iconSampleManager.SetIconType (IconSampleManager.IconType.MiBao);
//		}
//		else if (cardInfo.itemType == 5)//秘宝碎片 MibaoSuiPian表
//		{
//			MiBaoSuipianXMltemp miBaoSuiPian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (cardInfo.itemId);
//			itemName = NameIdTemplate.GetName_By_NameId (miBaoSuiPian.m_name);
//			iconSampleManager.SetIconType(IconSampleManager.IconType.MiBaoSuiPian);
//		}
//		else
//		{
//			ItemTemp item = ItemTemp.getItemTempById (cardInfo.itemId);
//			itemName = NameIdTemplate.GetName_By_NameId (item.itemName);
//			if (cardInfo.itemType == 0 || cardInfo.itemType == 3 || cardInfo.itemType == 6 || cardInfo.itemType == 9)
//			{
//				iconSampleManager.SetIconType(IconSampleManager.IconType.equipment);
//			}
//			else if (cardInfo.itemType == 7 || cardInfo.itemType == 8)
//			{
//				iconSampleManager.SetIconType(IconSampleManager.IconType.FuWen);
//			}
//		}
//		
//		iconSampleManager.SetIconBasic(24,cardInfo.itemId.ToString ());
//		
//		string mdesc = DescIdTemplate.GetDescriptionById(cardInfo.itemId);
//
//		iconSampleManager.SetIconBasicDelegate (true,true,ClickItem);
//		iconSampleManager.BgSprite.gameObject.SetActive (false);
//		iconSampleManager.SetIconPopText(cardInfo.itemId, itemName, mdesc, 1);
//	}
//
//	void ShowBorder (int pinZhi,UISprite border)
//	{
//		border.spriteName = "pinzhi" + (pinZhi - 1);
//	}
//
//	//点击卡背
//	public void ClickCardBg (GameObject obj)
//	{	
//		cardHandler.GetComponent<BoxCollider> ().enabled = false;
//
//		float time = 0;
//		if (cardInfo.itemType == 4)
//		{	
//			UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,cardBg.gameObject,EffectIdTemplate.GetPathByeffectId(100155));
//			TanBaoManager.tbManager.CameraShake ();
//			time = 0.2f;
//
//			if (tanBaoType == 10 || tanBaoType == 12)
//			{
//				MultipleReward multip = multipObj.GetComponent<MultipleReward> ();
//				//multip.TurnMiBao (true);
//				if (multip.miBaoAwardList.Count <= 0)
//				{
//					multip.GetMiBaoCardInfo (cardInfo);
//				}
//			}
//		}
//		else
//		{
//			UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,shiningObj,EffectIdTemplate.GetPathByeffectId(100141));
//		}
//		StartCoroutine (WaitForTurn (time));
//	}
//
//	IEnumerator WaitForTurn (float waitTime)
//	{
//		yield return new WaitForSeconds (waitTime);
//
//		Hashtable bgRotate = new Hashtable ();
//		bgRotate.Add ("easetype",iTween.EaseType.easeOutQuart);
//		bgRotate.Add ("time",time1);
//		bgRotate.Add ("rotation",new Vector3(0,90,0));
//		bgRotate.Add ("oncomplete","BgRotateEnd");
//		bgRotate.Add ("oncompletetarget",gameObject);
//		iTween.RotateTo (cardBg.gameObject,bgRotate);
//	}
//
//	//卡背旋转完毕
//	void BgRotateEnd ()
//	{
//		//		Debug.Log ("卡背旋转完毕");
//		cardBg.spriteName = "CardBg";
//
//		UI3DEffectTool.Instance ().ClearUIFx (cardBg.gameObject);
//
//		cardTexObj.SetActive (true);
//		CardObjRotate ();
//	}
//	
//	//奖励卡旋转
//	void CardObjRotate ()
//	{
//		Hashtable cardRotate = new Hashtable ();
//		
//		cardRotate.Add ("easetype",iTween.EaseType.easeOutQuart);
//		cardRotate.Add ("time",time2);
//		cardRotate.Add ("rotation",new Vector3(0,0,0));
//		cardRotate.Add ("oncomplete","CardTexRotateEnd");
//		cardRotate.Add ("oncompletetarget",gameObject);
//		
//		iTween.RotateTo (cardBg.gameObject,cardRotate);
//	}
//	
//	void CardTexRotateEnd ()
//	{
//		//		Debug.Log ("卡片旋转完毕");
//		if (cardInfo.itemType == 9)
//		{
//			ItemTemp itemXml = ItemTemp.getItemTempById (cardInfo.itemId);
//			
//			int iconId = int.Parse (itemXml.icon);
//			
//			MoneyManager money = moneyManagerObj.GetComponent<MoneyManager> ();
//			
//			switch (iconId)
//			{
//			case 920001:
//				
//				money.GetJtOrQt (tanBaoType,cardInfo.itemNumber);
//				
//				break;
//				
//			case 920002:
//				
//				money.GetJtOrQt (tanBaoType,cardInfo.itemNumber);
//				
//				break;
//			}
//		}
//		
//		if (tanBaoType == 10 || tanBaoType == 12)
//		{
//			MultipleReward multip = multipObj.GetComponent<MultipleReward> ();
//			multip.GetTurnCardEndNum (1);
//		}
//		else
//		{
//			if (cardInfo.itemType != 4)
//			{
//				this.gameObject.GetComponent<BoxCollider> ().enabled = true;
//			}
//		}
//
//		if (cardInfo.itemType == 4)
//		{
//			int showId = 0;//0 | 1.2 | 3.4.5 | 6.7.8 | 9.10
//			if (pinZhiId == 0)
//			{
//				showId = 0;
//			}
//			else if (pinZhiId == 1 || pinZhiId == 2)
//			{
//				showId = 1;
//			}
//			else if (pinZhiId == 3 || pinZhiId == 4 || pinZhiId == 5)
//			{
//				showId = 2;
//			}
//			else if (pinZhiId == 6 || pinZhiId == 7 || pinZhiId == 8)
//			{
//				showId = 3;
//			}
//			else if (pinZhiId == 9 || pinZhiId == 10)
//			{
//				showId = 4;
//			}
//			effectId = 100150 + showId;
//			borderId = 100142 + showId;
//			
//			UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,cardBg.gameObject,
//			                                               EffectIdTemplate.GetPathByeffectId(effectId));
//			UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,cardBg.gameObject,
//			                                               EffectIdTemplate.GetPathByeffectId(borderId));
//
//			if (tanBaoType == 0 || tanBaoType == 1 || tanBaoType == 11)
//			{
//				GameObject miBaoWin = (GameObject)Instantiate (mibaoWinObj);
//				
//				miBaoWin.SetActive (true);
//				miBaoWin.name = "MiBaoWindow";
//				
//				miBaoWin.transform.parent = mibaoWinObj.transform.parent;
//				miBaoWin.transform.localPosition = mibaoWinObj.transform.localPosition;
//				miBaoWin.transform.localScale = mibaoWinObj.transform.localScale;
//				
//				ShowMiBaoReward showMiBao = miBaoWin.GetComponent<ShowMiBaoReward> ();
//				showMiBao.miBaoRewardInfo = cardInfo;
//				showMiBao.tanBaoType = tanBaoType;
//				showMiBao.ShowMiBao ();
//			}
//			else if (tanBaoType == 10 || tanBaoType == 12)
//			{
//				MultipleReward multip = multipObj.GetComponent<MultipleReward> ();
//				//multip.TurnMiBao (true);
//				if (multip.miBaoAwardList.Count > 0)
//				{
//					multip.GetMiBaoCardInfo (cardInfo);
//				}
//			}
//		}
//	}
//
//	void OnClick ()
//	{
//		ClickItem (gameObject);
//	}
//
//	void ClickItem (GameObject obj)
//	{
//		if (tanBaoType == 0 || tanBaoType == 1 || tanBaoType == 11)
//		{
//			SingleReward singleReward = singleObj.GetComponent<SingleReward> ();
//			singleReward.ZheZhaoControl (false);
//
//			List<RewardData> dataList = new List<RewardData>();
//			RewardData data1;
//			int itemId = 0;
//			int itemCnt = 0;
//			RewardData data2;
//			int awardId = 0;
//			awardId = tanBaoType == 0 ? 920001 : 920002;
//
//			if (cardInfo.itemId == awardId)
//			{
//				cardInfo.itemNumber += 1;
//				itemId = cardInfo.itemId;
//				itemCnt = cardInfo.itemNumber;
//			}
//			else
//			{
//				data2 = new RewardData(awardId,1);
//				dataList.Add (data2);
//				
//				itemId = cardInfo.itemId;
//				itemCnt = cardInfo.itemNumber;
//			}
//			data1 = new RewardData(cardInfo.itemId,cardInfo.itemNumber);
//			dataList.Add (data1);
//	
//			GeneralRewardManager.Instance ().CreateReward (dataList);
//
//			Destroy (singleObj);
//		}
//	}
}