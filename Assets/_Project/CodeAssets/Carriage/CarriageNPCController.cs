using UnityEngine;
using System.Collections;
using Carriage;

namespace Carriage
{
    public class CarriageNPCController : MonoBehaviour
    {
        public GameObject m_ShadowObject;
        public GameObject m_UIParentObject;
        public Camera TrackCamera;

        public void OnClick()
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
                RootManager.Instance.m_SelfPlayerController.StartNavigation(transform.position, 2);
            }
        }

        void LateUpdate()
        {
            if (TrackCamera == null) return;

            m_UIParentObject.transform.eulerAngles = new Vector3(TrackCamera.transform.eulerAngles.x, TrackCamera.transform.eulerAngles.y, 0);
        }

        void Awake()
        {
            m_ShadowObject.SetActive(Quality_Shadow.BattleField_ShowSimpleShadow());
        }
    }
}
