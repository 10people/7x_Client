using UnityEngine;
using System.Collections;

public class NpcCapsuleCollider : MonoBehaviour
{
    private NpcCityTemplate m_currentNpcTemplate;
	void Start () 
    {
	
	}


    void OnClick()
    {
        m_currentNpcTemplate = transform.GetComponent<NpcObjectItem>().m_template;
        PlayerModelController.m_playerModelController.m_iMoveToNpcID = m_currentNpcTemplate.m_npcId;
        /*******************************
此处省略新寻路特效开始代码
**********************************/
        PlayerModelController.m_playerModelController.SelfNavigation(this.transform.position);

    }
}
