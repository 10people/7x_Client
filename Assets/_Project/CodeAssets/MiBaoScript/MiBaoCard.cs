using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class MiBaoCard : MonoBehaviour,SocketProcessor {

	Vector3 endposition;
	Vector3 scale;
	[HideInInspector]public int MiBaonum;
	[HideInInspector]public Vector3 Startposition;
	[HideInInspector]public MibaoInfo  My_mibaoinfo ;  //	技能暴击减免
	List<UISprite> stars = new List<UISprite> ();

	public UISprite spriteStar;
	public UISprite spriteIconBtn;
	public UISprite spritePinzhi;
	public UITexture spriteText;

	public UILabel Level;
	public UILabel GongJi;
	public UILabel FangYu;
	public UILabel Shengming;
	public UILabel Gaoji1;
	public UILabel Gaoji2;
    string Gaoji;
	string GaojiData;
	public UILabel Intrudution;
	public UILabel skillinstroduce;

	public UILabel NeedSuiPian;
	public UILabel NeedMoney;

	public UISlider mUISlider;

	public UILabel MiBao_Name;
	[HideInInspector]public List<string> Shuxing = new List<string> ();
	[HideInInspector]public List<string> ShuxingData = new List<string> ();
	float time = 0.5f;

	GameObject AddstarUI;
	GameObject PathUI;

	public UILabel GjSx1;//高级属性1
	public UILabel GjSx2;//高级属性2
	
	public UILabel UpLv_JInJie;
	public GameObject UpLv_JJ;
	public GameObject MiBaoStarUP;
	private string jiStr;//级

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
		jiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_UP_LEVEL);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
		
	}


	void Start () {



		endposition = new Vector3 (0,-46,0);
		scale = new Vector3 (1f,1f,1);
		startmove ();
	}
	
	void startmove()
	{
		iTween.MoveTo(this.gameObject, iTween.Hash("position", endposition, "time",time,"islocal",true));
		iTween.ScaleTo (this.gameObject,iTween.Hash("scale",scale,"time",time));
	}
	
	void Update () {
		
	}
	
	public void init()
	{
		ShowShuXing ();
		ShowStar ();
	}

	void ShowShuXing()//显示所有属性
	{
		//Debug.Log ("My_mibaoinfo.miBaoId = " +My_mibaoinfo.miBaoId);
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(My_mibaoinfo.miBaoId);
		DescIdTemplate mDescIdTemplate = DescIdTemplate.getDescIdTemplateByNameId (mMiBaoXmlTemp.descId);

		Intrudution.text = mDescIdTemplate.description;//显示描述
		MiBaoSuipianXMltemp mMiBaosuipian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (mMiBaoXmlTemp.suipianId);
		//spriteIcon.spriteName = mMiBaoXmlTemp.icon.ToString();
		spriteIconBtn.spriteName = mMiBaosuipian.icon.ToString();

		spritePinzhi.spriteName = "pinzhi" + (mMiBaoXmlTemp.pinzhi-1).ToString ();

		spriteText.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON) + mMiBaoXmlTemp.icon.ToString ());

		if(mMiBaoXmlTemp.pinzhi > 1)
		{
			MiBaoXmlTemp m_MiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempByPinZhi(mMiBaoXmlTemp.pinzhi-1);
			int m_lv =  My_mibaoinfo.level - m_MiBaoXmlTemp.maxLv;
			Level.text = m_lv.ToString()+ jiStr;
		}else
		{
			Level.text = My_mibaoinfo.level.ToString()+ jiStr;
		}

		//Debug.Log("My_mibaoinfo.miBaoId" +My_mibaoinfo.miBaoId);

		GongJi.text = My_mibaoinfo.gongJi.ToString();
		FangYu.text =  My_mibaoinfo.fangYu.ToString();
		Shengming.text = My_mibaoinfo.shengMing.ToString();
//		Debug.Log ("My_mibaoinfo.wqSH"+My_mibaoinfo.wqSH);
//		Debug.Log ("My_mibaoinfo.wqJM"+My_mibaoinfo.wqJM);
//		Debug.Log ("My_mibaoinfo.wqBJ"+My_mibaoinfo.wqBJ);
//		Debug.Log ("My_mibaoinfo.wqRX"+My_mibaoinfo.wqRX);
//		Debug.Log ("My_mibaoinfo.jnSH"+My_mibaoinfo.jnSH);
//		Debug.Log ("My_mibaoinfo.jnJM"+My_mibaoinfo.jnJM);
//		Debug.Log ("My_mibaoinfo.jnBJ"+My_mibaoinfo.jnBJ);
//		Debug.Log ("My_mibaoinfo.jnRX"+My_mibaoinfo.jnRX);

		Shuxing.Clear ();
		ShuxingData.Clear ();

