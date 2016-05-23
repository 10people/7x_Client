using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlayerNameManager : MonoBehaviour
{
    private static List<EnterScene> _PlayerNameInfo = new List<EnterScene>();
    public static Dictionary<int, GameObject> m_playrNameDic = new Dictionary<int, GameObject>(); //玩家名字集合
    private static List<PlayersManager.OtherPlayerInfo> _Player_MainCity_NameInfo = new List<PlayersManager.OtherPlayerInfo>();
    public Camera m_mainCamera;

    public static float m_NameHeightOffset = 4.1f;
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
    }

    public static void DicClear()
    {
        m_playrNameDic.Clear();
    }
    private static float _ScaleSize = 0f;
    /******************************************************************************************/
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

			if (TreasureCityRoot.m_instance != null)
			{
				bool isPlayer = _PlayerNameInfo[0].roleId == 600 ? false : true;
				if (!isPlayer)
				{
					tempPlayerName.transform.parent = m_PlayerNamesParent.transform;
				}
				else
				{
					tempPlayerName.transform.parent = TCityPlayerManager.m_instance.NameParentObj ().transform;
				}
			}
			else
			{
				tempPlayerName.transform.parent = m_PlayerNamesParent.transform;
			}

            tempPlayerName.transform.position = Vector3.zero;
 
            PlayerNameInCity tempName = tempPlayerName.GetComponent<PlayerNameInCity>();
			tempName.m_ObjController.GetComponent<UIWidget>().m_camera_oriented = true;
			tempName.m_ObjController.transform.localScale = new Vector3(_ScaleSize, _ScaleSize, 1.0f);
			tempName.IsPlayer = _PlayerNameInfo[0].roleId == 600 ? false : true;
			//tempName.Init(_PlayerNameInfo[0].senderName, "", "","",0);
            m_playrNameDic.Add(_PlayerNameInfo[0].uid, tempPlayerName);
            _PlayerNameInfo.RemoveAt(0);
       
        }
        else
        {
            p_object = null;
        }
    }
    /******************************************************************************************/

    public static void Create_MainCity_PlayerName(PlayersManager.OtherPlayerInfo u_info, float scale = 0.01f)
    {
        _ScaleSize = scale;
        int size = _Player_MainCity_NameInfo.Count;
        for (int i = 0; i < size; i++)
        {
            if (_Player_MainCity_NameInfo[i]._UID == u_info._UID)
            {
                return;
            }
        }
        _Player_MainCity_NameInfo.Add(u_info);
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_PLAYER_NAME),
                               Load_MainCity_Callback);
    }
    private static void Load_MainCity_Callback(ref WWW p_www, string p_path, Object p_object)
    {
        if (!m_playrNameDic.ContainsKey(_Player_MainCity_NameInfo[0]._UID))
        {
            GameObject tempPlayerName = (GameObject)Instantiate(p_object);
            tempPlayerName.transform.parent = m_PlayerNamesParent.transform;
            tempPlayerName.transform.position = Vector3.zero;
            PlayerNameInCity tempName = tempPlayerName.GetComponent<PlayerNameInCity>();
            tempName.m_ObjController.GetComponent<UIWidget>().m_camera_oriented = true;
            tempName.m_ObjController.transform.localScale = new Vector3(_ScaleSize, _ScaleSize, 1.0f);
            m_playrNameDic.Add(_Player_MainCity_NameInfo[0]._UID, tempPlayerName);
            tempName.Init_Maincity_PlayerHeadInfo(_Player_MainCity_NameInfo[0]);
            _Player_MainCity_NameInfo.RemoveAt(0);

        }
        else
        {
            _Player_MainCity_NameInfo.RemoveAt(0);
            p_object = null;
        }
    }
    public static void CheckPlayer(PlayersManager.OtherPlayerInfo u_info)
    {
        if (m_playrNameDic.ContainsKey(u_info._UID))
        {
            u_info._Name = m_playrNameDic[u_info._UID].GetComponent<PlayerNameInCity>().m_NameSend;
            UpdateOtherPlayerInfo(u_info);
        }
    }
    static void UpdateOtherPlayerInfo(PlayersManager.OtherPlayerInfo u_info)
    {
        if (u_info._UID != PlayersManager.m_Self_UID)
        {
            PlayerNameInCity playerName = m_playrNameDic[u_info._UID].GetComponent<PlayerNameInCity>();
            playerName.Init_Maincity_PlayerHeadInfo(u_info);
        }
        else if (u_info._UID == PlayersManager.m_Self_UID)
        {
            if (string.IsNullOrEmpty(u_info._Greet))
            {
              m_ObjSelfName.GetComponent<PlayerNameInCity>().UpdateZH(u_info._Greet);
            }
        }
    }
    public static void UpdateAllLabel(ErrorMessage u_info)
    {
        if (m_playrNameDic.ContainsKey(u_info.errorCode))
        {
			PlayerNameInCity playerName = m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>();
			if (playerName.IsPlayer)
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
							playerName.m_isShowChengHao = true;
							playerName.m_SpriteChengHao.gameObject.SetActive(true);
							playerName.m_SpriteChengHao.spriteName = ccc;
						}
						else
						{
							playerName.m_isShowChengHao = false;
							playerName.m_SpriteChengHao.gameObject.SetActive(false);
						}
					}
					else
					{
						playerName.m_isShowChengHao = false;
						playerName.m_SpriteChengHao.gameObject.SetActive(false);
					}
					
					string ll = info[2].ToString();
					string zw = info[6].ToString();
					if (ll.IndexOf(':') > -1 && ll.IndexOf("***") < 0)
					{
						string[] lm = ll.Split(':');
						string[] zhiw = zw.Split(':');
						playerName.m_LabAllianceName.text = MyColorData.getColorString(12, "<" + lm[1].ToString() + ">  " + FunctionWindowsCreateManagerment.GetIdentityById(int.Parse(zhiw[1])));
					}
					else
					{
						playerName.m_LabAllianceName.text = MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT));
					}
					
					string vip = info[4].ToString();
					if (vip.IndexOf(':') > -1)
					{
						string[] vp = vip.Split(':');
						string name = m_playrNameDic[u_info.errorCode].GetComponent<PlayerNameInCity>().m_NameSend;
						if (!string.IsNullOrEmpty(vp[1]) && int.Parse(vp[1]) > 0)
						{
                            playerName.m_SpriteVip.transform.localPosition = new Vector3(-55 - (FunctionWindowsCreateManagerment.DistanceCount(name) - 1) * 20, -11, 0);
							playerName.m_SpriteVip.gameObject.SetActive(true);
							playerName.m_SpriteVip.spriteName = "vip" + vp[1].ToString();
						}
						else
						{
                            playerName.m_SpriteVip.transform.localPosition = new Vector3(-55 - (FunctionWindowsCreateManagerment.DistanceCount(name) - 1) * 20, -11, 0);
                            playerName.m_SpriteVip.gameObject.SetActive(false);
						}
					}
					
					if (u_info.errorDesc.IndexOf("Face") > -1)
					{
						string zhaohu = info[8].ToString();
						
						if (zhaohu.IndexOf("|") > -1)
						{
							playerName.UpdateZH(zhaohu);
						}
					}
				}
				else
				{
					playerName.m_SpriteChengHao.gameObject.SetActive(false);
					playerName.m_LabAllianceName.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT);
				}
			}
        }
        else if (u_info.errorCode == PlayersManager.m_Self_UID)
        {
            //if (!string.IsNullOrEmpty(JunZhuData.m_iChenghaoID.ToString()) && !JunZhuData.m_iChenghaoID.ToString().Equals("-1"))
            //{
            //    m_ObjSelfName.GetComponent<PlayerNameInCity>().m_isShowChengHao = true;
            //}
            //else
            //{
            //    m_ObjSelfName.GetComponent<PlayerNameInCity>().m_isShowChengHao = false;
            //}

            if (u_info.errorDesc.IndexOf("Face") > -1)
            {
                string[] info = u_info.errorDesc.Split('#');
                string zhaohu = info[8].ToString();
                if (zhaohu.IndexOf("|") > -1)
                {
                    m_ObjSelfName.GetComponent<PlayerNameInCity>().UpdateZH(zhaohu);
                }
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
    public static void UpdatePlayerNamePosition(int tempUID,GameObject tempTarget)
    {
		if (!m_playrNameDic.ContainsKey(tempUID))
        {
            return;
        }
		PlayerNameInCity tempPlayerName = m_playrNameDic[tempUID].GetComponent<PlayerNameInCity>();
		Vector3 v = tempTarget.transform.position;
        v.y += m_NameHeightOffset;
        tempPlayerName.transform.position = v;
    }
 
    public static void CreateSelfeName(Object p_object,float pos_Y = 85.0f,float scale = 0.6f)
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
        PlayersManager.OtherPlayerInfo head_info = new PlayersManager.OtherPlayerInfo();
        head_info._Name = JunZhuData.Instance().m_junzhuInfo.name;
        if (AllianceData.Instance.g_UnionInfo != null)
        {
            head_info._AllianceName = AllianceData.Instance.g_UnionInfo.name;
        }
        else
        {
            head_info._AllianceName = "";
        }
        head_info._Designation = JunZhuData.m_iChenghaoID.ToString();
        head_info._VInfo = JunZhuData.Instance().m_junzhuInfo.vipLv.ToString();
        if (AllianceData.Instance.g_UnionInfo != null)
        {
            head_info._Duty = AllianceData.Instance.g_UnionInfo.identity;
        }
       
        tempName.Init_Maincity_PlayerHeadInfo(head_info);
    }
 
    public static void UpdateSelfName()
    {
        PlayersManager.OtherPlayerInfo head_info = new PlayersManager.OtherPlayerInfo();
        head_info._Name = JunZhuData.Instance().m_junzhuInfo.name;
        if (AllianceData.Instance.g_UnionInfo != null)
        {
            head_info._AllianceName = AllianceData.Instance.g_UnionInfo.name;
        }
        else
        {
            head_info._AllianceName = "";
        }
        head_info._Designation = JunZhuData.m_iChenghaoID.ToString();
        head_info._VInfo = JunZhuData.Instance().m_junzhuInfo.vipLv.ToString();
        if (AllianceData.Instance.g_UnionInfo != null)
        {
            head_info._Duty = AllianceData.Instance.g_UnionInfo.identity;
        }
        m_ObjSelfName.GetComponent<PlayerNameInCity>().Init_Maincity_PlayerHeadInfo(head_info);
    }
}
