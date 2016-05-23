using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SaoDangAwardTemp : MonoBehaviour {

	public UILabel mSaoDangTime;

	public int mSaoDangTimeNum;

	public UILabel Exp;

	public UILabel Money;

	public GameObject RewardRoot;

	public PveSaoDangAward m_tempInfo ;

	private float mDistance = 110;

	private Vector3 TempPosition = new Vector3(-165,0,0);

	public UILabel NoAwardLable;

	[HideInInspector]
	public GameObject IconSamplePrefab;

	void Start () {
	
		//Init ();////////
	}
	

	void Update () {
	
	}

	public void Init()
	{
		ShowMoneyAndExp ();

		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}


	}

	void ShowMoneyAndExp()
	{
		Exp.text = m_tempInfo.exp.ToString ();

		Money.text = m_tempInfo.money.ToString ();

		mSaoDangTime.text = mSaoDangTimeNum.ToString ();
	}
	float x = -180;
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}

		StartCoroutine (StartCreateAward( ));
		
	}
	IEnumerator StartCreateAward()
	{

		if( m_tempInfo.awardItems == null ||m_tempInfo.awardItems.Count <= 0)
		{
			NoAwardLable.gameObject.SetActive(true);

		}
		else{
			int Max = m_tempInfo.awardItems.Count;
			if(Max > 4)Max =4;
			for (int n = 0; n < Max; n++)
			{
				yield return new WaitForSeconds(0.1f);
				//Debug.Log("m_tempInfo.awardItems[n].itemId = "+m_tempInfo.awardItems[n].itemId);	
				GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
				iconSampleObject.SetActive(true);
				iconSampleObject.transform.parent = RewardRoot.transform;

				iconSampleObject.transform.localScale= Vector3.one;
				iconSampleObject.transform.localPosition = new Vector3(n * 70 - (Max - 1) * 35, 0, 0);
				var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
				
				NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(m_tempInfo.awardItems[n].itemId);
				string mdesc = DescIdTemplate.GetDescriptionById(m_tempInfo.awardItems[n].itemId);
				
				iconSampleManager.SetIconByID(m_tempInfo.awardItems[n].itemId, m_tempInfo.awardItems[n].itemNum.ToString(), 10);
				//iconSampleManager.SetAwardNumber(m_tempInfo.awardItems[n].itemNum);
				iconSampleManager.SetIconPopText(m_tempInfo.awardItems[n].itemId, mNameIdTemplate.Name, mdesc, 1);
				iconSampleManager.transform.localScale = new Vector3(0.6f,0.6f,1);
			}

		}
	}
}








