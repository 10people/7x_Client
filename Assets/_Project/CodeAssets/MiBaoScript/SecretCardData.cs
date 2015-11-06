using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class SecretCardData : MonoBehaviour
{
   
    public static SecretCardData m_instance;

    public MibaoInfoResp m_MiBaoInfo;//返回秘宝信息

    public GameObject myScrollVeiw;

    public GameObject myRoot;

    void Awake()
    {
       
		m_instance = this;
    }

    void Start()
    {
		m_MiBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;
		InitData();
    }

    void Update()
    {

    }

    //初始化秘宝数据
    public void InitData()
    {
        if (myScrollVeiw == null)
        {
           // Debug.Log("myScrollVeiw == null");
            return;
        }
		myScrollVeiw.transform.localPosition = new Vector3 (3,0,0);

        SecretPageManeger mSecretPageManeger = myScrollVeiw.GetComponent<SecretPageManeger>();

		mSecretPageManeger.MibaoInfo_ZuHe1.Clear ();

		mSecretPageManeger.MibaoInfo_ZuHe2.Clear ();

		mSecretPageManeger.MibaoInfo_ZuHe3.Clear ();

		mSecretPageManeger.MibaoInfo_ZuHe4.Clear ();

		mSecretPageManeger.MibaoInfo_ZuHe5.Clear ();

		mSecretPageManeger.MibaoInfo_ZuHe6.Clear ();

		mSecretPageManeger.MibaoInfo_ZuHe7.Clear ();
//
//        if (m_MiBaoInfo.mibaoInfo != null)
//        {
//            for (int i = 0; i < m_MiBaoInfo.mibaoInfo.Count; i++)
//            {
//                //Debug.Log("m_MiBaoInfo.mibaoInfo[i].miBaoId"+m_MiBaoInfo.mibaoInfo[i].miBaoId);
//                MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(m_MiBaoInfo.mibaoInfo[i].miBaoId);
//
//                if (mMiBaoXmlTemp.zuheId == 1)
//                {
//                    mSecretPageManeger.MibaoInfo_ZuHe1.Add(m_MiBaoInfo.mibaoInfo[i]);
//                }
//                if (mMiBaoXmlTemp.zuheId == 2)
//                {
//                    mSecretPageManeger.MibaoInfo_ZuHe2.Add(m_MiBaoInfo.mibaoInfo[i]);
//                }
//                if (mMiBaoXmlTemp.zuheId == 3)
//                {
//                    mSecretPageManeger.MibaoInfo_ZuHe3.Add(m_MiBaoInfo.mibaoInfo[i]);
//                }
//                if (mMiBaoXmlTemp.zuheId == 4)
//                {
//                    mSecretPageManeger.MibaoInfo_ZuHe4.Add(m_MiBaoInfo.mibaoInfo[i]);
//                }
//                if (mMiBaoXmlTemp.zuheId == 5)
//                {
//                    mSecretPageManeger.MibaoInfo_ZuHe5.Add(m_MiBaoInfo.mibaoInfo[i]);
//                }
//                if (mMiBaoXmlTemp.zuheId == 6)
//                {
//                    mSecretPageManeger.MibaoInfo_ZuHe6.Add(m_MiBaoInfo.mibaoInfo[i]);
//                }
//                if (mMiBaoXmlTemp.zuheId == 7)
//                {
//                    mSecretPageManeger.MibaoInfo_ZuHe7.Add(m_MiBaoInfo.mibaoInfo[i]);
//                }
//            }
//
//            mSecretPageManeger.Init();
//        }
    }

    //关闭秘宝界面
    public void CloseFun()
    {
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = false;
        }
        Destroy(myRoot);
    }

//    void OnDestroy()
//    {
//        SocketTool.UnRegisterMessageProcessor(this);
//    }
}
