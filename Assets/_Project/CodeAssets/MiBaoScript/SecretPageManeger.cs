using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class SecretPageManeger : MonoBehaviour {

	public static List<MiBaodelIfon> localMiBao = new List<MiBaodelIfon> ();//相同组合id的秘宝list

	public static List<MiBaodelIfon> localSkill = new List<MiBaodelIfon> ();//用于激活技能的秘宝list

	public List<MibaoInfo> MibaoInfo_ZuHe1 = new List<MibaoInfo> ();
	public List<MibaoInfo> MibaoInfo_ZuHe2 = new List<MibaoInfo> ();
	public List<MibaoInfo> MibaoInfo_ZuHe3 = new List<MibaoInfo> ();
	public List<MibaoInfo> MibaoInfo_ZuHe4 = new List<MibaoInfo> ();
	public List<MibaoInfo> MibaoInfo_ZuHe5 = new List<MibaoInfo> ();
	public List<MibaoInfo> MibaoInfo_ZuHe6 = new List<MibaoInfo> ();
	public List<MibaoInfo> MibaoInfo_ZuHe7 = new List<MibaoInfo> ();

	public List<List<MibaoInfo>> MibaoInfo_ZuHe = new List<List<MibaoInfo>>();//所有秘宝list的组合list

	Transform MiBaoTemp;

	private UIPanel scrollor;

	private int Secret_num;//秘宝组合个数

	public GameObject Secret_Temp;//秘宝组合obj
	public GameObject mSecretRoot;//秘宝Grid
	public GameObject RightBtn;
	public GameObject LeftBtn;

	public List<GameObject> MiBaoTempOBJ = new List<GameObject> ();
 

	void Start () 
	{
		scrollor = this.gameObject.GetComponent<UIPanel>();

		LeftBtn.SetActive(false);
		RightBtn.SetActive(false);
		//Init ();
	}

	//实例化秘宝页面
	public void Init ()
	{
		MibaoInfo_ZuHe.Clear ();

		MibaoInfo_ZuHe.Add (MibaoInfo_ZuHe1);
		MibaoInfo_ZuHe.Add (MibaoInfo_ZuHe2);
		MibaoInfo_ZuHe.Add (MibaoInfo_ZuHe3);
		MibaoInfo_ZuHe.Add (MibaoInfo_ZuHe4);
		MibaoInfo_ZuHe.Add (MibaoInfo_ZuHe5);
		MibaoInfo_ZuHe.Add (MibaoInfo_ZuHe6);
		MibaoInfo_ZuHe.Add (MibaoInfo_ZuHe7);

		Secret_num = MibaoInfo_ZuHe.Count;

//		foreach( MiBaodelIfon mMiBaodelIfon in localMiBao)
//		{
//			Destroy(mMiBaodelIfon.gameObject);
//		}
		localMiBao.Clear ();
		localSkill.Clear ();
		foreach (GameObject mbObj in MiBaoTempOBJ)
		{
			Destroy(mbObj);
		}
		MiBaoTempOBJ.Clear ();

		for(int i = 0; i < MibaoInfo_ZuHe.Count; i++)
		{
			GameObject mSecret = Instantiate (Secret_Temp) as GameObject;

			mSecret.SetActive(true);

			mSecret.transform.parent = Secret_Temp.transform.parent;
			//mSecret.transform.localPosition = Secret_Temp.transform.localPosition;
			mSecret.transform.localPosition = new Vector3(-300+i*150,0,0);
			mSecret.transform.localScale = Secret_Temp.transform.localScale;

			Transform MiBaoSkill = mSecret.transform.FindChild("Skill1");
			MiBaodelIfon SkillIfon = MiBaoSkill.GetComponent<MiBaodelIfon> ();
			SkillIfon.MiBaoZuHeId = i + 1;
			SkillIfon.MibaoInfo_ZH = MibaoInfo_ZuHe[i];
			//Debug.Log("MibaoInfo_ZuHe[i].count"+MibaoInfo_ZuHe[i].Count);

			int minPinZhi = 5;//技能等级限定
			int Activenum = 0;//同组中秘宝个数

			for (int j = 0; j < MibaoInfo_ZuHe[i].Count; j++)
			{
				if (MibaoInfo_ZuHe[i][j].level > 0)
				{
					Activenum += 1;
				}

				MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(MibaoInfo_ZuHe[i][j].miBaoId);

				if(mMiBaoXmlTemp.pinzhi < minPinZhi)
				{
					minPinZhi = mMiBaoXmlTemp.pinzhi;  
				}

				switch (j)
				{
					case 0:

						MiBaoTemp = mSecret.transform.FindChild("one");

						break;

					case 1:

						MiBaoTemp = mSecret.transform.FindChild("two");

						break;

					case 2:

						MiBaoTemp = mSecret.transform.FindChild("three");

						break;

					default:break;
				}

				//Debug.Log("minPinZhi   "+minPinZhi);
				MiBaodelIfon mMiBaodelIfon = MiBaoTemp.GetComponent<MiBaodelIfon> ();

				mMiBaodelIfon.m_mibaoinfo = MibaoInfo_ZuHe[i][j];
				mMiBaodelIfon.MibaoInfo_ZH = MibaoInfo_ZuHe[i];
				mMiBaodelIfon.MiBaoZuHeId = i+1;
				mMiBaodelIfon.Init();
				localMiBao.Add (mMiBaodelIfon);
			}

			SkillIfon.MiBaoPinZhi = minPinZhi;
			
			SkillIfon.Activenums = Activenum;
			SkillIfon.InitSkill();
			localSkill.Add(SkillIfon);
			MiBaoTempOBJ.Add(mSecret);
		}
		//ShowGuid ();
		//mSecretRoot.GetComponent<UIGrid> ().repositionNow = true;
	}
	void ShowGuid()
	{
		//Debug.Log ("FreshGuide.Instance().IsActive(300200) = " +FreshGuide.Instance().IsActive(300200));
		//Debug.Log ("TaskData.Instance.m_TaskInfoDic[300200].progress = " +TaskData.Instance.m_TaskInfoDic[300200].progress);

		if(FreshGuide.Instance().IsActive(300200)&& TaskData.Instance.m_TaskInfoDic[300200].progress < 0)
		{
			//Debug.Log("进入PVE 第一个任务 点击右边按钮翻页");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[300200];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		}

	}

	void FixedUpdate () {

		scrollor.clipOffset = new Vector2(-scrollor.gameObject.transform.localPosition.x, 0);
		int Move_x =  Secret_num*150 -750+(int)scrollor.cachedGameObject.transform.localPosition.x;
		if(MibaoInfo_ZuHe.Count<= 5)
		{
			LeftBtn.SetActive(false);
			RightBtn.SetActive(false);
			return;
		}
		if(Move_x <= 10)
		{
			RightBtn.SetActive(false);
			LeftBtn.SetActive(true);
		}
		else if(scrollor.cachedGameObject.transform.localPosition.x >= -10)
		{
			RightBtn.SetActive(true);
			LeftBtn.SetActive(false);
		}else{
			RightBtn.SetActive(true);
			LeftBtn.SetActive(true);
		}

	}

	public void RightMove()
	{
		StartCoroutine( StartMove (1,Secret_num));
	}
	public void LeftMove()
	{
		StartCoroutine( StartMove (-1,Secret_num));
	}
	IEnumerator StartMove(int i,int j)
	{
		int moveX ;
		if(i == 1)//向右移动
		{

			moveX = j*150 -750+(int)scrollor.cachedGameObject.transform.localPosition.x;

			if(moveX > 750)
			{
				SpringPanel.Begin (scrollor.cachedGameObject,
				                   new Vector3(scrollor.cachedGameObject.transform.localPosition.x - 750, 0f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				//Debug.Log("moveX" +moveX);
				//Debug.Log("scrollor.cacx" +scrollor.cachedGameObject.transform.localPosition.x);
				SpringPanel.Begin (scrollor.cachedGameObject,
				                   new Vector3(scrollor.cachedGameObject.transform.localPosition.x - moveX, 0f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
		}
		else
		{
			moveX = (int)(-scrollor.cachedGameObject.transform.localPosition.x);
			if(moveX > 750)
			{
				SpringPanel.Begin (scrollor.cachedGameObject,
				                   new Vector3(scrollor.cachedGameObject.transform.localPosition.x + 750, 0f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				SpringPanel.Begin (scrollor.cachedGameObject,
				                   new Vector3(scrollor.cachedGameObject.transform.localPosition.x + moveX, 0f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
		}
	}
}
