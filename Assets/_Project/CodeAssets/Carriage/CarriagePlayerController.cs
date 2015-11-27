using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

namespace Carriage
{
    public class CarriagePlayerController : SinglePlayerController
    {
        public static CarriagePlayerController s_CarriagePlayerController;
        public RootManager m_RootManager;

        public override void OnPlayerRun()
        {
            m_Animator.SetBool("inRun", true);
        }

        public override void OnPlayerStop()
        {
            m_Animator.SetBool("inRun", false);
        }

        new void Start()
        {
            base.Start();

            //Init track camera.
            TrackCameraOffsetPosUp = 5.1f;
            TrackCameraOffsetPosBack = 5.6f;
            TrackCameraOffsetUpDownRotation = 29f;

            //Init nav ui.
            m_StartNavDelegate = AutoNav;
            m_EndNavDelegate = DestroyAutoNav;
        }

        public void AutoNav()
        {
            DestroyAutoNav();
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.AUTO_NAV), LoadSelfCallback);
        }

        private GameObject m_ObjAutoNav;

        private void LoadSelfCallback(ref WWW p_www, string p_path, Object p_object)
        {
            m_ObjAutoNav = (GameObject)Instantiate(p_object);
            m_ObjAutoNav.transform.parent = m_RootManager.m_CarriageUi.transform;
            m_ObjAutoNav.transform.localPosition = new Vector3(0, -170, 0);
            m_ObjAutoNav.transform.localScale = Vector3.one;
        }

        public void DestroyAutoNav()
        {
            if (m_ObjAutoNav != null)
            {
                Destroy(m_ObjAutoNav);
            }
        }
    }
}
