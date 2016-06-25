using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class My_DamageRank : MonoBehaviour {

	public List<UIEventListener> BtnList = new List<UIEventListener>();

	private int mrank;
	public MaxDamageRankResp mRankList;

	public GameObject mRanktemp;

	public GameObject UIgrid;

	public GameObject MyRankBtn;

	public GameObject AllBtn;

	public GameObject m_Scollview;

	mFixUniform mmFixUniform;

	public float MRankPosition;

	float Relative_MRankPosition_Y;

	float Dis = 45;

	public UILabel my_RankNumber;

	float mScrollView_y;

	[HideInInspector]public int m_levelid;
	void Awake()
	{
		BtnList.ForEach(item => SetBtnMoth(item));

	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	public void BtnManagerMent(GameObject mbutton)
	{
		Debug.Log("mbutton.name = " + mbutton.name);
		switch (mbutton.name)
		{
		case "Button_1": // 查看w d 
			Showmyrank();
			break;
		case "Button_2": // 查看信息All
			LookAllRank();
			MyRankBtn.SetActive(true);
			AllBtn.SetActive(false);
			break;
		case "Button_Close": // 
			Close();
			break;
		default:
			break;
		}
	}
	void Start () {
	
	}
	

	void Update () {
	
		//Relative_MRankPosition_Y = MRankPosition -8;
	}
	public void Init()
	{
		if(mRankList.damageInfo == null  || mRankList.damageInfo.Count == 0)
		{
			Debug.Log("_____排行榜 = null");
			return;
		}
		for(int i = 0 ; i < mRankList.damageInfo.Count; i ++)
		{
			GameObject m_mRanktemp = Instantiate(mRanktemp) as GameObject;
			
			m_mRanktemp.SetActive(true);
			
			m_mRanktemp.transform.parent = mRanktemp.transform.parent;
			
			m_mRanktemp.transform.localPosition = new Vector3(0,-i*Dis,0);
			
			m_mRanktemp.transform.localScale = Vector3.one;
			
			RankItem mm__RankItem = m_mRanktemp.GetComponent<RankItem>();
		
			mm__RankItem.mDamageInfo = mRankList.damageInfo[i];

			if(mRankList.damageInfo[i].junZhuName == JunZhuData.Instance().m_junzhuInfo.name)
			{
				MyRankBtn.SetActive(true);

				mrank = mRankList.damageInfo[i].rank;

				MRankPosition = m_mRanktemp.transform.localPosition.y;

				mm__RankItem.iSMyself = true;
			}
			else
			{
				mm__RankItem.iSMyself = false;
			}
			mm__RankItem.Init(m_levelid);
		}
		if(mrank <= 0)
		{
			my_RankNumber.text ="无";
		}
		else{
			my_RankNumber.text = mrank.ToString ();
		}
		mScrollView_y = m_Scollview.gameObject.transform.localPosition.y;
	}

	public void Showmyrank()
	{
		if(mrank <= 0)
		{
			string pop = "当前无伤害排名";
			ClientMain.m_UITextManager.createText(pop);
			return;
		}
		if(mRankList.damageInfo.Count <= 6)
		{
			return;
		}
		MyRankBtn.SetActive(false);
		AllBtn.SetActive(true);

		mmFixUniform = UIgrid.GetComponent<mFixUniform>();

		if(m_Scollview.transform.localPosition.y + MRankPosition  > mScrollView_y +45 )
		{

			Debug.Log("11");
			float S_y = m_Scollview.transform.localPosition.y +MRankPosition - mScrollView_y;
			Debug.Log("S_y = "+S_y);
			mmFixUniform.offset = new Vector3(0,-S_y+45,0);

			mmFixUniform.enabled = true;

			StopCoroutine("Closescripte");

			StartCoroutine("Closescripte");
		}
		else if(m_Scollview.transform.localPosition.y + MRankPosition  < -280+mScrollView_y)
		{

			float X_y = m_Scollview.transform.localPosition.y + MRankPosition -mScrollView_y;

			mmFixUniform.offset = new Vector3(0,-X_y-280,0);

			mmFixUniform.enabled = true;

			StopCoroutine("Closescripte");

			StartCoroutine("Closescripte");
		}
		else
		{
			return;
		}
	
	}
	IEnumerator Closescripte()
	{
		yield return new WaitForSeconds (0.50f);

		 mmFixUniform.enabled = false;
	}

	void LookAllRank()
	{
		m_Scollview.transform.localPosition = new Vector3 (0,mScrollView_y,0);
	}
	public void Close()
	{
		Destroy (this.gameObject);
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
	}
}
