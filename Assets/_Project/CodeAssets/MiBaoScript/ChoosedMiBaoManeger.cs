using UnityEngine;
using System.Collections;
using qxmobile.protobuf;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;

public class ChoosedMiBaoManeger : MonoBehaviour
{

    [HideInInspector]
    public Vector3 position1;
    [HideInInspector]
    public Vector3 position2;
    [HideInInspector]
    public Vector3 position3;
    [HideInInspector]
    public GameObject IconSamplePrefab;
    public List<Vector3> poses = new List<Vector3>();
    public List<GameObject> MiBaochoose = new List<GameObject>();

    public List<MibaoInfo> bechooseMiBao = new List<MibaoInfo>();//被选择的秘宝

    //public GameObject MiBapTemp;
    public static ChoosedMiBaoManeger mChoosedMiBaoManeger;
    [HideInInspector]
    public MibaoInfo mibaoin_bechoosed;
    public UILabel SkillLabel;
    public UISprite skillIcom;
    public GameObject enebleSkill;
    public UILabel ZhanLinumber;
    int[] m_zuheId = new int[3];
    public int ZhanLi = 0;
    //public JunZhuInfoRet Junzu_Data;

    public GameObject numAddObj;//数字

    void Awake()
    {
        mChoosedMiBaoManeger = this;
    }
    void Start()
    {
        //	Junzu_Data = (JunZhuInfoRet)JunZhuData.Instance ().m_junzhuInfo.Public_MemberwiseClone();
        //		MiBaochoose.Clear ();
        //		position1 = new Vector3(-150,0,0);
        //		position2 = new Vector3(0,0,0);
        //		position3 = new Vector3(150,0,0);
        //		//ZhanLi = JunZhuData.Instance ().m_junzhuInfo.zhanLi;
        //		poses.Clear ();
        //		poses.Add (position1);
        //		poses.Add (position2);
        //		poses.Add (position3);
    }
    void Update()
    {
        ZhanLinumber.text = ZhanLi.ToString();

        if (bechooseMiBao.Count >= 3)
        {
            int letterPinz = 5;

            for (int i = 0; i < bechooseMiBao.Count; i++)
            {
                MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(bechooseMiBao[i].miBaoId);
                //Debug.Log("mMiBaoXmlTemp"+mMiBaoXmlTemp.pinzhi);
                if (mMiBaoXmlTemp.pinzhi < letterPinz)
                {
                    letterPinz = mMiBaoXmlTemp.pinzhi;
                }
                m_zuheId[i] = mMiBaoXmlTemp.zuheId;
            }
            if (m_zuheId[0] == m_zuheId[1] && m_zuheId[1] == m_zuheId[2])
            {
                //技能被激活
                //				MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(bechooseMiBao[0].miBaoId);
                SkillLabel.text = "";//技能名字  需要读表
                enebleSkill.SetActive(false);
                skillIcom.gameObject.SetActive(true);
                //需要计算个最低品质
                //				MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi(mMiBaoXmlTemp.zuheId,letterPinz);
                //				SkillTemplate mSkillTemplate = SkillTemplate.getSkillTemplateById(mMiBaoSkillTemp.skill);
                //skillIcom.spriteName = mSkillTemplate.skillType.ToString();//icon 读表的。。。。。。。。。
            }
            else
            {
                SkillLabel.text = "未激活";
                enebleSkill.SetActive(true);
                skillIcom.gameObject.SetActive(false);
            }
        }
        else
        {
            SkillLabel.text = "未激活";
            enebleSkill.SetActive(true);
            skillIcom.gameObject.SetActive(false);
        }
    }

    public void SortMiBao()
    {
        if (MiBaochoose.Count == 0) return;
        for (int i = 0; i < MiBaochoose.Count; i++)
        {
            TweenPosition.Begin(MiBaochoose[i], 0.2f, poses[i]);
        }
    }
	public void InitSaveMiBao()
	{
		if (MiBaochoose.Count == 0) return;
		for (int i = 0; i < MiBaochoose.Count; i++)
		{
			MiBaochoose[i].transform.localPosition =  poses[i];
		}
	}
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = p_object as GameObject;
        }

        GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
        iconSampleObject.transform.parent = transform;
        var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
        choosemiBaoTemp mchoosemiBaoTemp = iconSampleObject.AddComponent<choosemiBaoTemp>();
        mchoosemiBaoTemp.m_IconSampleManager = iconSampleManager;

        MiBaochoose.Add(iconSampleObject);
        MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(mibaoin_bechoosed.miBaoId);
        bechooseMiBao.Add(mibaoin_bechoosed);
        //EnterLoading.m_EnterLoading.MiBaoidList.Add(mibaoin_bechoosed.dbId);
        mchoosemiBaoTemp.mibao_bechoosed = mibaoin_bechoosed;
        mchoosemiBaoTemp.numAddObj = numAddObj;

        mchoosemiBaoTemp.SetIcon(miBaoPos,isSaveMiBao);

        //Play tween.

		if(isSaveMiBao)
		{
			InitSaveMiBao();
		}
		else{
			float dis = Vector3.Distance(MiBaochoose[MiBaochoose.Count - 1].transform.localPosition, poses[MiBaochoose.Count - 1]);
			float m_v = 500.0f;
			float mTime = dis / m_v;

			Hashtable flyTo = new Hashtable();
			
			flyTo.Add("time", mTime);
			flyTo.Add("position", poses[MiBaochoose.Count - 1]);
			flyTo.Add("easetype", iTween.EaseType.easeOutQuart);
			
			if (!isSaveMiBao)
			{
				switch (MiBaochoose.Count)
				{
				case 1:

					flyTo.Add("oncomplete", "GetAnimInfo1");

					break;

				case 2:

					flyTo.Add("oncomplete", "GetAnimInfo2");

					break;

				case 3:

					flyTo.Add("oncomplete", "GetAnimInfo3");

					break;
				}

				flyTo.Add("oncompletetarget", gameObject);
			}
			flyTo.Add("islocal", true);
			iTween.MoveTo(iconSampleObject, flyTo);
		}
