using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace LimitActivity
{
    public class ActivityDetailController : MonoBehaviour, SocketListener
    {
        public RootController m_RootController;

        public UILabel m_ActivityDescLabelTall;
        public UILabel m_ActivityDescLabelShort;
        public UILabel m_ActivityTimeCalcLabel;
        public UILabel m_ActivityStateLabel;
        //public UISprite m_ActivityDescSprite;

        public UIScrollView m_ScrollView;
        public UIScrollBar m_ScrollBar;
        public UIGrid m_Grid;
        public GameObject m_ReceiveItemPrefab;

        public List<ReceiveItemController> m_ReceiveItemControllerList = new List<ReceiveItemController>();

        /// <summary>
        /// Activity item data
        /// </summary>
        public OpenXianShi m_OpenXianShi;

        /// <summary>
        /// Activity detail data
        /// </summary>
        public XinShouXianShiInfo m_ShouXianShiInfo;

        public void RefreshTimeCalc(int second)
        {
            if (!gameObject.activeInHierarchy) return;

            if (m_ShouXianShiInfo.remainTime - second > 0)
            {
                m_ActivityTimeCalcLabel.gameObject.SetActive(true);
                m_ActivityTimeCalcLabel.text = ColorTool.Color_Red_c40000 + TimeHelper.SecondToClockTime(m_ShouXianShiInfo.remainTime - second) + "[-]";
            }
            else
            {
                m_ActivityTimeCalcLabel.gameObject.SetActive(false);
                TimeHelper.Instance.RemoveFromTimeCalc("LimitActivityDetail");
            }
        }

        public void Refresh()
        {
            //Read some xmls to set labels and sprite.
            XianshiControlTemp tempControl = XianshiControlTemp.templates.Where(item => item.id == m_OpenXianShi.typeId).FirstOrDefault();

            if (m_ShouXianShiInfo.remainTime >= 0)
            {
                m_ActivityDescLabelTall.gameObject.SetActive(false);
                m_ActivityDescLabelShort.gameObject.SetActive(true);
                m_ActivityTimeCalcLabel.gameObject.SetActive(true);

                m_ActivityDescLabelShort.text = LanguageTemplate.GetText(LanguageTemplate.Text.LIMIT_TIME_ACTIVITIES_9);

                if (TimeHelper.Instance.IsTimeCalcKeyExist("LimitActivityDetail"))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("LimitActivityDetail");
                }
                TimeHelper.Instance.AddEveryDelegateToTimeCalc("LimitActivityDetail", m_ShouXianShiInfo.remainTime, RefreshTimeCalc);
            }
            else
            {
                //m_ActivityDescLabelTall.gameObject.SetActive(true);
                m_ActivityDescLabelShort.gameObject.SetActive(false);
                m_ActivityTimeCalcLabel.gameObject.SetActive(false);

               // m_ActivityDescLabelTall.text = tempControl.Desc;
            }
            m_ActivityStateLabel.text = tempControl.jinduDesc.Replace("**", ColorTool.Color_Green_00ff00 + m_ShouXianShiInfo.beizhu + "[-]");
//            m_ActivityDescSprite.gameObject.SetActive(true);
//            m_ActivityDescSprite.spriteName = tempControl.PicId.ToString();

            //Set receive item list.
            while (m_Grid.transform.childCount > 0)
            {
                var child = m_Grid.transform.GetChild(0);

                child.parent = null;
                Destroy(child.gameObject);
            }
            m_ReceiveItemControllerList.Clear();

            List<HuoDongInfo> tempList = m_ShouXianShiInfo.huodong;
            tempList.Sort((item, item2) => item.huodongId.CompareTo(item2.huodongId));
            for (int i = 0; i < tempList.Count; i++)
            {
                var temp = Instantiate(m_ReceiveItemPrefab) as GameObject;
                TransformHelper.ActiveWithStandardize(m_Grid.transform, temp.transform);

                var controller = temp.GetComponent<ReceiveItemController>();
                controller.m_ActivityDetailController = this;
                controller.m_OpenXianShi = m_OpenXianShi;
                controller.m_HuoDongInfo = tempList[i];

                controller.Refresh();

                m_ReceiveItemControllerList.Add(controller);
            }

            m_Grid.Reposition();

			NGUIHelper.SetScrollBarValue(m_ScrollView, m_ScrollBar, 0.01f);
        }

        public bool OnSocketEvent(QXBuffer p_message)
        {
            if (p_message != null)
            {
                switch (p_message.m_protocol_index)
                {
                    //Receive award response.
                    case ProtoIndexes.S_XIANSHI_AWARD_RESP:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            ReturnAward tempInfo = new ReturnAward();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            switch (tempInfo.result)
                            {
                                case 10:
                                    {
                                        //m_PopupReceiveResult.text = "成功";

                                        object temp = new object();

                                        lock (temp)
                                        {
                                            var tempList = m_ReceiveItemControllerList.Where(item => item.m_HuoDongInfo.huodongId == tempInfo.huodongId).ToList();
                                            if (tempList != null && tempList.Count == 1)
                                            {
                                                //Set received sprite.
                                                tempList[0].m_ReceiveButtonHandler.gameObject.SetActive(false);
                                                tempList[0].ReceiveInfoSprite.gameObject.SetActive(true);
                                                tempList[0].ReceiveInfoSprite.spriteName = ReceiveItemController.ReceivedSpriteName;

                                                //Pop up received response.
                                                var iconList = new List<RewardData>();
                                                tempList[0].m_IconList.ForEach(item =>
                                                {
                                                    iconList.Add(new RewardData(item.id, item.num));
                                                });
                                                GeneralRewardManager.Instance().CreateReward(iconList);
                                            }

                                            //Refresh limit activity data.
                                            LimitActivityData.Instance.RequestData();
                                        }

                                        break;
                                    }
                                case 20:
                                    {
                                        //m_PopupReceiveResult.text = "已领取";
                                        break;
                                    }
                                case 30:
                                    {
                                        //m_PopupReceiveResult.text = "活动已关闭";
                                        break;
                                    }
                                case 40:
                                    {
                                        //m_PopupReceiveResult.text = "活动未开启";
                                        break;
                                    }
                                case 50:
                                    {
                                        //m_PopupReceiveResult.text = "条件未达成";
                                        break;
                                    }
                                default:
                                    {
                                        Debug.LogError("Not defined result:" + tempInfo.result + " in limited activity receive response.");
                                        return false;
                                    }
                            }
                            return true;
                        }
                    default:
                        return false;
                }
            }
            return false;
        }

        void OnDisable()
        {
            if (TimeHelper.m_ApplicationIsQuitting) return;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("LimitActivityDetail"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("LimitActivityDetail");
            }
        }

        void Awake()
        {
            SocketTool.RegisterSocketListener(this);
        }

        void OnDestroy()
        {
            SocketTool.UnRegisterSocketListener(this);
        }
    }
}
