using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIPanelTongzhi : MYNGUIPanel
{
    public UIPanelTongzhiData m_UIPanelTongzhiData;
    public List<UIPanelTongzhiData> m_listData = new List<UIPanelTongzhiData>();
    public ScaleEffectController m_ScaleEffectController;

    public List<TongzhiData> m_CurrentTongzhiDataList
    {
        get
        {
            if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY_YEWAN || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY_YEWAN)
            {
                return Global.m_listMainCityData;
            }
            else if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE)
            {
                return Global.m_listJiebiaoData;
            }
            else
            {
                return new List<TongzhiData>();
            }
        }
    }

    public MainCityUITongzhi m_CurrentTongzhi
    {
        get
        {
            if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY_YEWAN || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY_YEWAN)
            {
                return MainCityUI.m_MainCityUI.m_MainCityUITongzhi;
            }
            else if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE)
            {
                return Carriage.RootManager.Instance.m_CarriageMain.m_MainCityUiTongzhi;
            }
            else
            {
                return null;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        int y = 193;
        for (int i = 0; i < m_CurrentTongzhiDataList.Count; i++)
        {
            m_CurrentTongzhiDataList[i].m_isLooked = true;
            UIPanelTongzhiData temp = GameObject.Instantiate(m_UIPanelTongzhiData.gameObject).GetComponent<UIPanelTongzhiData>();
            temp.transform.parent = m_UIPanelTongzhiData.transform.parent;
            temp.transform.localPosition = new Vector3(0, y, 0);

            temp.m_labelDes.text = m_CurrentTongzhiDataList[i].m_SuBaoMSG.subao;
			temp.m_listButtonBG[0].gameObject.SetActive(false);
			temp.m_listButtonBG[1].gameObject.SetActive(false);
            for (int q = 0; q < m_CurrentTongzhiDataList[i].m_ButtonIndexList.Count; q++)
            {
                string tempShowText = "";
                switch (m_CurrentTongzhiDataList[i].m_ButtonIndexList[q])
                {
                    case 0:
                        tempShowText = "显 示";
                        break;
                    case 1:
                        tempShowText = "忽 略";
                        break;
                    case 2:
                        tempShowText = "前 往";
                        break;
                    case 3:
                        tempShowText = "祝 福";
                        break;
                    case 4:
                        tempShowText = "安 慰";
                        break;
                    case 5:
                        tempShowText = "领 取";
                        break;
                    case 6:
                        tempShowText = "知道了";
                        break;
                }
                temp.m_listButtonBG[q].gameObject.name = "TongzhiButton_" + i + "_" + q;
                temp.m_listButtonBG[q].gameObject.SetActive(true);
                temp.m_listButtonLabel[q].text = tempShowText;
            }

            temp.transform.localScale = Vector3.one;
            y -= 110;

            m_listData.Add(temp);
        }
        m_UIPanelTongzhiData.gameObject.SetActive(false);
        m_ScaleEffectController.OpenCompleteDelegate += scaleOver;

        if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY_YEWAN || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY_YEWAN)
        {
            MainCityUI.TryAddToObjectList(gameObject);
            CityGlobalData.m_isRightGuide = true;
            MainCityUI.m_MainCityUI.m_MainCityUITongzhi.upDataShow();
        }
        else if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE)
        {
            Carriage.RootManager.Instance.m_CarriageMain.m_MainCityUiTongzhi.upDataShow();
        }
    }

    public void scaleOver()
    {

    }

    public override void MYClick(GameObject ui)
    {
        if (ui.name.IndexOf("Close") != -1)
        {
            GameObject.Destroy(gameObject);
            MainCityUI.TryRemoveFromObjectList(gameObject);
        }
        else if (ui.name.IndexOf("TongzhiButton_") != -1)
        {
            string tempName = ui.name.Substring(14, ui.name.Length - 14);

            int dataIndex = int.Parse(tempName.Substring(0, tempName.IndexOf("_")));
            int tempIndex = int.Parse(tempName.Substring(tempName.IndexOf("_") + 1, 1));

            PromptActionReq req = new PromptActionReq();

            req.reqType = m_CurrentTongzhiDataList[dataIndex].m_ButtonIndexList[tempIndex];

            req.suBaoId = m_CurrentTongzhiDataList[dataIndex].m_SuBaoMSG.subaoId;

            MemoryStream tempStream = new MemoryStream();

            QiXiongSerializer t_qx = new QiXiongSerializer();

            t_qx.Serialize(tempStream, req);

            byte[] t_protof;

            t_protof = tempStream.ToArray();

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_Prompt_Action_Req, ref t_protof);

            Global.m_listAllTheData.Remove(m_CurrentTongzhiDataList[dataIndex]);
            Global.upDataTongzhiData(null);

            GameObject.Destroy(m_listData[dataIndex].gameObject);
            m_listData.RemoveAt(dataIndex);
            for (int i = dataIndex; i < m_listData.Count; i++)
            {
                m_listData[i].gameObject.transform.localPosition = new Vector3(m_listData[i].gameObject.transform.localPosition.x, m_listData[i].gameObject.transform.localPosition.y + 110, m_listData[i].gameObject.transform.localPosition.z);

                for (int j = 0; j < m_CurrentTongzhiDataList[i].m_ButtonIndexList.Count; j++)
                {
                    m_listData[i].m_listButtonBG[j].gameObject.name = "TongzhiButton_" + i + "_" + j;
                }
            }
            m_CurrentTongzhi.upDataShow();
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

    // Update is called once per frame
    void Update()
    {

    }
}
