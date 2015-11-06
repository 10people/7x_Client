using UnityEngine;
using System.Collections;

public class JunZhuChangeLayer : MonoBehaviour 
{
	public GameObject m_currentLayer1;
	public GameObject m_currentLayer2;
	public GameObject m_nextLayer;
	public GameObject m_Mask;
	public bool m_IsOn = false;

	public 
	
	void OnClick()
	{
		if(transform.name.Equals("Button_Back"))
		{
			if(!string.IsNullOrEmpty(PlayerPrefs.GetString("JunZhu")))
			{
			  PlayerPrefs.DeleteKey("JunZhu");
			}
			Change(false);
		}
		else 
		{
			if(m_nextLayer.name.Equals("Technology"))
				m_IsOn = true;
			
			Change(true);
		}
	
	}
	
    private	void Change(bool isshow)
	{
		if (m_currentLayer1 != null)
		{
			m_currentLayer1.SetActive(!isshow);
		}
		if (m_currentLayer2 != null && !m_nextLayer.name.Equals("Equip"))
		{
//			m_currentLayer2.GetComponent<PlayerData>().list[CityGlobalData.m_king_model_Id-1].GetComponent<Animation>().enabled =!isshow;

			m_currentLayer2.SetActive(!isshow);

		}
		if (m_nextLayer != null)
		{
			m_nextLayer.SetActive(isshow);
			if (UIYindao.m_UIYindao.m_isOpenYindao) 
			{
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			}
//			if (UIYindao.m_UIYindao.m_isOpenYindao && m_nextLayer.name.Equals("Technology")) 
//			{
//				TaskLoadData tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
//				Debug.Log( "enter:" + tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
//				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
//			}
		}
//		if(m_Mask != null)
//		{
//			if(m_nextLayer.name.Equals("Equip"))
//			m_Mask.GetComponent<ActivityGuideController>().MoveAhead(0);
//		}

	}
	
}
