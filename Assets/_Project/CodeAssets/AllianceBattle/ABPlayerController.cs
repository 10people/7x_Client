using System;
using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class ABPlayerController : PlayerController
    {
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
