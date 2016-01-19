using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreshGuide {

	private static FreshGuide m_instance = null;


	public static FreshGuide Instance(){
		if( m_instance == null ){
			m_instance = new FreshGuide();
		}

		return m_instance;
	}


	// 请@雷庆 将任务的引导id于此判断，并返回
	public bool IsActive(int  taskid )
	{
		foreach(KeyValuePair<int,ZhuXianTemp> taskifo in TaskData.Instance.m_TaskInfoDic){
			if(taskifo.Value.id ==  taskid){
			  return true;
			}
		}
		return false;
	}

	public void LogActiveTask(){
		Debug.Log( "--------- FreshGuide.LogActiveTask() ---------" );

//		foreach(KeyValuePair<int,TaskLoadData> t_task in TaskData.Instance.m_TaskInfoDic){
//			Debug.Log( t_task.Key + ", " + t_task.Value.id + ", " + t_task.Value.m_iCurIndex );
//		}
	}
}
