using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

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

	public GameObject anchorTopRight;

	private bool isOpenFuWen = false;

	void Awake ()
	{
		fuWenMainPage = this;
	}

	void OnDestroy ()
	{
		fuWenMainPage = null;
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (anchorTopRight);
	}

	/// <summary>
	/// 初始化符文首页
	/// </summary>
	public void InItFuWenPage (QueryFuwenResp tempResp)
	{
		m_ScaleEffectController.gameObject.transform.localScale = Vector3.one;

		fuWenResp = tempResp;

		if (!isOpenFuWen)
		{
			zhanLi = tempResp.zhanli;
			zhanLiLabel.text = tempResp.zhanli.ToString ();

			pageType = ShowPageType.PAGE_XIANGQIAN;

			isOpenFuWen = true;
		}
		else
		{
			AddZhanLi (tempResp.zhanli);
		}

		FuWenListSort (tempResp);

//		Debug.Log ("QXComData.CheckYinDaoOpenState (100470):" + QXComData.CheckYinDaoOpenState (100470));
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100470,3);

		btnsHandlerList [0].m_click_handler -= XiangQianBtn;
		btnsHandlerList [0].m_click_handler += XiangQianBtn;//镶嵌按钮
		btnsHandlerList [1].m_click_handler -= HeChengBtn;
		btnsHandlerList [1].m_click_handler += HeChengBtn;//合成按钮
	}

	#region Rank FuShi
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

		SwitchFuWenPage ();
	}
	#endregion

	void SwitchFuWenPage ()
	{
		xiangQianPage.SetActive (pageType == ShowPageType.PAGE_XIANGQIAN ? true : false);
		heChengPage.SetActive (pageType == ShowPageType.PAGE_HECHENG ? true : false);

		btnsHandlerList [0].GetComponent<UISprite> ().color = pageType == ShowPageType.PAGE_XIANGQIAN ? Color.white : Color.gray;
		btnsHandlerList [1].GetComponent<UISprite> ().color = pageType == ShowPageType.PAGE_HECHENG ? Color.white : Color.gray;

		switch (pageType)
		{
		case ShowPageType.PAGE_XIANGQIAN:

			InItXiangQianPage (fuWenResp);

			break;
			
		case ShowPageType.PAGE_HECHENG:

			InItHeChengPage (bagFuWenList);

			break;
		default:
			break;
		}
	}

	#region XiangQian
	/// <summary>
	/// 镶嵌页
	/// </summary>
	public UIScrollView pageSc;
	public List<FuWenPageItem> fuWenPageList = new List<FuWenPageItem> ();
	public List<UILabel> attributeLabels = new List<UILabel> ();//属性labels
	private JunzhuAttr totleAttributes;//总属性值
	private JunzhuAttr bonuses;//属性加成值
	
	private int nextOpenLevel;//下几个栏位开启等级
	public int NextOpenLevel  {set{nextOpenLevel = value;} get{return nextOpenLevel;} }

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

		string colorCode = FuWenData.Instance.colorCode;

		attributeLabels [0].text = MyColorData.getColorString (3,totleAttributes.gongji.ToString () + colorCode + "(" + bonuses.gongji.ToString () + ")[-]");//攻击
		attributeLabels [1].text = MyColorData.getColorString (3,totleAttributes.fangyu.ToString () + colorCode + "(" + bonuses.fangyu.ToString () + ")[-]");//防御
		attributeLabels [2].text = MyColorData.getColorString (3,totleAttributes.shengming.ToString () + colorCode + "(" + bonuses.shengming.ToString () + ")[-]");//生命
		attributeLabels [3].text = MyColorData.getColorString (6,totleAttributes.wqSH.ToString () + colorCode + "(" + bonuses.wqSH.ToString () + ")[-]");//武器伤害加深
		attributeLabels [4].text = MyColorData.getColorString (6,totleAttributes.wqBJ.ToString () + colorCode + "(" + bonuses.wqBJ.ToString () + ")[-]");//武器暴击加深
		attributeLabels [5].text = MyColorData.getColorString (7,totleAttributes.jnSH.ToString () + colorCode + "(" + bonuses.jnSH.ToString () + ")[-]");//技能伤害加深
		attributeLabels [6].text = MyColorData.getColorString (7,totleAttributes.jnBJ.ToString () + colorCode + "(" + bonuses.jnBJ.ToString () + ")[-]");//技能暴击加深
		attributeLabels [7].text = MyColorData.getColorString (6,totleAttributes.wqJM.ToString () + colorCode + "(" + bonuses.wqJM.ToString () + ")[-]");//武器伤害抵抗
		attributeLabels [8].text = MyColorData.getColorString (6,totleAttributes.wqRX.ToString () + colorCode + "(" + bonuses.wqRX.ToString () + ")[-]");//武器暴击抵抗
		attributeLabels [9].text = MyColorData.getColorString (7,totleAttributes.jnJM.ToString () + colorCode + "(" + bonuses.jnJM.ToString () + ")[-]");//技能伤害抵抗
		attributeLabels [10].text = MyColorData.getColorString (7,totleAttributes.jnRX.ToString () + colorCode + "(" + bonuses.jnRX.ToString () + ")[-]");//技能暴击抵抗

		//对符文栏位进行开启顺序排序
		List<int> openLevelList = new List<int> ();
		List<int> wOpenLevelList = new List<int> ();//未开启栏位开启等级list

		for (int i = 0;i < tempResp.lanwei.Count;i ++)
		{
			FuWenOpenTemplate fuWenOpenTemp = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (tempResp.lanwei[i].lanweiId);

			openLevelList.Add (fuWenOpenTemp.level);

			if (tempResp.lanwei[i].itemId == -1)
			{
				wOpenLevelList.Add (fuWenOpenTemp.level);
			}
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

		for (int i = 0;i < wOpenLevelList.Count;i ++)
		{
			if (i == 0)
			{
				NextOpenLevel = wOpenLevelList[i];
			}
			else
			{
				NextOpenLevel = NextOpenLevel < wOpenLevelList[i] ? NextOpenLevel : wOpenLevelList[i];
			}
		}
//		Debug.Log ("NextOpenLevel：" + NextOpenLevel);

		for (int i = 0;i < fuWenPageList.Count;i ++)
		{
			fuWenPageList[i].InItFuWenPageInfo (i + 1,tempResp.lanwei);
		}
		Debug.Log ("QXComData.CheckYinDaoOpenState (100470):" + QXComData.CheckYinDaoOpenState (100470));
		pageSc.enabled = !QXComData.CheckYinDaoOpenState (100470);
		btnsHandlerList [2].m_click_handler -= FuShiMixBtn;
		btnsHandlerList [2].m_click_handler += FuShiMixBtn;//普通合成
		btnsHandlerList [3].m_click_handler -= FuShiYiJianMixBtn;
		btnsHandlerList [3].m_click_handler += FuShiYiJianMixBtn;//一键合成
	}
	#endregion

	#region HeCheng
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
	public int CurHeChengItemId { set{curHeChengItemId = value;} get{return curHeChengItemId;} }

	//初始化合成页
	void InItHeChengPage (List<Fuwen> tempList)
	{
		ShowMixBtns ();
	
		isBtnClick = false;

		fuWenBagItemList = QXComData.CreateGameObjectList (fuWenBagItemObj,tempList.Count,fuWenBagItemList);

		desLabel.text = fuWenBagItemList.Count > 0 ? "" : "背包空空的什么也没有";

		for (int i = 0;i < tempList.Count;i ++)
		{
			fuWenBagItemList[i].transform.localPosition = new Vector3(0,-100 * i,0);
			bagSc.UpdateScrollbars (true);

			FuWenBagItem fuWenBag = fuWenBagItemList[i].GetComponent<FuWenBagItem> ();
			fuWenBag.GetFuWenInfo (tempList[i]);
		}

		if (tempList.Count <= 4)
		{
			bagSb.value = 0;
			bagSc.ResetPosition ();
		}

		bagSc.enabled = tempList.Count <= 4 ? false : true;
		bagSb.gameObject.SetActive (tempList.Count <= 4 ? false : true);
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
				UI3DEffectTool.ClearUIFx (heFuWenSprite.gameObject);
				break;
			default:
				break;
			}
		}
	}
	
	#endregion

	#region FuShiSelect
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
	private List<int> exceptList = new List<int>();

	//// <summary>
	/// 选择符石窗口
	/// </summary>
	/// <param name="tempType">选择类型（镶嵌，合成）</param>
	/// <param name="lanWeiId">符石栏位id</param>
	/// <param name="tempFuShiType">符石属性（主属性，高级属性）</param>
	/// <param name="tempExceptList">已镶嵌的符石属性list</param>
	public void SelectFuWen (FuWenSelect.SelectType tempType,FuwenLanwei tempLanWei,FuShiType tempFuShiType,List<int> tempExceptList)
	{
//		Debug.Log ("TempType:" + tempType);
		selectType = tempType;

		s_FuWenWindow.SetActive (true);

		switch (tempType)
		{
		case FuWenSelect.SelectType.XIANGQIAN:
		{
			exceptList.Clear ();;
			foreach (int i in tempExceptList)
			{
				exceptList.Add (i);
//				Debug.Log ("exceptList[i]:" + i);
			}

			if (tempFuShiType == FuShiType.MAIN_FUSHI)
			{
				//筛选当前颜色符石
				int curMaxLevel = tempLanWei.itemId > 0 ? FuWenTemplate.GetFuWenTemplateByFuWenId (tempLanWei.itemId).fuwenLevel : -1;//当前符石等级
				int curInlayColor = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (tempLanWei.lanweiId).inlayColor;//当前符石镶嵌颜色
//				Debug.Log ("curMaxLevel:" + curMaxLevel + "||curInlayColor:" + curInlayColor);
				Fuwen curFuWen = new Fuwen();
//				Debug.Log ("curFuWen.itemId:" + curFuWen.itemId);
				List<Fuwen> selectList = new List<Fuwen>();
				for (int i = 0;i < mainFuWenList.Count;i ++)
				{
					FuWenTemplate fuwenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (mainFuWenList[i].itemId);
//					Debug.Log ("fuwenTemp.inlayColor:" + fuwenTemp.inlayColor);
					if (fuwenTemp.inlayColor == curInlayColor)
					{
						if (fuwenTemp.fuwenLevel > curMaxLevel)
						{
							curMaxLevel = fuwenTemp.fuwenLevel;
							curFuWen = mainFuWenList[i];
						}
						else
						{
							if (mainFuWenList[i].itemId == tempLanWei.itemId)
							{
								selectList.Add (mainFuWenList[i]);
							}
						}
					}
				}

				if (curFuWen.itemId != 0)
				{
					selectList.Add (curFuWen);
				}

				if (tempLanWei.itemId > 0)
				{
					bool isContain = false;
					foreach (Fuwen fuWen in selectList)
					{
						if (fuWen.itemId == tempLanWei.itemId)
						{
							//							fuWen.cnt += 1;
							isContain = true;
							break;
						}
						else
						{
							isContain = false;
						}
					}
					if (!isContain)
					{
						Fuwen fuWen = new Fuwen();
						fuWen.itemId = tempLanWei.itemId;
						fuWen.cnt = 0;
						fuWen.isLock = 2;
						selectList.Add (fuWen);
					}
				}

				FuWenSelect.fuWenSelect.GetSelectFuWenInfo (tempType,selectList,tempLanWei);
			}
			else if (tempFuShiType == FuShiType.GAOJI_FUSHI)
			{
				//刷选当前颜色符石
				int curMaxLevel = tempLanWei.itemId > 0 ? FuWenTemplate.GetFuWenTemplateByFuWenId (tempLanWei.itemId).fuwenLevel : -1;
				int curInlayColor = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (tempLanWei.lanweiId).inlayColor;
				int curShuXing = tempLanWei.itemId > 0 ? FuWenTemplate.GetFuWenTemplateByFuWenId (tempLanWei.itemId).shuxing : -1;

				Fuwen curFuWen = new Fuwen();

				List<Fuwen> selectList = new List<Fuwen>();
				for (int i = 0;i < gaoJiFuWenList.Count;i ++)
				{
					FuWenTemplate fuwenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (gaoJiFuWenList[i].itemId);

					if (fuwenTemp.inlayColor == curInlayColor)
					{
						if (fuwenTemp.shuxing == curShuXing)
						{
							if (fuwenTemp.fuwenLevel > curMaxLevel)
							{
								curMaxLevel = fuwenTemp.fuwenLevel;
								curFuWen = gaoJiFuWenList[i];
							}
							else
							{
								if (gaoJiFuWenList[i].itemId == tempLanWei.itemId)
								{
									selectList.Add (gaoJiFuWenList[i]);
								}
							}
						}
						else
						{
							if (!exceptList.Contains (fuwenTemp.shuxing))
							{
								exceptList.Add (fuwenTemp.shuxing);
								selectList.Add (gaoJiFuWenList[i]);
							}
							else
							{
								for (int j = 0;j < selectList.Count;j ++)
								{
									if (fuwenTemp.fuwenLevel > FuWenTemplate.GetFuWenTemplateByFuWenId (selectList[j].itemId).fuwenLevel)
									{
										selectList[j] = gaoJiFuWenList[i];
									}
								}
							}
						}
					}
				}

				if (tempLanWei.itemId > 0)
				{
					if (curMaxLevel > FuWenTemplate.GetFuWenTemplateByFuWenId (tempLanWei.itemId).fuwenLevel)
					{
						selectList.Add (curFuWen);
					}

					bool isContain = false;
					foreach (Fuwen fuWen in selectList)
					{
						if (fuWen.itemId == tempLanWei.itemId)
						{
//							fuWen.cnt += 1;
							isContain = true;
							break;
						}
						else
						{
							isContain = false;
						}
					}
					if (!isContain)
					{
						Fuwen fuWen = new Fuwen();
						fuWen.itemId = tempLanWei.itemId;
						fuWen.cnt = 0;
						fuWen.isLock = 2;
						selectList.Add (fuWen);
					}
				}

				FuWenSelect.fuWenSelect.GetSelectFuWenInfo (tempType,selectList,tempLanWei);
			}
			
			break;
		}
		case FuWenSelect.SelectType.HECHENG:
		{
			foreach (Fuwen fuwen in mixFuWenList)
			{
				Debug.Log ("fuwen:" + fuwen.itemId);
			}
			FuWenSelect.fuWenSelect.GetSelectFuWenInfo (tempType,mixFuWenList,tempLanWei);
			break;
		}
		default:
			break;
		}
	}
	#endregion

	#region FuShiOperate
	/// <summary>
	/// 符石操作页
	/// </summary>
	public GameObject o_FuWenWindow;//符石操作窗口
	private FuShiOperate.OperateType operateType = FuShiOperate.OperateType.XIANGQIAN;//符石操作类型
	public void OperateFuWen (FuShiOperate.OperateType tempType,int tempItemId,int tempLanWeiId)
	{
//		Debug.Log ("TempType:" + tempType);
		operateType = tempType;

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
	void XiangQianBtn (GameObject obj)
	{
		if (!isBtnClick)
		{
			isBtnClick = true;

			pageType = ShowPageType.PAGE_XIANGQIAN;

			SwitchFuWenPage ();
		}

		isBtnClick = false;
	}
	//合成页按钮
	void HeChengBtn (GameObject obj)
	{
		if (!isBtnClick)
		{
			isBtnClick = true;

			pageType = ShowPageType.PAGE_HECHENG;

			SwitchFuWenPage ();
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
		
		UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,heFuWenSprite.gameObject,EffectIdTemplate.GetPathByeffectId(effectId));
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
//			UIJunZhu.m_UIJunzhu.MYClick (gameObject);
			UIJunZhu.m_UIJunzhu.closeFuwen ();
			OnCloseWindow ();
//			m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindow;
//			m_ScaleEffectController.OnCloseWindowClick();
		}
	}

	/// <summary>
	/// Ruleses the button.
	/// </summary>
	public void RulesBtn ()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.FUSHI_HELP_DESC));
	}

	void OnCloseWindow ()
	{
		isOpenFuWen = false;
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
