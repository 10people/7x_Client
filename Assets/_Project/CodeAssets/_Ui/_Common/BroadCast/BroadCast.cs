using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using qxmobile.protobuf;

/// <summary>
/// scroll 1s for each additional character, 
/// wait 3s after scroll ends.
/// </summary>
public class BroadCast : MonoBehaviour, SocketListener
{
    public static bool IsOpenBroadCast = true;

    /// <summary>
    /// how much character per second.
    /// </summary>
    public const int BroadCastSpeed = 4;

    public UIPanel m_InfoPanel;

    public UILabel m_InfoLabel;
    public UISprite m_BgSprite;

    private float LabelFontSize
    {
        get { return m_InfoLabel.fontSize; }
    }

    private float ScrollSpeed
    {
        get { return LabelFontSize * BroadCastSpeed; }
    }
    private const float StartStayDuration = 1;
    private const float CompleteStayDuration = 3;

    private List<string> m_storedDataList = new List<string>();

    public bool IsInBroadCast;

    public void StartBroadCast()
    {
        if (!IsInBroadCast && m_storedDataList.Count != 0)
        {
            IsInBroadCast = true;
            var temp = m_storedDataList[0];
            m_storedDataList.Remove(temp);

            SetInfo(temp);

            ScrollInfo();
        }
    }

    private void SetInfo(string info)
    {
        List<char> countStr = info.ToArray().ToList();

        for (int i = 0; i < countStr.Count; i++)
        {
            while (countStr[i] == '[')
            {
                while (countStr[i] != ']')
                {
                    countStr.Remove(countStr[i]);

                    if (i >= countStr.Count)
                    {
                        break;
                    }
                }

                if (i >= countStr.Count)
                {
                    break;
                }

                countStr.Remove(countStr[i]);

                if (i >= countStr.Count)
                {
                    break;
                }
            }
        }

        //Cannot use ToString().
        m_InfoLabel.width = (int)Math.Ceiling(m_InfoLabel.fontSize * UtilityTool.GetBytesNumOfString(new String(countStr.ToArray())) / 2.0f);
        m_InfoLabel.text = info;
        m_InfoLabel.gameObject.SetActive(true);
        m_BgSprite.gameObject.SetActive(true);
    }

    private void ScrollInfo()
    {
        StartCoroutine(DoScrollInfo());
    }

    private IEnumerator DoScrollInfo()
    {
        if (m_InfoLabel.width > m_InfoPanel.finalClipRegion.z)
        {
            //display for 1 second.
            m_InfoLabel.transform.localPosition = new Vector3(m_InfoPanel.finalClipRegion.z / 2.0f + m_InfoLabel.width / 2.0f - m_BgSprite.width / 2.0f, 0, 0);
            yield return new WaitForSeconds(StartStayDuration);

            float startPosX = m_InfoLabel.transform.localPosition.x;
            float endPosX = (m_InfoPanel.finalClipRegion.z - m_InfoLabel.width) / 2.0f;
            iTween.ValueTo(gameObject, iTween.Hash("from", startPosX, "to", endPosX, "speed", ScrollSpeed, "easetype", "linear", "onupdate", "SetInfoLabelPosition", "oncompletetarget", gameObject, "oncomplete", "StartClearCount"));
        }
        else
        {
            //display for 1 second.
            m_InfoLabel.transform.localPosition = new Vector3((m_InfoPanel.finalClipRegion.z - m_InfoLabel.width) / 2.0f, 0, 0);
            yield return new WaitForSeconds(StartStayDuration);

            float startPosX = m_InfoLabel.transform.localPosition.x;
            float endPosX = 0;
            iTween.ValueTo(gameObject, iTween.Hash("from", startPosX, "to", endPosX, "speed", ScrollSpeed, "easetype", "linear", "onupdate", "SetInfoLabelPosition", "oncompletetarget", gameObject, "oncomplete", "StartClearCount"));
        }
    }

    private void SetInfoLabelPosition(float value)
    {
        m_InfoLabel.transform.localPosition = new Vector3(value, 0, 0);
    }

    private void StartClearCount()
    {
        if (TimeHelper.Instance.IsTimeCalcKeyExist("BroadCastClear"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("BroadCastClear");
        }
        TimeHelper.Instance.AddOneDelegateToTimeCalc("BroadCastClear", CompleteStayDuration, GotoNextOrClear);
    }

    private void GotoNextOrClear()
    {
        if (m_storedDataList.Count != 0)
        {
            //Continue to next.
            var temp = m_storedDataList[0];
            m_storedDataList.Remove(temp);

            SetInfo(temp);

            ScrollInfo();
        }
        else
        {
            Clear();
        }
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_BROAD_CAST:
                    {
                        //open broadcast interface.
                        if (!IsOpenBroadCast)
                        {
                            return true;
                        }

                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        ErrorMessage tempResp = new ErrorMessage();
                        t_qx.Deserialize(t_tream, tempResp, tempResp.GetType());

                        m_storedDataList.Add(tempResp.errorDesc);
                        StartBroadCast();

                        return true;
                    }
            }
        }
        return false;
    }

    public void RegisterListener()
    {
        SocketTool.RegisterSocketListener(this);
    }

    public void Clear()
    {
        //Clear and stop.
        StopCoroutine("DoScrollInfo");
        m_storedDataList.Clear();
        m_InfoLabel.gameObject.SetActive(false);
        m_BgSprite.gameObject.SetActive(false);
        IsInBroadCast = false;
    }

    void OnLevelWasLoaded()
    {
        IsOpenBroadCast = true;
    }

    void Start()
    {
        m_BgSprite.width = 425 + ClientMain.m_iMoveX * 2;
        m_InfoPanel.baseClipRegion = new Vector4(m_InfoPanel.baseClipRegion.x, m_InfoPanel.baseClipRegion.y, m_BgSprite.width, m_InfoPanel.baseClipRegion.w);
    }

    void Awake()
    {
        RegisterListener();
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }
}
