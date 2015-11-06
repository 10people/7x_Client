using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenMainPage : MonoBehaviour {

	public static FuWenMainPage fuWenMainPage;

	private QueryFuwenResp fuWenResp;//符石首页返回信息

	public GameObject xiangQianPage;//镶嵌页面
	public GameObject heChengPage;//合成页面

	public UILabel zhanLiLabel;//战力

	public List<EventHandler> btnsHandlerList = new List<EventHandler> ();

	public UILabel tipsLabel;//提示label
	private long zhanLi;//战力
	private long endZhanLi;//最终战力
	private bool refreshZhanLi = false;//刷新战力

	public enum ShowPageType
	{
		PAGE_XIANGQIAN,
		PAGE_HECHENG,
	}
	private ShowPageType pageType = ShowPageType.PAGE_XIANGQIAN;//默认显示镶嵌页

	public ScaleEffectController m_ScaleEffectController;

	void Awake ()
	{
		fuWenMainPage = this;
	}

	void Start ()
	{
//		FuWenData.Instance.FuWenDataReq ();
	}

	/// <summary>
	/// 初始化符文首页
	/// </summary>
	public void InItFuWenPage (QueryFuwenResp tempResp)
	{
		m_ScaleEffectController.gameObject.transform.localScale = Vector3.one;

		fuWenResp = tempResp;

		if (zhanLiLabel.text == "")
		{
			zhanLi = tempResp.zhanli;
			zhanLiLabel.text = tempResp.zhanli.ToString ();
		}
		else
		{
			AddZhanLi (tempResp.zhanli);
		}

		FuWenListSort (tempResp);

		if(FreshGuide.Instance().IsActive(100300) && TaskData.Instance.m_TaskInfoDic[100300].progress >= 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100300];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
		}

		btnsHandlerList[0].m_handler += XiangQianBtn;//镶嵌按钮
		btnsHandlerList[1].m_handler += HeChengBtn;//合成按钮
	}

	#region
	/// <summary>
	/// 筛选未锁定的符石，对符石进行排序
	/// </summary>
	private List<Fuwen> mixFuWenList = new List<Fuwen> ();//可合成的符石list
	private List<Fuwen> bagFuWenList = new List<Fuwen> ();//背包的符石list
	private List<Fuwen> mainFuWenList = new List<Fuwen>();//主属性符石list
	private List<Fuwen> gaoJiFuWenList = new List<Fuwen>();//高级符文list

	void FuWenListSort (QueryFuwenResp tempResp)
	{
		List<int> levelList = new List<int> ();//符石等级
		for (int i = 0;i < tempResp.fuwens.Count;i ++)
		{
			FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (tempResp.fuwens[i].itemId);
			levelList.Add (fuWenTemp.fuwenLevel);
		}
		
		for (int i = 0;i < levelList.Count - 1;i ++)
		{
			for (int j = 0;j < levelList.Count - i - 1;j ++)
			{
				if (levelList[j] == levelList[j + 1])
				{
					if (tempResp.fuwens[j].itemId > tempResp.fuwens[j + 1].itemId)
					{
						int tempLevel = levelList[j];
						levelList[j] = levelList[j + 1];
						levelList[j + 1] = tempLevel;
						
						Fuwen tempFuWen = tempResp.fuwens[j];
						tempResp.fuwens[j] = tempResp.fuwens[j + 1];
						tempResp.fuwens[j + 1] = tempFuWen;
					}
				}
				else
				{
					if (levelList[j] < levelList[j + 1])
					{
						int tempLevel = levelList[j];
						levelList[j] = levelList[j + 1];
						levelList[j + 1] = tempLevel;
						
						Fuwen tempFuWen = tempResp.fuwens[j];
						tempResp.fuwens[j] = tempResp.fuwens[j + 1];
						tempResp.fuwens[j + 1] = tempFuWen;
					}
				}
			}
		}

		bagFuWenList.Clear ();
		mixFuWenList.Clear ();
		mainFuWenList.Clear ();
		gaoJiFuWenList.Clear ();

		for (int i = 0;i < tempResp.fuwens.Count;i ++)
		{
			bagFuWenList.Add (tempResp.fuwens[i]);
			//			Debug.Log ("bagFuWenList:" + bagFuWenList[i].itemId + "||" + bagFuWenList[i].cnt);

			if (tempResp.fuwens[i].isLock == 2 && tempResp.fuwens[i].cnt >= 4)//未锁定
			{
				mixFuWenList.Add (tempResp.fuwens[i]);
			}
		}
//		for (int i = 0;i < mixFuWenList.Count;i ++)
//		{
//			Debug.Log ("mixFuWenList:" + mixFuWenList[i].itemId + "||" + mixFuWenList[i].cnt);
//		}

		//区分主属性符石和高级属性符石
		for (int i = 0;i < bagFuWenList.Count;i ++)
		{
			FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (bagFuWenList[i].itemId);
			if (fuWenTemp.type == 7)
			{
				mainFuWenList.Add (bagFuWenList[i]);
			}
			else if (fuWenTemp.type == 8)
			{
				gaoJiFuWenList.Add (bagFuWenList[i]);
			}
		}

		switch (pageType)
		{
		case ShowPageType.PAGE_XIANGQIAN:
			XiangQianBtn (gameObject);
			break;

		case ShowPageType.PAGE_HECHENG:
			HeChengBtn (gameObject);
			break;
		default:
			break;
		}

		InItXiangQianPage (fuWenResp);
		InItHeChengPage (bagFuWenList);
	}
	#endregion

	#region
	/// <summary>
	/// 镶嵌页
	/// </summary>
	public UIScrollView pageSc;
	public UIScrollBar pageSb;
	public GameObject fuWenPageGrid;
	public GameObject fuWenPageItemObj;//符文页item
	private List<GameObject> fuWenPageItemList = new List<GameObject> ();
	public List<UILabel> attributeLabels = new List<UILabel> ();//属性labels
	private JunzhuAttr totleAttributes;//总属性值
	private JunzhuAttr bonuses;//属性加成值

	private int openLanWeiCount = 0;//开启的栏位个数
	public int GetOpenLanWeiCount
	{
		get{return openLanWeiCount;}
	}

	private int curXiangQianId;//点击当前栏位上镶嵌的符石id
	public int CurXiangQianId
	{
		set{curXiangQianId = value;}
		get{return curXiangQianId;}
	}

	private float pageSbValue;

	//初始化镶嵌页
	void InItXiangQianPage (QueryFuwenResp tempResp)
	{
		for (int i = 0;i < tempResp.attr.Count;i ++)
		{
			switch (tempResp.attr[i].type)
			{
			case 1:

				totleAttributes = tempResp.attr[i];

				break;

			case 2:

				bonuses = tempResp.attr[i];

				break;

			default:
				break;
			}
		}

		attributeLabels [0].text = MyColorData.getColorString (3,totleAttributes.gongji.ToString () + "[00ff00](" + bonuses.gongji.ToString () + ")[-]");//攻击
		attributeLabels [1].text = MyColorData.getColorString (3,totleAttributes.fangyu.ToString () + "[00ff00](" + bonuses.fangyu.ToString () + ")[-]");//防御
		attributeLabels [2].text = MyColorData.getColorString (3,totleAttributes.shengming.ToString () + "[00ff00](" + bonuses.shengming.ToString () + ")[-]");//生命
		attributeLabels [3].text = MyColorData.getColorString (6,totleAttributes.wqSH.ToString () + "[00ff00](" + bonuses.wqSH.ToString () + ")[-]");//武器伤害加深
		attributeLabels [4].text = MyColorData.getColorString (6,totleAttributes.wqBJ.ToString () + "[00ff00](" + bonuses.wqBJ.ToString () + ")[-]");//武器暴击加深
		attributeLabels [5].text = MyColorData.getColorString (7,totleAttributes.jnSH.ToString () + "[00ff00](" + bonuses.jnSH.ToString () + ")[-]");//技能伤害加深
		attributeLabels [6].text = MyColorData.getColorString (7,totleAttributes.jnBJ.ToString () + "[00ff00](" + bonuses.jnBJ.ToString () + ")[-]");//技能暴击加深
		attributeLabels [7].text = MyColorData.getColorString (6,totleAttributes.wqJM.ToString () + "[00ff00](" + bonuses.wqJM.ToString () + ")[-]");//武器伤害抵抗
		attributeLabels [8].text = MyColorData.getColorString (6,totleAttributes.wqRX.ToString () + "[00ff00](" + bonuses.wqRX.ToString () + ")[-]");//武器暴击抵抗
		attributeLabels [9].text = MyColorData.getColorString (7,totleAttributes.jnJM.ToString () + "[00ff00](" + bonuses.jnJM.ToString () + ")[-]");//技能伤害抵抗
		attributeLabels [10].text = MyColorData.getColorString (7,totleAttributes.jnRX.ToString () + "[00ff00](" + bonuses.jnRX.ToString () + ")[-]");//技能暴击抵抗
	
		openLanWeiCount = 0;//开启的栏位个数
		for (int i = 0;i < tempResp.lanwei.Count;i ++)
		{
			if (tempResp.lanwei[i].itemId != -1)
			{
				openLanWeiCount ++;
			}
		}

		//对符文栏位进行开启顺序排序
		List<int> openLevelList = new List<int> ();
		for (int i = 0;i < tempResp.lanwei.Count;i ++)
		{
			FuWenOpenTemplate fuWenOpenTemp = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (tempResp.lanwei[i].lanweiId);

			openLevelList.Add (fuWenOpenTemp.level);
		}
		for (int i = 0;i < openLevelList.Count - 1;i ++)
		{
			for (int j = 0;j < openLevelList.Count - i - 1;j ++)
			{
				if (openLevelList[j] > openLevelList[j + 1])
				{
					int tempLevel = openLevelList[j];
					openLevelList[j] = openLevelList[j + 1];
					openLevelList[j + 1] = tempLevel;

					FuwenLanwei tempLanWei = tempResp.lanwei[j];
					tempResp.lanwei[j] = tempResp.lanwei[j + 1];
					tempResp.lanwei[j + 1] = tempLanWei;
				}
			}
		}

		int fuWenPageNum = tempResp.lanwei.Count % 11 > 0? (tempResp.lanwei.Count / 11) + 1 : tempResp.lanwei.Count / 11;
		Debug.Log ("符文页：" + fuWenPageNum);
//		foreach (GameObject obj in fuWenPageItemList)
//		{
//			Destroy (obj);
//		}
//		fuWenPageItemList.Clear ();

		if (fuWenPageItemList.Count == 0)
		{
			for (int i = 0;i < fuWenPageNum;i ++)
			{
				GameObject fuWenPageItem = (GameObject)Instantiate (fuWenPageItemObj);
				
				fuWenPageItem.SetActive (true);
				fuWenPageItem.transform.parent = fuWenPageGrid.transform;
				fuWenPageItem.transform.localPosition = new Vector3(0,-445 * i,0);
				fuWenPageItem.transform.localScale = Vector3.one;
				
				fuWenPageItemList.Add (fuWenPageItem);

				pageSc.UpdateScrollbars (true);
			}
		}

		for (int i = 0;i < fuWenPageNum;i ++)
		{
			FuWenPageItem fuWenPage = fuWenPageItemList[i].GetComponent<FuWenPageItem> ();
			fuWenPage.InItFuWenPageInfo (i + 1,tempResp.lanwei);
		}

		if(FreshGuide.Instance().IsActive(100300) && TaskData.Instance.m_TaskInfoDic[100300].progress >= 0)
		{
			pageSc.enabled = false;
		}
		else
		{
			pageSc.enabled = true;
		}

		btnsHandlerList [2].m_handler += FuShiMixBtn;//普通合成
		btnsHandlerList [3].m_handler += FuShiYiJianMixBtn;//一键合成
	}
	#endregion

	#region
	/// <summary>
	/// 合成页
	/// </summary>
	public UIScrollView bagSc;
	public UIScrollBar bagSb;
	public GameObject fuWenBagGrid;
	public GameObject fuWenBagItemObj;//背包符文item
	private List<GameObject> fuWenBagItemList = new List<GameObject> ();
	public UISprite heFuWenSprite;//合成后的符文icon
	public List<UIButton> fuShiBtnList = new List<UIButton> ();//合成按钮list
	public UILabel desLabel;
	
	private int curHeChengItemId;//当前合成孔上的符石itemid
	public int CurHeChengItemId
	{
		set{curHeChengItemId = value;}
		get{return curHeChengItemId;}
	}

	private float bagSbValue;

	//初始化合成页
	void InItHeChengPage (List<Fuwen> tempList)
	{
		ShowMixBtns ();
	
		isBtnClick = false;

		desLabel.text = tempList.Count > 0 ? "" : "背包空空的什么也没有";
//		foreach (GameObject obj in fuWenBagItemList)
//		{
//			Destroy (obj);
//		}
//		fuWenBagItemList.Clear ();

		int bagItemCount = tempList.Count - fuWenBagItemList.Count;
		if (bagItemCount > 0)
		{
			for (int i = 0;i < bagItemCount;i ++)
			{
				GameObject fuWenBagItem = (GameObject)Instantiate (fuWenBagItemObj);
				
				fuWenBagItem.SetActive (true);
				fuWenBagItem.transform.parent = fuWenBagGrid.transform;
				fuWenBagItem.transform.localPosition = new Vector3(0,-100 * (fuWenBagItemList.Count + i),0);
				fuWenBagItem.transform.localScale = Vector3.one;
				
				fuWenBagItemList.Add (fuWenBagItem);
			}
		}
		else if (bagItemCount < 0)
		{
			for (int i = 0;i < Mathf.Abs (bagItemCount);i ++)
			{
				Destroy (fuWenBagItemList[fuWenBagItemList.Count - 1]);
				fuWenBagItemList.RemoveAt (fuWenBagItemList.Count - 1);
			}
		}
		bagSc.UpdateScrollbars (true);
		for (int i = 0;i < tempList.Count;i ++)
		{
			FuWenBagItem fuWenBag = fuWenBagItemList[i].GetComponent<FuWenBagItem> ();
			fuWenBag.GetFuWenInfo (tempList[i]);
		}

		bagSc.enabled = tempList.Count < 4 ? false : true;
	}

	//显示合成按钮状态
	public void ShowMixBtns ()
	{
		bool reset = false;
		if (curHeChengItemId > 0)
		{
			int fuShiCnt = 0;
			for (int i = 0;i < bagFuWenList.Count;i ++)
			{
				if (bagFuWenList[i].itemId == curHeChengItemId)
				{
					fuShiCnt += bagFuWenList[i].cnt;
				}
			}

			reset = fuShiCnt >= 4 ? true : false;
			btnsHandlerList[2].GetComponent<UISprite> ().color = fuShiCnt >= 4 ? Color.white : Color.gray;
			btnsHandlerList[2].GetComponent<BoxCollider> ().enabled = fuShiCnt >= 4 ? true : false;

			btnsHandlerList[3].GetComponent<UISprite> ().color = fuShiCnt >= 8 ? Color.white : Color.gray;
			btnsHandlerList[3].GetComponent<BoxCollider> ().enabled = fuShiCnt >= 8 ? true : false;
		}
		else
		{
			reset = false;
			btnsHandlerList[2].GetComponent<UISprite> ().color = Color.gray;
			btnsHandlerList[2].GetComponent<BoxCollider> ().enabled = false;
			
			btnsHandlerList[3].GetComponent<UISprite> ().color = Color.gray;
			btnsHandlerList[3].GetComponent<BoxCollider> ().enabled = false;
		}

		for (int i = 0;i < fuShiBtnList.Count;i ++)
		{
			FuWenMixBtn fuWenMixBtn = fuShiBtnList[i].GetComponent<FuWenMixBtn> ();
			fuWenMixBtn.GetFuWenItemId (curHeChengItemId,reset);
		}

		isBtnClick = false;
	}

	/// <summary>
	/// 关闭合成页面特效
	/// </summary>
	public void FxController (FuWenMixBtn.FxType fxType)
	{
		for (int i = 0;i < fuShiBtnList.Count;i ++)
		{
			FuWenMixBtn fuWenMixBtn = fuShiBtnList[i].GetComponent<FuWenMixBtn> ();
			switch (fxType)
			{
			case FuWenMixBtn.FxType.OPEN:
				fuWenMixBtn.OpenFx ();
				break;
			case FuWenMixBtn.FxType.CLEAR:
				fuWenMixBtn.ClearFx ();
				UI3DEffectTool.Instance ().ClearUIFx (heFuWenSprite.gameObject);
				break;
			default:
				break;
			}
		}
	}
	
	#endregion

	#region
	/// <summary>
	/// 打开符石选择窗口 
	/// </summary>
	public GameObject s_FuWenWindow;//符文选择窗口
	public enum FuShiType
	{
		MAIN_FUSHI,
		GAOJI_FUSHI,
		OTHER,
	}
	private FuWenSelect.SelectType selectType = FuWenSelect.SelectType.XIANGQIAN;
	//// <summary>
	/// 选择符石窗口
	/// </summary>
	/// <param name="tempType">选择类型（镶嵌，合成）</param>
	/// <param name="lanWeiId">符石栏位id</param>
	/// <param name="tempFuShiType">符石属性（主属性，高级属性）</param>
	/// <param name="tempExceptList">已镶嵌的符石属性list</param>
	public void SelectFuWen (FuWenSelect.SelectType tempType,int lanWeiId,FuShiType tempFuShiType,List<int> tempExceptList)
	{
//		Debug.Log ("TempType:" + tempType);
		selectType = tempType;
//		GameObject fuWenWindow = (GameObject)Instantiate (s_FuWenWindow);
//		fuWenWindow.SetActive (true);
//		fuWenWindow.transform.parent = s_FuWenWindow.transform.parent;
//		fuWenWindow.transform.localPosition = Vector3.zero;
//		fuWenWindow.transform.localScale = Vector3.one;
//		Debug.Log ("tempExceptList:" + tempExceptList);
		s_FuWenWindow.SetActive (true);
		FuWenSelect fuWenSelect = s_FuWenWindow.GetComponent<FuWenSelect> ();
		switch (tempType)
		{
		case FuWenSelect.SelectType.XIANGQIAN:
		{
			if (tempFuShiType == FuShiType.MAIN_FUSHI)
			{
				//去掉已镶嵌的主属性符石
				List<Fuwen> selectList = new List<Fuwen>();
				for (int i = 0;i < mainFuWenList.Count;i ++)
				{
					FuWenTemplate fuwenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (mainFuWenList[i].itemId);
					if (!tempExceptList.Contains (fuwenTemp.shuxing))
					{
//						Debug.Log ("m_Contain:" + fuwenTemp.shuxing);
						selectList.Add (mainFuWenList[i]);
					}
				}

				fuWenSelect.GetSelectFuWenInfo (tempType,selectList);
			}
			else if (tempFuShiType == FuShiType.GAOJI_FUSHI)
			{
				//去掉已镶嵌的高级属性符石
				List<Fuwen> selectList = new List<Fuwen>();
				for (int i = 0;i < gaoJiFuWenList.Count;i ++)
				{
					FuWenTemplate fuwenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (gaoJiFuWenList[i].itemId);
					if (!tempExceptList.Contains (fuwenTemp.shuxing))
					{
						selectList.Add (gaoJiFuWenList[i]);
					}
				}

				fuWenSelect.GetSelectFuWenInfo (tempType,selectList);
			}
			fuWenSelect.RefreshSelectFuShiItem (curXiangQianId);
			fuWenSelect.GetXiangQianLanWeiId = lanWeiId;
			
			break;
		}
		case FuWenSelect.SelectType.HECHENG:
		{
			fuWenSelect.GetSelectFuWenInfo (tempType,mixFuWenList);
			fuWenSelect.RefreshSelectFuShiItem (curHeChengItemId);
			
			break;
		}
		default:
			break;
		}
	}
	#endregion

	#region
	/// <summary>
	/// 符石操作页
	/// </summary>
	public GameObject o_FuWenWindow;//符石操作窗口
	private FuShiOperate.OperateType operateType = FuShiOperate.OperateType.XIANGQIAN;//符石操作类型
	public void OperateFuWen (FuShiOperate.OperateType tempType,int tempItemId,int tempLanWeiId)
	{
		Debug.Log ("TempType:" + tempType);
		operateType = tempType;
//		GameObject operateWindow = (GameObject)Instantiate (o_FuWenWindow);
//		operateWindow.SetActive (true);
//		operateWindow.transform.parent = s_FuWenWindow.transform.parent;
//		operateWindow.transform.localPosition = Vector3.zero;
//		operateWindow.transform.localScale = Vector3.one;
		o_FuWenWindow.SetActive (true);
		FuShiOperate fuShiOperate = o_FuWenWindow.GetComponent<FuShiOperate> ();
		fuShiOperate.GetOperateInfo (tempType,tempItemId,tempLanWeiId);
	}
	#endregion

	//符石合成按钮
	void FuShiMixBtn (GameObject obj)
	{
		//发送普通合成符石请求
		if (!isBtnClick)
		{
			EffectPanel (true);
			isBtnClick = true;
			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.GENERAL_HECHENG,curHeChengItemId,0);
		}
	}

	void FuShiYiJianMixBtn (GameObject obj)
	{
		//发送一键合成符石请求
		if (!isBtnClick) 
		{
			EffectPanel (true);
			isBtnClick = true;
			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.YIJIAN_HECHENG, curHeChengItemId, 0);
		}
	}

	//镶嵌页按钮
	public void XiangQianBtn (GameObject obj)
	{
		if (!isBtnClick)
		{
			isBtnClick = true;

			btnsHandlerList [0].GetComponent<UISprite> ().color = Color.white;
			btnsHandlerList [1].GetComponent<UISprite> ().color = Color.gray;
			xiangQianPage.SetActive (true);
			heChengPage.SetActive (false);
			
			pageType = ShowPageType.PAGE_XIANGQIAN;

			bagSbValue = bagSb.value;
			pageSb.value = pageSbValue;
		}

		isBtnClick = false;
	}
	//合成页按钮
	public void HeChengBtn (GameObject obj)
	{
		if (!isBtnClick)
		{
			isBtnClick = true;

			btnsHandlerList [0].GetComponent<UISprite> ().color = Color.gray;
			btnsHandlerList [1].GetComponent<UISprite> ().color = Color.white;
			xiangQianPage.SetActive (false);
			heChengPage.SetActive (true);

			pageType = ShowPageType.PAGE_HECHENG;

			pageSbValue = pageSb.value;
			bagSb.value = bagSbValue;
		}

		isBtnClick = false;
	}

	//合成成功提示
	public void SuccessTips (FuWenData.FuWenOperateType tempType)
	{
//		tipsLabel.text = "";
		string tipStr = "";
		if (tempType == FuWenData.FuWenOperateType.EQUIP_FUWEN)
		{
			tipStr = "符石镶嵌成功！";
		}
		else if (tempType == FuWenData.FuWenOperateType.GENERAL_HECHENG || tempType == FuWenData.FuWenOperateType.YIJIAN_HECHENG)
		{
			tipStr = "符石合成成功！";
		}

		MainCityUI.m_MainCityUI.m_MainCityUIRB.m_FuncNotOpenInfoLabel.text = tipStr;
		PopUpLabelTool.Instance().AddPopLabelWatcher(MainCityUI.m_MainCityUI.m_MainCityUIRB.m_FuncNotOpenInfoObject, 
		                                             new Vector3 (135,220,0), 
		                                             Vector2.zero, 
		                                             iTween.EaseType.easeOutBack, 1.0f, iTween.EaseType.linear, 2f);
	}

	public void AddZhanLi (long tempZhanLi)
	{
		if (tempZhanLi == zhanLi)
		{
			return;
		}

		long addZhanLi = tempZhanLi - zhanLi;
		string s = "";

		tipsLabel.GetComponent<LabelColorTool> ().m_ColorID = addZhanLi > 0 ? 4 : 5;

		s = addZhanLi > 0 ? "+" : "";

		tipsLabel.text = s + addZhanLi.ToString () + "战力";

		PopUpLabelTool.Instance().AddPopLabelWatcher(tipsLabel.gameObject, 
		                                             new Vector3 (440,245,0), 
		                                             new Vector2(0,25),
		                                             iTween.EaseType.easeOutBack, -1.0f, iTween.EaseType.linear, 1.5f);

		endZhanLi = tempZhanLi;
		refreshZhanLi = true;
	}

	void Update ()
	{
		if (refreshZhanLi)
		{
			NumCount (zhanLiLabel,endZhanLi);
		}
	}

	void NumCount (UILabel tempLabel,long tempZhanLi)
	{
		if (zhanLi == tempZhanLi)
		{
			if(tempLabel.gameObject.transform.localScale.x != 1)
			{
				Vector3 tempScal = tempLabel.gameObject.transform.localScale;
				
				tempScal.x -= 0.02f;
				tempScal.y -= 0.02f;
				tempScal.z -= 0.02f;
				
				if(tempScal.x < 1)
				{
					tempScal.x = 1f;
					tempScal.y = 1f;
					tempScal.z = 1f;
				}
				
				tempLabel.gameObject.transform.localScale = tempScal;
			}
		}
		else
		{
			if (tempLabel.gameObject.transform.localScale.x < 1.1f)
			{
				Vector3 tempScal = tempLabel.gameObject.transform.localScale;
				
				tempScal.x += 0.1f;
				tempScal.y += 0.1f;
				tempScal.z += 0.1f;
				
				if(tempScal.x > 1.1)
				{
					tempScal.x = 1.1f;
					tempScal.y = 1.1f;
					tempScal.z = 1.1f;
				}
				
				tempLabel.gameObject.transform.localScale = tempScal;
			}
			else
			{
				float tempAddNum = (zhanLi - tempZhanLi) / 10;
				
				if (Mathf.Abs(tempAddNum) < 1)
				{
					zhanLi = tempZhanLi;
				}
				
				else
				{
					zhanLi = (long)(zhanLi - tempAddNum);
				}
				
				tempLabel.text = zhanLi.ToString ();
			}
		}
	}

	//显示合成后的符石icon
	public void ShowHeChengFuShi (int itemId,bool isShowFx)
	{
		heFuWenSprite.gameObject.SetActive (true);
		FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (itemId);
		heFuWenSprite.spriteName = fuWenTemp.fuwenNext.ToString ();

		if (!isShowFx)
		{
			return;
		}

		int shuXingId = fuWenTemp.shuxing;//1-攻击(红色) 2-防御(橙色) 3-生命(绿色) 其它(蓝色)
		int effectId = 0;
		if (shuXingId == 1)
		{
			effectId = 100164;
		}
		else if (shuXingId == 2)
		{
			effectId = 100161;
		}
		else if (shuXingId == 3)
		{
			effectId = 100162;
		}
		else
		{
			effectId = 100163;
		}
		
		UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,heFuWenSprite.gameObject,EffectIdTemplate.GetPathByeffectId(effectId));
		StartCoroutine (WaitForClick (1));
	}

	private int showEffectCount;//显示特效个数
	public void ShowEffectCount (int num)
	{
		showEffectCount += num;
		if (showEffectCount % 4 == 0)
		{
			StartCoroutine (WaitForClick (1));
		}
	}

	IEnumerator WaitForClick (float time)
	{
		yield return new WaitForSeconds (time);
		EffectPanel (false);
	}

	public UISprite effectStop;//合成特效显示期间阻挡点击任何地方
	public void EffectPanel (bool isActive)
	{
		effectStop.gameObject.SetActive (isActive);
		effectStop.alpha = 0.05f;
	}

	//返回按钮
	public void BackBtn ()
	{
		if (!isBtnClick)
		{
			UIJunZhu.m_UIJunzhu.closeFuwen ();
			OnCloseWindow ();
		}
	}

	//关闭符文
	public void CloseRoot ()
	{
		if (!isBtnClick)
		{
			FxController (FuWenMixBtn.FxType.CLEAR);
			UIJunZhu.m_UIJunzhu.MYClick (gameObject);
			m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindow;
			m_ScaleEffectController.OnCloseWindowClick();
		}
	}

	void OnCloseWindow ()
	{
		Global.m_isOpenFuWen = false;
		gameObject.SetActive (false);
//		Destroy(gameObject);
	}

	#region
	//界面多按钮点击屏蔽
	private bool isBtnClick = false;
	public bool IsBtnClick
	{
		set{isBtnClick = value;}
		get{return isBtnClick;}
	}
	#endregion
}
