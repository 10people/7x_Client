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

	public GameObject redObj;

	public UILabel openLevel;

	private List<int> m_exceptList = new List<int> ();//已镶嵌的主属性符石类型list
	private List<int> g_exceptList = new List<int> ();//已镶嵌的高级属性符石类型list

	/// <summary>
	/// 符石栏位信息
	/// </summary>
	/// <param name="tempLanWei">栏位信息</param>
	/// <param name="tempIndex">显示解锁等级的符石下标</param>
	/// <param name="m_TempExceptList">已镶嵌的主属性符石类型list</param>
	/// <param name="g_TempExceptList">已镶嵌的高级属性符石类型list</param>
	public void GetLanWeiInfo (FuwenLanwei tempLanWei,int tempIndex,List<int> m_TempExceptList,List<int> g_TempExceptList)
	{
//		Debug.Log (tempIndex + "lanweiId:" + tempLanWei.lanweiId);
//		Debug.Log (tempIndex + "itemId:" + tempIndex + "||" + tempLanWei.itemId);

		fuWenLanWeiInfo = tempLanWei;

		FuWenOpenTemplate fuWenOpenTemp = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (tempLanWei.lanweiId);
		lanWeiType = fuWenOpenTemp.lanweiType;

		m_exceptList = m_TempExceptList;
		g_exceptList = g_TempExceptList;

		redObj.SetActive (tempLanWei.flag);

		//有符文为itemId，没有为0，未解锁为-1
		if (tempLanWei.itemId == -1)
		{
			fuShiIcon.spriteName = "";
			lockIcon.gameObject.SetActive (true);

			if (tempIndex == FuWenMainPage.fuWenMainPage.GetOpenLanWeiCount)
			{
				openLevel.text = fuWenOpenTemp.level + "级解锁";
			}
			else
			{
				openLevel.text = "";
			}
			return;
		}

		lockIcon.gameObject.SetActive (false);

		fuShiIcon.spriteName = tempLanWei.itemId.ToString ();

		openLevel.text = "";
	}

	void OnClick ()
	{
		if (!FuWenMainPage.fuWenMainPage.IsBtnClick && fuWenLanWeiInfo.itemId >= 0)
		{
			FuWenMainPage.fuWenMainPage.IsBtnClick = true;
//			Debug.Log ("fuWenLanWeiInfo.itemId:" + fuWenLanWeiInfo.itemId);
			if (fuWenLanWeiInfo.itemId == 0)
			{
				FuWenMainPage.fuWenMainPage.CurXiangQianId = fuWenLanWeiInfo.itemId;
				if (lanWeiType == 1 || lanWeiType == 2 || lanWeiType == 3)
				{
					FuWenMainPage.fuWenMainPage.SelectFuWen (FuWenSelect.SelectType.XIANGQIAN,fuWenLanWeiInfo.lanweiId,FuWenMainPage.FuShiType.MAIN_FUSHI,m_exceptList);
				}
				else
				{
					FuWenMainPage.fuWenMainPage.SelectFuWen (FuWenSelect.SelectType.XIANGQIAN,fuWenLanWeiInfo.lanweiId,FuWenMainPage.FuShiType.GAOJI_FUSHI,g_exceptList);
				}
			}
			else if (fuWenLanWeiInfo.itemId > 0)
			{
				FuWenMainPage.fuWenMainPage.OperateFuWen (FuShiOperate.OperateType.XIANGQIAN,fuWenLanWeiInfo.itemId,fuWenLanWeiInfo.lanweiId);
			}
		}
	}
}
