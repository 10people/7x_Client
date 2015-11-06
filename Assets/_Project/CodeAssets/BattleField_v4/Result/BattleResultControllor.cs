using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class BattleResultControllor : MonoBehaviour, SocketListener
{
	public delegate void CloseCallback ();

	public UISprite spriteFlag;

	public GameObject btnStronger;

	public GameObject btnHelp;

	public GameObject layerTime;

	public GameObject layerGuoguan_1;

	public GameObject layerGuoguan_2;

	public BattleResultPvpWin layerBaizhan_1;

	public GameObject layerBaizhan_2_Win;

	public GameObject layerBaizhan_2_Lose;

	//public GameObject layerHuangye_1;

//	public GameObject layerHuangye_2_win;

//	public GameObject layerHuangye_2_win_1;

//	public GameObject layerHuangye_2_win_2;

//	public GameObject layerHuangye_2_lose;

	public GameObject layerHuangYe_1;
	
	public GameObject layerHuangYe_2;

	public GameObject layerLueduo_win;

	public GameObject layerLueduo_lose;

	public GameObject layer_3;

	public BattleResultGeneral layerGeneral;

	public List<BattleResultStar> stars;

	public BattleResultStar star_win;

	public UISprite spriteWinLevel;

	public UILabel labelExp;

	public UILabel labelCoin;

	//public UILabel labelBuild;

	//public UILabel labelWeiWang;

	public UILabel labelDKP;

	//public UILabel labelHeroLevel;

	//public UILabel labelSoldierLevel;

	public UILabel labelGongJIn;

	public UILabel labelTime;

	public List<GameObject> labelNonItem = new List<GameObject>();

	public BattleResultHint resultHint;

	public GameObject hintHelp;

	public GameObject hintHelpError;

	public UILabel winDesc;

	public UILabel winDescNum;


	[HideInInspector] public int winLevel;

	[HideInInspector] public int iExp;

	[HideInInspector] public int iCoin;

    [HideInInspector] public GameObject itemTemple;

	[HideInInspector] public List<bool> bList_achivment;

	[HideInInspector] public List<AwardItem> lmAwards = new List<AwardItem>();


	private List<GameObject> tips = new List<GameObject>();

	private List<GameObject> tipsInShow = new List<GameObject>();

	private float actionTime = .3f;

	private CloseCallback mCloseCallback;


    private int[,] positionDictX = new int[,] {
		{0, 0, 0},
		{0, 0, 0},
		{-65, 65, 0},
		{-120, 0, 120}
	};

	void OnEnable()
    {
        CityGlobalData.m_isBattleField_V4_2D = true;
    }

    void OnDestroy()
    {
 		CityGlobalData.m_isBattleField_V4_2D = false;

		SocketTool.UnRegisterSocketListener(this);
    }

    public void OnResultQuit()
    {
		UIYindao.m_UIYindao.CloseUI ();
		
		if(mCloseCallback != null)
		{
			mCloseCallback();

			return;
		}

		AudioListener al = BattleControlor.Instance().getKing().gameCamera.target.GetComponent<AudioListener>();
		
		Destroy ( al );
		
		if (CityGlobalData.m_isWhetherOpenLevelUp == false) return;

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_TaskReq,
            //		                                        "29502|10000" );
                                                true,
                                                ProtoIndexes.S_TaskList);
        
		ClientMain.m_ClientMainObj.AddComponent<AudioListener> ();
		
		SocketTool.RegisterSocketListener(this);
    }

	public void OnResultHelp()
	{
		if (JunZhuData.Instance ().m_junzhuInfo.lianMengId > 0)//有联盟
		{
			BattleUIControlor.Instance ().sendHelp ();
		}
		else//无联盟
		{
			btnHelp.SetActive (false);
			
			hintHelpError.SetActive (true);
		}
	}

	public void OnResultHelpCallback()
	{
		btnHelp.SetActive (false);
		
		hintHelp.SetActive (true);
	}

	public void CloseHelpHint()
	{
		hintHelp.SetActive (false);
	}

	public void CloseHelpErrorHint()
	{
		hintHelpError.SetActive (false);
	}

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message == null)
        {
            return false;
        }

        switch (p_message.m_protocol_index)
        {
            case ProtoIndexes.S_TaskList:
                {
                    ExecQuit();

                    return true;
                }

            default:
                {
                    return false;
                }
        }
    }

    private void ExecQuit()
    {
        GameObject root3d = GameObject.Find("BattleField_V4_3D");

        GameObject root2d = GameObject.Find("BattleField_V4_2D");

	 	foreach( BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			node.setNavEnabled(false);
		}

		foreach( BaseAI node in BattleControlor.Instance().selfNodes)
		{
			node.setNavEnabled(false);
		}

		foreach( BaseAI node in BattleControlor.Instance().midNodes)
		{
			node.setNavEnabled(false);
		}

        //Destroy(root3d);

        //Destroy(root2d);

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao)
		{
			CarriageSceneManager.Instance.ReturnCarriage(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN);
		}
        else if (JunZhuData.Instance().m_junzhuInfo.lianMengId <=0)
        {
            SceneManager.EnterMainCity();
        }
        else
        {
            SceneManager.EnterAllianceCity();
        }

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("JunZhu")))
        {
            CityGlobalData.m_JunZhuCreate = true;
        }
    }

    public void OnResultRetry()
    {
        GameObject root3d = GameObject.Find("BattleField_V4_3D");

        GameObject root2d = GameObject.Find("BattleField_V4_2D");

        //Destroy(root3d);

        //Destroy(root2d);

        EnterBattleField.EnterBattlePve(CityGlobalData.m_tempSection, CityGlobalData.m_tempLevel, CityGlobalData.m_levelType);
    }

    public void OnStronger()
    {
        OnResultQuit();
		
		MainCityUILT.IsDoDelegateAfterInit = true;//goto main city tip window.
		
		MainCityUILT.m_MainCityUILTDelegate += MainCityUILT.ShowMainTipWindow;
		
		Global.m_isOpenPVP = false;

		Global.m_isOpenBaiZhan = false;

		if (CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve) Global.m_isOpenHuangYe = false;
    }

	private void closeAll()
	{
		spriteFlag.gameObject.SetActive (false);

		btnStronger.SetActive(false);

		layerTime.SetActive(false);

		layerGuoguan_1.SetActive(false);

		layerGuoguan_2.SetActive(false);

		layerHuangYe_1.SetActive (false);

		layerHuangYe_2.SetActive (false);

		layerBaizhan_1.gameObject.SetActive(false);
		
		layerBaizhan_2_Win.SetActive(false);

		layerBaizhan_2_Lose.SetActive(false);

		//layerHuangye_1.SetActive(false);

		//layerHuangye_2_win.SetActive(false);

		//layerHuangye_2_lose.SetActive (false);

		layerLueduo_win.SetActive (false);

		layerLueduo_lose.SetActive (false);

		layer_3.SetActive(false);

		layerGeneral.gameObject.SetActive (false);

		foreach(BattleResultStar star in stars)
		{
			star.gameObject.SetActive(false);
		}

		star_win.gameObject.SetActive(false);

		winDesc.gameObject.SetActive (false);
	
		winDescNum.gameObject.SetActive (false);
	}

	public void showResultGeneral(BattleControlor.BattleResult t_result, List<Enums.Currency> t_currencies, List<int> t_nums, int t_battleTime_second, int totalTime, CloseCallback closeCallback)
	{
		closeAll ();
		
		ClientMain.m_sound_manager.shopBGSound ();
		
		SoundPlayEff speff = gameObject.AddComponent<SoundPlayEff>();
		
		if(t_result == BattleControlor.BattleResult.RESULT_WIN)
		{
			speff.PlaySound("200000");
		}
		else
		{
			speff.PlaySound("200100");
		}

		mCloseCallback = closeCallback;

		layerGeneral.refreshData (t_currencies, t_nums);

		refreshDate(t_battleTime_second, totalTime);

		StartCoroutine(resultActionGeneral(t_result, t_battleTime_second));
	}

	IEnumerator resultActionGeneral(BattleControlor.BattleResult result, float battleTime)
	{
		Vector3 targetFlagPos = spriteFlag.transform.localPosition;
		
		Vector3 targetFlagScale = new Vector3 (1, 1, 1);

		spriteFlag.transform.localPosition = Vector3.zero;

		if(result == BattleControlor.BattleResult.RESULT_LOSE)
		{
			spriteFlag.spriteName = "result_lose";
			
			spriteWinLevel.spriteName = "result_lose_0";
		}
		else
		{
			spriteWinLevel.spriteName = "result_win_3";
		}
		
		spriteFlag.gameObject.SetActive (true);

		if(result == BattleControlor.BattleResult.RESULT_WIN)
		{
			UI3DEffectTool.Instance().ShowTopLayerEffect( 
                 UI3DEffectTool.UIType.FunctionUI_1, 
                 spriteFlag.gameObject, 
                 EffectIdTemplate.GetPathByeffectId(100102) );
			
			UI3DEffectTool.Instance().ShowTopLayerEffect( 
	             UI3DEffectTool.UIType.FunctionUI_1, 
	             spriteFlag.gameObject, 
	             EffectIdTemplate.GetPathByeffectId(100105) );
		}
		else
		{
			UI3DEffectTool.Instance().ShowMidLayerEffect( 
	             UI3DEffectTool.UIType.FunctionUI_1, 
	             spriteFlag.gameObject, 
	             EffectIdTemplate.GetPathByeffectId(100104) );
		}

		yield return new WaitForSeconds (actionTime * 7);
		
		iTween.MoveTo(spriteFlag.gameObject, iTween.Hash(
			"name", "spriteFlag",
			"position", targetFlagPos,
			"islocal", true,
			"time", actionTime,
			"easeType", iTween.EaseType.linear
			));

		yield return new WaitForSeconds (actionTime);
		
		if(result == BattleControlor.BattleResult.RESULT_WIN)
		{
			UI3DEffectTool.Instance().ShowMidLayerEffect( 
                 UI3DEffectTool.UIType.FunctionUI_1, 
                 spriteFlag.gameObject, 
                 EffectIdTemplate.GetPathByeffectId(100100) );
		}
		
		layerTime.SetActive (battleTime > 1);

		yield return new WaitForSeconds (actionTime);

		layerGeneral.gameObject.SetActive (true);

		yield return new WaitForSeconds (actionTime);
		
		layer_3.SetActive (true);
		
		if(result == BattleControlor.BattleResult.RESULT_LOSE)
		{
			btnStronger.SetActive(true);
		}
		
		showActionDone();
	}

    public void showResult()
    {
		closeAll ();

		ClientMain.m_sound_manager.shopBGSound ();

		SoundPlayEff speff = gameObject.AddComponent<SoundPlayEff>();

		if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN)
		{
			speff.PlaySound("200000");
		}
		else
		{
			speff.PlaySound("200100");
		}

		refreshDate((int)BattleControlor.Instance().battleTime, BattleControlor.Instance().timeLast + (int)BattleControlor.Instance().battleTime);

		StartCoroutine(resultAction());
    }

	IEnumerator resultAction()
	{
		Vector3 targetFlagPos = spriteFlag.transform.localPosition;

		Vector3 targetFlagScale = new Vector3 (1, 1, 1);

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan 
		   && CityGlobalData.m_levelType == LevelType.LEVEL_ELITE)
		{
			targetFlagPos += new Vector3(0, 207.6f - targetFlagPos.y, 0);

			targetFlagScale = new Vector3(.7f, .7f, 1);

			layerTime.transform.localPosition += new Vector3(0, 200, 0);
		}

		spriteFlag.transform.localPosition = Vector3.zero;

		if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_LOSE)
		{
			spriteFlag.spriteName = "result_lose";

			spriteWinLevel.spriteName = "result_lose_0";
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan)
		{
			if(CityGlobalData.m_levelType == LevelType.LEVEL_NORMAL)
			{
				spriteWinLevel.spriteName = "result_win_3";
			}
			else
			{
				spriteWinLevel.spriteName = "result_win_" + (winLevel - 1);
			}
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve
		        || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
		{
			spriteWinLevel.spriteName = "result_over";
		}
		else
		{
			spriteWinLevel.spriteName = "result_win_3";
		}

		spriteFlag.gameObject.SetActive (true);

		if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN)
		{
			UI3DEffectTool.Instance().ShowTopLayerEffect( 
	             UI3DEffectTool.UIType.FunctionUI_1, 
	             spriteFlag.gameObject, 
			     EffectIdTemplate.GetPathByeffectId(100102) );

			UI3DEffectTool.Instance().ShowTopLayerEffect( 
	             UI3DEffectTool.UIType.FunctionUI_1, 
	             spriteFlag.gameObject, 
			     EffectIdTemplate.GetPathByeffectId(100105) );
		}
		else
		{
			UI3DEffectTool.Instance().ShowMidLayerEffect( 
	             UI3DEffectTool.UIType.FunctionUI_1, 
	             spriteFlag.gameObject, 
	             EffectIdTemplate.GetPathByeffectId(100104) );
		}

		yield return new WaitForSeconds (actionTime * 7);

		TweenAlpha.Begin (spriteFlag.gameObject, actionTime, 0);

		yield return new WaitForSeconds (actionTime);

		iTween.MoveTo(spriteFlag.gameObject, iTween.Hash(
			"name", "spriteFlag",
			"position", targetFlagPos,
			"islocal", true,
			"time", 0,
			"easeType", iTween.EaseType.linear
			));

		iTween.ScaleTo(spriteFlag.gameObject, iTween.Hash(
			"name", "spriteFlag",
			"scale", targetFlagScale,
			"time", .05f,
			"easeType", iTween.EaseType.linear
			));

		yield return new WaitForSeconds (.1f);

		TweenAlpha.Begin (spriteFlag.gameObject, actionTime, 1);

		yield return new WaitForSeconds (actionTime);

		iTween.ScaleTo(spriteFlag.gameObject, iTween.Hash(
			"name", "spriteFlag",
			"scale", targetFlagScale,
			"time", 0f,
			"easeType", iTween.EaseType.linear
			));

		if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN)
		{
			UI3DEffectTool.Instance().ShowMidLayerEffect( 
	             UI3DEffectTool.UIType.FunctionUI_1, 
	             spriteFlag.gameObject, 
	             EffectIdTemplate.GetPathByeffectId(100100) );
		}

		layerTime.SetActive (BattleControlor.Instance().battleTime > 1);

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan)
		{
			if(CityGlobalData.m_levelType == LevelType.LEVEL_ELITE)//精英
			{
				for(int i = 0; i < stars.Count; i++)
				{
					BattleResultStar star = stars[i];

					star.refreshData(bList_achivment[i] && BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN,
					                 BattleControlor.Instance().achivement.descs[i]);
				}

				for(int i = 0; i < stars.Count; i++)
				{
					yield return new WaitForSeconds(.3f);

					BattleResultStar star = stars[i];

					star.onShow();
				}

				star_win.refreshData_2(winLevel == 3 && BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN, null);

				yield return new WaitForSeconds(.3f);

				star_win.onShow_2();
			}
			else if(CityGlobalData.m_levelType == LevelType.LEVEL_TALE)//传奇
			{
				star_win.refreshData_2(winLevel == 3 && BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN, null);

				star_win.onShow_2();
			}
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
		{
			winDesc.text = BattleUIControlor.Instance().winDesc.text;

			winDescNum.text = BattleUIControlor.Instance().winDescNum.text;

			winDesc.gameObject.SetActive(true);

			winDescNum.gameObject.SetActive(true);
		}

		yield return new WaitForSeconds (actionTime);

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
		{
			layerGuoguan_1.SetActive (true);

			labelCoin.text = "0";
			
			labelExp.text = "0";
			
			for (float i = 0; i <= 1; i += .04f)
			{
				yield return new WaitForSeconds(actionTime / 6);
				
				int coin = (int)(iCoin * i);
				
				int exp = (int)(iExp * i);
				
				labelCoin.text = coin + "";
				
				labelExp.text = exp + "";
			}
			
			labelCoin.text = iCoin + "";
			
			labelExp.text = iExp + "";
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)
		{
			layerHuangYe_1.SetActive(true);

			labelDKP.text = "0";

			for (float i = 0; i <= 1; i += .04f)
			{
				yield return new WaitForSeconds(actionTime / 6);
				
				int coin = (int)(iCoin * i);
				
				labelDKP.text = coin + "";
			}

			labelDKP.text = iCoin + "";
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan)
		{
			layerBaizhan_1.gameObject.SetActive(true);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pvp)
		{
			//layerHuangye_1.gameObject.SetActive(true);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_LueDuo)
		{
			btnHelp.gameObject.SetActive(false);
			
			if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN)
			{
				layerLueduo_win.SetActive(true);
			}
			else
			{
				layerLueduo_lose.SetActive(true);			
			}
		}

		yield return new WaitForSeconds (actionTime);

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia
		   )
		{
			layerGuoguan_2.SetActive (true);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)
		{
			layerHuangYe_2.SetActive(true);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan)
		{
			if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN)
			{
				layerBaizhan_2_Win.gameObject.SetActive(true);

				layerBaizhan_2_Lose.gameObject.SetActive(false);
			}
			else
			{
				layerBaizhan_2_Win.gameObject.SetActive(false);

				layerBaizhan_2_Lose.gameObject.SetActive(true);
			}
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pvp)
		{
			//layerHuangye_2_win.gameObject.SetActive(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN);

			//layerHuangye_2_lose.gameObject.SetActive(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_LOSE);
		}

		yield return new WaitForSeconds (actionTime);

		if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_WIN && CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan && CityGlobalData.m_tempSection == 1 && CityGlobalData.m_tempLevel == 1)
		{
			UIYindao.m_UIYindao.setOpenYindao(20108);
		}

		if(lmAwards.Count != 0)
		{
			resultHint.refreshdata(lmAwards, this);
		}
		else
		{
			closeHint();
		}
	}

	public void closeHint()
	{
		layer_3.SetActive (true);
		
		if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_LOSE)
		{
			if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan && CityGlobalData.m_tempSection == 1 && CityGlobalData.m_tempLevel == 1)
			{
				
			}
			else
			{
				btnStronger.SetActive(true);
			}

			if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_LueDuo)
			{
				//btnHelp.gameObject.SetActive(JunZhuData.Instance().m_junzhuInfo.lianMengId > 0);

				btnHelp.gameObject.SetActive(true);
			}
		}
		
		showActionDone();
	}

	public void speedUp()
	{
		actionTime = 0;
	}

    private void showActionDone()
    {
        CityGlobalData.m_showLevelupEnable = true;

		if(CityGlobalData.autoFightDebug == true)
		{
			BattleUIControlor.Instance().exitBattleWithAutoFightDebug();
		}
    }

    public int getStarCount()
    {
        float blood = 0;

        foreach (BaseAI node in BattleControlor.Instance().selfNodes)
        {
			blood += node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp );
        }

        float rate = blood / BattleControlor.Instance().totalBloodSelf;

        if (rate > .6f) return 3;

        else if (rate > .4f) return 2;

        return 1;
    }

    private void SetGameObjectAlpha(GameObject HideObj, float duration, float alpha)
    {
        for (int i = 0; i < HideObj.transform.childCount; i++)
        {
            GameObject gc = HideObj.transform.GetChild(i).gameObject;

            if (gc.activeSelf == false) continue;

            TweenAlpha.Begin(gc, duration, alpha);

            SetGameObjectAlpha(gc, duration, alpha);
        }
    }

    private void refreshDate(int battleTime, int totalTime = 0)
    {
		tips.Clear ();

		tipsInShow.Clear ();

		if(battleTime >= totalTime)
		{
			labelTime.text = LanguageTemplate.GetText((LanguageTemplate.Text)1233);
		
			labelTime.color = Color.red;
		}
		else
		{
			int miao = battleTime % 60;

			int fen = battleTime / 60;

		    string strMiao = miao < 10 ? ("0" + miao) : ("" + miao);

		    labelTime.text = fen + ":" + strMiao;
		}
		//layerTime.SetActive (BattleControlor.Instance().battleTime > 1);
    }

    public void addAward(AwardItem awardItem, int index, int count)
    {
		//Debug.Log ("AAAAAAAAAAAAAA    " + awardItem.awardId + ", " + count);

		foreach(GameObject label in labelNonItem)
		{
			label.SetActive (false);
		}

        int c = count > 3 ? 3 : count;

        float y = -72f;

        if (count > 3)
        {
			if (index < 3) y = -16f;

			if (index >= 3) y = -130f;
        }

        GameObject gc = (GameObject)Instantiate(itemTemple.gameObject);

        gc.SetActive(true);

        if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)
		{
			gc.transform.parent = layerHuangYe_2.transform;
		}
		else
		{
			gc.transform.parent = layerGuoguan_2.transform;
		}

        gc.transform.localPosition = new Vector3(positionDictX[c, index % 3], y, 0);

        gc.transform.eulerAngles = itemTemple.transform.eulerAngles;

        gc.transform.localScale = itemTemple.transform.localScale;

        IconSampleManager ism = gc.GetComponent<IconSampleManager>();

        ism.SetIconByID(awardItem.awardIconId, awardItem.awardNum + "", 500);

		GameObject gcTip = ism.SetIconPopText (awardItem.awardId);

		tips.Add (gcTip);
        //ism.SetIconType (IconSampleManager.IconType.item);

        /*
        0普通道具; 2装备; 3当铺材料; 4秘宝； 5秘宝碎片； 6进阶材料； 7武将; 8精魄; 9强化材料

        0普通道具;		读ItemTemp表
        2装备;			读ZhuangBei表
        3当铺材料;		读ItemTemp表
        4秘宝;			读MiBao表
        5秘宝碎片;		读MibaoSuiPian表
        6装备进阶材料;	读ItemTemp表
        7武将;			读HeroGrow表
        8精魄;			读JingPo表
        9强化材料		读ItemTemp表
        */

    }

    public void SetExpMoney(int exp, int money)
    {
        iExp = exp;

        iCoin = money;
    }

	public void setPVPData(BaiZhanResultResp _resp)
	{
		layerBaizhan_1.refreshData (_resp);
	}

	public void setHYPvpData(BattleResultHYPvp _resp)
	{
		//labelDKP.text = _resp.dkp + "";

		//labelHeroLevel.text = _resp.heroLevel + "";

		//labelSoldierLevel.text = _resp.soldierLevel + "";

		//layerHuangye_2_win_1.gameObject.SetActive (_resp.isLevelup == 1);

		//layerHuangye_2_win_2.gameObject.SetActive (_resp.isLevelup != 1);
	}

	public void setLueDuoData(LveBattleResult _resp)
	{
		labelGongJIn.text = _resp.lostGongJin + "";
	}

	void FixedUpdate ()
	{
		foreach(GameObject gc in tips)
		{
			if(gc.activeSelf == true)
			{
				bool flag = CheackRule(gc);

				if(flag == true)
				{
					tipsInShow.Add(gc);

					UI3DEffectTool.Instance().ShowMidLayerOverLayNGUI(
						UI3DEffectTool.UIType.FunctionUI_1, 
						spriteFlag.gameObject, gc);
				}
			}
			else
			{
				bool flag = CheackRule(gc);

				if(flag == false)
				{
					tipsInShow.Remove(gc);
				}
			}
		}
	}

	private bool CheackRule(GameObject _gc)
	{
		foreach(GameObject gc in tipsInShow)
		{
			if(gc.Equals(_gc))
			{
				return false;
			}
		}

		return true;
	}

}
