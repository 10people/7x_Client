using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MiBaoDesInfo : MonoBehaviour , SocketProcessor{
	private int MaxPoint ; //最大点数 

	public UILabel MiBaoLevel;

	public UILabel MiBaoZl;

	public UILabel GongJi;

	public UILabel FangYu;

	public UILabel  Life;

	public UISprite MiBaoSuiPianIcon;

	public UITexture MiBao_Icon;

	public UILabel SuipianNum;

	public UILabel miBaoName;

	public UILabel miBaobig_Name;

	public UILabel MiBaoInstrution;

	public UILabel UP_Money;

	public UISprite Star;

	public List<UISprite> Stars = new List<UISprite>();

	public MibaoInfo ShowmMiBaoinfo;

	private float StarDis = 0;

	public UISlider mUISlider;

	public GameObject StarUpbtn;

	public SparkleEffectItem mSprite;

	public GameObject Gre_StarUpbtn;

	public GameObject LevelUp;

	public GameObject Gre_dLevelUp;

	public UISprite MiBaoSuipianIcon;

	private int Cru_MiBao_Zhanli;

	private int m_Life;

	private int m_Gongji;

	private int m_Fangyu;

	public GameObject mLock;

	public GameObject mCollectBtn;

	public GameObject UpArt;

	public GameObject StarAndMaker_UpArt;

	private int mibaolevel;
	public bool IsCloseArt;

	MiBaoXmlTemp mmibaoxml;
	
	private bool Is_LevelUp = false;

	public bool IsOPenEffect = false;
	
	public MibaoStarUpResp m_iBaoActiveInfo;
	
	private bool MibaoUpCallback = true;
	
	private bool StartUsecountNunber = false;
	
	private bool MiBAostarStartUsecountNunber = false;

	public UILabel PointNum;// 点数
	
	public UILabel CountTime; // 到时间

	public GameObject ActiveBtn;

	public UILabel[] AwardforGrad;

	public UILabel[] Proprety;
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
		IsCloseArt = false;

		MiBAostarStartUsecountNunber = false;

		MibaoUpCallback = true;
		
	    StartUsecountNunber = false;
	}
	void Update () {
	
		if(mibaolevel > 0)
		{

			ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mmibaoxml.expId, mibaolevel);
			
			if(mExpXxmlTemp.needExp > JunZhuData.Instance().m_junzhuInfo.jinBi)
			{
				UP_Money.text = MyColorData.getColorString(5, mExpXxmlTemp.needExp.ToString ());
			}
			else
			{
				UP_Money.text = MyColorData.getColorString(3, mExpXxmlTemp.needExp.ToString ());
			}
			MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(ShowmMiBaoinfo.miBaoId);
		
			if(NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint > 0 && JunZhuData.Instance().m_junzhuInfo.jinBi >= mExpXxmlTemp.needExp &&
			   ShowmMiBaoinfo.level < JunZhuData.Instance().m_junzhuInfo.level&&ShowmMiBaoinfo.level < 100 )
			{
				UpArt.SetActive(true);
				IsCloseArt =false ;
			}
			else
			{
				if(!IsCloseArt)
				{
					IsCloseArt = true;
					//Debug.Log("Close Art ----- ");
					CantUpMiBao();
					UpArt.SetActive(false);
				}
			}
		}

		if(MaxPoint > NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint)
		{
			mtime = NewMiBaoManager.Instance().m_MiBaoInfo.remainTime;
			int M = (int)(mtime/60);
			int S = (int)(mtime%60);
			string s = "";
			if( S < 10)
			{
				s = "0"+S.ToString();
			}
			else
			{
				s = S.ToString();
			}
			CountTime.text = "("+M.ToString()+":"+s+")";
		}else{
			CountTime.text = "("+"已满"+")";
		}
		PointNum.text = NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint.ToString ();
	}
	
	void CreateLifeMove(GameObject move, int content)
	{
		GameObject clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
		clone.transform.localPosition = move.transform.localPosition;
		clone.transform.localRotation = move.transform.localRotation;
		clone.transform.localScale = move.transform.localScale;
		clone.GetComponent<UILabel>().text = "";
		
		clone.GetComponent<UILabel>().text = MyColorData.getColorString(4, "+"+content.ToString());
		
		
		clone.AddComponent< TweenPosition>();
		clone.AddComponent<TweenAlpha>();
		clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
		clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
		clone.GetComponent<TweenPosition>().duration = 0.5f;
		clone.GetComponent<TweenAlpha>().from = 1.0f;
		clone.GetComponent<TweenAlpha>().to = 0;
		clone.GetComponent<TweenPosition>().duration = 0.8f;
		StartCoroutine(WatiFor(clone));
	}
	IEnumerator WatiFor(GameObject obj)
	{
		yield return new WaitForSeconds(0.8f);
		Destroy(obj);
	}


	IEnumerator showTime()
	{
		while(mtime > 0)
		{
			yield return new WaitForSeconds (1.0f);
			mtime -= 1;
			NewMiBaoManager.Instance().m_MiBaoInfo.remainTime = mtime;
			int M = (int)(mtime/60);
			
			int S = (int)(mtime%60);
			
			string s = "";
			
			if( S < 10)
			{
				s = "0"+S.ToString();
			}
			else
			{
				s = S.ToString();
			}
			CountTime.text = "("+M.ToString()+":"+s+")";	
		}
		NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint += 1;
	
		if(MaxPoint > NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint)
		{
			//mtime = 10*60;
			mtime = (int )CanshuTemplate.GetValueByKey("ADD_MIBAODIANSHU_INTERVAL_TIME");
			StopCoroutine("showTime");
			StartCoroutine("showTime");
			
		}else{
			CountTime.text = "("+"已满"+")";
		}
	}
	int mtime = 0 ;
	/// <summary>
	/// 升级临时参数
	/// </summary>
	/// 
	public int LinShi_ZhanLi;
	public void InitLevel()
	{
		mibaolevel = 0;
	}
	public void Init()
	{
		//JunzhuZhaoli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
		int viplv = JunZhuData.Instance().m_junzhuInfo.vipLv;
		if(viplv > 7)
		{
			viplv = 7;
		}
		VipTemplate mVip = VipTemplate.GetVipInfoByLevel (viplv);

		MaxPoint = mVip.MiBaoLimit;

		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(ShowmMiBaoinfo.miBaoId);
		if(ShowmMiBaoinfo.level>mibaolevel)
		{
			mibaolevel = ShowmMiBaoinfo.level;
		}

		if(!Is_LevelUp)
		{
			Cru_MiBao_Zhanli = ShowmMiBaoinfo.zhanLi;
			
			MiBaoZl.text = "战力: "+ShowmMiBaoinfo.zhanLi.ToString();

			m_Life = ShowmMiBaoinfo.shengMing;

			m_Gongji = ShowmMiBaoinfo.gongJi;

		    m_Fangyu = ShowmMiBaoinfo.fangYu;
		}
		else
		{
			Is_LevelUp = false;

			if(m_Life < ShowmMiBaoinfo.shengMing)
			{
				int mContant = ShowmMiBaoinfo.shengMing - m_Life;

				m_Life = ShowmMiBaoinfo.shengMing;

				CreateLifeMove(Life.gameObject, mContant);
			}
			if(m_Gongji < ShowmMiBaoinfo.gongJi)
			{
				int mContant = ShowmMiBaoinfo.gongJi - m_Gongji;
				
				CreateLifeMove(GongJi.gameObject, mContant);

				m_Gongji = ShowmMiBaoinfo.gongJi;
			}
			if(m_Fangyu < ShowmMiBaoinfo.fangYu)
			{
				int mContant = ShowmMiBaoinfo.fangYu - m_Fangyu;
				
				CreateLifeMove(FangYu.gameObject, mContant);

				m_Fangyu = ShowmMiBaoinfo.fangYu;
			}
			if(LinShi_ZhanLi < ShowmMiBaoinfo.zhanLi)
			{
				LinShi_ZhanLi = ShowmMiBaoinfo.zhanLi;
			}

			StopCoroutine("Update_ZhanliAndLevel");
			
			StartCoroutine("Update_ZhanliAndLevel");
		}

		ShowSuipian ();

		GongJi.text = MyColorData.getColorString(10, m_Gongji.ToString ());

		FangYu.text = MyColorData.getColorString(10, m_Fangyu.ToString ());

		Life.text = MyColorData.getColorString(10, m_Life.ToString ());

		ShowStar ();
	
		ShowGetAward ();
		ShowMiBaoYInDao (tems);
		ShowMiBaoYInDao2 (tems);
	}

	public void ShowSuipian()
	{
		MiBaoSuipianXMltemp m_Mibaosuipian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (ShowmMiBaoinfo.tempId);
		if(ShowmMiBaoinfo.level < 1)
		{
			ActiveBtn.SetActive(true);
			mLock.SetActive(true);
			
			LevelUp.SetActive(false);
			
			Gre_dLevelUp.SetActive(true);
			
			Gre_dLevelUp.GetComponent<BoxCollider>().enabled = false;
			
			StarUpbtn.SetActive(false);
			//mSprite.enabled = false;
			SuipianNum.text = ShowmMiBaoinfo.suiPianNum.ToString()+"/"+m_Mibaosuipian.hechengNum.ToString();
			
			mUISlider.value = (float)( ShowmMiBaoinfo.suiPianNum )/ (float)(m_Mibaosuipian.hechengNum);
			
			if(ShowmMiBaoinfo.suiPianNum >= m_Mibaosuipian.hechengNum)
			{
				StarAndMaker_UpArt.SetActive(true);
			}
			else
			{
				StarAndMaker_UpArt.SetActive(false);
			}
			if(mUISlider.value > 1f)mUISlider.value = 1f;
		}
		else
		{
			ActiveBtn.SetActive(false);
			mLock.SetActive(false);
			if(ShowmMiBaoinfo.star >= 5 )
			{
				SuipianNum.text = ShowmMiBaoinfo.suiPianNum.ToString()+"/"+ShowmMiBaoinfo.needSuipianNum.ToString();
				Gre_StarUpbtn.SetActive(true);
				StarUpbtn.SetActive(false);
				mUISlider.value = 1.0f;
			}
			else
			{
				SuipianNum.text = ShowmMiBaoinfo.suiPianNum.ToString()+"/"+ShowmMiBaoinfo.needSuipianNum.ToString();
				
				mUISlider.value = (float)( ShowmMiBaoinfo.suiPianNum )/ (float)(ShowmMiBaoinfo.needSuipianNum);
			}
			if(mUISlider.value > 1f)mUISlider.value = 1f;
			
			if(ShowmMiBaoinfo.suiPianNum >= ShowmMiBaoinfo.needSuipianNum && ShowmMiBaoinfo.star < 5)
			{
				StarUpbtn.SetActive(true);
				mSprite.enabled = true;
				//ShowEffect();   暂时先关闭特效
				MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (ShowmMiBaoinfo.star);
				if(mMiBaoStarTemp.needMoney <= JunZhuData.Instance().m_junzhuInfo.jinBi)
				{
					StarAndMaker_UpArt.SetActive(true);
				}
			}
			else
			{
				if(ShowmMiBaoinfo.star >= 5 )
				{
					StarUpbtn.SetActive(false);
				}
				else
				{
					StarUpbtn.SetActive(true);
				}

				mSprite.enabled = false;
				StarAndMaker_UpArt.SetActive(false);	
			}

			if(mibaolevel >= JunZhuData.Instance().m_junzhuInfo.level)
			{
				LevelUp.SetActive(false);
				UpArt.SetActive(false);
				Gre_dLevelUp.SetActive(true);
			}
			else
			{
				LevelUp.SetActive(true);
				
				Gre_dLevelUp.SetActive(false);
			}
		}
		mmibaoxml = MiBaoXmlTemp.getMiBaoXmlTempById (ShowmMiBaoinfo.miBaoId);
		
		MiBaoSuipianIcon.spriteName = m_Mibaosuipian.icon.ToString ();
		
		DescIdTemplate mDescIdTemplate = DescIdTemplate.getDescIdTemplateByNameId (mmibaoxml.descId);
		
		MiBaoInstrution.text = mDescIdTemplate.description;
		
		MiBao_Icon.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON) + mmibaoxml.icon.ToString ());
		
		miBaoName.text = NameIdTemplate.GetName_By_NameId (mmibaoxml.nameId);
		
		miBaobig_Name.text = miBaoName.text;
		
		MiBaoLevel.text = mibaolevel.ToString ();

	}

	/// <summary>
	/// IShow yindao
	/// </summary>
	/// 
	void ShowMiBaoYInDao(int times)
	{
		if(FreshGuide.Instance().IsActive(100170)&& TaskData.Instance.m_TaskInfoDic[100170].progress >= 0)
		{
			Debug.Log("choose one mibao222 ");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[times]);
			return;
		}
		if(FreshGuide.Instance().IsActive(100330)&& TaskData.Instance.m_TaskInfoDic[100330].progress >= 0)
		{
			Debug.Log("Make one mibao222 ");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100330];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[4]);
			return;
		}
	}
	void ShowMiBaoYInDao2(int times)
	{//4 5
		//Debug.Log("choose one mibaolianxu =  "+times);
		if(times > 6)
		{
			times = 6;
		}
		if(FreshGuide.Instance().IsActive(100170)&& TaskData.Instance.m_TaskInfoDic[100170].progress < 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[times]);
			return;
		}
	}
	public void ShowGetAward()
	{
		int mGrad = 10;
		for(int i = 0 ; i < Proprety.Length ; i++)
		{
			string mstr = "等级升到";
			if((i+1)*10 <= ShowmMiBaoinfo.level)
			{
				MiBaoExtrattributeTemplate mMiBaoExt = MiBaoExtrattributeTemplate.GetMiBaoExtrattributeTemplate_By_Id_and_level(ShowmMiBaoinfo.miBaoId,(i+1)*10);
				AwardforGrad[i].text =MyColorData.getColorString (4,mstr+mGrad.ToString()+"级");
				Proprety[i].text = MyColorData.getColorString (4,mMiBaoExt.Name +"+"+mMiBaoExt.Num.ToString());
			}
			else
			{
				MiBaoExtrattributeTemplate mMiBaoExt = MiBaoExtrattributeTemplate.GetMiBaoExtrattributeTemplate_By_Id_and_level(ShowmMiBaoinfo.miBaoId,(i+1)*10);
				AwardforGrad[i].text =MyColorData.getColorString (8,mstr+mGrad.ToString()+"级");
				Proprety[i].text = MyColorData.getColorString (8,mMiBaoExt.Name +"+"+mMiBaoExt.Num.ToString());
			}
			mGrad += 10;
		}
	}
    public void ShowEffect()
	{
		int effectid = 600153;
		IsOPenEffect = true;
		UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,StarUpbtn,EffectIdTemplate.GetPathByeffectId(effectid));
	}
	public void CloseEffect()
	{
		IsOPenEffect = false;
		UI3DEffectTool.ClearUIFx (StarUpbtn);
		UI3DEffectTool.ClearUIFx (MiBaoeffect);
	}
	IEnumerator Update_ZhanliAndLevel()
	{
		if(LinShi_ZhanLi > Cru_MiBao_Zhanli+1)
		{
			int m = (LinShi_ZhanLi - Cru_MiBao_Zhanli);
			float mTime = 0.01f;

			while(m > 0)
			{

				if(m > 10000)
				{
					m -= 10000;
					Cru_MiBao_Zhanli += 10000;
				}
				if(m > 1000)
				{
					m -= 1000;
					Cru_MiBao_Zhanli += 1000;
				}
				if(m > 100)
				{
					m -= 100;
					Cru_MiBao_Zhanli += 100;
				}
				if(m > 10)
				{
					m -= 10;
					Cru_MiBao_Zhanli += 10;
				}
				else
				{
					m -= 1;
					Cru_MiBao_Zhanli += 1;
				}
			    MiBaoZl.text = "战力: "+ (Cru_MiBao_Zhanli).ToString();

				MiBaoZl.gameObject.transform.localScale = new Vector3(1.2f,1.2f,1.2f);

				yield return new WaitForSeconds(mTime);
			}
			MiBaoZl.gameObject.transform.localScale = Vector3.one;
		}
		else{
			MiBaoZl.text = "战力: "+ LinShi_ZhanLi.ToString();
		}
	}

	void ShowStar()
	{
	
		foreach(UISprite s in Stars)
		{
			Destroy(s.gameObject);
		}
		Stars.Clear ();
		
		for(int i = 0 ; i < ShowmMiBaoinfo.star; i ++)
		{
			GameObject StarTemp = Instantiate(Star.gameObject) as GameObject;
			
			StarTemp.SetActive(true);
			
			StarTemp.transform.parent = Star.gameObject.transform.parent;
			
			StarTemp.transform.localPosition = new Vector3(0,-40+25*i,0);
			
			StarTemp.transform.localScale = Star.gameObject.transform.localScale;
			
			UISprite mUISprite = StarTemp.GetComponent<UISprite>();
			
			Stars.Add(mUISprite);
		}

	}

	IEnumerator countNunber()
	{
		yield return new WaitForSeconds (1.0f);
	}

	int tems = 3; 

	public void MibaoUp() //秘宝升级
	{
	    tems++;

		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(ShowmMiBaoinfo.miBaoId);

		ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId,mibaolevel);
		//Debug.Log ("NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint  = "+NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint);
		if(NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint <= 0)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false,false,true);
			if(IsOPenEffect)
			{
				CloseEffect ();
			}
