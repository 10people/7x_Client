using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class MiBaotemp : MonoBehaviour {

	public MibaoInfo mMiBaoinfo;
	public UISprite MiBao_pinzhi;
	public UISprite MiBao_Star;
	public UISprite MiBao_Icon;

	public UISprite MiBao_IsBeChoosed;
	List<UISprite> stars = new List<UISprite> ();

	public GameObject IsBeChoosed_Root;
	public GameObject SameZuHe_IdTitle;//秘宝提示 如果是同一组的话就提示红点
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		BeChoose_miBao_Bg mBeChoose_miBao_Bg = IsBeChoosed_Root.GetComponent<BeChoose_miBao_Bg>();


		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById (mMiBaoinfo.miBaoId);

		if(mBeChoose_miBao_Bg.bechooseMiBao.Contains(mMiBaoinfo))
		{
			SameZuHe_IdTitle.SetActive(false);
			MiBao_IsBeChoosed.gameObject.SetActive(true);
		}
		else{
			MiBao_IsBeChoosed.gameObject.SetActive(false);
			if( mBeChoose_miBao_Bg.bechooseMiBao.Count == 0)
			{
				SameZuHe_IdTitle.SetActive(false);
			}
			for(int i = 0; i < mBeChoose_miBao_Bg.bechooseMiBao.Count; i++)
			{
				MiBaoXmlTemp gtMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById (mBeChoose_miBao_Bg.bechooseMiBao[i].miBaoId);

				if(gtMiBaoXmlTemp.zuheId != mMiBaoXmlTemp.zuheId)
				{
					SameZuHe_IdTitle.SetActive(false);
					return;
				}
				SameZuHe_IdTitle.SetActive(true);
			}

		}
	}
	public void init()
	{
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById (mMiBaoinfo.miBaoId);

		//Debug.Log ("mMiBaoinfo.miBaoId = "+mMiBaoinfo.miBaoId);
		//Debug.Log ("mMiBaoXmlTemp.zuheid = "+mMiBaoXmlTemp.zuheId);
		MiBao_pinzhi.spriteName = "pinzhi" + (mMiBaoXmlTemp.pinzhi - 1).ToString ();//mMiBaoXmlTemp.pinzhi.ToString ();
		MiBao_Icon.spriteName = mMiBaoXmlTemp.icon.ToString ();//mMiBaoXmlTemp.icon.ToString ();
		ShowStar ();
	}
	void ShowStar()
	{
		foreach(UISprite sprite in stars)
		{
			Destroy(sprite.gameObject);
		}
		
		stars.Clear();
		
		for(int i = 0; i < mMiBaoinfo.star; i++)
		{
			GameObject spriteObject = (GameObject)Instantiate(MiBao_Star.gameObject);
			
			spriteObject.SetActive(true);
			
			spriteObject.transform.parent = MiBao_Star.transform.parent;
			
			spriteObject.transform.localScale = MiBao_Star.transform.localScale;
			
			spriteObject.transform.localPosition =  new Vector3(i * 20 - (mMiBaoinfo.star - 1) * 10, -40, 0);
			
			UISprite sprite = (UISprite)spriteObject.GetComponent("UISprite");
			
			stars.Add(sprite);
		}
	}
	void OnClick()
	{
		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[7]);
		}

		BeChoose_miBao_Bg mBeChoose_miBao_Bg = IsBeChoosed_Root.GetComponent<BeChoose_miBao_Bg>();
		mBeChoose_miBao_Bg.AddMiBaoPosition ();
		if(mBeChoose_miBao_Bg.bechooseMiBao.Contains(mMiBaoinfo))
		{
			return;
		}
		mBeChoose_miBao_Bg.m_MiBaoinfo = mMiBaoinfo;
		float x = this.transform.localPosition.x + this.transform.parent.parent.transform.localPosition.x;
		float y = this.transform.localPosition.y + this.transform.parent.parent.transform.localPosition.y+190;

		Vector3 miBao_Position = new Vector3 (x,y,0);
		bool IsSave = false;
		mBeChoose_miBao_Bg.Instan_MiBao (miBao_Position,IsSave);
	}
}
