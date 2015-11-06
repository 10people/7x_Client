using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class beChoosedMiBaoTemp : MonoBehaviour {

	List<UISprite> stars = new List<UISprite> ();
	public MibaoInfo mMiBaoinfo;

	public UISprite MiBao_pinzhi;
	public UISprite MiBao_Star;
	public UISprite MiBao_Icon;
	public Transform MiBao_Root;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void init()
	{
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById (mMiBaoinfo.miBaoId);

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
//		//Debug.Log ("删除这个秘宝1，，，，，，，");
//		BeChoose_miBao_Bg mBeChoose_miBao_Bg = MiBao_Root.GetComponent<BeChoose_miBao_Bg>();
//		if(mBeChoose_miBao_Bg.bechooseMiBao.Contains(mMiBaoinfo))
//		{
//			//Debug.Log ("删除这个秘宝2，，，，，，，");
//			for(int i = 0; i < BaiZhanChooseMiBao.Active_MiBao.Count; i++)
//			{
//
//				if(BaiZhanChooseMiBao.Active_MiBao[i].mMiBaoinfo == mMiBaoinfo)
//				{
//
//					JunZhuInfoRet mJunzu_Data =  BaiZhanChooseMiBao.mBaiZhanChooseMiBao.tempInfo1;
//					mJunzu_Data.shengMing -= mMiBaoinfo.shengMing;
//					mJunzu_Data.gongJi -= mMiBaoinfo.gongJi;
//					mJunzu_Data.fangYu -= mMiBaoinfo.fangYu;
//					mJunzu_Data.wqSH -= mMiBaoinfo.wqSH;
//					mJunzu_Data.wqJM -= mMiBaoinfo.wqJM;
//					mJunzu_Data.wqBJ -= mMiBaoinfo.wqBJ;
//					mJunzu_Data.wqRX -= mMiBaoinfo.wqRX;
//					mJunzu_Data.jnSH -= mMiBaoinfo.jnSH;
//					mJunzu_Data.jnJM -= mMiBaoinfo.jnJM;
//					mJunzu_Data.jnBJ -= mMiBaoinfo.jnBJ;
//					mJunzu_Data.jnRX -= mMiBaoinfo.jnRX;
//					//Debug.Log("减掉秘宝属性。。。。。。。。。。。。。。。。");
//
//					mJunzu_Data.zhanLi = Global.getZhanli( mJunzu_Data);
//					BaiZhanChooseMiBao.mBaiZhanChooseMiBao.Request_JunZhuInfo(mJunzu_Data);
//
//				
//
//					float x = BaiZhanChooseMiBao.Active_MiBao[i].gameObject.transform.localPosition.x +
//						BaiZhanChooseMiBao.Active_MiBao[i].gameObject.transform.parent.parent.transform.localPosition.x;
//					float y = BaiZhanChooseMiBao.Active_MiBao[i].gameObject.transform.localPosition.y +
//						BaiZhanChooseMiBao.Active_MiBao[i].gameObject.transform.parent.parent.transform.localPosition.y+190;
//					Vector3 m_pos = new Vector3 (x,y,0);
//					float dis = Vector3.Distance(this.transform.localPosition,m_pos);
//					float mtime = (dis/500.0f);
//					//Debug.Log("2ssssssssssss2" +endpos);
//					//Debug.Log("mtime" +mtime);
//					iTween.MoveTo(this.gameObject, iTween.Hash("position", m_pos, "time",mtime,"islocal",true));
//					mBeChoose_miBao_Bg.bechooseMiBao.Remove(mMiBaoinfo);
//					mBeChoose_miBao_Bg.MiBaochoose.Remove(this.gameObject);
//					mBeChoose_miBao_Bg.SortMiBao();
//					if(BaiZhanChooseMiBao.MiBaolist.Contains(mMiBaoinfo.dbId))
//					{
//						//Debug.Log ("删除这个mMiBaoinfo.dbId = "+mMiBaoinfo.dbId);
//						//.Log ("删除这个mMiBaoinfo.mibaoid = "+mMiBaoinfo.miBaoId);
//						BaiZhanChooseMiBao.MiBaolist.Remove(mMiBaoinfo.dbId);
//					}
//					Destroy(this.gameObject,mtime*2/3);
//
//				}
//			}
//		}
//
//		if (BaiZhanChooseMiBao.MiBaolist.Count == 0)
//		{
//			if(FreshGuide.Instance().IsActive(400210) && TaskData.Instance.m_TaskInfoDic[400210].progress >= 0)
//			{
//				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400210];
//				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[6]);
//			}
//		}
	}
}