//			Debug.Log ("levelPoint <= 0 = ");
			MibaoUpCallback = true;
			return;
		}
		if(JunZhuData.Instance().m_junzhuInfo.jinBi < mExpXxmlTemp.needExp)
		{
			CloseYInDao();
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
			if(IsOPenEffect)
			{
				CloseEffect ();
			}
//			Debug.Log ("m_junzhuInfo.jinBi < mExpXxmlTemp.needExp ");
			return;
		}
		if(mibaolevel >= JunZhuData.Instance().m_junzhuInfo.level)
		{
			if(IsOPenEffect)
			{
				CloseEffect ();
			}
//			Debug.Log ("mibaolevel >= JunZhuData.Instance().m_junzhuInfo.level ");
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadMiBaolvBack);
			return;
		}
		if(mibaolevel>= 100)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),Load_NoLevelUpBack);
			if(IsOPenEffect)
			{
				CloseEffect ();
			}
			return;
		}
		JunZhuData.Instance().m_junzhuInfo.jinBi -= mExpXxmlTemp.needExp;

		NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint  -= 1;
		PointNum.text = NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint.ToString ();

		int zhanli = GetMiBaoInfo.Initance ().JiSuanZhanLi(ShowmMiBaoinfo.miBaoId,ShowmMiBaoinfo.level+1,ShowmMiBaoinfo.star,4);
		int gongji = GetMiBaoInfo.Initance ().JiSuanZhanLi (ShowmMiBaoinfo.miBaoId,ShowmMiBaoinfo.level+1,ShowmMiBaoinfo.star,1);
		int fangyu = GetMiBaoInfo.Initance ().JiSuanZhanLi (ShowmMiBaoinfo.miBaoId,ShowmMiBaoinfo.level+1,ShowmMiBaoinfo.star,2);
		int shengming = GetMiBaoInfo.Initance ().JiSuanZhanLi (ShowmMiBaoinfo.miBaoId,ShowmMiBaoinfo.level+1,ShowmMiBaoinfo.star,3);
		ShowmMiBaoinfo.zhanLi = zhanli;
		ShowmMiBaoinfo.gongJi = gongji;
		ShowmMiBaoinfo.fangYu = fangyu;
		ShowmMiBaoinfo.shengMing = shengming;
		ShowmMiBaoinfo.level += 1;
