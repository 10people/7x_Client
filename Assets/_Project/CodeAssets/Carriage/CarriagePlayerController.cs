//#define DEBUG_MOVE

using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

namespace Carriage
{
    public class CarriagePlayerController : SinglePlayerController
    {
        public RootManager m_RootManager;

        public override void OnPlayerRun()
        {
            m_Animator.SetBool("Move", true);
        }

        public override void OnPlayerStop()
        {
            m_Animator.SetBool("Move", false);
        }

        //public Vector3 RestrictPosition = Vector3.zero;
        public bool IsSetToRestrictPosition = false;

        new void Update()
        {
            base.Update();

            if (Time.realtimeSinceStartup - PlayerManager.m_LatestServerSyncTime >
                NetworkHelper.GetPingSecWithMin(Console_SetNetwork.GetMinPingForPreRun()) * NetworkHelper.GetValidRunC())
            {
#if DEBUG_MOVE
                Debug.LogWarning("+++++++++++++Limit self msg.");
#endif

                if (!IsSetToRestrictPosition)
                {
                    IsUploadPlayerPosition = false;

                    //RestrictPosition = transform.localPosition;
                    IsSetToRestrictPosition = true;
                }
            }
            else
            {
                if (IsSetToRestrictPosition)
                {
#if DEBUG_MOVE
                    Debug.LogWarning("+++++++++++Set restrict position from " + transform.localPosition + " to " + RestrictPosition);
#endif

                    //transform.localPosition = RestrictPosition;

                    IsSetToRestrictPosition = false;
                }

                IsUploadPlayerPosition = true;
            }
        }

        new void Start()
        {
            base.Start();

            //Init track camera.
            TrackCameraOffsetPosUp = 4.09f;
            TrackCameraOffsetPosBack = 4.96f;
            TrackCameraOffsetUpDownRotation = 26.4f;
        }

        void OnDestroy()
        {
            base.OnDestroy();

            m_RootManager = null;
        }
    }
}
