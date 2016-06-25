using UnityEngine;
using System.Collections;

public class Showinstruction : MonoBehaviour {
	[HideInInspector]public int star1;
	[HideInInspector]public int star2;
	[HideInInspector]public int Lvs;
//	GameObject gbj;
//	void Start () {
//		gbj = GameObject.Find ("Map(Clone)");
//	}

	public void loadback(ref WWW p_www, string p_path,  UnityEngine.Object p_object)
	{
		star1 = MapData.mapinstance.LingJiangStar;
		
		star2 = MapData.mapinstance.LingJiangStared;
		
		GameObject tempOjbect = Instantiate(p_object) as GameObject;
		
		//tempOjbect.transform.parent = gbj.transform;
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		
		tempOjbect.transform.localPosition = new Vector3 (10,-24,0);
		
		//tempOjbect.GetComponent<PveStarsMissionManagerment>().DataTidy(Lvs,star1,star2);
	}
	void OnClick()
	{

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_GRADE_REWARD ),
		                        loadback );
		if (UIYindao.m_UIYindao.m_isOpenYindao){
//             if (star1 == 111 && star2 == 1)
//            {
//                TaskLoadData tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId];
//                tempTaskData.m_iCurIndex += 2;
//                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
//            }
//            else

            {
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100007];
				 
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);

				//Debug.Log("tempTaskData.m_iCurIndex++tempTaskData.m_iCurIndex++tempTaskData.m_iCurIndex++" + tempTaskData.m_iCurIndex);
            }
        }
//		Debug .Log (" 弹出升星级的UI、。。。。。。。。。。。。。" +tempOjbect.name);
		//Debug .Log ("star1"+star1);
		//Debug .Log ("star2"+star2);
		//Debug .Log ("Lvs"+Lvs);
	}
}
