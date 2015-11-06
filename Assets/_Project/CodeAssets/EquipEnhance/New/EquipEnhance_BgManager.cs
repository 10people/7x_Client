using UnityEngine;
using System.Collections;

public class EquipEnhance_BgManager : MonoBehaviour {

	public GameObject m_gb_drop_target;


	
	public void SetDropIndicator( bool tempState ){
        m_gb_drop_target.SetActive(tempState);
	}

}
