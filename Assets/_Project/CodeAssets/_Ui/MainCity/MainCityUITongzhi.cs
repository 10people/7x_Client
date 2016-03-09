using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MainCityUITongzhi : MYNGUIPanel
{
    public UILabel m_labelDes;
    public GameObject m_objAlert;
    public GameObject m_objButton0;
    public GameObject m_objButton1;
    public UILabel m_labelButtonDes0;
    public UILabel m_labelButtonDes1;
    public GameObject m_objBGShow;
    public List<GameObject> m_listObjButton = new List<GameObject>();
    public List<UILabel> m_listButtonDes = new List<UILabel>();
    public TongzhiData m_TongzhiData;
	public bool m_isOpen = false;

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

	void Awake()
	{
		m_listObjButton.Add(m_objButton0);
		m_listObjButton.Add(m_objButton1);
		m_listButtonDes.Add(m_labelButtonDes0);
		m_listButtonDes.Add(m_labelButtonDes1);
	}

    void Start()
    {
    }

    public void upDataShow()
    {
		bool isHave = false;

//		m_isOpen = false;
//        m_objBGShow.SetActive(false);
//        for (int i = 0; i < m_listObjButton.Count; i++)
//        {
//            m_listObjButton[i].SetActive(false);
//        }
		if(m_CurrentTongzhiDataList == null || m_CurrentTongzhiDataList.Count == 0)
		{
			gameObject.SetActive(false);
			return;
		}
		else
		{
			gameObject.SetActive(true);
		}
        for (int i = m_CurrentTongzhiDataList.Count - 1; i >= 0; i--)
        {
            if (m_CurrentTongzhiDataList[i].IsInReceiveScene() && m_CurrentTongzhiDataList[i].IsReportShowType())
            {
                m_labelDes.text = m_CurrentTongzhiDataList[i].m_SuBaoMSG.subao;
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
//					Debug.Log(q);
//					Debug.Log(m_listObjButton);
//					Debug.Log(m_listObjButton.Count);
                    m_listObjButton[q].SetActive(true);
                    m_listButtonDes[q].text = tempShowText;
                }
                m_TongzhiData = m_CurrentTongzhiDataList[i];
                m_objBGShow.SetActive(true);
//				m_isOpen = true;
				isHave = true;
                break;
            }
        }
		if(isHave != m_isOpen)
		{
			m_isOpen = isHave;
			if(m_isOpen)
			{
				iTween.MoveTo (m_objBGShow, iTween.Hash(
					"name", "func",
					"position", m_objBGShow.transform.localPosition + new Vector3(-315, 0, 0),
					"time", .4f,
					"easeType", iTween.EaseType.easeOutElastic,
					"islocal",true
					));
			}
			else
			{
				iTween.MoveTo (m_objBGShow, iTween.Hash(
					"name", "func",
					"position", m_objBGShow.transform.localPosition + new Vector3(315, 0, 0),
					"time", .4f,
					"easeType", iTween.EaseType.easeOutElastic,
					"islocal",true
					));
			}
		}
        m_objAlert.SetActive(false);
        for (int i = 0; i < m_CurrentTongzhiDataList.Count; i++)
        {
            if (!m_CurrentTongzhiDataList[i].m_isLooked)
            {
                m_objAlert.SetActive(true);
                break;
            }
        }
    }

    public override void MYClick(GameObject ui)
    {
        if (ui.name.IndexOf("TongzhiButton_") != -1)
        {
            int tempIndex = int.Parse(ui.name.Substring(14, 1));

            PromptActionReq req = new PromptActionReq();

            req.reqType = m_TongzhiData.m_ButtonIndexList[tempIndex];

            req.suBaoId = m_TongzhiData.m_SuBaoMSG.subaoId;

            MemoryStream tempStream = new MemoryStream();

            QiXiongSerializer t_qx = new QiXiongSerializer();

            t_qx.Serialize(tempStream, req);

            byte[] t_protof;

            t_protof = tempStream.ToArray();

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_Prompt_Action_Req, ref t_protof);


			m_labelDes.text = "";
			for (int q = 0; q < 2; q++)
			{
				m_listObjButton[q].SetActive(false);
			}



            Global.m_listAllTheData.Remove(m_TongzhiData);
            Global.upDataTongzhiData(null);

			iTween.MoveTo (m_objBGShow, iTween.Hash(
				"name", "func",
				"position", m_objBGShow.transform.localPosition + new Vector3(315, 0, 0),
				"time", .4f,
				"easeType", iTween.EaseType.easeOutElastic,
				"islocal",true,
				"oncomplete", "moveOver",
				"oncompletetarget", gameObject
				));
			m_isOpen = false;

			if(m_TongzhiData.m_ButtonIndexShowDesList[tempIndex] != -1)
			{
				ClientMain.m_UITextManager.createText(DescIdTemplate.GetDescriptionById(m_TongzhiData.m_ButtonIndexShowDesList[tempIndex]));
			}
//			;
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
		upDataShow();
	}
}