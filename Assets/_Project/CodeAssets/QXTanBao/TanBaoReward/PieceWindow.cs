using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PieceWindow : MonoBehaviour {

	public UILabel label1;
	
	public UILabel label2;

	private int iconId;
	private int pinZhiId;

	private int cnt;
	private string pieceName;
	private string pieceDes;
	private int itemId;
	
	private float time = 0.5f;

	public GameObject miBaoWinObj;

//	public GameObject singleObj;
//	public GameObject multipObj;

	public GameObject pieceWinObj;

	public int tanBaoType;//探宝类型

	private GameObject iconSamplePrefab;
	
	//获得奖励信息
	public void GetAwardInfo (Award tempAward)
	{
		itemId = tempAward.itemId;
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (tempAward.pieceId);
		iconId = commonTemp.icon;
		int nameId = commonTemp.nameId;
		pieceName = NameIdTemplate.GetName_By_NameId (nameId);
		pieceDes = DescIdTemplate.GetDescriptionById (tempAward.itemId);

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_PIECEWINDOW_DES1);
		string str21 = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_PIECEWINDOW_DES2);
		string str22 = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_PIECEWINDOW_DES3);

		//		Debug.Log ("itemId:" + tempAward.itemId);
//		Debug.Log ("pieceId:" + tempAward.pieceId);

		MiBaoXmlTemp mibaoTemp = MiBaoXmlTemp.getMiBaoXmlTempById (tempAward.itemId);
		int mibaoNameId = mibaoTemp.nameId;
		cnt = tempAward.pieceNumber;
		pinZhiId = mibaoTemp.pinzhi;

//		Debug.Log ("pinZhiId:" + pinZhiId);

		label1.text = str1 + "<" + NameIdTemplate.GetName_By_NameId (mibaoNameId) + ">";
		
		label2.text = str21 + "<" + NameIdTemplate.GetName_By_NameId (mibaoNameId) + ">" + str22;

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			WWW tempWww = null;
			IconSampleLoadCallBack(ref tempWww, null, iconSamplePrefab);
		}

		PieceWindowAnim ();
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (iconSamplePrefab == null)
		{
			iconSamplePrefab = p_object as GameObject;
		}
		
		GameObject iconSample = (GameObject)Instantiate (iconSamplePrefab);
		
		iconSample.SetActive(true);
		iconSample.transform.parent = pieceWinObj.transform;
		iconSample.transform.localPosition = new Vector3 (0,-35,0);
		
		IconSampleManager fuShiIconSample = iconSample.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconType(IconSampleManager.IconType.equipment);
		fuShiIconSample.SetIconBasic(2,iconId.ToString (),"x" + cnt.ToString (),"pinzhi" + pinZhiId);
		fuShiIconSample.SetIconPopText(iconId, pieceName, pieceDes, 1);
	}

	//秘宝碎片窗口动画
	void PieceWindowAnim ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("time",time);
		scale.Add ("scale",Vector3.one);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		iTween.ScaleTo(pieceWinObj,scale);
	}
	
	//点击确定按钮
	public void ClickSureBtn ()
	{
		if (tanBaoType == 10 || tanBaoType == 12)
		{
			MultipleReward multipleReward = GameObject.Find ("MultipleReward(Clone)").GetComponent<MultipleReward> ();
			multipleReward.MiBaoIndex ++;
			multipleReward.CheckExitMiBao ();
			multipleReward.SetClickOver = false;
		}
		else
		{
			GameObject singleObj = GameObject.Find ("SingleReward(Clone)");
			SingleReward singleReward = singleObj.GetComponent<SingleReward> ();
			singleReward.ZheZhaoControl (false);

			List<RewardData> dataList = new List<RewardData>();
			RewardData data1;
			data1 = new RewardData(iconId,cnt);
			dataList.Add (data1);

			RewardData data2;
			data2 = new RewardData(tanBaoType == 0 ? 920001 : 920002,1);
			dataList.Add (data2);
	
			GeneralRewardManager.Instance ().CreateReward (dataList);

			Destroy (singleObj);
		}

		Destroy (miBaoWinObj);
	}
}
