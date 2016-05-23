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

	private FuwenLanwei mCurrFuWenlanwei ;


	public GameObject RongHeBtn;

	public static FuWenInfoShow mFuWenInfoShow;

	public List<FuwenInBag> fuwensinBag = new List<FuwenInBag>();

	public UILabel DataMove;

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
	
	}
	public void Init()
	{
//		FuWenItem_Id
		fuwensinBag.Clear ();
		RongHeBtn.SetActive (false);

		FuWenTemplate mFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mFuWenlanwei.itemId);

		mCurrFuWenlanwei = (FuwenLanwei)mFuWenlanwei.Public_MemberwiseClone();

		InitLevelInfo ();
	}
	void InitLevelInfo()
	{
//		Debug.Log ("mFuWenlanwei.exp = "+mFuWenlanwei.exp);
//
//		Debug.Log ("mCurrFuWenlanwei.exp = "+mCurrFuWenlanwei.exp);

		FuWenTemplate mFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mCurrFuWenlanwei.itemId);
		mFuwenIcon.spriteName = mFUwen.icon.ToString ();
		Debug.Log ("mFUwen.icon = "+mFUwen.icon);
		mFuwenpinzhi.spriteName = "pinzhi"+(mFUwen.color-1).ToString();

		mFuwenName.text = NameIdTemplate.GetName_By_NameId (mFUwen.name);
		Apply_Exp.text = "被融合时提给的经验："+mCurrFuWenlanwei.exp.ToString ();//...........................................................................
		if(mCurrFuWenlanwei.exp < mFUwen.lvlupExp) // 判断是否该升级了
		{
			//Debug.Log ("mFUwen.fuwenFront = "+mFUwen.fuwenFront);
			if(mFUwen.fuwenFront > 0)
			{
				FuWenTemplate mFrontFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mFUwen.fuwenFront);
				//Debug.Log ("mFrontFUwen.exp = "+mFrontFUwen.lvlupExp);
				if(mCurrFuWenlanwei.exp < mFrontFUwen.lvlupExp && mFUwen.fuwenLevel > 1)// 判断是否该降级了
				{
					Debug.Log ("判断是否该降级了 ");
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
			InitLevelInfo();
//			FuWenTemplate m_mFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (mFUwen.fuwenNext);
//			Up_Need_Exp.text = "升级经验："+mCurrFuWenlanwei.exp.ToString()+"/"+m_mFUwen.lvlupExp.ToString();	
//
//			if(m_mFUwen.fuwenLevel < m_mFUwen.levelMax)
//			{
//				LevelInfo.SetActive(true);
//				MaxLevel.text = "";
//				mFuwenLevel.text = m_mFUwen.fuwenLevel.ToString() +"级";
//				mNextFuwenLevel.text = (m_mFUwen.fuwenLevel+1).ToString() +"级";
//			}
//			else
//			{
//				LevelInfo.SetActive(false);
//				MaxLevel.text = "等级已满";
//				
//			}
//			CurrProperty.text = GetFuWenProperty (m_mFUwen.shuxing)+m_mFUwen.shuxingValue.ToString();
//			if(m_mFUwen.fuwenNext > 0)
//			{
//				FuWenTemplate m_FUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (m_mFUwen.fuwenNext);
//				NextProperty.text = GetFuWenProperty (m_FUwen.shuxing)+m_FUwen.shuxingValue.ToString();
//			}
//			else{
//				NextProperty.text = "等级已满";
//			}
//			mSlider.value = (float)(mCurrFuWenlanwei.exp)/(float)(m_mFUwen.lvlupExp);
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
					NewFuWenPage.Instance().GetBagInfo();
					NewFuWenPage.Instance().Init();
					mFuWenlanwei.exp = mFuwenRongHeResp.exp;
					FuWenItem_Id = mFuwenRongHeResp.itemId;
					Init();
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
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockRongHeLoadBack);
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
	void ComRong_He(int i)
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
		}
	}
}
