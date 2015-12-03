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

        new void Start()
        {
            base.Start();

            //Init track camera.
            TrackCameraOffsetPosUp = 4.09f;
            TrackCameraOffsetPosBack = 4.96f;
            TrackCameraOffsetUpDownRotation = 26.4f;
        }
    }
}
