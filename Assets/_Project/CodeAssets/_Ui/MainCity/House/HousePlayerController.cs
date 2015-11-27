using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

public class HousePlayerController : SinglePlayerController, SocketListener
{
    public static HousePlayerController s_HousePlayerController;

    public override void OnPlayerRun()
    {
        m_Animator.SetBool("inRun", true);
    }

    public override void OnPlayerStop()
    {
        m_Animator.SetBool("inRun", false);
    }

    /// <summary>
    /// Receive character sync message from server.
    /// </summary>
    /// <param name="p_message"></param>
    /// <returns></returns>
    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.Enter_HouseScene: //有玩家进入
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EnterScene tempScene = new EnterScene();
                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());

                        if (tempScene.senderName == JunZhuData.Instance().m_junzhuInfo.name)
                        {
                            s_uid = tempScene.uid;
                        }

                        HousePlayerManager.Instance.CreatePlayer(tempScene);

                        return true;
                    }
                case ProtoIndexes.Sprite_Move: //玩家移动
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        SpriteMove tempMove = new SpriteMove();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        t_qx.Deserialize(t_stream, tempMove, tempMove.GetType());
                        Debug.Log("uiduiduiduiduiduiduiduiduiduid ::: " + tempMove.uid);
                        HousePlayerManager.Instance.UpdatePlayerPosition(tempMove);

                        return true;
                    }
                case ProtoIndexes.Exit_HouseScene: //玩家退出
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        ExitScene tempScene = new ExitScene();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());

                        HousePlayerManager.Instance.DestroyPlayer(tempScene);

                        return true;
                    }
                default:
                    break;
            }
        }
        return false;
    }

    #region Wow camera effect controller

    private RaycastHit cameraHit;
    private RaycastHit playerHit;

    new void LateUpdate()
    {
        base.LateUpdate();

        if (TrackCamera == null)
        {
            return;
        }

        //Check and move
        DoCameraMove();
    }

    void DoCameraMove()
    {
        int count = 0;
        while (true)
        {
            //Check unreal camera that out of wall.
            RaycastHit[] tempHits = Physics.RaycastAll(new Ray(TrackCamera.transform.position, Vector3.down), Mathf.Infinity);
            cameraHit = tempHits.Where(item => item.transform.tag == "AreaCalc")
                    .FirstOrDefault();
            tempHits = Physics.RaycastAll(new Ray(transform.position, Vector3.down), Mathf.Infinity);

            playerHit = tempHits.Where(item => item.transform.tag == "AreaCalc")
                    .FirstOrDefault();

            //Move camera if necessary.
            if (playerHit.transform != null && cameraHit.transform != null)
            {
                if (cameraHit.transform == playerHit.transform)
                {
                    return;
                }
                else
                {
                    MoveCameraForward();
                }
            }
            else if (playerHit.transform == null && cameraHit.transform != null)
            {
                return;
            }
            else if (playerHit.transform != null && cameraHit.transform == null)
            {
                MoveCameraForward();
            }
            //Well, this is used for player and camera both in the gap of 2 areas, but this happens in very small oppotunity.
            //So, use this as a additional error check when player and camera both at the bounder of house.
            else if (playerHit.transform == null && cameraHit.transform == null)
            {
                MoveCameraForward();
            }

            count++;
            if (count > 200)
            {
                //[TAG]preformance issue in slow machine.
                Debug.LogWarning("Move camera in house fail, restore initial position.");
                TrackCamera.transform.localPosition = transform.localPosition;
                TrackCamera.transform.localEulerAngles = new Vector3(0, TrackCamera.transform.localEulerAngles.y, 0);
                TrackCamera.transform.Translate(Vector3.up * TrackCameraOffsetPosUp);
                TrackCamera.transform.Translate(Vector3.back * TrackCameraOffsetPosBack);
                TrackCamera.transform.localEulerAngles = new Vector3(TrackCameraOffsetUpDownRotation, TrackCamera.transform.localEulerAngles.y, 0);
                return;
            }
        }
    }

    void MoveCameraForward()
    {
        TrackCamera.transform.Translate(Vector3.forward * 0.05f);
    }

    void MoveCameraBackward()
    {
        TrackCamera.transform.Translate(Vector3.back * 0.05f);

        TrackCamera.transform.localPosition += new Vector3(TrackCamera.transform.forward.x, 0, TrackCamera.transform.forward.z).normalized * (-0.05f);
    }

    #endregion

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }

    new void Start()
    {
        base.Start();

        m_Transform = transform;
        m_CharacterController.enabled = true;
        m_NavMeshAgent.enabled = false;

        //Init track camera.
        TrackCameraOffsetPosUp = 2.8f;
        TrackCameraOffsetPosBack = 4.19f;
        TrackCameraOffsetUpDownRotation = 18.36f;
    }

    new void Awake()
    {
        base.Awake();

        s_HousePlayerController = this;

        SocketTool.RegisterSocketListener(this);
    }
}
