using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class SaoDangManeger : MonoBehaviour {

	public PveSaoDangRet m_PveSaoDangRet ;

	public GameObject SaoDangTemp;

	private float mDistance = -230;

	public GameObject mPanle;

	public GameObject ConfirmBtn;

	public GameObject SaoDangfinshed;

	public int SaodangType ;// 1 为pve 2为游侠
	void Start () {
	
	}
	

	void Update () {
	
	}

	public void Init()
	{

		StartCoroutine ( CreateAwardTemp() );
	}

	IEnumerator CreateAwardTemp()
	{
		if(m_PveSaoDangRet == null ||m_PveSaoDangRet.awards.Count == 0)
		{
			Debug.Log("m_PveSaoDangRet == null");
			SaoDangfinshed.SetActive(true);
		}
		for (int i = 0; i< m_PveSaoDangRet.awards.Count; i++)
		{
			yield return new WaitForSeconds(0.2f);

			GameObject mSaoDangTemp = Instantiate(SaoDangTemp) as GameObject;

			mSaoDangTemp.SetActive(true);
			
			mSaoDangTemp.transform.parent = SaoDangTemp.transform.parent;
			
			mSaoDangTemp.transform.localScale = SaoDangTemp.transform.localScale;
			
			mSaoDangTemp.transform.localPosition = new Vector3(0 , mDistance*i, 0);

			SaoDangAwardTemp mSaoDangAwardTemp = mSaoDangTemp.GetComponent<SaoDangAwardTemp>();

			mSaoDangAwardTemp.m_tempInfo = m_PveSaoDangRet.awards[i];

			mSaoDangAwardTemp.mSaoDangTimeNum = i+1;
//			Debug.Log("m_PveSaoDangRet.awards[i].awardItems.Count = "+m_PveSaoDangRet.awards[i].awardItems.Count);

			mSaoDangAwardTemp.Init();

			float mTime = 0;

			if(m_PveSaoDangRet.awards[i].awardItems == null||m_PveSaoDangRet.awards[i].awardItems.Count == 0)
			{
				mTime = 1.0f;
			}
			else{

				mTime = m_PveSaoDangRet.awards[i].awardItems.Count*0.4f+0.6f;
			}

			yield return new WaitForSeconds(mTime);

			PanleMove(i+1, mSaoDangTemp.transform.localPosition);
		}
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
	}

	void PanleMove(int mSaodangTime, Vector3 UiPosition)
	{

		Vector3 v1 = mPanle.transform.localPosition;

		Vector3 v2 = v1 + new Vector3 (0 ,-mDistance, 0);

		//Debug.Log ("v2 = " +v2);

		if(mSaodangTime == m_PveSaoDangRet.awards.Count)
		{
			SaoDangfinshed.SetActive(true);

			SaoDangfinshed.transform.localPosition  = UiPosition + new Vector3 (0 ,mDistance, 0);

			ConfirmBtn.SetActive(true);
		}

		iTween.MoveTo(mPanle, iTween.Hash("position", v2,	"easeType", iTween.EaseType.easeInOutQuad,"time",0.4f,"islocal",true));

	}

	public void ConfrimBtn()
	{
//		if(SaodangType == 1)
//		{
//			PveLevelUImaneger.mPveLevelUImaneger.IsSaodang = false;
//			
//			PveLevelUImaneger.mPveLevelUImaneger.ShowEffect ();
//
//			PveLevelUImaneger.mPveLevelUImaneger.ISCanSaodang = true;
//		}

		Destroy (this.gameObject);
	}
}










