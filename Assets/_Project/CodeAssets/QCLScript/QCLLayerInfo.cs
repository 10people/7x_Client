using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QCLLayerInfo : MYNGUIPanel {

	public GameObject mTestBtn;

    public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	public UISprite TuiJianSkill;

	public UISprite CurrSKill;

	public UISprite AddSKill;

	public GameObject LockSprite;

	public UILabel LayerName;

	public  List<int> First_Awars = new List<int>();
	public List<int> Awards = new List<int>();

	public GameObject AwardRoot;

	public GameObject EnemysRoot;

	[HideInInspector]
	public GameObject IconSamplePrefab;

	[HideInInspector]public int m_Layer;

	[HideInInspector]public int m_Skillid;

	private bool YindaoIsopen;

	public delegate void CheckYindao();

	private CheckYindao mCheckYindao;

	List<ChongLouNpcTemplate> mChongLouNpcTemplateList = new List<ChongLouNpcTemplate>();

	public NGUILongPress EnergyDetailLongPress;

	public UISprite GreeyBtn;

	void Awake()
	{
		// reigster trigger delegate
		{
			UIWindowEventTrigger.SetOnTopAgainDelegate( gameObject, YinDaoManager );
		}
		BtnList.ForEach (item => SetBtnMoth(item));

		EnergyDetailLongPress.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress.OnLongPress = OnEnergyDetailClick1;
		
	}

	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//显示体力恢复提示
	{
		int mibaoid = JuanghunSkillId;
		if(mibaoid<=0)
		{
			return;
		}
		ShowTip.showTip (mibaoid);
	}

	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}

	void Start () {
	
	}
	void YinDaoManager()
	{
		YindaoIsopen = false;
		if(FreshGuide.Instance().IsActive(200020)&& TaskData.Instance.m_TaskInfoDic[200020].progress >= 0)
		{
			Debug.Log("千重楼战前准备引导");
			YindaoIsopen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[4]);
			StartCoroutine("SetBtnEnble");
		}else
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	IEnumerator SetBtnEnble()
	{
		yield return new WaitForSeconds (0.5f);
		YindaoIsopen = false;
	}
	public void BtnManagerMent(GameObject mbutton)
	{
		if(YindaoIsopen)
		{
			return;
		}
		switch(mbutton.name)
		{
		case "Add":
			ChangerSkillBtn();
			break;
		case "Button_1":
			ChangerSkillBtn();
			break;
		case "Button_2":
			EnterBattleBtn();
			break;
		case "Sprite":
			Close();
			break;
		default:
			break;
		}
	}
	GameObject mChoose_MiBao;
	void ChangerSkillBtn()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeMiBaoSkillLoadBack);
	}
	void ChangeMiBaoSkillLoadBack (ref WWW p_www, string p_path, Object p_object)
	{
		if(mChoose_MiBao)
		{
			return;
		}
		 mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();
		
		mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.QianChongLiu ), m_Skillid ,initData);
		
		MainCityUI.TryAddToObjectList(mChoose_MiBao);
	}
	public UILabel TextCengShu;
	void EnterBattleBtn()
	{
		int x = -1;
		bool success = int.TryParse(TextCengShu.text,out x);
		if(x > 0)
		{
			m_Layer = x;
		}
		Debug.Log ("m_Layer = "+m_Layer);
		if(m_Layer > 70)
		{
			ClientMain.m_UITextManager.createText("此版本只可挑战到70层，敬请期待正式版！");
			return;
		}

		EnterBattleField.EnterBattleChongLou (m_Layer);
	}

	public void Init(int skill_id ,int layer,CheckYindao m_CheckYindao = null)
	{
		if(m_CheckYindao != null)
		{
			mCheckYindao = m_CheckYindao;
		}
		YinDaoManager ();
		m_Layer = layer;
		m_Skillid = skill_id;

		initData (m_Skillid);
		InitEnemys ();
		InitAwards ();

		GreeyBtn.gameObject.SetActive(m_Layer > 70);

		mTestBtn.SetActive (ConfigTool.GetBool(ConfigTool.CONST_QUICK_CHOOSE_LEVEL));
	}
	private int JuanghunSkillId;
	public void initData(int m_mSkillid,bool ISChange = false)
	{
		JuanghunSkillId = m_mSkillid;

		if(m_mSkillid > 0)
		{
			MiBaoSkillTemp mMiBaoSkill  = MiBaoSkillTemp.getMiBaoSkillTempBy_id(m_mSkillid);
			CurrSKill.spriteName = mMiBaoSkill.icon.ToString ();
			AddSKill.gameObject.SetActive(false);
		}
		else
		{
			CurrSKill.spriteName = "";
	
			AddSKill.gameObject.SetActive(MiBaoGlobleData.Instance().GetMiBaoskillOpen());
			LockSprite.SetActive(!MiBaoGlobleData.Instance().GetMiBaoskillOpen());
		}
		if(m_Layer < 1)
		{
			m_Layer = 1;
		}
		ChonglouPveTemplate mChonglouPve = ChonglouPveTemplate.Get_QCL_PVETemplate_By_Layer (m_Layer);
		TuiJianSkill.spriteName = mChonglouPve.recMibaoSkill;
		TuiJianSkill.gameObject.GetComponent<MiBaoSkillTips> ().Skillid = int.Parse (mChonglouPve.recMibaoSkill);
		LayerName.text = "第" + m_Layer.ToString () + "层";

		if(ISChange)
		{
			QianChongLouManagerMent.Instance().mMainInfo.zuheSkill = m_mSkillid;
		}
	}

	void InitEnemys()
	{
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), CreateEnemys);
		}
		else
		{
			WWW temp = null;
			CreateEnemys(ref temp, null, IconSamplePrefab);
		}
	}
	private void CreateEnemys(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		ChonglouPveTemplate mChonglouPve = ChonglouPveTemplate.Get_QCL_PVETemplate_By_Layer (m_Layer);

		int mnpcId = mChonglouPve.npcId;

	    mChongLouNpcTemplateList = ChongLouNpcTemplate.GeChongLouNpcTemplates_By_npcid(mnpcId);
	
		for (int i = 0; i < mChongLouNpcTemplateList.Count-1; i ++)
		{
			for(int j = i+1; j < mChongLouNpcTemplateList.Count; )
			{
				if(mChongLouNpcTemplateList[i].modelId == mChongLouNpcTemplateList[j].modelId)
				{
					if(mChongLouNpcTemplateList[i].type < mChongLouNpcTemplateList[j].type)
					{
						mChongLouNpcTemplateList[i] = mChongLouNpcTemplateList[j];
					}
				
					mChongLouNpcTemplateList.RemoveAt(j);
				}
				else{
					j ++;
				}
			}
			
		}
		//Debug.Log("npcid t = " +npcid);
		for (int i = 0; i < mChongLouNpcTemplateList.Count-1; i ++)
		{
			for(int j = i+1 ; j < mChongLouNpcTemplateList.Count; j++)
			{
				if(mChongLouNpcTemplateList[i].type < mChongLouNpcTemplateList[j].type)
				{
					ChongLouNpcTemplate mLegendNpc = mChongLouNpcTemplateList[i];
					mChongLouNpcTemplateList[i] = mChongLouNpcTemplateList[j];
					mChongLouNpcTemplateList[j] = mLegendNpc ;
				}
			}
		}
		int EnemyCount = mChongLouNpcTemplateList.Count;
		if(EnemyCount > 6)
		{
			EnemyCount = 6 ;
		}
		for (int n = 0; n < EnemyCount; n++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			
			iconSampleObject.SetActive (true);
			iconSampleObject.transform.parent = EnemysRoot.transform;
	
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			ChongLouNpcTemplate m_ChongLouNpcTemplate = ChongLouNpcTemplate.Get_QCL_NpcTemplate_By_Layer(mChongLouNpcTemplateList[n].id);
			//Debug.Log("m_NpcTemplate.EnemyName = "+m_ChongLouNpcTemplate.name);
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(m_ChongLouNpcTemplate.name);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + m_ChongLouNpcTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(m_ChongLouNpcTemplate.desc).description;
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";
			
			if(m_ChongLouNpcTemplate.type == 4)
			{
				//				Debug.Log("boss");
				rightButtomSpriteName = "boss";
				
			}
			if(m_ChongLouNpcTemplate.type == 5)
			{
				rightButtomSpriteName = "JunZhu";
			}
			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
			iconSampleManager.SetIconBasic(3, m_ChongLouNpcTemplate.icon.ToString());
			iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			if(m_ChongLouNpcTemplate.type == 4 ||m_ChongLouNpcTemplate.type == 5)
			{
				iconSampleObject.transform.localScale = new Vector3(0.55f,0.55f,1);
				iconSampleManager.ShowBOssName(Enemy_Namestr.Name);
			}
			else
			{
//				iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -23, 0);
				iconSampleObject.transform.localScale = new Vector3(0.50f,0.50f,1);
			}
		}
	}
	void InitAwards()
	{
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		int numPara = First_Awars.Count + Awards.Count;
		List<AwardTemp> mAwardTemp = new List<AwardTemp> ();
		int AwardCount = 0;
		for (int n = 0; n < numPara; n++)
		{
			if(AwardCount > 6)
			{
				break;
			}
			if(n < First_Awars.Count)
			{
				mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(First_Awars[n]);
			}
			else
			{
				mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(Awards[n - First_Awars.Count]);
			}
			
			for (int i = 0; i < mAwardTemp.Count; i++)
			{
				if(mAwardTemp[i].weight != 0)
				{
					AwardCount++;
					GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;

					iconSampleObject.SetActive(true);
					
					iconSampleObject.transform.parent = AwardRoot.transform;
					
					//iconSampleObject.transform.localPosition = new Vector3((n) * m_Dis, 0, 0);
					
					//FirstAwardPos = iconSampleObject.transform.localPosition;
					
					var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
					
					var iconSpriteName = "";
					
					CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardTemp[i].itemId);
					
					iconSpriteName = mItemTemp.icon.ToString();
					
					iconSampleManager.SetIconType(IconSampleManager.IconType.item);
					
					NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[i].itemId);
					
					string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[i].itemId);
					
					var popTitle = mNameIdTemplate.Name;
					
					var popDesc = mdesc;
					
					iconSampleManager.SetIconByID(mItemTemp.id, mAwardTemp[i].itemNum.ToString(), 3);
					iconSampleManager.SetIconPopText(mAwardTemp[i].itemId, popTitle, popDesc, 1);
					iconSampleObject.transform.localScale = new Vector3(0.55f,0.55f,1);
					if(n < First_Awars.Count)
						iconSampleManager.FirstWinSpr.gameObject.SetActive(true);
				}
				
			}
		}
		
	}
	void Close()
	{
		if(mCheckYindao != null)
		{
			mCheckYindao();
			mCheckYindao = null;
		}
		Destroy (this.gameObject);
	}
	#region fulfil my ngui panel
	
	/// <summary>
	/// my click in my ngui panel
	/// </summary>
	/// <param name="ui"></param>
	public override void MYClick(GameObject ui)
	{
		
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
	#endregion
}
