using UnityEngine;
using System.Collections;

public class OperationButtons : MonoBehaviour { //右下角 按钮显示 隐藏

	public UISprite m_sprite;

	public GameObject m_buttons;

	public float m_actionTime;

	public float m_actionDis;

	public float m_actionAlpha;

	public float m_actionAngle;

	public static bool m_isShow;

	private Vector3 m_startPosition;


	void Awake()
	{
		m_startPosition = m_buttons.transform.localPosition;
	}

	void Start () {

	}

	void OnEnable()
	{
		m_isShow = false;

		m_buttons.gameObject.SetActive (false);

		m_buttons.GetComponent<UIRect> ().alpha = 0.0f;
	}

	void OnClick()
	{
		Debug.Log("m_isShow1111111111111="+m_isShow);
		m_isShow = !(m_isShow);
		Debug.Log("m_isOpenButtons="+m_isShow);
		float tempDis;
		float tempAlpha;
		float tempAngle;

		if(m_isShow == true)
		{
			m_buttons.SetActive(true);
			tempDis = -m_actionDis;
			tempAlpha = 1.0f;
			tempAngle = 0;
		}else
		{
			tempDis = m_actionDis;
			tempAlpha = m_actionAlpha;
			tempAngle = m_actionAngle;
		}

		Vector3 tempVec = m_startPosition + new Vector3 (tempDis,0,0);

		TweenAlpha.Begin(m_buttons,m_actionTime,tempAlpha);
		
		TweenPosition.Begin(m_buttons,m_actionTime,tempVec);

		TweenRotation.Begin(m_sprite.gameObject, m_actionTime, Quaternion.AngleAxis (tempAngle, Vector3.forward));

		StartCoroutine (HideObject());

//		 if(GameObject.Find("MainMaskLayer(Clone)") != null && CityGlobalData.m_LingQuJiangLi)
//		{
//			GameObject.Find("MainMaskLayer(Clone)").GetComponent<MainMaskManager>().MovePosition(1);
//
//		}
//		if (TaskData.Instance.ShowId == 100001) {
//			if (UIYindao.m_UIYindao.m_isOpenYindao) 
//			{
//				TaskLoadData tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId];
//				Debug.Log( "operaBtn:" + tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
//				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
//			}
//		}
		if (UIYindao.m_UIYindao.m_isOpenYindao) 
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
			Debug.Log( "operaBtn:" + tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
		}
	}

	IEnumerator HideObject()
	{
		yield return new WaitForSeconds (m_actionTime);

		if(m_isShow == false)
		{
			m_buttons.SetActive (false);
		}
	}
}
