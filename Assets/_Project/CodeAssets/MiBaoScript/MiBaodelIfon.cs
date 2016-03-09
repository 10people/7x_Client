using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;


public class MiBaodelIfon : MonoBehaviour {
	GameObject theRoot;
	public GameObject mPanel;
	public GameObject mParenrPanel;

	[HideInInspector]public MibaoInfo  m_mibaoinfo ;  //	技能暴击减免
	List<UISprite> stars = new List<UISprite> ();

	[HideInInspector]public List<MibaoInfo> MibaoInfo_ZH = new List<MibaoInfo> ();

	public UISprite Icon;
	public UISprite Piece_Icon;
	public UISprite spriteStar;
	public UISprite Boxcolider;
	public UISprite PinZhi;
	[HideInInspector]public int  MiBaoPinZhi;
	[HideInInspector]public int  MiBaoZuHeId;
	public UILabel allnum_Suipiannum;//碎片总数和需要的个数

	public UISprite CanMakeMibaoTips;

	[HideInInspector]public int  Activenums;//碎片总数和需要的个数

	void Start () {
	
		 theRoot = GameObject.Find ("SkillRoot");

	}
	void Update () {
	
	}
	public void InitSkill()
	{
		//Debug.Log ("MibaoInfo_ZHsasssssssssss" +MibaoInfo_ZH.Count);
		if(Activenums >= 3)
		{
			Boxcolider.gameObject.SetActive(false);
		}else{
			Boxcolider.gameObject.SetActive(true);
		}

	}

	public void Init()
	{
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(m_mibaoinfo.miBaoId);
		MiBaoSuipianXMltemp mMiBaosuipian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (mMiBaoXmlTemp.suipianId);
		int mIcon = mMiBaoXmlTemp.icon;
		if(m_mibaoinfo.level > 0)
		{
			Icon.gameObject.SetActive(true);

			Icon.spriteName = mIcon.ToString ();//暂时无UI资源 。。。。。。。。。。。

			Piece_Icon.gameObject.SetActive(false);
		}
		else
		{
			Icon.gameObject.SetActive(false);

			Piece_Icon.gameObject.SetActive(true);

//			Debug.Log("mMiBaosuipian.icon = " +mMiBaosuipian.icon);

			Piece_Icon.spriteName = mMiBaosuipian.icon.ToString ();
		}



		int mlv = m_mibaoinfo.level;

		PinZhi.spriteName = "pinzhi" + (mMiBaoXmlTemp.pinzhi - 1).ToString ();

		MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (m_mibaoinfo.tempId);


		if(mlv == 0)//没有激活的状态
		{
			Boxcolider.gameObject.SetActive(true);
			if(m_mibaoinfo.suiPianNum >= mMiBaoSuipianXMltemp.hechengNum )
			{
				CanMakeMibaoTips.gameObject.SetActive(true); // 特效来了以后换为特效
			}else
			{
				CanMakeMibaoTips.gameObject.SetActive(false);
			}
			allnum_Suipiannum.gameObject.SetActive(true);
			ShowDisActiveMiBaoInfo();
		}
		else{//显示激活的状态信息
		
			Boxcolider.gameObject.SetActive(false);
			allnum_Suipiannum.gameObject.SetActive(false);
			if(m_mibaoinfo.suiPianNum >= m_mibaoinfo.needSuipianNum )
			{
				CanMakeMibaoTips.gameObject.SetActive(true); // 特效来了以后换为特效
			}else
			{
				CanMakeMibaoTips.gameObject.SetActive(false);
			}
			ShowMiBaoInfo ();
		}
	
	}

	void ShowDisActiveMiBaoInfo()
	{
		MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (m_mibaoinfo.tempId);
		allnum_Suipiannum.text = m_mibaoinfo.suiPianNum.ToString() + "/" + mMiBaoSuipianXMltemp.hechengNum.ToString();

	}
	void ShowMiBaoInfo()
	{

		CreateStars (m_mibaoinfo.star);
	}

	void CreateStars(int num)
	{

		
		foreach(UISprite sprite in stars)
		{
			Destroy(sprite.gameObject);
		}
		stars.Clear();
		
		for(int i = 0; i < num; i++)
		{
			GameObject spriteObject = (GameObject)Instantiate(spriteStar.gameObject);
			
			spriteObject.SetActive(true);
			
			spriteObject.transform.parent = spriteStar.transform.parent;
			
			spriteObject.transform.localScale = spriteStar.transform.localScale;
			
			spriteObject.transform.localPosition = spriteStar.transform.localPosition + new Vector3(i * 20 - (num - 1) * 10, 0, 0);
			
			UISprite sprite = (UISprite)spriteObject.GetComponent("UISprite");
			
			stars.Add(sprite);
		}
	}
	public void hitDisActiveMiBao()
	{
		//Debug.Log ("没有呗激活。。。。。。。。");
		if(theRoot)
		{

			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			mSkillControl.M_mibaoinfo = m_mibaoinfo;
			mSkillControl.ShowPicePath();
		}
	
	}
	public void hitSkill()
	{
		int posx = (int)(mParenrPanel.transform.localPosition.x + mPanel.transform.localPosition.x);
		int posy = -240;
		
		Vector3 pos = new Vector3 (posx,posy,0);

		
		if(theRoot)
		{
			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			mSkillControl.Starposition = pos;
			mSkillControl.M_mibaoinfo = m_mibaoinfo;
			mSkillControl.mMibaoInfo_ZH = MibaoInfo_ZH;
			//Debug.Log("MiBaoZuHeId"+MiBaoZuHeId);
			//Debug.Log("MiBaoPinZhi"+MiBaoPinZhi);
			mSkillControl.ZH_ID = MiBaoZuHeId;
			mSkillControl.MinPZ = MiBaoPinZhi;

			mSkillControl.Init();
		}
	}
	public void hitSecret1()
	{
		int posx = (int)(mParenrPanel.transform.localPosition.x + mPanel.transform.localPosition.x);
		int posy = 120;
		
		Vector3 pos = new Vector3 (posx,posy,0);

		
		if(theRoot)
		{
			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			//mSkillControl.Starposition = pos;
			mSkillControl.M_mibaoinfo = m_mibaoinfo;
			mSkillControl.createMiBao(pos,1);
		}
	}
	public void hitSecret2()
	{
		int posx = (int)(mParenrPanel.transform.localPosition.x + mPanel.transform.localPosition.x);
		int posy = 0;
		
		Vector3 pos = new Vector3 (posx,posy,0);

		
		if(theRoot)
		{
			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			//mSkillControl.Starposition = pos;
			mSkillControl.M_mibaoinfo = m_mibaoinfo;
			mSkillControl.createMiBao(pos,1);
		}
	}
	public void hitSecret3()
	{
		int posx = (int)(mParenrPanel.transform.localPosition.x + mPanel.transform.localPosition.x);
		int posy = -120;
		
		Vector3 pos = new Vector3 (posx,posy,0);

		
		if(theRoot)
		{
			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			//mSkillControl.Starposition = pos;
			mSkillControl.M_mibaoinfo = m_mibaoinfo;
			mSkillControl.createMiBao(pos,1);
		}
	}
}
