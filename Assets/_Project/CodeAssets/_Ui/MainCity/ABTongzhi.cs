using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class ABTongzhi : MYNGUIPanel
    {
        public AllianceBattleMain m_AllianceBattleMain;

        public UILabel m_labelDes;
        public GameObject m_objBGShow;
        public bool m_isOpen = false;
        private bool m_isMove = false;

        public int TargetUID;
        public Vector3 TargetPosition = Vector3.zero;

        void Awake()
        {

        }

        void Start()
        {

        }

        public void ShowUI(string data)
        {
            gameObject.SetActive(true);
            if (!m_isOpen)
            {
                m_isOpen = true;
                iTween.MoveTo(m_objBGShow, iTween.Hash(
                    "name", "func",
                    "position", new Vector3(-340, m_objBGShow.transform.localPosition.y, m_objBGShow.transform.localPosition.z),
                    "time", .4f,
                    "easeType", iTween.EaseType.easeOutElastic,
                    "islocal", true
                    ));
                m_isMove = true;
            }
            m_labelDes.text = data;
        }

        public void CloseWindow()
        {
            TargetUID = -1;
            TargetPosition = Vector3.zero;

            m_isOpen = false;
            gameObject.SetActive(false);
            m_objBGShow.transform.localPosition = new Vector3(-25f, m_objBGShow.transform.localPosition.y, m_objBGShow.transform.localPosition.z);
        }

        public override void MYClick(GameObject ui)
        {
            if (ui.name.IndexOf("TongzhiButton_") != -1)
            {
                int tempIndex = int.Parse(ui.name.Substring(14, 1));

                if (tempIndex == 0)
                {
                    if (m_AllianceBattleMain.m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(TargetUID))
                    {
                        //m_AllianceBattleMain.TpToPosition(new Vector2(m_AllianceBattleMain.m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[TargetUID].transform.position.x, m_AllianceBattleMain.m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[TargetUID].transform.position.z));

                        SpriteMove tempInfo = new SpriteMove()
                        {
                            uid = PlayerSceneSyncManager.Instance.m_MyselfUid,
                            posX = m_AllianceBattleMain.m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[TargetUID].transform.position.x,
                            posY = m_AllianceBattleMain.m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[TargetUID].transform.position.y,
                            posZ = m_AllianceBattleMain.m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[TargetUID].transform.position.z,
                            dir = RootManager.Instance.m_SelfPlayerController.transform.eulerAngles.y
                        };

                        SocketHelper.SendQXMessage(tempInfo, ProtoIndexes.POS_JUMP);
                    }
                    else if (TargetPosition != Vector3.zero)
                    {
                        //m_AllianceBattleMain.TpToPosition(new Vector2(TargetPosition.x, TargetPosition.z));

                        SpriteMove tempInfo = new SpriteMove()
                        {
                            uid = PlayerSceneSyncManager.Instance.m_MyselfUid,
                            posX = TargetPosition.x,
                            posY = TargetPosition.y,
                            posZ = TargetPosition.z,
                            dir = RootManager.Instance.m_SelfPlayerController.transform.eulerAngles.y
                        };

                        SocketHelper.SendQXMessage(tempInfo, ProtoIndexes.POS_JUMP);
                    }
                }

                CloseWindow();
            }
        }

        public override void MYMouseOver(GameObject ui)
        {
        }

        public override void MYMouseOut(GameObject ui)
        {
        }

        public override void MYPress(bool isPress, GameObject ui)
        {
        }

        public override void MYelease(GameObject ui)
        {
        }

        public override void MYondrag(Vector2 delta)
        {

        }

        public override void MYoubleClick(GameObject ui)
        {
        }

        public override void MYonInput(GameObject ui, string c)
        {
        }

        public void moveOver()
        {
            if (m_isOpen)
            {
                m_objBGShow.transform.localPosition = new Vector3(-340f, m_objBGShow.transform.localPosition.y, m_objBGShow.transform.localPosition.z);
            }
            else
            {
                m_objBGShow.transform.localPosition = new Vector3(-25f, m_objBGShow.transform.localPosition.y, m_objBGShow.transform.localPosition.z);
            }
        }
    }
}