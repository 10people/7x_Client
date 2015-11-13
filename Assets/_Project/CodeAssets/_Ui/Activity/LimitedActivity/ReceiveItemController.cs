using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace LimitActivity
{
    public class ReceiveItemController : MonoBehaviour
    {
        public ActivityDetailController m_ActivityDetailController;

        public UILabel m_ReceiveItemLabel;
        public UILabel m_TimeCalcLabel;
        public UIGrid m_Grid;

        [HideInInspector]
        public GameObject m_IconSamplePrefab;

        public EventHandler m_ReceiveButtonHandler;
        public UISprite ReceiveInfoSprite;

        public List<IconSampleManager> m_ItemIconManagerList = new List<IconSampleManager>();

        /// <summary>
        /// Receive item data
        /// </summary>
        public HuoDongInfo m_HuoDongInfo;

        /// <summary>
        /// Activity item data
        /// </summary>
        public OpenXianShi m_OpenXianShi;

        public struct IconItem
        {
            public int type;
            public int id;
            public int num;
        }
        public List<IconItem> m_IconList = new List<IconItem>();

        public const string ReceivedSpriteName = "alredayReceived";
        public const string OutOfTimeSpriteName = "outOfTime";

        /// <summary>
        /// Refresh Time Calc
        /// </summary>
        /// <param name="secondTime"></param>
        public void RefreshTimeCalc(int secondTime)
        {
            if (!gameObject.activeInHierarchy) return;

            if (m_HuoDongInfo.shengTime - secondTime > 0)
            {
                m_TimeCalcLabel.gameObject.SetActive(true);
                m_TimeCalcLabel.text = ColorTool.Color_Red_c40000 + TimeHelper.SecondToClockTime(m_HuoDongInfo.shengTime - secondTime) + "[-]";
            }
            else
            {
                m_TimeCalcLabel.gameObject.SetActive(false);
                TimeHelper.Instance.RemoveFromTimeCalc("LimitActivityReceiveItem" + m_HuoDongInfo.huodongId);
            }
        }

        public void Refresh()
        {
            if (m_IconSamplePrefab != null)
            {
                DoRefresh();
            }
            else
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconLoadCallBack);
            }
        }

        private void OnIconLoadCallBack(ref WWW www, string path, object loadedObject)
        {
            m_IconSamplePrefab = loadedObject as GameObject;
            DoRefresh();
        }

        private void DoRefresh()
        {
            XianshiHuodongTemp tempHuodong = XianshiHuodongTemp.templates.Where(item => item.id == m_HuoDongInfo.huodongId).FirstOrDefault();

            //Set top label.
            m_ReceiveItemLabel.text = tempHuodong.desc.Replace("*", (m_HuoDongInfo.state == 10 || m_HuoDongInfo.state == 20) ? ColorTool.Color_Green_00ff00 : ColorTool.Color_Red_c40000).Replace("#", "[-]");

            //Set time calc.
            if ((m_HuoDongInfo.state == 30 || m_HuoDongInfo.state == 40))
            {
                if (TimeHelper.Instance.IsTimeCalcKeyExist("LimitActivityReceiveItem" + m_HuoDongInfo.huodongId))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("LimitActivityReceiveItem" + m_HuoDongInfo.huodongId);
                }
                TimeHelper.Instance.AddEveryDelegateToTimeCalc("LimitActivityReceiveItem" + m_HuoDongInfo.huodongId, m_HuoDongInfo.shengTime, RefreshTimeCalc);
            }

            //Set icons.
            while (m_Grid.transform.childCount > 0)
            {
                var child = m_Grid.transform.GetChild(0);

                child.parent = null;
                Destroy(child.gameObject);
            }
            m_ItemIconManagerList.Clear();

            List<string> itemList = m_HuoDongInfo.jiangli.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            m_IconList = itemList.Select(item => item.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)).Select(item => new IconItem()
            {
                type = int.Parse(item[0]),
                id = int.Parse(item[1]),
                num = int.Parse(item[2]),
            }).ToList();

            if (m_IconList != null && m_IconList.Count > 0)
            {
                for (int i = 0; i < m_IconList.Count; i++)
                {
                    var temp = Instantiate(m_IconSamplePrefab) as GameObject;
                    UtilityTool.ActiveWithStandardize(m_Grid.transform, temp.transform);

                    var controller = temp.GetComponent<IconSampleManager>();
                    controller.SetIconByID(m_IconList[i].id, m_IconList[i].num.ToString(), 5);

                    controller.SetIconPopText(m_IconList[i].id);

                    m_ItemIconManagerList.Add(controller);

                    //Set icon scale to 0.5
                    temp.transform.localScale = Vector3.one * 0.5f;
                }
                m_Grid.Reposition();
            }

            //Set receive info and button.
            switch (m_HuoDongInfo.state)
            {
                //can receive
                case 10:
                    {
                        m_ReceiveButtonHandler.gameObject.SetActive(true);
                        m_ReceiveButtonHandler.GetComponent<UIButton>().isEnabled = true;
                        ReceiveInfoSprite.gameObject.SetActive(false);
                        break;
                    }
                //received
                case 20:
                    {
                        m_ReceiveButtonHandler.gameObject.SetActive(false);
                        ReceiveInfoSprite.gameObject.SetActive(true);
                        ReceiveInfoSprite.spriteName = ReceivedSpriteName;
                        break;
                    }
                //out of time
                case 30:
                    {
                        m_ReceiveButtonHandler.gameObject.SetActive(false);
                        ReceiveInfoSprite.gameObject.SetActive(true);
                        ReceiveInfoSprite.spriteName = OutOfTimeSpriteName;
                        break;
                    }
                //cannot receive
                case 40:
                    {
                        m_ReceiveButtonHandler.gameObject.SetActive(true);
                        m_ReceiveButtonHandler.GetComponent<UIButton>().isEnabled = false;
                        ReceiveInfoSprite.gameObject.SetActive(false);
                        break;
                    }
                default:
                    {
                        Debug.LogError("Not defined state:" + m_HuoDongInfo.state + " in receive item controller.");
                        break;
                    }
            }
        }

        private void OnReceiveButtonClick(GameObject go)
        {
            GainAward temp = new GainAward()
            {
                typeId = m_OpenXianShi.typeId,
                huodongId = m_HuoDongInfo.huodongId
            };

            UtilityTool.SendQXMessage(temp, ProtoIndexes.C_XIANSHI_AWARD_REQ);
        }

        void OnDisable()
        {
            if (TimeHelper.m_ApplicationIsQuitting) return;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("LimitActivityReceiveItem" + m_HuoDongInfo.huodongId))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("LimitActivityReceiveItem" + m_HuoDongInfo.huodongId);
            }
        }

        void Awake()
        {
            m_ReceiveButtonHandler.m_handler += OnReceiveButtonClick;
        }

        void OnDestroy()
        {
            m_ReceiveButtonHandler.m_handler -= OnReceiveButtonClick;
        }
    }
}
