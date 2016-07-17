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

	private float mDistance = -148;

	public GameObject mPanle;

	public GameObject Skip;
	public GameObject SaoDangAgain;
	public GameObject ConfirmBtn;

	public GameObject SaoDangfinshed;

	public int SaodangType ;// 1 为pve 2为游侠
	public int SaodangTime ;
	public UILabel BtnLabel;
	public delegate void SaoDangFinshDo();

	private SaoDangFinshDo mSaoDangFinshDo;

	private int myindaoid;

	public void Init(int yindaoid = 0, SaoDangFinshDo mFunction = null)
	{
		if(yindaoid > 0 )
		{
			ShowSaoDangYinDao();
			myindaoid = yindaoid;
		}

		mSaoDangFinshDo = mFunction;
		Global.m_isZhanli = true;
		StartCoroutine ( CreateAwardTemp() );
	}
	void ShowSaoDangYinDao()
	{
		ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100404];
		UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[7]);
	}
	float mTime = 0;
	IEnumerator CreateAwardTemp()
	{
		if(m_PveSaoDangRet == null ||m_PveSaoDangRet.awards.Count == 0)
		{
			//Debug.Log("m_PveSaoDangRet == null");
			SaoDangfinshed.SetActive(true);
		}
		if (m_PveSaoDangRet.awards.Count > 1) {
			Skip.SetActive (true);
			ConfirmBtn.SetActive (false);
			SaoDangAgain.SetActive (false);
		} else {
			Skip.SetActive (false);
			ConfirmBtn.SetActive (false);
			SaoDangAgain.SetActive (false);
		}
		mTime = 0.80f;
		SaodangTime = m_PveSaoDangRet.awards.Count;
		for (int i = 0; i< m_PveSaoDangRet.awards.Count; i++)
		{
			GameObject mSaoDangTemp = Instantiate(SaoDangTemp) as GameObject;

			mSaoDangTemp.SetActive(true);
			
			mSaoDangTemp.transform.parent = SaoDangTemp.transform.parent;
			
			mSaoDangTemp.transform.localScale = SaoDangTemp.transform.localScale;
			
			mSaoDangTemp.transform.localPosition = new Vector3(0 , 18+mDistance*i, 0);

			SaoDangAwardTemp mSaoDangAwardTemp = mSaoDangTemp.GetComponent<SaoDangAwardTemp>();

			mSaoDangAwardTemp.m_tempInfo = m_PveSaoDangRet.awards[i];

			mSaoDangAwardTemp.mSaoDangTimeNum = i+1;
//			Debug.Log("m_PveSaoDangRet.awards[i].awardItems.Count = "+m_PveSaoDangRet.awards[i].awardItems.Count);

			mSaoDangAwardTemp.Init();

			yield return new WaitForSeconds(mTime);
			if(SaodangTime > 1)
			{
				if(i>0)
				{
					PanleMove(i+1, mSaoDangTemp.transform.localPosition);
				}
			}
			else
			{
				PanleMove(i+1, mSaoDangTemp.transform.localPosition);
			}


		}
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
		if(SaodangTime <= 5)
		{
			BtnLabel.text = "再扫1次";
		}
		else if(SaodangTime == 10)
		{
			BtnLabel.text = "再扫10次";
		}
	}

	void PanleMove(int mSaodangTime, Vector3 UiPosition)
	{
	
		Vector3 v1 = mPanle.transform.localPosition;

		Vector3 v2 = v1 + new Vector3 (0 ,-mDistance, 0);

		//Debug.Log ("v2 = " +v2);

		if(mSaodangTime == m_PveSaoDangRet.awards.Count)
		{
			if(SaodangType == 1)
			{
				Skip.SetActive (false);
				ConfirmBtn.SetActive (true);
				SaoDangAgain.SetActive (true);
				SaoDangfinshed.SetActive(true);
			}
			else
			{
				Skip.SetActive (false);
				ConfirmBtn.SetActive (true);
				ConfirmBtn.transform.localPosition = new Vector3(0,-159,0);
				SaoDangAgain.SetActive (false);
				SaoDangfinshed.SetActive(true);
			}
			int id = 100178;
			SaoDangfinshed.transform.localPosition  = new Vector3 (0 ,-60, 0);
			UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,SaoDangfinshed,EffectIdTemplate.GetPathByeffectId(id));
			StartCoroutine("mCloseEffect");
		}else
		{
			iTween.MoveTo(mPanle, iTween.Hash("position", v2,	"easeType", iTween.EaseType.easeInOutQuad,"time",0.4f,"islocal",true));
		}
	}

	public void SaodangAngainBtn()
	{
		if(SaodangType == 1)
		{
			if(SaodangTime > 5)
			{
				NewPVEUIManager.Instance().SaoDangBtn(10);
			}
			else
			{
				NewPVEUIManager.Instance().SaoDangBtn(1);
			}
		}
	
		ConfrimBtn ();
	}
	public void SkipBtn()
	{
		mTime = 0.0f;
	}
	public void ConfrimBtn()
	{
		Global.m_isZhanli = false;
        TaskData.Instance.IsCanShowComplete = true;
        //FunctionWindowsCreateManagerment.m_IsSaoDangNow = false;
		if(mSaoDangFinshDo != null && myindaoid > 0)
		{
			mSaoDangFinshDo();
			myindaoid = 0;
			mSaoDangFinshDo = null;
		}
		Destroy (this.gameObject);
	}
	IEnumerator mCloseEffect()
	{
		yield return new WaitForSeconds (1.9f);
		SaoDangfinshed.SetActive (false);
		UI3DEffectTool.ClearUIFx (SaoDangfinshed);
	}
	public void CloseEffect()
	{
		SaoDangfinshed.SetActive (false);
		UI3DEffectTool.ClearUIFx (SaoDangfinshed);
	}
}










