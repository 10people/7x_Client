using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class GetSaodongRewards : MonoBehaviour {

	public UILabel Exp;
	public UILabel Money;
	public GameObject RewardTemp_Root;
	public GameObject RewardTemp;
	public GameObject RewGameobj;
	public int m_Exp = 0;
	public int m_Money = 0;
	int allequps = 0;
	int page;
	public UILabel mSaoDangTime;


	public PveSaoDangRet m_tempInfo ;
	void Start ()
	{


	}
	public void init()
	{
	//	showExp_And_Money ();
		ShowGetRewards ();
	}
	void showExp_And_Money()
	{
		Exp.text = m_Exp.ToString();
		Money.text = m_Money.ToString();

	}
	void ShowGetRewards()
	{
		StartCoroutine (StartMovePlane());
	}
	int mtimes = 0;
	IEnumerator StartMovePlane( )
	{

		for(int i = 0; i< m_tempInfo.awards.Count; i++)
		{
			allequps = 0;
			GameObject tempOjbectRoot = Instantiate(RewardTemp_Root) as GameObject;
			tempOjbectRoot.name = "Root+"+i.ToString();
			tempOjbectRoot.transform.parent = RewardTemp_Root.transform.parent;
			tempOjbectRoot.transform.localScale = RewardTemp_Root.transform.localScale;
			tempOjbectRoot.transform.localPosition = new Vector3(0,-200,0);
			m_Exp += m_tempInfo.awards[i].exp;
			m_Money += m_tempInfo.awards[i].money;
			mtimes += 1;

			Exp.text = m_Exp.ToString();
			Money.text = m_Money.ToString();
			mSaoDangTime.text = mtimes.ToString();
			if(m_tempInfo.awards[i].awardItems != null)
			{
				//Debug.Log("m_tempInfo.awards[i].awardItems.Count = "+m_tempInfo.awards[i].awardItems.Count);
				for(int j = 0 ; j < m_tempInfo.awards[i].awardItems.Count; j++)
				{
					GameObject tempOjbect = Instantiate(RewardTemp) as GameObject;
					tempOjbect.SetActive(true);
					tempOjbect.transform.parent = tempOjbectRoot.transform;
					tempOjbect.transform.localScale = RewardTemp.transform.localScale;
					Equps my_equps = (Equps)tempOjbect.GetComponent("Equps");
					my_equps.mSaoDangAwarditem = m_tempInfo.awards[i].awardItems[j];
					my_equps.init();
					allequps += 1;
				}
				page = (int )(allequps / 4);
				tempOjbectRoot.GetComponent<UIGrid> ().repositionNow = true;
				StartCoroutine (AwardMove(page,tempOjbectRoot));
			}
			else
			{
				//Debug.Log ("m_tempInfo.awards[i].awardItems = null" );
				StartCoroutine (AwardMove(page,tempOjbectRoot));
			}
		
			yield return new WaitForSeconds(1.5f);
		}

	}

	IEnumerator AwardMove(int p, GameObject mroot)
	{

		//Debug.Log ("P = " +p);
		Vector3 newpos = new Vector3(0,150*(p+1),0);
		if(mtimes >= m_tempInfo.awards.Count)
		{
			newpos = new Vector3(0,0,0);
		}
		TweenPosition.Begin(mroot,2.5f,newpos);

		if(mtimes >= m_tempInfo.awards.Count)
		{
			yield return new WaitForSeconds(3.5f);
			Saodangfinished ();
		}

	}

	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object)as GameObject;
		
		GameObject obj = GameObject.Find ("Mapss");
		
		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		
		tempOjbect.transform.localPosition = new Vector3 (1,1,1);
		
		Destroy(this.gameObject);
	}
	void Saodangfinished()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_SAO_DANG_DONE), LoadResourceCallback);

	}

}
