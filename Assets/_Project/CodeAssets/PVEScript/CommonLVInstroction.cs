using UnityEngine;
using System.Collections;

public class CommonLVInstroction : MonoBehaviour {



	void Start () {

		UILabel introduction = GetComponent<UILabel> ();
		int GuanqianId= Pve_Level_Info.CurLev;
		PveTempTemplate m_item = PveTempTemplate.GetPveTemplate_By_id (GuanqianId);
		//int descId = m_item.smaDesc;

		string Desc =  DescIdTemplate.GetDescriptionById (m_item.smaDesc);
		introduction.text = Desc;
			
	}
	


}
