using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class LianmengMuBiaomanager : MYNGUIPanel,SocketProcessor {

	public UILabel allianceExp;// 联盟经验进度
	
	public UILabel alliance_Lv ;// 联盟等级到达
	
	public UISlider mSlider;
	
	public UILabel Builds;

	public AllianceHaveResp Lianmeng_Alliance;

	public GameObject mItem;

	public UILabel LingQuLabel;

	public GameObject LingQuAlert;

	public UISprite LingQuBox;

	public NGUILongPress EnergyDetailLongPress1;

	public UIButton mLingQuBtn;

	private bool ISLingQU = false;

	public AllianceTargetResp mAllianceTargetResp;
	public GetAllianceLevelAwardResp mpGetAllianceLevelAwardResp;
	void Awake()
	{ 
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//
	{
		//Debug.Log ("Pess le ");
		//ShowTip.showTip (0);
	}
	void Start () {
	
	}

	void Update () {
	
	}
	public void Init()
	{
		SendMasege ();

		InitItems ();
	}
	private void SendMasege()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_TARGET_INFO_Resp);
		mLingQuBtn.enabled = false;
	}
	public   bool OnProcessSocketMessage(QXBuffer p_message){ //领取奖励
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_ALLIANCE_TARGET_INFO_Resp: 
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();	
				
				AllianceTargetResp tempInfo = new AllianceTargetResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				mAllianceTargetResp = tempInfo;

			//	Debug.Log ("请求联盟目标信息返回 ");

				InitInfo ();

				return true;
			}
			case ProtoIndexes.S_GET_ALLIANCEL_LEVEL_AWARD_RESP: 
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();	
				
				GetAllianceLevelAwardResp tempInfo = new GetAllianceLevelAwardResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				mpGetAllianceLevelAwardResp = tempInfo;
				
				//Debug.Log ("l领取奖励返回 ");
				if(mpGetAllianceLevelAwardResp.result == 0)
				{
					GetAwardBack();
				}
				else
				{
					//Debug.Log ("等级未达到 ");
				}
				
				return true;
			}
			default: return false;
			}
		}
		
		return false;
	}
	void GetAwardBack()
	{
		LianMengTemplate mLM = LianMengTemplate.GetLianMengTemplate_by_lv (mAllianceTargetResp.level);
		
		string m_award = mLM.targetAward;
		
		string [] s = m_award.Split (':');
		int m_id = int.Parse(s[1]);
		int Awardsnum = int.Parse(s[2]);
		RewardData data = new RewardData ( m_id, Awardsnum); 
		
		GeneralRewardManager.Instance ().CreateReward (data); 
		mAllianceTargetResp.level = mpGetAllianceLevelAwardResp.nextLevel;
		MyAllianceInfo.m_MyAllianceInfo.m_Alliance.lmTargetLevel = mAllianceTargetResp.level;
		MyAllianceInfo.m_MyAllianceInfo.InitUI ();
		NewAlliancemanager.Instance ().m_allianceHaveRes.lmTargetLevel = mAllianceTargetResp.level;
		InitInfo ();
	}
	void InitInfo()
	{
		mLingQuBtn.enabled = true;
		Builds.text = "建设值：" + Lianmeng_Alliance.build.ToString ()+"([10ff2b]升级联盟建筑所需[-])";

		int LmMuBiaoLevel = mAllianceTargetResp.level;
		
		if(LmMuBiaoLevel == -1)
		{
			LmMuBiaoLevel = Lianmeng_Alliance.level;
		}
		int mLMExp = 0;
		int FontmLMExp = 0;
		if(Lianmeng_Alliance.level > 1)
		{
			int AllExp1 =  LianMengTemplate.GetLianMengTemplate_AllExp_by_lv (Lianmeng_Alliance.level-1);
			int AllExp3 =  LianMengTemplate.GetLianMengTemplate_AllExp_by_lv (LmMuBiaoLevel-1);
			mLMExp = AllExp3;
			allianceExp.text = "联盟经验: " + (AllExp1+Lianmeng_Alliance.exp).ToString()+ "/" + (AllExp3).ToString ();
			FontmLMExp = AllExp1+Lianmeng_Alliance.exp;
		}
		else
		{
			int AllExp3 =  LianMengTemplate.GetLianMengTemplate_AllExp_by_lv (LmMuBiaoLevel-1);
			allianceExp.text = "联盟经验: " + (Lianmeng_Alliance.exp).ToString()+ "/" + (AllExp3).ToString ();
			mLMExp = AllExp3; 
			FontmLMExp = Lianmeng_Alliance.exp;
		}


		float mvalue = (float)FontmLMExp/ (float)mLMExp;
		if(mvalue < 0.01f)
		{
			mvalue = 0.01f;
		}
		mSlider.value = mvalue;
		if(mAllianceTargetResp.level != -1){
			
			LingQuBox.spriteName = "xiangzi";
			if(Lianmeng_Alliance.level >= LmMuBiaoLevel )
			{
				ISLingQU = true;
				LingQuLabel.text = "可领取";
				LingQuAlert.SetActive(true);
				EnergyDetailLongPress1.enabled = false;
				alliance_Lv.text = "联盟等级到达Lv "+(mAllianceTargetResp.level).ToString()+"[c40000](已达成)[-]";
			}
			else
			{
				EnergyDetailLongPress1.enabled = false;
				ISLingQU = false;
				LingQuAlert.SetActive(false);
				alliance_Lv.text = "联盟等级到达Lv "+(mAllianceTargetResp.level).ToString();
				LingQuLabel.text = "目标达成后领取";
			}
		} else {
			EnergyDetailLongPress1.enabled = false;
			mLingQuBtn.enabled = false;
			LingQuBox.spriteName = "zz";
			LingQuLabel.text = "已领完";
			LingQuAlert.SetActive(false);
			alliance_Lv.text = "联盟等级到达上限";
		}
	}
	void InitItems()
	{
		int count = LMTargetTemplate.getLMTargetTemplateCOunt ();
		for(int i = 0 ; i < count; i ++)
		{
			GameObject m_mItem = Instantiate(mItem) as GameObject;
			
			m_mItem.SetActive(true);
			
			m_mItem.transform.parent = mItem.transform.parent;
			
			m_mItem.transform.localPosition = new Vector3(0,130-90*i,0);
			
			m_mItem.transform.localScale = mItem.transform.localScale;

			MuBiaoItem mMuBiaoItem = m_mItem.GetComponent<MuBiaoItem>();

			mMuBiaoItem.Id = i+1;
			mMuBiaoItem.Init();
		}
	}
	public void LingQuBtn()
	{
	
		if(ISLingQU)
		{
			mLingQuBtn.enabled = false;
			GetAllianceLevelAward mGetAllianceLevelAward = new GetAllianceLevelAward ();
			
			MemoryStream mapStream = new MemoryStream ();
			
			QiXiongSerializer maper = new QiXiongSerializer ();
			
			mGetAllianceLevelAward.level = mAllianceTargetResp.level;
			
			maper.Serialize (mapStream,mGetAllianceLevelAward);
			
			byte[] t_protof;
			
			t_protof = mapStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(
				ProtoIndexes.C_GET_ALLIANCEL_LEVEL_AWARD , 
				ref t_protof,
				true,
				ProtoIndexes.S_GET_ALLIANCEL_LEVEL_AWARD_RESP  );
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_START_MADE ),LoadShowpathBack);
		}
	}
	GameObject mAwardobg;
	void LoadShowpathBack(ref WWW p_www,string p_path, Object p_object)
	{
		if(mAwardobg)
		{
			return;
		}
		mAwardobg = Instantiate ( p_object )as GameObject;
		
		mAwardobg.SetActive (true);
		
		mAwardobg.transform.parent = this.transform;
		
		mAwardobg.transform.localScale = new Vector3 (0.5f,0.5f,1);
		
		mAwardobg.transform.localPosition = new Vector3 (166,90,0);
		
		LMMuBiaoAward mLMMuBiaoAward = mAwardobg.GetComponent<LMMuBiaoAward> ();
		
		mLMMuBiaoAward.Init (mAllianceTargetResp.level);
	
	}

	public void Close()
	{
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
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