//		if(My_mibaoinfo.wqSH > 0)
//		{
//			Gaoji = LanguageTemplate.GetText (LanguageTemplate.Text.DAMAGE_CHANGE_1)+":";
//		    GaojiData = My_mibaoinfo.wqSH.ToString();
//
//			Shuxing.Add(Gaoji);
//			ShuxingData.Add(GaojiData);
//		}
//		if(My_mibaoinfo.wqJM > 0)
//		{
//			Gaoji = LanguageTemplate.GetText (LanguageTemplate.Text.DAMAGE_CHANGE_2)+":";
//			GaojiData = My_mibaoinfo.wqJM.ToString();
//			Shuxing.Add(Gaoji);
//			ShuxingData.Add(GaojiData);
//		}
//		if(My_mibaoinfo.wqBJ  > 0)
//		{
//			Gaoji = LanguageTemplate.GetText (LanguageTemplate.Text.DAMAGE_CHANGE_3)+":";
//			GaojiData = My_mibaoinfo.wqBJ.ToString();
//			Shuxing.Add(Gaoji);
//			ShuxingData.Add(GaojiData);
//		}
//		if(My_mibaoinfo.wqRX > 0)
//		{
//			Gaoji = LanguageTemplate.GetText (LanguageTemplate.Text.DAMAGE_CHANGE_4)+":";
//			GaojiData = My_mibaoinfo.wqRX.ToString();
//			Shuxing.Add(Gaoji);
//			ShuxingData.Add(GaojiData);
//		}
//
//
//
//		if(My_mibaoinfo.jnSH  > 0)
//		{
//			Gaoji = LanguageTemplate.GetText (LanguageTemplate.Text.DAMAGE_CHANGE_5)+":";
//			
//			GaojiData = My_mibaoinfo.jnSH.ToString();
//			Shuxing.Add(Gaoji);
//			ShuxingData.Add(GaojiData);
//		}
//		if(My_mibaoinfo.jnJM  > 0)
//		{
//			Gaoji = LanguageTemplate.GetText (LanguageTemplate.Text.DAMAGE_CHANGE_6)+":";
//			
//			GaojiData = My_mibaoinfo.jnJM.ToString();
//			Shuxing.Add(Gaoji);
//			ShuxingData.Add(GaojiData);
//		}
//		if(My_mibaoinfo.jnBJ  > 0)
//		{
//			Gaoji = LanguageTemplate.GetText (LanguageTemplate.Text.DAMAGE_CHANGE_7)+":";
//			
//			GaojiData = My_mibaoinfo.jnBJ.ToString();
//			Shuxing.Add(Gaoji);
//			ShuxingData.Add(GaojiData);
//		}
//		if(My_mibaoinfo.jnRX  > 0)
//		{
//			Gaoji = LanguageTemplate.GetText (LanguageTemplate.Text.DAMAGE_CHANGE_8)+":";
//			
//			GaojiData = My_mibaoinfo.jnRX.ToString();
//			Shuxing.Add(Gaoji);
//			ShuxingData.Add(GaojiData);
//		}
		if(Shuxing.Count == 2)
		{
			//Debug.Log("Shuxing.count" +Shuxing.Count);
			Gaoji1.text = Shuxing[0];
			Gaoji2.text = Shuxing[1];
			GjSx1.text = ShuxingData[0];
			GjSx2.text = ShuxingData[1];
		}

		//Debug.Log ("mMiBaoXmlTemp.expId = "+mMiBaoXmlTemp.expId);
		int mlv = My_mibaoinfo.level;
	
		//Debug.Log ("mlv = "+mlv);
		ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId,mlv);//假设等级为1
		//ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId,My_mibaoinfo.level);

		NeedMoney.text = mExpXxmlTemp.needExp.ToString ();

	

		SkillTemplate mSkillTemplate = SkillTemplate.getSkillTemplateById (mMiBaoXmlTemp.skill); //每一个秘宝的技能介绍
		skillinstroduce.text = DescIdTemplate.GetDescriptionById(mSkillTemplate.funDesc);//读取技能介绍表。、。。
//		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi (mMiBaoXmlTemp.zuheId,My_mibaoinfo.level);
//
//		SkillTemplate mSkillTemplate = SkillTemplate.getSkillTemplateById (mMiBaoSkillTemp.skill);
//	
//		skillinstroduce.text = DescIdTemplate.GetDescriptionById(mSkillTemplate.funDesc);//读取技能介绍表。、。。一个秘宝组的技能介绍

		//MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (My_mibaoinfo.star);

		//int mneedSuipianNum = mMiBaoStarTemp.needNum;

		NeedSuiPian.text = My_mibaoinfo.suiPianNum.ToString()+"/"+My_mibaoinfo.needSuipianNum.ToString ();

		mUISlider.value = (float)( My_mibaoinfo.suiPianNum )/ (float)(My_mibaoinfo.needSuipianNum);

		if(mUISlider.value > 1f)mUISlider.value = 1f;

		MiBao_Name.text = NameIdTemplate.GetName_By_NameId (mMiBaoXmlTemp.nameId);

	//	Debug.Log ("FreshGuide.Instance().IsActive(300110) = " +FreshGuide.Instance().IsActive(300110));
