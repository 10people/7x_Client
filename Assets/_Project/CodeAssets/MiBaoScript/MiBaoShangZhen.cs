using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class MiBaoShangZhen : MonoBehaviour {

	public	MibaoInfoResp m_MiBaoInfo;
	public GameObject myScrollVeiw; 
	public  JunZhuInfoRet GJunzu_Data;
	public static MiBaoShangZhen mMiBaoShangzhen;

	public void InitData()
	{
		mMiBaoShangzhen = this;
		//ShowGruid ();
		if(myScrollVeiw == null)
		{
			//Debug.Log("myScrollVeiw == null");
			return ;
		}
		m_MiBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;
		//Debug.Log("_MiBaoInfo.mibaoInfo.count   "+m_MiBaoInfo.mibaoInfo.Count);
		MiBaoShangZhenScrollView mMiBaoShangZhenScrollView = myScrollVeiw.GetComponent<MiBaoShangZhenScrollView>();
//		if(m_MiBaoInfo.mibaoInfo != null)
//		{
//			for(int i = 0; i <m_MiBaoInfo.mibaoInfo.Count; i++ )
//			{
//				//Debug.Log("m_MiBaoInfo.mibaoInfo[i].miBaoId"+m_MiBaoInfo.mibaoInfo[i].miBaoId);
//				MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(m_MiBaoInfo.mibaoInfo[i].miBaoId);
//				if(mMiBaoXmlTemp.zuheId == 1)
//				{
//					mMiBaoShangZhenScrollView.MibaoInfo_ZuHe1.Add(m_MiBaoInfo.mibaoInfo[i]);
//				}
//				if(mMiBaoXmlTemp.zuheId == 2)
//				{
//					mMiBaoShangZhenScrollView.MibaoInfo_ZuHe2.Add(m_MiBaoInfo.mibaoInfo[i]);
//				}
//				if(mMiBaoXmlTemp.zuheId == 3)
//				{
//					mMiBaoShangZhenScrollView.MibaoInfo_ZuHe3.Add(m_MiBaoInfo.mibaoInfo[i]);
//				}
//				if(mMiBaoXmlTemp.zuheId == 4)
//				{
//					mMiBaoShangZhenScrollView.MibaoInfo_ZuHe4.Add(m_MiBaoInfo.mibaoInfo[i]);
//				}
//				if(mMiBaoXmlTemp.zuheId == 5)
//				{
//					mMiBaoShangZhenScrollView.MibaoInfo_ZuHe5.Add(m_MiBaoInfo.mibaoInfo[i]);
//				}
//				if(mMiBaoXmlTemp.zuheId == 6)
//				{
//					mMiBaoShangZhenScrollView.MibaoInfo_ZuHe6.Add(m_MiBaoInfo.mibaoInfo[i]);
//				}
//				if(mMiBaoXmlTemp.zuheId == 7)
//				{
//					mMiBaoShangZhenScrollView.MibaoInfo_ZuHe7.Add(m_MiBaoInfo.mibaoInfo[i]);
//				}
//			}
//			mMiBaoShangZhenScrollView.Init();
//		}
	}

}
