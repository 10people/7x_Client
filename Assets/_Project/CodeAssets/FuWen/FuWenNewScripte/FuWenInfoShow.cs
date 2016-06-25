using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenInfoShow : MonoBehaviour ,SocketProcessor {

	public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	public UISprite mFuwenIcon;

	public UISprite mFuwenpinzhi;

	public UILabel mFuwenName;

	public UILabel mFuwenLevel;

	public UILabel mNextFuwenLevel;

	public UILabel Up_Need_Exp;

	public UILabel Apply_Exp;

	public UILabel CurrProperty;

	public UILabel NextProperty;

	public UILabel Add_Exp;

	public int FuWenItem_Id;

	public GameObject LevelInfo;

	public UILabel MaxLevel;

	public UISlider mSlider;

	public FuwenLanwei mFuWenlanwei;

	public FuwenLanwei mCurrFuWenlanwei ;


	public GameObject RongHeBtn;

	public UISprite RongHeBtnSprite;

	public static FuWenInfoShow mFuWenInfoShow;

	public List<FuwenInBag> fuwensinBag = new List<FuwenInBag>();

	public UILabel DataMove;

	[HideInInspector]public int CurrLevelUpExp;

	[HideInInspector]public int LanweiExp;// 栏位上的经验值

	public static FuWenInfoShow Instance()
	{
		if (!mFuWenInfoShow)
		{
			mFuWenInfoShow = (FuWenInfoShow)GameObject.FindObjectOfType (typeof(FuWenInfoShow));
		}
		
		return mFuWenInfoShow;
	}
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
		AddEventListener ();
		
	}
	public void AddEventListener()
	{
		BtnList.ForEach (item => SetBtnMoth(item));
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
		mFuWenInfoShow = null;
	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	public void BtnManagerMent(GameObject mbutton)
	{
		switch(mbutton.name)
		{
		case "TackOff":
			ChaiJie();
			break;
		case "MixBtn":
			RongHe();
			break;
		default:
			break;
		}
	}

	void Start () {
	
	}
	

	void Update () {
	
		if(fuwensinBag.Count <= 0)
		{
			RongHeBtnSprite.color = Color.gray;
		}
		else
		{
			RongHeBtnSprite.color = Color.white;
		}
	}
	public void Init()
	{
//		FuWenItem_Id
		LanweiExp = 0;

		fuwensinBag.Clear ();

		FuWenTemplate mFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mFuWenlanwei.itemId);

		mCurrFuWenlanwei = (FuwenLanwei)mFuWenlanwei.Public_MemberwiseClone();
		CurrLevelUpExp = mFUwen.lvlupExp;
		InitLevelInfo ();
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	void InitLevelInfo()
	{
		FuWenTemplate mFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mCurrFuWenlanwei.itemId);

		mFuwenIcon.spriteName = mFUwen.icon.ToString ();

		mFuwenpinzhi.spriteName = "pinzhi"+(mFUwen.color-1).ToString();

		mFuwenName.text = NameIdTemplate.GetName_By_NameId (mFUwen.name);

//		Apply_Exp.text = "被熔合时提供的经验："+mCurrFuWenlanwei.exp.ToString ();//...........................................................................

		if(mFUwen.fuwenLevel >= mFUwen.levelMax)// 等级已满
		{
			LevelInfo.SetActive(false);
			MaxLevel.gameObject.SetActive(true);
			MaxLevel.text = "等级已满";
			Up_Need_Exp.text = "升级经验：等级已满";
			mSlider.value = 1.0f;
			CurrProperty.text = GetFuWenProperty (mFUwen.shuxing)+mFUwen.shuxingValue.ToString();
			NextProperty.text = "等级已满";
		}
		else
		{
			if(mCurrFuWenlanwei.exp > LanweiExp)
			{
				LanweiExp = mCurrFuWenlanwei.exp;
			}
			if(mCurrFuWenlanwei.exp < mFUwen.lvlupExp) // 判断是否该升级了
			{
				//Debug.Log ("mFUwen.fuwenFront = "+mFUwen.fuwenFront);
				if(mFUwen.fuwenFront > 0)
				{
					FuWenTemplate mFrontFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mFUwen.fuwenFront);
					//Debug.Log ("mFrontFUwen.exp = "+mFrontFUwen.lvlupExp);
					if(mCurrFuWenlanwei.exp <0 && mFUwen.fuwenLevel > 1)// 判断是否该降级了
					{
						Debug.Log ("判断是否该降级了 ");
						mCurrFuWenlanwei.exp = mFrontFUwen.lvlupExp + mCurrFuWenlanwei.exp;
						mCurrFuWenlanwei.itemId = mFUwen.fuwenFront;
						InitLevelInfo();
						return;
					}
				}
				if(mFUwen.fuwenLevel < mFUwen.levelMax)
				{
					LevelInfo.SetActive(true);
					MaxLevel.text = "";
					mFuwenLevel.text = mFUwen.fuwenLevel.ToString() +"级";
					mNextFuwenLevel.text = (mFUwen.fuwenLevel+1).ToString() +"级";
				}
				else
				{
					LevelInfo.SetActive(false);
					MaxLevel.text = "等级已满";
					
				}
				Up_Need_Exp.text = "升级经验："+mCurrFuWenlanwei.exp.ToString()+"/"+mFUwen.lvlupExp.ToString();
				
				CurrProperty.text = GetFuWenProperty (mFUwen.shuxing)+mFUwen.shuxingValue.ToString();
				if(mFUwen.fuwenNext > 0)
				{
					FuWenTemplate m_FUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mFUwen.fuwenNext);
					NextProperty.text = GetFuWenProperty (m_FUwen.shuxing)+m_FUwen.shuxingValue.ToString();
				}
				else{
					NextProperty.text = "等级已满";
				}
				mSlider.value = (float)(mCurrFuWenlanwei.exp)/(float)(mFUwen.lvlupExp);
			}
			else
			{
				Debug.Log ("可以升级 ");
				mCurrFuWenlanwei.itemId = mFUwen.fuwenNext;
				//			FuWenTemplate m_NextFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mCurrFuWenlanwei.itemId);
				mCurrFuWenlanwei.exp = mCurrFuWenlanwei.exp - mFUwen.lvlupExp ;
				InitLevelInfo();
			}
		}
	
	}
	string GetFuWenProperty(int index)
	{
		string mstr = "";
		switch(index)
		{
		case 1:
			mstr = "攻击";
			break;
		case 2:
			mstr = "防御";
			break;
		case 3:
			mstr = "生命";
			break;
		case 4:
			mstr = "武器伤害加深";
			break;
		case 5:
			mstr = "武器伤害抵抗";
			break;
		case 6:
			mstr = "武器暴击加深";
			break;
		case 7:
			mstr = "武器暴击抵抗";
			break;
		case 8:
			mstr = "技能伤害加深";
			break;
		case 9:
			mstr = "技能伤害抵抗";
			break;
		case 10:
			mstr = "技能暴击加深";
			break;
		case 11:
			mstr = "技能暴击抵抗";
			break;
		default:
			break;
		}
		return mstr;
	}
	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_FUWEN_RONG_HE_RESP: //符文融合请求返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenRongHeResp mFuwenRongHeResp  = new FuwenRongHeResp();
				
				t_qx.Deserialize(t_stream, mFuwenRongHeResp, mFuwenRongHeResp.GetType());
				
				if(mFuwenRongHeResp.result == 0)
				{
					string mStr = "熔合成功！";
					ClientMain.m_UITextManager.createText( mStr);
//					NewFuWenPage.Instance().BackToFirst();
					NewFuWenPage.Instance().Init(NewFuWenPage.Instance().mQueryFuwen.tab);
					mFuWenlanwei.exp = mFuwenRongHeResp.exp;
					FuWenItem_Id = mFuwenRongHeResp.itemId;

				}
				else
				{
					string mStr = "熔合失败，没有找到相应的符文！";
					ClientMain.m_UITextManager.createText( mStr);
				}
				return true;
			}
		
			default: return false;
			}
			
		}
		else
		{
			Debug.Log ("p_message == null");
		}
		//		
		return false;
	}
	public UILabel mlabel;
	public void CreateLifeMoveNow(int content)
	{

		CreateLifeMove (mlabel.gameObject,content);
		mCurrFuWenlanwei.exp += content;
		RongHeBtn.SetActive (true);
		InitLevelInfo ();
	}
	public void _DeleltLifeMoveNow(int content)
	{
		mCurrFuWenlanwei.exp -= content;
		Debug.Log ("fuwensinBag.Count = "+fuwensinBag.Count);
		if(fuwensinBag.Count <= 0)
		{
			RongHeBtn.SetActive (false);
		}
		InitLevelInfo ();
	}
	public void CreateLifeMove(GameObject move , int content)
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
	public void ChaiJie() // 拆卸
	{
		NewFuWenPage.Instance ().ChaiJieFuWenOprection ();
	}
	public void RongHe() //融合
	{
		if(fuwensinBag.Count <= 0)
		{ComRong_He (1);}
		else
		{
			ComRong_He(2);
		}

		//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockRongHeLoadBack);
	}
	void LockRongHeLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "\n确定要熔合吗？";//被熔合的符文中包含高品质符文
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,CancleBtn,confirmStr,ComRong_He,null,null,null);
	}
	void ComRong_He(int i = 2)
	{
		if(i == 2)
		{
			FuwenRongHeReq  mFuwenRongHeReq  = new FuwenRongHeReq  ();
			MemoryStream MiBaoinfoStream = new MemoryStream ();
			QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
			
			mFuwenRongHeReq.tab = NewFuWenPage.Instance ().mQueryFuwen.tab;
			
			mFuwenRongHeReq.lanweiId = mFuWenlanwei.lanweiId;
			
			mFuwenRongHeReq.bagList = fuwensinBag;
			
			MiBaoinfoer.Serialize (MiBaoinfoStream,mFuwenRongHeReq);
			
			byte[] t_protof;
			t_protof = MiBaoinfoStream.ToArray();
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FUWEN_RONG_HE,ref t_protof,ProtoIndexes.S_FUWEN_RONG_HE_RESP.ToString());
			fuwensinBag.Clear();
		}
		else
		{
			ClientMain.m_UITextManager.createText("未选择熔合符文！");
		}
	}
}
