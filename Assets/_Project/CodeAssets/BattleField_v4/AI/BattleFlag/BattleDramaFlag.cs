using UnityEngine;
using System.Collections;

public class BattleDramaFlag : MonoBehaviour
{
	public int flagId;

	public int eventId;


	public void OnTriggerEnter(Collider other)
	{
		KingControllor node = (KingControllor)other.gameObject.GetComponent("KingControllor");

		bool f = CityGlobalData.getDramable ();

		if(node == null || !node.isAlive || f == false)
		{
			return;
		}

		int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

		GuideTemplate template = GuideTemplate.getTemplateByLevelAndEvent (level, eventId);

		bool flag = BattleControlor.Instance ().havePlayedGuide (template);
		
		if (flag == true) return;

		BattleUIControlor.Instance ().showDaramControllor (level, eventId);
	
		Destroy (gameObject);
	}

	public void OnTriggerExit(Collider other)
	{

	}

}
