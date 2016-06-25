using UnityEngine;
using System.Collections;

public class ModelTouchShowOrMoveManagerment : MonoBehaviour
{
    private NpcCityTemplate m_currentNpcTemplate;
    private UICamera.MouseOrTouch m_MouseOrTouch;
    void OnClick()
    {
        if (gameObject.name.IndexOf("PlayerObject") > -1)
        {
            if (MainCityUI.IsWindowsExist())
            {
                return;
            }
            m_MouseOrTouch = UICamera.GetTouch(UICamera.currentTouchID);
            EquipSuoData.CreateChaKan(gameObject.name, m_MouseOrTouch.pos);
            return;
        }
        else if (gameObject.name == "NpcInCity" || gameObject.name == "EffectPortal")
        {
            PlayerModelController.m_playerModelController.m_agent.enabled = true;
            m_currentNpcTemplate = transform.GetComponent<NpcObjectItem>().m_template;

            PlayerModelController.m_playerModelController.m_iMoveToNpcID = m_currentNpcTemplate.m_npcId;
            if (Vector3.Distance(transform.position, PlayerModelController.m_playerModelController.m_ObjHero.transform.position) > 2)
            {
                PlayerModelController.m_playerModelController.SelfNavigation(transform.position);
            }
            else
            {
                if (m_currentNpcTemplate.m_npcId == 801)
                {
                    NewEmailData.Instance().OpenEmail(0);
                }
                else
                {
                    PlayerModelController.m_playerModelController.TidyNpcInfo();
                }
            }

        }
    }
	

}
