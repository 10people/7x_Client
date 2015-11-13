using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlayerNameManager : MonoBehaviour
{
	public static PlayerNameManager m_this;
    private static List<EnterScene> _PlayerNameInfo = new List<EnterScene>();
    public static Dictionary<int, GameObject> m_playrNameDic = new Dictionary<int, GameObject>(); //玩家名字集合

    public Camera m_mainCamera;

    public static float m_NameHeightOffset = 4.74f;
    [HideInInspector]
    public static GameObject m_PlayerNamesParent;
    [HideInInspector]
    public static GameObject m_SelfName;

    [HideInInspector]
    public static GameObject m_ObjSelfName;

    void Awake()
    {
       
    }

    void Start()
    {
		m_this = this;
        //[]Remove later.
        if (MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
        {
            gameObject.SetActive(true);

            GameObject tempObj = GameObject.Find("MainCity");
            //if (Global.GetObj(ref tempObj, "Camera").GetComponent<Camera>() != null)
            //m_mainCamera = Global.GetObj(ref tempObj, "Camera").GetComponent<Camera>();

//            m_scale = m_root.activeHeight / (CityGlobalData.m_ScreenHeight);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        //		Debug.Log( "PlayerNameManager.OnDestroy()" );

        m_playrNameDic.Clear();
    }
  

    public static void CreatePlayerName(EnterScene p_player_data)
    {
        int size = _PlayerNameInfo.Count;
        for (int i = 0; i < size; i++)
        {
            if (_PlayerNameInfo[i].uid == p_player_data.uid)
            {
                return;
            }
        }
        _PlayerNameInfo.Add(p_player_data);
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_PLAYER_NAME),
                               LoadCallback);
    }


    //创建玩家名字 并添加到集合中
    //IEnumerator LoadingPlayerName(EnterScene p_player_data)
    // {
    //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_PLAYER_NAME),
    //                        LoadCallback);

    //while (m_player_name_obj == null)
    //{
    //    yield return new WaitForEndOfFrame();
    //}

    ////		Debug.Log( "PlayerName.Loaded: " + p_player_data.uid );

    //GameObject tempPlayerName = (GameObject)Instantiate(m_player_name_obj);

    //tempPlayerName.transform.parent = GameObject.Find("MainCity").transform.FindChild("PlayerNamesInCityManager");

    //tempPlayerName.transform.position = Vector3.zero;

    //tempPlayerName.transform.localScale = new Vector3(0.03f, 0.03f, 1);

    //PlayerNameInCity tempName = tempPlayerName.GetComponent<PlayerNameInCity>();

    //tempName.Init(p_player_data.senderName, "");

    //m_playrNameDic.Add(p_player_data.uid, tempName);
    //  }
    //void PlayerNameShow()
    //{
    //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_PLAYER_NAME),
    //                               LoadCallback);

    //}
    private Object m_player_name_obj = null;

    private static void LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        //if (GameObject.Find("MainCity").transform.FindChild("PlayerNamesInCityManager") && !m_playrNameDic.ContainsKey(_PlayerNameInfo[indexName_Num].uid))

        //		Debug.Log("_PlayerNameInfo[0].uid=" + _PlayerNameInfo[0].uid);

        if (!m_playrNameDic.ContainsKey(_PlayerNameInfo[0].uid))
        {
            GameObject tempPlayerName = (GameObject)Instantiate(p_object);
            tempPlayerName.transform.parent = m_PlayerNamesParent.transform;
            tempPlayerName.transform.position = Vector3.zero;

            tempPlayerName.transform.localScale = new Vector3(1.0f, 1.0f, 1);

            PlayerNameInCity tempName = tempPlayerName.GetComponent<PlayerNameInCity>();
            tempPlayerName.GetComponent<PlayerNameInCity>().m_playerName.transform.localPosition = new Vector3(0,-0.1f,0);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_playerName.transform.localScale = new Vector3(0.01f, 0.01f, 1);

            tempPlayerName.GetComponent<PlayerNameInCity>().m_playerVip.transform.localPosition = new Vector3(-3.16f, -0.1f, 0);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_playerVip.transform.localScale = new Vector3(0.01f, 0.01f, 1);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_LabAllianceName.transform.localPosition = new Vector3(0,-0.75f,0);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_LabAllianceName.transform.localScale = new Vector3(0.0094f, 0.0094f, 1);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_SpriteChengHao.transform.localPosition = new Vector3(0, 0.94f, 0);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_SpriteChengHao.transform.localScale = new Vector3(0.021f, 0.021f, 1);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_SpriteChengHao.transform.localEulerAngles = new Vector3(38, 0, 0);

            tempPlayerName.GetComponent<PlayerNameInCity>().m_SpriteVip.transform.localPosition = new Vector3(-0.64f, -0.1f, 0);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_SpriteVip.transform.localScale = new Vector3(0.014f, 0.012f, 1);
            tempPlayerName.GetComponent<PlayerNameInCity>().m_SpriteVip.transform.localEulerAngles = new Vector3(38, 0, 0);

            tempName.Init(_PlayerNameInfo[0].senderName, "", "","",0);
            m_playrNameDic.Add(_PlayerNameInfo[0].uid, tempPlayerName);
            if (PlayersManager.m_playrHeadInfo.ContainsKey(_PlayerNameInfo[0].uid))
            {
                PlayerNameManager.UpdateAllLabel(PlayersManager.m_playrHeadInfo[_PlayerNameInfo[0].uid]);
                PlayersManager.m_playrHeadInfo.Remove(_PlayerNameInfo[0].uid);
            }
            _PlayerNameInfo.RemoveAt(0);
        }
        else
        {
            p_object = null;
        }

     
    }

    public static void UpdateAllLabel(ErrorMessage u_info)
    {
        if (m_playrNameDic.ContainsKey(u_info.errorCode))
        {
            string sss = u_info.errorDesc;
            if (sss.IndexOf('#') > -1)
            {
                string[] info = sss.Split('#');
                string chenghao = info[0].ToString();
                if (chenghao.IndexOf(':') > -1)
                {
                    string[] cc = chenghao.Split(':');
                    string ccc = cc[1].ToString();
                    if (!string.IsNullOrEmpty(ccc) && !ccc.Equals("-1"))
                    {
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteChengHao.gameObject.SetActive(true);
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteChengHao.spriteName = ccc;
                    }
                    else
                    {
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteChengHao.gameObject.SetActive(false);
                    }
                }
                else
                {
                    m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteChengHao.gameObject.SetActive(false);
                }
                string ll = info[2].ToString();
                string zw = info[6].ToString();
                if (ll.IndexOf(':') > -1 && ll.IndexOf("***") < 0)
                {
                    string[] lm = ll.Split(':');
                    string[] zhiw = zw.Split(':');
                    m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_LabAllianceName.text = MyColorData.getColorString(12,"<" +lm[1].ToString()+ ">" + FunctionWindowsCreateManagerment.GetIdentityById(int.Parse(zhiw[1])));
                }
                else
                {
                    m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_LabAllianceName.text = MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT));
                }

                string vip = info[4].ToString();
                if (vip.IndexOf(':') > -1)
                {
                    string[] vp = vip.Split(':');
                    string name = m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_playerName.text;
                    if (int.Parse(vp[1]) > 0)
                    {
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_playerName.alignment = NGUIText.Alignment.Left;
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_playerName.transform.localPosition = new Vector3(2.88f, -0.1f, 0);
                    }
                    else
                    {
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_playerName.alignment = NGUIText.Alignment.Center;
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_playerName.transform.localPosition = new Vector3(0, -0.1f, 0);
                    }
                    if (!string.IsNullOrEmpty(vp[1]) && int.Parse(vp[1]) > 0)
                    {
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteVip.gameObject.SetActive(true);
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteVip.spriteName = "vip" + vp[1].ToString();
                    }
                    else
                    {
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteVip.gameObject.SetActive(false);
                    }
                    
                }
            }
            else
            {
              
                m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteChengHao.gameObject.SetActive(false);
                m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_LabAllianceName.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT);
            }
        }
    }

    public static void DestroyPlayerName(ExitScene tempPlayer) //删除玩家名字
    {
        if (tempPlayer != null)
        {
            if (m_playrNameDic.ContainsKey(tempPlayer.uid))
            {
                Destroy(m_playrNameDic[tempPlayer.uid]);

                m_playrNameDic.Remove(tempPlayer.uid);
            }
        }

    }

    //更新玩家名字位置
    public static void UpdatePlayerNamePosition(PlayerInCity tempPlayer)
    {

        if (!m_playrNameDic.ContainsKey(tempPlayer.m_playerID))
        {
            return;
        }
        PlayerNameInCity tempPlayerName = m_playrNameDic[tempPlayer.m_playerID].GetComponent<PlayerNameInCity>();
        Vector3 v = tempPlayer.transform.position;
        v.y += m_NameHeightOffset;
        tempPlayerName.transform.position = v;
    }

    public static void CreateSelfeName(Object p_object)
    {
        //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_PLAYER_NAME),
        //                       LoadSelfCallback);
        GameObject tempPlayerName = (GameObject)Instantiate(p_object);
        tempPlayerName.transform.parent = m_SelfName.transform;
        m_ObjSelfName = tempPlayerName;
        tempPlayerName.transform.localPosition = new Vector3(0, 70, 0);
        tempPlayerName.transform.localScale = Vector3.one;
        PlayerNameInCity tempName = tempPlayerName.GetComponent<PlayerNameInCity>();

        tempName.m_playerVip.transform.localPosition = new Vector3(-144, -22f, 0);
        tempName.m_LabAllianceName.transform.localPosition = new Vector3(0, -45.2f, 0);
        tempName.m_SpriteChengHao.transform.localPosition = new Vector3(0, 20, 0);
        tempName.m_SpriteVip.transform.localPosition = new Vector3(-40, -20, 0);
        tempName.m_playerName.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        tempName.m_playerVip.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        tempName.m_LabAllianceName.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        tempName.m_SpriteVip.transform.localScale = new Vector3(0.68f, 0.52f, 1);
        tempPlayerName.GetComponent<PlayerNameInCity>().m_playerName.transform.localEulerAngles = Vector3.zero;
        tempPlayerName.GetComponent<PlayerNameInCity>().m_playerVip.transform.localEulerAngles = Vector3.zero;
        tempPlayerName.GetComponent<PlayerNameInCity>().m_LabAllianceName.transform.localEulerAngles = Vector3.zero;
        if (JunZhuData.Instance().m_junzhuInfo.vipLv > 0)
        {
            tempName.m_playerName.alignment = NGUIText.Alignment.Left;
            tempName.m_playerName.transform.localPosition = new Vector3(132, -22f, 0);
        }
        else
        {
            tempName.m_playerName.alignment = NGUIText.Alignment.Center;
            tempName.m_playerName.transform.localPosition = new Vector3(0, -22f, 0);
        }

        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
        {
            tempName.Init(JunZhuData.Instance().m_junzhuInfo.name
               , AllianceData.Instance.g_UnionInfo.name, JunZhuData.m_iChenghaoID.ToString()
               , JunZhuData.Instance().m_junzhuInfo.vipLv.ToString()
               , AllianceData.Instance.g_UnionInfo.identity);
        }
        else
        {
            tempName.Init(JunZhuData.Instance().m_junzhuInfo.name
               , "", JunZhuData.m_iChenghaoID.ToString(), JunZhuData.Instance().m_junzhuInfo.vipLv.ToString(), 0);
        }
    }
    private static void LoadSelfCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempPlayerName = (GameObject)Instantiate(p_object);
        tempPlayerName.transform.parent = m_SelfName.transform;
        m_ObjSelfName = tempPlayerName;
        tempPlayerName.transform.localPosition = new Vector3(0,70,0);
        tempPlayerName.transform.localScale = Vector3.one;
        PlayerNameInCity tempName = tempPlayerName.GetComponent<PlayerNameInCity>();

        tempName.m_playerVip.transform.localPosition = new Vector3(-144, -22f, 0);
        tempName.m_LabAllianceName.transform.localPosition = new Vector3(0, -45.2f, 0);
        tempName.m_SpriteChengHao.transform.localPosition = new Vector3(0, 20, 0);
        tempName.m_SpriteVip.transform.localPosition = new Vector3(-40,-20,0);
        tempName.m_playerName.transform.localScale = new Vector3(0.5f,0.5f,1);
        tempName.m_playerVip.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        tempName.m_LabAllianceName.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        tempName.m_SpriteVip.transform.localScale = new Vector3(0.68f, 0.52f,1);
        tempPlayerName.GetComponent<PlayerNameInCity>().m_playerName.transform.localEulerAngles = Vector3.zero;
        tempPlayerName.GetComponent<PlayerNameInCity>().m_playerVip.transform.localEulerAngles = Vector3.zero;
        tempPlayerName.GetComponent<PlayerNameInCity>().m_LabAllianceName.transform.localEulerAngles = Vector3.zero;
        if (JunZhuData.Instance().m_junzhuInfo.vipLv > 0)
        {
            tempName.m_playerName.alignment = NGUIText.Alignment.Left;
            tempName.m_playerName.transform.localPosition = new Vector3(132, -22f, 0);
        }
        else
        {
            tempName.m_playerName.alignment = NGUIText.Alignment.Center;
            tempName.m_playerName.transform.localPosition = new Vector3(0, -22f, 0);
        }
        
        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
        { 
             tempName.Init(JunZhuData.Instance().m_junzhuInfo.name
                , AllianceData.Instance.g_UnionInfo.name, JunZhuData.m_iChenghaoID.ToString()
                ,JunZhuData.Instance().m_junzhuInfo.vipLv.ToString()
                , AllianceData.Instance.g_UnionInfo.identity);
        }
        else
        {
            tempName.Init(JunZhuData.Instance().m_junzhuInfo.name
               , "", JunZhuData.m_iChenghaoID.ToString(), JunZhuData.Instance().m_junzhuInfo.vipLv.ToString(),0);
        }
    }

    public static void UpdateSelfName()
    {
        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0 && AllianceData.Instance.g_UnionInfo != null)
        {
            m_ObjSelfName.GetComponent<PlayerNameInCity>().Init(JunZhuData.Instance().m_junzhuInfo.name
            , AllianceData.Instance.g_UnionInfo.name, JunZhuData.m_iChenghaoID.ToString(), 
            JunZhuData.Instance().m_junzhuInfo.vipLv.ToString(), AllianceData.Instance.g_UnionInfo.identity);
        }
        else
        {
            m_ObjSelfName.GetComponent<PlayerNameInCity>().Init(JunZhuData.Instance().m_junzhuInfo.name
          ,"", JunZhuData.m_iChenghaoID.ToString(), JunZhuData.Instance().m_junzhuInfo.vipLv.ToString(),0);
        }
    }
   
}
