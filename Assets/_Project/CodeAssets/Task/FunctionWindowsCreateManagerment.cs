using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;
public class FunctionWindowsCreateManagerment : MonoBehaviour
{
   private static int BigHouseId = 0;
   private static int SmallHouseId = 0;
    public static void FunctionWindowCreate(int id)
    {
        switch (id)
        {
            //setting up
            case 2:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SETTINGS_UP_LAYER),
                                     SettingUpLoadCallback);
                }
                break;
            //goto rank
            case 210:
                {
                    //Add rank sys here.
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.RANK_WINDOW),
                                    RankWindowLoadBack);
                }
                break;
            //Recharge
            case 13:
                {
                    TopUpLoadManagerment.m_instance.LoadPrefab(true);
                }
                break;
            //bag sys
            case 3:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_BAG),
                                        BagLoadCallback);
                }
                break;
            //friend sys
            case 4:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FRIEND_OPERATION),
                                      FriendLoadCallback);
                }
                break;
            //serach treasure
            case 11:
                {

                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TANBAO),
                                            SerachTreasureLoadCallback);
                }
                break;
            //task sys
            case 5:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TASK),
                                  TaskLoadCallback);
                }
                break;
            //treasure sys
            case 6:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_SECRET),
                                            TreasureLoadCallback);
                }
                break;
            //alliance sys
            case 104:
                {
                    //                        AllianceData.Instance.RequestData();

                    if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_NO_SELF_ALLIANCE),
                                                 NoAllianceLoadCallback);
                    }
                    if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_HAVE_ROOT),
                                                AllianceHaveLoadCallback);
                    }
                }
                break;
            case 14:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ACTIVITY_LAYER), OnActivityLoadCallBack);
                }
                break;
            //新手在线礼包
            case 15:
                {
                    CityGlobalData.m_Limite_Activity_Type = 1542000;

                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_REWARD_ROOT),
                                RewardCallback);

                }
                break;
            //新手七日礼包
            case 16:
                {
                    CityGlobalData.m_Limite_Activity_Type = 1543000;

                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_REWARD_ROOT),
                                  RewardCallback);
                }
                break;
            //king
            case 200:
                {
                    //Add king here.
               

                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.JUN_ZHU_LAYER_AMEND),
                                            JunzhuLayerLoadCallback);
                    break;
                }
            case 900001: //买铜币 900001
                {
                    //Add king here.

                    JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);

                    break;
                }
            case 900002: //买体力  900002
                {
                    //Add king here.


                    JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
                    break;
                }
            case 7: //买体力  900002
                {
                    //Add king here.


                    DoGoHome();
                    break;
                }
            default:

                break;
        }

    }

    private static void DoGoHome()
    {
        //  Debug.Log("7777777777777777777777777777777777777");
        foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
        {
            if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId > 50)
            {
                //   Debug.Log("7777777777777777777777777777777777777");
                BigHouseId = item.Value.locationId;
                break;
            }
        }
        if (BigHouseId > 0)
        {
            //  Debug.Log("7777777777777777777777777777777777777");
            CityGlobalData.m_isNavToHouse = true;
            NpcManager.m_NpcManager.setGoToNpc(BigHouseId + 1000);
        }
        else
        {
            // Debug.Log("7777777777777777777777777777777777777");
            CityGlobalData.m_isNavToHome = true;
            foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
            {
                //    Debug.Log("7777777777777777777777777777777777777");
                if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId <= 50)
                {
                    //     Debug.Log("7777777777777777777777777777777777777");
                    SmallHouseId = item.Value.locationId;
                    break;
                }
            }
            if (CityGlobalData.m_isAllianceTenentsScene)
            {
                //  Debug.Log("7777777777777777777777777777777777777");
                if (!NpcManager.m_NpcManager.m_npcObjectItemDic.ContainsKey(SmallHouseId + 1000))
                {
                    //        Debug.Log("7777777777777777777777777777777777777");
                    NpcManager.m_NpcManager.setGoToNpc(SmallHouseId + 1000);
                    CityGlobalData.m_isNavToAllianCityToTenement = true;
                }
                else
                {
                    //   Debug.Log("7777777777777777777777777777777777777");
                    NpcManager.m_NpcManager.setGoToSelfTenement(SmallHouseId + 1000);
                }
            }
            else
            {
                //   Debug.Log("7777777777777777777777777777777777777");
                NpcManager.m_NpcManager.setGoToTenementNpc(SmallHouseId + 1000);
            }
        }
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
       string [] value = PlayerPrefs.GetString("IsCurrentJunZhuPos").Split(':');
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


    public static void SetChangeSceneInfo( bool change)
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

    public static void  SetFenChengNum(int num)
    {
        PlayerPrefs.SetInt("IsFenChengNum", num);
    }

    public static int IsFenChengNum()
    {
      return  PlayerPrefs.GetInt("IsFenChengNum");
    }
    public static string m_EquipSaveInfo = "1:3";
    public static void SetSelectEquipInfo(int index,int equip_num)
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

    public static int ParentPosOffset(int count, int distance)
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

    public static string  GetNeedString(string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            char[] input = content.ToCharArray();
            int size = input.Length;
            string resuilt = "";
            for (int i = 0; i < size; i++)
            {
                if (i < 7)
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
      string [] pos = PlayerPrefs.GetString("IsCurrentJunZhuPos").Split(':');
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
                    return "(" +LanguageTemplate.GetText(LanguageTemplate.Text.IDENTITY_1) +")";
                }
                break;
            case 2:
                {
                    return "(" + LanguageTemplate.GetText(LanguageTemplate.Text.IDENTITY_2) + ")";
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

        if (!string.IsNullOrEmpty(str) && !str .Equals("-1"))
        {
            ClientMain.m_UITextManager.createText(str);
        }
        return;
    }


}
