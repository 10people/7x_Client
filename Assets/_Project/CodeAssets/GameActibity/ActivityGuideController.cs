using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ActivityGuideController : MonoBehaviour 
{
	public GameObject m_GuideSprite;
	public GameObject m_EquipGuideSprite;
	public GameObject m_MashGuideMain;
	public GameObject m_MashGuideTechnology;
	public GameObject m_TechnologyBtn;
	public GameObject m_EquipBtn;
	List<Vector3> listMovePos = new List<Vector3>();

	void Start (){
		Vector3 [] pos = {new Vector3(114,-150,-100),new Vector3(-221,-174,-100),new Vector3(204,-197,-100),new Vector3(369,279,-100)};
		listMovePos.AddRange(pos);
	}
	
 
	void Update (){
//		if(m_TechnologyBtn.GetComponent<JunZhuChangeLayer> ().m_IsOn)
//		{
////		   if (FreshGuide.Instance ().IsActive ((FreshGuide.GuideState)6)) 
////		  {
////			this.gameObject.SetActive(true);
////			m_TechnologyBtn.GetComponent<JunZhuChangeLayer> ().m_IsOn = false;
////			m_MashGuideMain.SetActive(false);
////		//	m_MashGuideTechnology.SetActive(true);
////			m_GuideSprite.GetComponent<TweenPosition>().enabled = true;
////		//	GameObject.Find("Technology").transform.FindChild("Scroll View").GetComponent<UIScrollView>().enabled =false;
////		 }
//		}
//		else 
//		{
//			//this.gameObject.SetActive(false);
//		}

//		if(CityGlobalData.m_JunZhuEquipGuide)
//		{
//			CityGlobalData.m_JunZhuEquipGuide = false;
		 if (CityGlobalData.m_JunZhuEquipGuide) {
//			CityGlobalData.m_JunZhuEquipGuide = false;
//			m_EquipGuideSprite.SetActive(FreshGuide.Instance().IsActive ((FreshGuide.GuideState)1));
			if (UIYindao.m_UIYindao.m_isOpenYindao){
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId];

				Debug.Log( "active:" + tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);

				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			}
		 }
//		}
//		else 
//		{
//			m_EquipGuideSprite.SetActive(false);
//		}
		 
	}

	public void MoveAhead(int index){
		m_EquipGuideSprite.GetComponent<TweenPosition> ().from = listMovePos[index];

		m_EquipGuideSprite.GetComponent<TweenPosition> ().to = listMovePos[index+1];

		m_EquipGuideSprite.GetComponent<TweenPosition> ().enabled = true;

		if( index == listMovePos.Count - 2 ){
		  m_EquipGuideSprite.GetComponent<TweenRotation>().enabled = true;
		}

	}
}
