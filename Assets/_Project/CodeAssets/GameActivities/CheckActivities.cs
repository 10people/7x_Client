using UnityEngine;
using System.Collections;

public class CheckActivities : MonoBehaviour {

	public enum ActivitiesStype
	{
		Common = 1,
		CountFeedback,
		Login,
		EveryDay,
		Feedback,
	};

	public ActivitiesStype m_stype = ActivitiesStype.Common;

	void OnClick()
	{
		switch (m_stype)
		{
			case ActivitiesStype.Common:
			{
				
				break;
			}
			case ActivitiesStype.CountFeedback:
			{
				break;
			}
			case ActivitiesStype.Login:
			{
				break;
			}
			case ActivitiesStype.EveryDay:
			{
				break;
			}
			case ActivitiesStype.Feedback:
			{
				break;
			}
		}
		this.transform.parent.GetComponent<ActivitiesManager> ().ShowLayer ((int)m_stype);
	}
}
