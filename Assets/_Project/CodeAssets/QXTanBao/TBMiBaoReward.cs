using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TBMiBaoReward : MonoBehaviour {

	public static TBMiBaoReward tbMibaoReward;

	private Award awardInfo;
	private TanBaoData.TanBaoType tbType;

	public EventHandler blockHandler;
	public GameObject miBaoObj;
	public GameObject pieceObj;

	public GameObject topBorderEffectObj;
	public UITexture mbTexTure;//秘宝图标
	public UISprite border;
	public UILabel miBaoName;
	public GameObject starObj;
	private List<GameObject> starList = new List<GameObject>();

	public UILabel pieceDesLabel;
	private GameObject iconSamplePrefab;
	public EventHandler sureHandler;
	private int suiPianTempId;

	void Awake ()
	{
		tbMibaoReward = this;
	}

	void Start ()
	{
		starList.Add (starObj);
		for (int i = 0;i < 4;i ++)
		{
			GameObject star = (GameObject)Instantiate (starObj);
			
			star.transform.parent = starObj.transform.parent;
			star.transform.localPosition = Vector3.zero;
			star.transform.localScale = Vector3.one;
			starList.Add (star);
		}
	}

	/// <summary>
	/// Shows the mibao reward.
	/// </summary>
	/// <param name="tempAward">Temp award.</param>
	public void ShowMibaoReward (Award tempAward,TanBaoData.TanBaoType tempType)
	{
		//reset mibaoReward
		blockHandler.gameObject.SetActive (true);
		blockHandler.m_handler -= BlockHandlerBack;
		sureHandler.m_handler -= SureHandlerBack;

		awardInfo = tempAward;
		tbType = tempType;

		InItMibaoCardInfo ();
	}

	/// <summary>
	/// Ins it mibao card info.
	/// </summary>
	void InItMibaoCardInfo ()
	{
		miBaoObj.transform.localScale = Vector3.zero;
		MiBaoXmlTemp mbXml = MiBaoXmlTemp.getMiBaoXmlTempById(awardInfo.itemId);
		
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (awardInfo.itemId);
		int iconId = commonTemp.icon;
		int nameId = commonTemp.nameId;
		int pinZhiId = commonTemp.color;

		mbTexTure.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON)
		                                                + iconId.ToString ());

		border.spriteName = "pinzhi" + (pinZhiId - 1);

		for (int i = 0;i < starList.Count;i ++)
		{
			if (i < awardInfo.miBaoStar)
			{
				starList[i].SetActive (true);
				starList[i].transform.localPosition = new Vector3(i * 35 - (awardInfo.miBaoStar - 1) * 17.5f,0,0);
			}
			else
			{
				starList[i].SetActive (false);
			}
		}
		
		miBaoName.text = NameIdTemplate.GetName_By_NameId (nameId);

		miBaoObj.SetActive (true);
		TanBaoReward.tbReward.ItweenScale (Vector3.one,0.5f,iTween.EaseType.easeOutQuart,"MiBaoScaleEnd",miBaoObj,gameObject);
	}

	void MiBaoScaleEnd ()
	{
		QXComData.InstanceEffect (QXComData.EffectPos.TOP,topBorderEffectObj,100148);
		QXComData.InstanceEffect (QXComData.EffectPos.MID,miBaoObj,100157);

		blockHandler.m_handler += BlockHandlerBack;
	}

	void BlockHandlerBack (GameObject obj)
	{
		blockHandler.m_handler -= BlockHandlerBack;
		miBaoObj.SetActive (false);
		UI3DEffectTool.Instance ().ClearUIFx (miBaoObj);
		UI3DEffectTool.Instance ().ClearUIFx (topBorderEffectObj);

		if (awardInfo.pieceNumber != null && awardInfo.pieceNumber != 0)
		{
			//can turn to piece
			InItPieceInfo ();
		}
		else
		{
//			CloseMiBaoReward ();
			if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE)
			{
				CloseMiBaoReward ();
			}
			else
			{
				TanBaoReward.tbReward.CheckMiBaoCard ();
			}
		}
	}

	/// <summary>
	/// Ins it piece info.
	/// </summary>
	void InItPieceInfo ()
	{
		pieceObj.SetActive (true);
		pieceObj.transform.localScale = Vector3.zero;
		sureHandler.m_handler -= SureHandlerBack;

		MiBaoXmlTemp mibaoTemp = MiBaoXmlTemp.getMiBaoXmlTempById (awardInfo.itemId);
		suiPianTempId = mibaoTemp.tempId;

		//您已拥有<秘宝名五字>
		//此秘宝将转化为<秘宝名五字>碎片
		//碎片可用于提升秘宝星级
		string miBaoName = NameIdTemplate.GetName_By_NameId (mibaoTemp.nameId);
		pieceDesLabel.text = "您已拥有<" + miBaoName + ">" + "\n此秘宝将转化为<" + miBaoName + ">碎片\n碎片可用于提升秘宝星级";

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			PieceIconSample ();
		}

		TanBaoReward.tbReward.ItweenScale (Vector3.one,0.5f,iTween.EaseType.easeInOutQuart,"PieceScaleEnd",pieceObj,gameObject);
	}
	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive (true);
		iconSamplePrefab.transform.parent = pieceObj.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (0, -35, 0);
		
		PieceIconSample ();
	}
	void PieceIconSample ()
	{
		MiBaoSuipianXMltemp miBaoSuiPian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (suiPianTempId);
		string itemName = NameIdTemplate.GetName_By_NameId (miBaoSuiPian.m_name);
		string mdesc = DescIdTemplate.GetDescriptionById(miBaoSuiPian.funDesc);

		IconSampleManager iconSampleManager = iconSamplePrefab.GetComponent<IconSampleManager>();
		iconSampleManager.SetIconType(IconSampleManager.IconType.MiBaoSuiPian);
		iconSampleManager.SetIconBasic(1,miBaoSuiPian.icon.ToString ());

		iconSampleManager.SetIconBasicDelegate (true,true,null);
		iconSampleManager.BgSprite.gameObject.SetActive (false);
		iconSampleManager.SetIconPopText(miBaoSuiPian.id, itemName, mdesc, 1);
	}

	void PieceScaleEnd ()
	{
		sureHandler.m_handler += SureHandlerBack;
	}

	void SureHandlerBack (GameObject obj)
	{
		pieceObj.SetActive (false);
		if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE)
		{
			CloseMiBaoReward ();
		}
		else
		{
			TanBaoReward.tbReward.CheckMiBaoCard ();
		}
	}

	/// <summary>
	/// Closes the mi bao reward.
	/// </summary>
	public void CloseMiBaoReward ()
	{
		blockHandler.gameObject.SetActive (false);
		if (tbType == TanBaoData.TanBaoType.TONGBI_SPEND || tbType == TanBaoData.TanBaoType.YUANBAO_SPEND)
		{
			TanBaoReward.tbReward.BlockController (true);
			TanBaoReward.tbReward.MiBaoListClear ();
		}
	}
}