//        JunZhuInfoRet Junzu_Data = MiBaoShangZhen.mMiBaoShangzhen.GJunzu_Data;
//
//		Junzu_Data.shengMing += mibaoin_bechoosed.shengMing;
//		Junzu_Data.gongJi += mibaoin_bechoosed.gongJi;
//		Junzu_Data.fangYu += mibaoin_bechoosed.fangYu;
//		Junzu_Data.wqSH += mibaoin_bechoosed.wqSH;
//		Junzu_Data.wqJM += mibaoin_bechoosed.wqJM;
//		Junzu_Data.wqBJ += mibaoin_bechoosed.wqBJ;
//		Junzu_Data.wqRX += mibaoin_bechoosed.wqRX;
//		Junzu_Data.jnSH += mibaoin_bechoosed.jnSH;
//		Junzu_Data.jnJM += mibaoin_bechoosed.jnJM;
//		Junzu_Data.jnBJ += mibaoin_bechoosed.jnBJ;
//		Junzu_Data.jnRX += mibaoin_bechoosed.jnRX;
//
//        ZhanLi = Global.getZhanli(Junzu_Data);
    }

    private Vector3 miBaoPos;
    private bool isSaveMiBao;

    public void InstanceMiBao(Vector3 getposition, bool isSave)
    {
        //Junzu_Data = (JunZhuInfoRet)JunZhuData.Instance ().m_junzhuInfo.Public_MemberwiseClone();

//		Debug.Log ("开始初始化秘宝了");

        position1 = new Vector3(-150, 15, 0);
        position2 = new Vector3(0, 15, 0);
        position3 = new Vector3(150, 15, 0);
        poses.Clear();
        poses.Add(position1);
        poses.Add(position2);
        poses.Add(position3);
        //		if(FreshGuide.Instance().IsActive(100009)&& TaskData.Instance.m_TaskInfoDic[100009].progress >= 0)
        //		{
        //			if(TaskData.Instance.m_TaskInfoDic[100009].m_listYindaoShuju.Count != 0)
        //			{
        //				
        //				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100009];
        //				Debug.Log( "task1:" + tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
        //				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //			}
        //			
        //		}
        
        if (MiBaochoose.Count < 3)
        {
            miBaoPos = getposition;
            isSaveMiBao = isSave;

            if (IconSamplePrefab == null)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
            }
            else
            {
                WWW temp = null;
                OnIconSampleLoadCallBack(ref temp, null, IconSamplePrefab);
            }
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MI_BAO_REMIND_MI_BAO),
                                    ResLoaded);
        }
    }

    public static void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject Remind = (GameObject)Instantiate(p_object);

        GameObject sup = GameObject.Find("Mapss");

        Remind.transform.parent = sup.transform;

        Remind.transform.localScale = new Vector3(1, 1, 1);

        Remind.transform.localPosition = new Vector3(0, 0, 0);
    }

    void GetAnimInfo1()
    {
        //实例化数字弹出
        GameObject numObj = (GameObject)Instantiate(numAddObj);

        numObj.SetActive(true);
        numObj.transform.parent = this.transform;

        numObj.transform.localPosition = position1 + new Vector3(0, 50, 0);
        numObj.transform.localScale = numAddObj.transform.localScale;

		Vector3 pos = position1 + new Vector3(0, 100, 0);

        MiBaoNumAddFly numFly = numObj.GetComponent<MiBaoNumAddFly>();
        numFly.GetMibao(1, mibaoin_bechoosed, pos);
    }

	void GetAnimInfo2()
	{
		//实例化数字弹出
		GameObject numObj = (GameObject)Instantiate(numAddObj);
		
		numObj.SetActive(true);
		numObj.transform.parent = this.transform;
		
		numObj.transform.localPosition = position2 + new Vector3(0, 50, 0);
		numObj.transform.localScale = numAddObj.transform.localScale;
		
		Vector3 pos = position2 + new Vector3(0, 100, 0);
		
		MiBaoNumAddFly numFly = numObj.GetComponent<MiBaoNumAddFly>();
		numFly.GetMibao(1, mibaoin_bechoosed, pos);
	}

	void GetAnimInfo3()
	{
		//实例化数字弹出
		GameObject numObj = (GameObject)Instantiate(numAddObj);
		
		numObj.SetActive(true);
		numObj.transform.parent = this.transform;
		
		numObj.transform.localPosition = position3 + new Vector3(0, 50, 0);
		numObj.transform.localScale = numAddObj.transform.localScale;
		
		Vector3 pos = position3 + new Vector3(0, 100, 0);
		
		MiBaoNumAddFly numFly = numObj.GetComponent<MiBaoNumAddFly>();
		numFly.GetMibao(1, mibaoin_bechoosed, pos);
	}
}






































