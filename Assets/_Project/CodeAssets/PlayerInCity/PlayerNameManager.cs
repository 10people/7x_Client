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

    public static float m_NameHeightOffset =3.82f;
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

    private static float _ScaleSize = 0f;
    public static void CreatePlayerName(EnterScene p_player_data,float scale = 0.01f)
    {
        _ScaleSize = scale;
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
 
    private Object m_player_name_obj = null;
    private static void LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        if (!m_playrNameDic.ContainsKey(_PlayerNameInfo[0].uid))
        {
            GameObject tempPlayerName = (GameObject)Instantiate(p_object);
            tempPlayerName.transform.parent = m_PlayerNamesParent.transform;
            tempPlayerName.transform.position = Vector3.zero;
 
            PlayerNameInCity tempName = tempPlayerName.GetComponent<PlayerNameInCity>();
            tempPlayerName.GetComponent<PlayerNameInCity>().m_ObjController.GetComponent<UIWidget>().m_camera_oriented = true;
             tempPlayerName.GetComponent<PlayerNameInCity>().m_ObjController.transform.localScale = new Vector3(_ScaleSize, _ScaleSize, 1.0f);
        //     tempPlayerName.GetComponent<PlayerNameInCity>().m_ObjController.transform.localEulerAngles = new Vector3(40, 311, 0);
        

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
                    string name = m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_NameSend;
                    if (!string.IsNullOrEmpty(vp[1]) && int.Parse(vp[1]) > 0)
                    {
                        m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_SpriteVip.transform.localPosition = new Vector3(-55 - (FunctionWindowsCreateManagerment.DistanceCount(name) - 1) * 17, -11, 0);
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
 
    public static void CreateSelfeName(Object p_object,float pos_Y = 107.0f,float scale = 0.6f)
    {
        //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_PLAYER_NAME),
        //                       LoadSelfCallback);
        GameObject tempPlayerName = (GameObject)Instantiate(p_object);
        tempPlayerName.transform.parent = m_SelfName.transform;
        m_ObjSelfName = tempPlayerName;
        tempPlayerName.transform.localPosition = new Vector3(0, pos_Y, 0);
        tempPlayerName.transform.localScale = Vector3.one;
        PlayerNameInCity tempName = tempPlayerName.GetComponent<PlayerNameInCity>();
        tempPlayerName.GetComponent<PlayerNameInCity>().m_ObjController.transform.localScale = new Vector3(scale, scale, 1.0f);

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
 
    public static void UpdateSelfName()
    {
     
        if (!AllianceData.Instance.IsAllianceNotExist && AllianceData.Instance.g_UnionInfo != null)
        {
            m_ObjSelfName.GetComponent<PlayerNameInCity>().Init(JunZhuData.Instance().m_junzhuInfo.name
            , AllianceData.Instance.g_UnionInfo.name, JunZhuData.m_iChenghaoID.ToString()
            , JunZhuData.Instance().m_junzhuInfo.vipLv.ToString(), AllianceData.Instance.g_UnionInfo.identity);
            
        }
        else
        {
            m_ObjSelfName.GetComponent<PlayerNameInCity>().Init(JunZhuData.Instance().m_junzhuInfo.name
               , ""
               , JunZhuData.m_iChenghaoID.ToString(), JunZhuData.Instance().m_junzhuInfo.vipLv.ToString(), 0);
        }
    }
}
