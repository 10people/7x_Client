using UnityEngine;
using System.Collections;
using Carriage;

namespace Carriage
{
    public class CarriageNPCController : MonoBehaviour
    {
        public void OnCarriageItemClick()
        {
            if (!RootManager.Instance.m_CarriageMain.isCanStartCarriage)
            {
                ClientMain.m_UITextManager.createText("正在运镖中...");
                return;
            }

            if (RootManager.Instance.m_SelfPlayerController != null && RootManager.Instance.m_SelfPlayerCultureController != null)
            {
                //Cancel chase.
                RootManager.Instance.m_CarriageMain.TryCancelChaseToAttack();

                RootManager.Instance.m_SelfPlayerController.m_CompleteNavDelegate = RootManager.Instance.m_CarriageMain.DoStartCarriage;
                RootManager.Instance.m_SelfPlayerController.StartNavigation(transform.position);
            }
        }
    }
}
