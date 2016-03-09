using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuShiItem : MonoBehaviour {

	private FuwenLanwei fuWenLanWeiInfo;

	private int lanWeiType;//栏位属性

	public UISprite fuShiIcon;
	public UISprite lockIcon;
	public UISprite colorIcon;

	public GameObject redObj;

	public UILabel openLevel;

	private List<int> exceptList = new List<int> ();//已镶嵌的符石属性list

	/// <summary>
	/// 符石栏位信息
	/// </summary>
	/// <param name="tempLanWei">栏位信息</param>
	public void GetLanWeiInfo (FuwenLanwei tempLanWei,List<int> tempExistList)
	{
//		Debug.Log (tempIndex + "lanweiId:" + tempLanWei.lanweiId);
//		Debug.Log (tempIndex + "itemId:" + tempIndex + "||" + tempLanWei.itemId);

		fuWenLanWeiInfo = tempLanWei;

		FuWenOpenTemplate fuWenOpenTemp = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (tempLanWei.lanweiId);
		lanWeiType = fuWenOpenTemp.lanweiType;

		exceptList = tempExistList;

		redObj.SetActive (tempLanWei.flag);

		//有符文为itemId，没有为0，未解锁为-1
		fuShiIcon.spriteName = tempLanWei.itemId > 0 ? tempLanWei.itemId.ToString () : "";
		lockIcon.gameObject.SetActive (tempLanWei.itemId == -1 ? true : false);
		colorIcon.spriteName = tempLanWei.itemId == 0 ? "inlayColor" + fuWenOpenTemp.inlayColor : "";
		openLevel.text = tempLanWei.itemId == -1 ? (fuWenOpenTemp.level == FuWenMainPage.fuWenMainPage.NextOpenLevel ? fuWenOpenTemp.level + "级解锁" : "") : "";

		this.GetComponent<EventHandler> ().m_click_handler -= FuShiHandlerClickBack;
		this.GetComponent<EventHandler> ().m_click_handler += FuShiHandlerClickBack;
	}

	void FuShiHandlerClickBack (GameObject obj)
	{
		if (!FuWenMainPage.fuWenMainPage.IsBtnClick && fuWenLanWeiInfo.itemId >= 0)
		{
			FuWenMainPage.fuWenMainPage.IsBtnClick = true;
//			Debug.Log ("fuWenLanWeiInfo.itemId:" + fuWenLanWeiInfo.itemId);

			bool isMainShuXing = lanWeiType == 1 || lanWeiType == 2 || lanWeiType == 3 ? true : false;

			FuWenMainPage.fuWenMainPage.SelectFuWen (FuWenSelect.SelectType.XIANGQIAN,
			                                         fuWenLanWeiInfo,
			                                         isMainShuXing ? FuWenMainPage.FuShiType.MAIN_FUSHI : FuWenMainPage.FuShiType.GAOJI_FUSHI,
			                                         exceptList);
		}
	}
}