//		Debug.Log ("TaskData.Instance.m_TaskInfoDic[300110].progress = " +TaskData.Instance.m_TaskInfoDic[300110].progress);
		if(FreshGuide.Instance().IsActive(300110)&& TaskData.Instance.m_TaskInfoDic[300110].progress>= 0&&!UpTask)
		{
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[300110];
			
			//Debug.Log("显示密保属性");
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		}
		//Debug.Log("My_mibaoinfo.level = "+My_mibaoinfo.level);
		if(My_mibaoinfo.level%20 == 0&&My_mibaoinfo.level < 100)
		{
			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_ENHANCE_1);
			UpLv_JInJie.text = str1;
		} 
		else if(My_mibaoinfo.level < 100)
		{
			string str = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_ENHANCE_2);
			UpLv_JInJie.text = str;
		}
		if(My_mibaoinfo.level == 100)
		{
			//UpLv_JInJie.text = "已满级";
			UpLv_JJ.SetActive(false);
		}
		if(My_mibaoinfo.star >= 5)
		{
			MiBaoStarUP.SetActive(false);
		}
	}
	void LoadMibaoUpBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_ENHANCE_3);
		string st1 = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_ENHANCE_4);
		string Confirmbtn = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		uibox.setBox(titleStr,null,st1 ,null,Confirmbtn,null,null);
	}

	void LoadAddmiBaoBack(ref WWW p_www,string p_path, Object p_object)
	{
		AddstarUI = Instantiate(p_object) as GameObject;
		
		AddstarUI.transform.parent = this.gameObject.transform.parent;
		
		AddstarUI.transform.localPosition = new Vector3(0,-46,0);
		
		AddstarUI.transform.localScale = new Vector3(1,1,1);
		
		MiBaoAddStar mMiBaoAddStar = AddstarUI.GetComponent<MiBaoAddStar>();
		
		mMiBaoAddStar.showMiBao = My_mibaoinfo;
		
		mMiBaoAddStar.init();

	}

	public void MiBaoshengXing()//秘宝升星
	{
		Debug.Log ("My_mibaoinfo.suiPianNum   " +My_mibaoinfo.suiPianNum);
		Debug.Log ("My_mibaoinfo.needSuipianNum   " +My_mibaoinfo.needSuipianNum);
		if(My_mibaoinfo.suiPianNum < My_mibaoinfo.needSuipianNum)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadMibaoUpBack);
			return;
		}
		if(My_mibaoinfo.star >= 5)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadSTAR_5Back);
			return;
		}
		if(AddstarUI)
		{
			return;
		}
		else{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_ADD_STAR ),LoadAddmiBaoBack);
		}
	}
	void LoadSTAR_5Back(ref WWW p_www,string p_path, Object p_object)
	{
		//UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		//uibox.setBox("提示",null, "秘宝星级已满，不能升级！",null,"确定",null,null,mbtn1Font,mbtn1Font);
		
	}

//	void LoadMiBaoshengjiBack(ref WWW p_www,string p_path, Object p_object)
//	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//		uibox.setBox("金币不足",null, "秘宝升级的金币不足，是否用元宝购买金币",null,"取消","确定",BuyJinBi,mtitleFont,mbtn1Font,mbtn1Font);
//
//	}
	void LoadMiBaolvBack(ref WWW p_www,string p_path, Object p_object)//秘宝等级不足回调函数
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.LEVEL_NOT_ENOUGH);
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_LEVEL_UP_FAILE);
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		uibox.setBox(titleStr,null,str1,null,confirmStr,null,null);
	}

	void Load_NoLevelUpBack(ref WWW p_www,string p_path, Object p_object)//秘宝等级不足回调函数
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);

		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		uibox.setBox(titleStr,null, "",null,confirmStr,null,null);
	}
	private float conut = 0.0f;
	IEnumerator countNunber()
	{
		yield return new WaitForSeconds (1.0f);
		conut = 0;
	}
