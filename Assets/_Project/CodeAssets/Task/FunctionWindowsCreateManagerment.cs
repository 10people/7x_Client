﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;
public class FunctionWindowsCreateManagerment : MonoBehaviour
{
    public static bool m_isJieBiao = false;
    public static bool m_isSwitchCountry = false;
    private static int BigHouseId = 0;
    private static int SmallHouseId = 0;
    public static int m_AllianceID = -1;
    public static int m_BuWeiNum = -1;
    private readonly static List<int> FrameQuality = new List<int>() { 2, 4, 5, 7, 8, 10 };
    public enum SettingType
    {
        NONE = -1,
        NATION_CHANGE = 0,
        NAME_CHANGE = 1,
        SWITCH_USER = 2,
    }
    public static SettingType m_SettingUpTYpe = SettingType.NONE;

    public struct EquipAdvanceInfo
    {
        public int _equipid;
        public int _nextid;
        public int _gong;
        public int _fang;
        public int _ming;
        public int _gongadd;
        public int _fanggadd;
        public int _minggadd;
    }
    public static EquipAdvanceInfo m_AdvanceInfo;
    public static bool m_IsEquipAdvance = false;
    public struct EquipTaoJiHuo
    {
        public int _quality;
        public int _gong;
        public int _fang;
        public int _ming;
        public int _gongadd;
        public int _fanggadd;
        public int _minggadd;
    }
    public static EquipTaoJiHuo m_JiHuoInfo;
    public static bool m_IsEquipJihuoShow = false;
    public static bool m_IsSaoDangNow = false;
    public static void CreateAllianceLayer(int id)
    {
        m_AllianceID = id;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_NO_SELF_ALLIANCE),
                                                       LoadCallback);
    }
    private static void LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);
    }
    public static void FunctionWindowShow(int id)
    {
        switch (id)
        {
            //Alliance
            case 104:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_HAVE_ROOT),
                                                AllianceHaveLoadCallback);
                    }
                }
                break;
            default:

                break;
        }
    }

    public static bool WetherHaveEquipsWearIsLowQuality(int buwei)
    {
        foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
        {
            int tempBuwei = 0;
            switch (item.Value.buWei)
            {
                case 1: tempBuwei = 3; break;//重武器
                case 2: tempBuwei = 4; break;//轻武器
                case 3: tempBuwei = 5; break;//弓
                case 11: tempBuwei = 0; break;//头盔
                case 12: tempBuwei = 8; break;//肩膀
                case 13: tempBuwei = 1; break;//铠甲
                case 14: tempBuwei = 7; break;//手套
                case 15: tempBuwei = 2; break;//裤子
                case 16: tempBuwei = 6; break;//鞋子
                default: break;
            }

            if (tempBuwei == buwei && item.Value.pinZhi > EquipsOfBody.Instance().m_equipsOfBodyDic[buwei].pinZhi)
            {
				MainCityUIRB.setSavePropUse(item.Value.itemId, 1);
                return true;
            }
        }
        return false;
    }

    private static void DoGoHome()
    {
        foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
        {
            if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId > 50)
            {
                BigHouseId = item.Value.locationId;
                break;
            }
        }
        if (BigHouseId > 0)
        {
            CityGlobalData.m_isNavToHouse = true;
            NpcManager.m_NpcManager.setGoToNpc(BigHouseId + 1000);
        }
        else
        {
            CityGlobalData.m_isNavToHome = true;
            foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
            {
                if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId <= 50)
                {
                    SmallHouseId = item.Value.locationId;
                    break;
                }
            }
            if (CityGlobalData.m_isAllianceTenentsScene)
            {
                if (!NpcManager.m_NpcManager.m_npcObjectItemDic.ContainsKey(SmallHouseId + 1000))
                {
                    NpcManager.m_NpcManager.setGoToNpc(SmallHouseId + 1000);
                    CityGlobalData.m_isNavToAllianCityToTenement = true;
                }
                else
                {
                    //NpcManager.m_NpcManager.setGoToSelfTenement(SmallHouseId + 1000);
                }
            }
            else
            {
                //NpcManager.m_NpcManager.setGoToTenementNpc(SmallHouseId + 1000);
            }
        }
    }

    public static void ShowSettingup()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SETTINGS_UP_LAYER),
                                           SettingUpLoadCallback);
    }

    private static void JunzhuLayerLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);
    }

    private static void OnActivityLoadCallBack(ref WWW www, string path, Object loadedObject)
    {
        GameObject tempObject = Instantiate(loadedObject) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);
    }

    private static void LoadYouXiaBack(ref WWW www, string path, Object loadedObject)
    {
        GameObject tempObject = Instantiate(loadedObject) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);
    }

    private static void RewardCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);

    }

    private static void TaskLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
    }

    private static void AllianceHaveLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);
    }


    private static void SerachTreasureLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.name = "QXTanBao";
    }

    private static void SettingUpLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);
    }

    private static void FriendLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);
    }

    private static void BagLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);
    }

    private static void TreasureLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject Secrtgb = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(Secrtgb);
    }

    private static void NoAllianceLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject Secrtgb = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(Secrtgb);
    }

    private static void RankWindowLoadBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject temp = (GameObject)Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(temp);
    }
    public static bool IsCurrentJunZhuID(long id)
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("IsCurrentJunZhuID")))
        {
            if (long.Parse(PlayerPrefs.GetString("IsCurrentJunZhuID")) == id)
            {
                return true;
            }
        }

        PlayerPrefs.SetString("IsCurrentJunZhuID", id.ToString());
        return false;
    }

    public static Vector3 IsCurrentJunZhuPos()
    {
        string[] value = PlayerPrefs.GetString("IsCurrentJunZhuPos").Split(':');
        return new Vector3(float.Parse(value[0]), float.Parse(value[1]), float.Parse(value[2]));
    }

    public static int IsCurrentJunZhuScene()
    {
        return PlayerPrefs.GetInt("IsCurrentJunZhuScene");
    }

    public static void SceneNumSet(int num)
    {
        PlayerPrefs.SetInt("IsCurrentJunZhuScene", num);

        // if (PlayerPrefs.GetInt("SceneNumSave") != null)

        if (GetSceneNum() == num)
        {
            SetChangeSceneInfo(false);
            SceneNumSave(num);
        }
        else
        {
            SetChangeSceneInfo(true);
            SceneNumSave(num);
        }
    }

    private static void SceneNumSave(int saveNum)
    {
        PlayerPrefs.SetInt("SceneNumSave", saveNum);
    }

    private static int GetSceneNum()
    {
        return PlayerPrefs.GetInt("SceneNumSave");
    }


    public static void SetChangeSceneInfo(bool change)
    {
        PlayerPrefs.SetString("IsChangeScene", change.ToString());
    }

    public static bool IsChangeScene()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("IsChangeScene")))
        {
            return bool.Parse(PlayerPrefs.GetString("IsChangeScene"));
        }
        return false;
    }

    public static void SetFenChengNum(int num)
    {
        PlayerPrefs.SetInt("IsFenChengNum", num);
    }

    public static int IsFenChengNum()
    {
        return PlayerPrefs.GetInt("IsFenChengNum");
    }
    public static string m_EquipSaveInfo = "1:3";
    public static void SetSelectEquipInfo(int index, int equip_num)
    {
        if (IsCurrentJunZhuID(JunZhuData.Instance().m_junzhuInfo.id))
        {
            m_EquipSaveInfo = index.ToString() + ":" + equip_num.ToString();
        }
        else
        {
            m_EquipSaveInfo = "1:3";
        }
    }
    public static void SetSelectEquipDefault()
    {
        m_EquipSaveInfo = "1:3";
    }

    public static int ParentPosOffset(int count, int distance)//坐标计算
    {
        if (count % 2 == 0)
        {
            if (count / 2 > 1)
            {
                return -1 * distance / 2 * (count / 2 + 1);
            }
            else
            {
                return -1 * distance / 2 * count / 2;
            }
        }
        else
        {
            return -1 * distance * (count / 2);
        }
    }

    public static string GetNeedString(string content,int length = 7)
    {
        if (!string.IsNullOrEmpty(content))
        {
            char[] input = content.ToCharArray();
            int size = input.Length;
            string resuilt = "";
            for (int i = 0; i < size; i++)
            {
                if (i < length)
                {
                    resuilt += input[i];
                }
            }
            return resuilt;
        }
        return null;
    }
    public static Vector3 GetCurrentPosition()
    {
        string[] pos = PlayerPrefs.GetString("IsCurrentJunZhuPos").Split(':');
        return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
    }

    public static string GetIdentityById(int id)
    {
        switch (id)
        {
            case 0:
                {
                    //  return LanguageTemplate.GetText(LanguageTemplate.Text.IDENTITY_0);
                    return "";
                }
                break;
            case 1:
                {
                    return  LanguageTemplate.GetText(LanguageTemplate.Text.IDENTITY_1)  ;
                }
                break;
            case 2:
                {
                    return  LanguageTemplate.GetText(LanguageTemplate.Text.IDENTITY_2);
                }
                break;
            default:
                break;

        }
        return null;

    }
    public static void ShowRAwardInfo(string _award)
    {
        List<RewardData> tempDataList = new List<RewardData>();
        if (_award.IndexOf('#') > -1)
        {
            string[] ss = _award.Split('#');
            for (int i = 0; i < ss.Length; i++)
            {
                string[] info = ss[i].Split(':');
                RewardData rr = new RewardData(int.Parse(info[1]), int.Parse(info[2]));
                tempDataList.Add(rr);
            }

        }
        else
        {
            string[] info = _award.Split(':');
            RewardData rr = new RewardData(int.Parse(info[1]), int.Parse(info[2]));
            //rr.itemId = int.Parse(info[1]);
            //rr.itemCount = int.Parse(info[2]);
            tempDataList.Add(rr);
        }
        GeneralRewardManager.Instance().CreateReward(tempDataList);
    }


    public static void ShowUnopen(int _id)
    {
        FunctionOpenTemp template = FunctionOpenTemp.GetTemplateById(_id);
        string str = template.m_sNotOpenTips;

        if (!string.IsNullOrEmpty(str) && !str.Equals("-1"))
        {
            ClientMain.m_UITextManager.createText(str);
        }
        return;
    }

    public struct RewardInfo
    {
        public int type;
        public int count;
        public int icon;
    }

    public static List<RewardInfo> GetRewardInfo(string _jiangli)
    {
        List<RewardInfo> listRewardInfo = new List<RewardInfo>();
        if (!string.IsNullOrEmpty(_jiangli))
        {
            if (_jiangli.IndexOf('#') > -1)
            {
                string[] tempAwardList = _jiangli.Split('#');
                for (int i = 0; i < tempAwardList.Length; i++)
                {
                    string[] tempAwardItemInfo = tempAwardList[i].Split(':');
                    RewardInfo rInfo = new RewardInfo();
                    rInfo.type = int.Parse(tempAwardItemInfo[0]);
                    rInfo.icon = int.Parse(tempAwardItemInfo[1]);
                    rInfo.count = int.Parse(tempAwardItemInfo[2]);
                    listRewardInfo.Add(rInfo);
                }
            }
            else
            {
                string[] tempAwardItemInfo = _jiangli.Split(':');
                RewardInfo rInfo = new RewardInfo();
                rInfo.type = int.Parse(tempAwardItemInfo[0]);
                rInfo.icon = int.Parse(tempAwardItemInfo[1]);
                rInfo.count = int.Parse(tempAwardItemInfo[2]);
                listRewardInfo.Add(rInfo);
            }

            return listRewardInfo;
        }
        else
        {
            return null;
//            Debug.Log("Award is Null !!!");
        }
    }
    public static bool SpecialSizeFit(int quality)
    {
        if (FrameQuality.Contains(quality - 1))
        {
            return true;
        }
        return false;
    }

    public static float DistanceCount(string name)
    {
        float distance = 0;
        string ss = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] dd = name.ToCharArray();
        int size = dd.Length;
        for (int i = 0; i < size; i++)
        {
            if (ss.IndexOf(dd[i]) > -1)
            {
                distance += 0.7f;
            }
            else
            {
                distance += 1.0f;
            }
        }
        return distance;
    }

    public static int RangNum(int min, int max)
    {
        return Random.Range(min, max);
    }
}
