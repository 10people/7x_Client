using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class My_DamageRank : MonoBehaviour {

	//public UILabel MyRank;
	private int mrank;
	public MaxDamageRankResp mRankList;

	public GameObject mRanktemp;

	public GameObject UIgrid;

	public GameObject MyRankBtn;

	public GameObject m_Scollview;

	mFixUniform mmFixUniform;

	public float MRankPosition;

	float Relative_MRankPosition_Y;

	float Dis = 70;
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		Relative_MRankPosition_Y = MRankPosition -8;
	}
	public void Init()
	{

		MyRankBtn.SetActive (false);

		if(mRankList.damageInfo == null  || mRankList.damageInfo.Count == 0)
		{
			//Debug.Log("_____排行榜 = null");
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
			mm__RankItem.Init();
		}
	}

	public void Showmyrank()
	{
//		if(mRankList.damageInfo.Count <= 5)
//		{
//			return;
//		}
		mmFixUniform = UIgrid.GetComponent<mFixUniform>();

		if(m_Scollview.transform.localPosition.y + MRankPosition  > 5 )
		{

			Debug.Log("11");
			float S_y = m_Scollview.transform.localPosition.y +MRankPosition;
			Debug.Log("S_y = "+S_y);
			mmFixUniform.offset = new Vector3(0,-S_y+70,0);

			mmFixUniform.enabled = true;

			StopCoroutine("Closescripte");

			StartCoroutine("Closescripte");
		}
		else if(m_Scollview.transform.localPosition.y + MRankPosition  < -365)
		{

			float X_y = m_Scollview.transform.localPosition.y + MRankPosition;

			mmFixUniform.offset = new Vector3(0,-X_y-360,0);

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
	public void Close()
	{
		HY_UIManager.Instance().ShowOrClose ();
		Destroy (this.gameObject);
	}
}