/// 秘宝升级

	private bool UpTask = false;
	public void MiBaoshengJi(){
		if(conut > 0)
		{
			StartCoroutine(countNunber());
			return;
		}
		conut = 1;
		StartCoroutine(countNunber());
		Debug.Log ("UpLvDataBack = "+UpLvDataBack);
		if(UpLvDataBack)
		{
			UpLvDataBack = false;
		}
		else{
			return;
		}
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(My_mibaoinfo.miBaoId);
		//DescIdTemplate mDescIdTemplate = DescIdTemplate.getDescIdTemplateByNameId (mMiBaoXmlTemp.descId);

		int mlv2 = My_mibaoinfo.level;

		ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId,mlv2);//假设等级为



		if(JunZhuData.Instance().m_junzhuInfo.jinBi < mExpXxmlTemp.needExp)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
			UpLvDataBack = true;
			return;
		}

		if(My_mibaoinfo.level >= JunZhuData.Instance().m_junzhuInfo.level)
		{

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadMiBaolvBack);
			UpLvDataBack = true;
			return;
		}
		if(mMiBaoXmlTemp.pinzhi >= 5&&mlv2 >= 100)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),Load_NoLevelUpBack);
			UpLvDataBack = true;
			return;
		}
		if(FreshGuide.Instance().IsActive(300110)&& TaskData.Instance.m_TaskInfoDic[300110].progress>= 0)
		{
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[300110];
			Debug.Log("显示秘宝引导第四步");
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[4]);
			UpTask = true;
		}
		MibaoLevelupReq MiBaoshengJ = new MibaoLevelupReq ();

		MemoryStream MiBaoStream = new MemoryStream ();

		QiXiongSerializer MiBaoer = new QiXiongSerializer ();

		MiBaoshengJ.mibaoId = My_mibaoinfo.miBaoId;

		MiBaoer.Serialize (MiBaoStream,MiBaoshengJ);
	//	Debug.Log ("o.jinBi111 =  " +JunZhuData.Instance().m_junzhuInfo.jinBi );
		byte[] t_protof;
		t_protof = MiBaoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_LEVELUP_REQ,ref t_protof);

	}
	void LockTongBiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,getTongBi);
	}
	void getTongBi(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
		}
	}

	private bool UpLvDataBack = true;
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
				
				if(Levelup.mibaoInfo != null)
				{
					//Debug.Log("dataBack" +MiBaoActiveInfo.mibaoInfo.);
					My_mibaoinfo = Levelup.mibaoInfo;

					SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
					init();
					//Debug.Log ("o.jinBi22222 =  " +JunZhuData.Instance().m_junzhuInfo.jinBi );
				}
				else{
					
					//Debug.Log("Levelup == null" );
					return  false;
				}
				//	InitData();

				UpLvDataBack = true;
				return true;
			}
				
			default: return false;
			}
			
		}else
		{
			//Debug.Log ("p_message == null");
		}
		
		return false;
	}

	void LoadPathBack(ref WWW p_www,string p_path, Object p_object)
	{
		PathUI = Instantiate ( p_object ) as GameObject;
		
		PathUI.SetActive (true);
		
		PathUI.transform.parent = this.transform;
		
		PathUI.transform.localScale = new Vector3 (1,1,1);
		PathUI.transform.localPosition = new Vector3 (0,-46,0);
		
		NoEnughPiece mNoEnughPiece = PathUI.GetComponent<NoEnughPiece> ();
		mNoEnughPiece.my_mibao = My_mibaoinfo;
		mNoEnughPiece.Init ();
	}
	/// 显示碎片路径
	public void ShowSuipianPath(){

		if(PathUI){
			return;
		}

		//Debug.Log ("d点击了路径按钮");
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_NOT_ENOUGH_PIECE ),LoadPathBack);

	}
	void ShowStar()//显示星星
	{

		foreach(UISprite sprite in stars)
		{
			Destroy(sprite.gameObject);
		}

		stars.Clear();
		
		for(int i = 0; i < My_mibaoinfo.star; i++)
		{
			GameObject spriteObject = (GameObject)Instantiate(spriteStar.gameObject);
			
			spriteObject.SetActive(true);
			
			spriteObject.transform.parent = spriteStar.transform.parent;
			
			spriteObject.transform.localScale = spriteStar.transform.localScale;
			
			spriteObject.transform.localPosition =  new Vector3(i * 40 - (My_mibaoinfo.star - 1) * 20, 0, 0);
			
			UISprite sprite = (UISprite)spriteObject.GetComponent("UISprite");
			
			stars.Add(sprite);
		}
	}
	public void mClose()
	{

		GameObject supGb = GameObject.Find ("Secret(Clone)");
		if(supGb)
		{
			if(UIYindao.m_UIYindao.m_isOpenYindao)
			{
				UIYindao.m_UIYindao.CloseUI();
			}
			TaskData.Instance.m_DestroyMiBao = false;
			MainCityUI.TryRemoveFromObjectList(supGb);
			Destroy(supGb);
		}
	}
	public void Back()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		scale = new Vector3 (0,0,0);
		iTween.MoveTo(this.gameObject, iTween.Hash("position", Startposition, "time",time,"islocal",true));
		iTween.ScaleTo (this.gameObject,iTween.Hash("scale",scale,"time",time));
		Destroy (this.gameObject,time);
	}

}