//		Debug.Log ("ShowmMiBaoinfo.zhanLi = "+ShowmMiBaoinfo.zhanLi);
//		Debug.Log ("ShowmMiBaoinfo.gongJi = "+ShowmMiBaoinfo.gongJi);
//		Debug.Log ("ShowmMiBaoinfo.fangYu = "+ShowmMiBaoinfo.fangYu);
//		Debug.Log ("ShowmMiBaoinfo.shengMing = "+ShowmMiBaoinfo.shengMing);
		Init ();
		MibaoLevelupReq MiBaoshengJ = new MibaoLevelupReq ();
		
		MemoryStream MiBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoer = new QiXiongSerializer ();
		
		MiBaoshengJ.mibaoId = ShowmMiBaoinfo.miBaoId;
		
		MiBaoer.Serialize (MiBaoStream,MiBaoshengJ);
	
		byte[] t_protof;
		t_protof = MiBaoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_LEVELUP_REQ,ref t_protof);
//		Debug.Log ("秘宝升级发送");
	}
    
	void CloseYInDao()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	void  CantUpMiBao()
	{
		// TODO
		// 秘宝升级全部完成后调用
		PushAndNotificationHelper.SetRedSpotNotification ( 600, false );

//
//		// 秘宝升星激活红点时
//		PushAndNotificationHelper.SetRedSpotNotification (602, true);
//
//		// 秘宝升星红点取消时
//		PushAndNotificationHelper.SetRedSpotNotification (602, false);



//
//		// 秘宝合成激活红点时
//		PushAndNotificationHelper.SetRedSpotNotification (605, true);
//		
//		// 秘宝合成红点取消时
//		PushAndNotificationHelper.SetRedSpotNotification (605, false);
//
//
//
//		// 秘宝技能激活，激活红点时
//		PushAndNotificationHelper.SetRedSpotNotification (610, true);
//		
//		// 秘宝技能激活，红点取消时
//		PushAndNotificationHelper.SetRedSpotNotification (610, false);
	}
	
    public bool OnProcessSocketMessage(QXBuffer p_message){
	
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MIBAO_LEVELUP_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoLevelupResp Levelup = new MibaoLevelupResp();
				
				t_qx.Deserialize(t_stream, Levelup, Levelup.GetType());
//				Debug.Log("密保升级返回");
				if(Levelup.mibaoInfo != null)
				{
//					Debug.Log("518  dataBack");

					ShowmMiBaoinfo = Levelup.mibaoInfo;

					SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);

					MibaoUpCallback = true;

					Is_LevelUp = true;

					Init();
					//MiBaoManager.Instance().ShowZhanLiAnmition();
				}
				else
				{
					Debug.Log("Levelup.mibaoInfo == null");
				}
				return true;
			 }
			case ProtoIndexes.s_Mibao_StarUp_Resp:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoStarUpResp MiBaoMibaoStarUpResp= new MibaoStarUpResp();
				
				t_qx.Deserialize(t_stream, MiBaoMibaoStarUpResp, MiBaoMibaoStarUpResp.GetType());

				m_iBaoActiveInfo = MiBaoMibaoStarUpResp;

				if(MiBaoMibaoStarUpResp.mibaoInfo != null)
				{
					ShowmMiBaoinfo = MiBaoMibaoStarUpResp.mibaoInfo;

					Is_LevelUp = true;

					PushAndNotificationHelper.SetRedSpotNotification (602, false);

					Init();
					StartCoroutine("countMiBaoStarUpNunber");
					//Debug.Log("90909090");
					//UI3DEffectTool.ShowBottomLayerEffect (UI3DEffectTool.UIType.PopUI_2,MiBaoeffect,EffectIdTemplate.GetPathByeffectId(100183));
					Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.MI_BAO_ADD_STAR),LoadResourceCallback2);
				}
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);

				//MiBaoManager.Instance().ShowZhanLiAnmition();
				return true;
			}
			case ProtoIndexes.S_MIBAO_ACTIVATE_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoActivateResp MiBaoActiveInfo = new MibaoActivateResp();
				
				t_qx.Deserialize(t_stream, MiBaoActiveInfo, MiBaoActiveInfo.GetType());
				
				if(MiBaoActiveInfo.mibaoInfo != null)
				{
					ShowmMiBaoinfo = MiBaoActiveInfo.mibaoInfo;
					
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_CARD_TEMP ),LoadBck_2);

					Init();

				}
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
				if(UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
				return true;
			}
			default: return false;
			}
		}
		return false;
	}

	public void LoadResourceCallback2(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object)as GameObject;
			
		tempOjbect.transform.localScale = Vector3.one;
		
		tempOjbect.transform.localPosition = new Vector3(200,200,0);
		
		MiBaoStarUpUI mMiBaoStarUpUI = tempOjbect.GetComponent<MiBaoStarUpUI>();
		
		mMiBaoStarUpUI.MiBao1 = needMibao;
		
		mMiBaoStarUpUI.MiBao2 = m_iBaoActiveInfo.mibaoInfo;

		mMiBaoStarUpUI.Init ();
		MainCityUI.TryAddToObjectList (tempOjbect, false );
	}

	public GameObject MiBaoeffect;

	void LoadBck_2(ref WWW p_www,string p_path, Object p_object) // 合成秘宝时候弹出的框 大秘宝
	{

		List<RewardData> data = new List<RewardData> ();

		RewardData data1 = new RewardData (ShowmMiBaoinfo.miBaoId,1,ShowmMiBaoinfo.star);
		data.Add (data1);
		GeneralRewardManager.Instance().CreateSpecialReward (data);
//		GameObject cardtemp = Instantiate(p_object) as GameObject;
//		
//		cardtemp.transform.parent = this.transform.parent;
//		
//		cardtemp.transform.localPosition = new Vector3(0,0,0);
//		
//		cardtemp.transform.localScale = new Vector3(0.9f,0.9f,0.9f);
//
//		mbCardTemp mmbCardTemp = cardtemp.GetComponent<mbCardTemp>();
//		
//		mmbCardTemp.mibaoTemp =  ShowmMiBaoinfo;
//		
//		mmbCardTemp.init();	
	}

	void LockTongBiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		MibaoUpCallback = true;
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,getTongBi,null,null,null);
	}

	void LoadMiBaolvBack(ref WWW p_www,string p_path, Object p_object)//秘宝等级不足回调函数
	{
		MibaoUpCallback = true;
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.LEVEL_NOT_ENOUGH);

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_LEVEL_UP_FAILE);

		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr,null,str1,null,confirmStr,null,null,null,null);
	}

	void Load_NoLevelUpBack(ref WWW p_www,string p_path, Object p_object)//秘宝等级不足回调函数
	{
		MibaoUpCallback = true;
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr,null, "",null,confirmStr,null,null,null,null);
	}

	void getTongBi(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
		}
	}

	public void MibaoStarUp()
	{
		if(CityGlobalData.MibaoSatrUpCallback)
		{
			return;
		}
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),MiBaoUpStarLoadBack);
	}

	private int StarNeedMoney;

	IEnumerator countMiBaoStarUpNunber()
	{
		yield return new WaitForSeconds (1.0f);
		
		CityGlobalData.MibaoSatrUpCallback = false;
	
	}
	MibaoInfo needMibao;//需要传递的参数，临时变量
	void MiBaoUpStarLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		CloseEffect ();

		if (ShowmMiBaoinfo.star >= 5)
		{
			UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
			
			string titleStr = "";
			string str = "";
			string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
			string cancel = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_19);
			
			str = "\r\n"+"星级已满, 不能在进行升星了！";
		
			uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirm,null,null);
		}
		else
		{
			if (ShowmMiBaoinfo.suiPianNum < ShowmMiBaoinfo.needSuipianNum)
			{
//				titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_ENHANCE_3);
//				
//				str = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_ENHANCE_4);
//				
//				uibox.setBox(titleStr,null,MyColorData.getColorString (1,str),null,confirm,null,null);
				uiname = "碎片不足";
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_NOT_ENOUGH_PIECE ),ShowPicecPath);
			}
			else 
			{
				UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
				
				string titleStr = "";
				string str = "";
				string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
				string cancel = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

				MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (ShowmMiBaoinfo.star);
				
				StarNeedMoney = mMiBaoStarTemp.needMoney;

				titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
				
				str ="\r\n"+"此秘宝升星需要消耗" + StarNeedMoney.ToString() + "铜币"+"\r\n"+"\r\n"+"是否现在升星？";

				uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,cancel,confirm,SendStarUpInfo);
				needMibao = ShowmMiBaoinfo;
			}
		}
	}

	void SendStarUpInfo(int i)
	{
		if(i == 2)
		{
			//JunZhuData.Instance().BuyTiliAndTongBi(false,true);
			if(StarNeedMoney > JunZhuData.Instance().m_junzhuInfo.jinBi)
			{
				JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
			}
			else
			{
				CityGlobalData.MibaoSatrUpCallback =  true;

				MibaoStarUpReq MiBaoinfo = new MibaoStarUpReq ();
				MemoryStream MiBaoinfoStream = new MemoryStream ();
				QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
				
				MiBaoinfo.mibaoId = ShowmMiBaoinfo.miBaoId;
				MiBaoinfoer.Serialize (MiBaoinfoStream,MiBaoinfo);
				
				byte[] t_protof;
				t_protof = MiBaoinfoStream.ToArray();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_STARUP_REQ,ref t_protof,ProtoIndexes.s_Mibao_StarUp_Resp.ToString());
			}
		}

	}
	string uiname;
	public void ConllectMiBao()
	{
		uiname = "";
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_NOT_ENOUGH_PIECE ),ShowPicecPath);

	}

	void ShowPicecPath(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject mNoPiece = Instantiate ( p_object )as GameObject;
		
		mNoPiece.SetActive (true);
	
		mNoPiece.transform.parent = GameObject.Find ("SecretCard").transform;;
		
		mNoPiece.transform.localScale = new Vector3 (1,1,1);
		
		mNoPiece.transform.localPosition = new Vector3 (0,0,0);

		LockPiece mLockPiece = mNoPiece.GetComponent<LockPiece>();
		
		mLockPiece.my_Diaoluomibao = ShowmMiBaoinfo; 
		mLockPiece.UINAme = uiname;
		mLockPiece.Init ();
		CloseEffect ();
		//MiBaoScrollView.FirstOPenPath = false;
	}
	private int Mony;
	public void ActiveMiBao()
	{
		MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (ShowmMiBaoinfo.tempId);
		
		Mony = mMiBaoSuipianXMltemp.money;

		//Debug.Log ("howmMiBaoinfo.suiPianNum = "+ShowmMiBaoinfo.suiPianNum);

		//Debug.Log ("mMiBaoSuipianXMltemp.hechengNum = "+mMiBaoSuipianXMltemp.hechengNum);

		if (ShowmMiBaoinfo.suiPianNum >= mMiBaoSuipianXMltemp.hechengNum) {
			CloseYInDao();
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ShowCHAT_UIBOX_INFO);
		} 
		else 
		{
			//Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LockOFPices);
			uiname = "碎片不足";
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_NOT_ENOUGH_PIECE ),ShowPicecPath);
		}
	}

	void ShowCHAT_UIBOX_INFO(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"合成此秘宝需要消耗" +Mony.ToString()+"铜币"+"\r\n"+"\r\n"+"是否合成？";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,CancleBtn,confirmStr,StarmakeMiBao,null,null);
	}

	void LockOFPices(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "碎片不足，无法激活！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, null,MyColorData.getColorString (1,str1),null,confirmStr,null,null);
	}
	void StarmakeMiBao(int i)
	{
		if(i == 2)
		{
			if(Mony <= JunZhuData.Instance().m_junzhuInfo.jinBi)
			{
				MibaoActivate MiBaoinfo = new MibaoActivate ();
				
				MemoryStream MiBaoinfoStream = new MemoryStream ();
				
				QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
				
				Debug.Log ("ShowmMiBaoinfo.tempId"+ShowmMiBaoinfo.tempId);
				
				MiBaoinfo.tempId = ShowmMiBaoinfo.tempId;
				
				MiBaoinfoer.Serialize (MiBaoinfoStream,MiBaoinfo);
				
				byte[] t_protof;
				t_protof = MiBaoinfoStream.ToArray();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_ACTIVATE_REQ,ref t_protof,ProtoIndexes.S_MIBAO_ACTIVATE_RESP.ToString());//秘宝激活
				PushAndNotificationHelper.SetRedSpotNotification (605, false);

			}
			else
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
			}
		}
		else
		{
			ShowMiBaoYInDao(0);
		}
	}
	public void CloseBtn()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		CloseEffect ();
		NewMiBaoManager.Instance().BackToFirstPage (this.gameObject);
	}
}
