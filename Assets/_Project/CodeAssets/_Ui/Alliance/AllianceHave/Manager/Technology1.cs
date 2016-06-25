using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class Technology1 : MonoBehaviour {
	public int Card;

	public GameObject KeJiTemp;

	public KeJiList m_JianZhuKeji;

	float Dis = 110;
	public UIScrollView mUIScrollView;
	int kejiNumber; 
	List<LianMengKeJiTemplate> m_LianMengKeJiTemplate = new List<LianMengKeJiTemplate>();
	void Start () {
	
	}


	public void Init()
	{
		Dis = 114;
		List<LianMengKeJiTemplate> LianMengKeJiTemplateList = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_type ( );

		//Debug.Log ("LianMengKeJiTemplateList.count = " +LianMengKeJiTemplateList.Count);
		m_LianMengKeJiTemplate.Clear ();

		for(int i =0 ; i < LianMengKeJiTemplateList.Count ; i++)
		{
			if(LianMengKeJiTemplateList[i].card == Card)
			{
				m_LianMengKeJiTemplate.Add(LianMengKeJiTemplateList[i]);
			}
		}
//		foreach(JianZhuInfo mjunzhu in m_JianZhuKeji.list)
//		{
//			Debug.Log("mjunzhu = "+mjunzhu.lv);
//		}
		//Debug.Log ("m_LianMengKeJiTemplate.count = " +m_LianMengKeJiTemplate.Count);
		//Debug.Log("_JianZhuKeji.list.count = "+m_JianZhuKeji.list.Count);
		foreach(Technologytemp mTe in TechnologyManager.Instance().mTechnologytempList)
		{
			Destroy(mTe.gameObject);
		}
		TechnologyManager.Instance().mTechnologytempList.Clear ();
		for(int i =0 ; i < m_LianMengKeJiTemplate.Count ; i++)
		{
			kejiNumber = 0;
			GameObject m_KeJiTemp = Instantiate(KeJiTemp) as GameObject;
			
			m_KeJiTemp.SetActive(true);
			
			m_KeJiTemp.transform.parent = KeJiTemp.transform.parent;
			
			m_KeJiTemp.transform.localPosition = new Vector3(8,129-i*Dis,0);
			//Debug.Log("localPosition= "+m_KeJiTemp.transform.localPosition.y);
			m_KeJiTemp.transform.localScale = Vector3.one;

			Technologytemp mTechnologytemp = m_KeJiTemp.GetComponent<Technologytemp>();

			mTechnologytemp.Keji_id = m_LianMengKeJiTemplate[i].id;

			mTechnologytemp.Keji_type = m_LianMengKeJiTemplate[i].type;

			if(Card == 1)
			{
				kejiNumber =i;
			}
			if(Card == 2)
			{
				kejiNumber = i+11;
			}
			if(Card == 3)
			{
				kejiNumber = i+13;
			}
			mTechnologytemp.Keji_Index = kejiNumber;
			mTechnologytemp.Keji_level = m_JianZhuKeji.list[kejiNumber].lv;
			mTechnologytemp.JiHuoLv = m_JianZhuKeji.list[kejiNumber].jiHuoLv;
			mTechnologytemp.Init();
			TechnologyManager.Instance().mTechnologytempList.Add(mTechnologytemp);
		}
		mUIScrollView.ResetPosition ();
	}
}
