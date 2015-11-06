using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HYApplicantList : MonoBehaviour {

	private List<GameObject> applicantList = new List<GameObject>();

	public GameObject applicantGrid;

	public GameObject applicantObj;

	public GameObject divideBtnColObj;
	
	public GameObject noApplayerLabel;
	
	//Get RewardItem Info
	public void GetRewardItemInfo (RewardItemInfo tempItemInfo)
	{
		foreach (GameObject item in applicantList)
		{
			Destroy (item);
		}
		applicantList.Clear ();
		
		if (tempItemInfo.applyerInfo.Count != 0)
		{
			noApplayerLabel.SetActive (false);

			int m = 0;
			foreach (ApplyerInfo applicant in tempItemInfo.applyerInfo)
			{
				GameObject applicantItem = (GameObject)Instantiate (applicantObj);
				
				applicantItem.SetActive (true);
				applicantItem.name = "Applicant";
				
				applicantItem.transform.parent = applicantGrid.transform;
				applicantItem.transform.localPosition = Vector3.zero + new Vector3(0,-105 * m,0);
				applicantItem.transform.localScale = applicantObj.transform.localScale;
				m ++;
				
				applicantList.Add (applicantItem);
				
				HYApplicantItem hyApplicant = applicantItem.GetComponent<HYApplicantItem> ();
				hyApplicant.GetApplicantInfo (applicant);
			}
			
			if (tempItemInfo.applyerInfo.Count < 4)
			{
				applicantGrid.GetComponent<ItemTopCol> ().enabled = true;
			}
			
			else
			{
				applicantGrid.GetComponent<ItemTopCol> ().enabled = false;
			}
		}

		else
		{
			//没有申请者
			noApplayerLabel.SetActive (true);
		}
	}
}
