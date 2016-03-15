using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class KingDetailInfo : MonoBehaviour
{
    public static int TransferBuwei(int original)
    {
        switch (original)
        {
            case 1: return 3;
            case 2: return 4;
            case 3: return 5;
            case 11: return 0;
            case 12: return 8;
            case 13: return 1;
            case 14: return 7;
            case 15: return 2;
            case 16: return 6;
            default: Debug.LogError("Error buwei in Transfer in DetailController, Buwei:" + original); return -1;
        }
    }

    /// <summary>
    /// Do not set this if not in rank sys.
    /// </summary>
    public KingDetailEquipInfo m_KingDetailEquipInfo;
    public ScaleEffectController m_ScaleEffectController;

    public UILabel m_LevelLabel;
    public UILabel m_JunxianLabel;
    public UILabel m_MoneyLabel;
    public UILabel m_BattleValue;
    public UILabel m_AttackLabel;
    public UILabel m_ProtectLabel;
    public UILabel m_LifeLabel;
    public UILabel m_KingNameLabel;
    public UILabel m_AllianceNameLabel;
    public GameObject m_PlayerParent;
    public GameObject m_PlayerModel;
    public GameObject m_PlayerLeft;
    public GameObject m_PlayerRight;

    public UISprite m_TitleSprite;

    public UISprite m_VotedSprite;

    public UIGrid m_Grid;
    public GameObject m_ButtonPrefab;

    public KingDetailData m_KingInfo;

    public struct KingDetailData
    {
        public int RoleID;
        public int Level;
        public string Junxian;
        public int JunxianRank;
        public int Money;
        public int BattleValue;
        public int Attack;
        public int Protect;
        public int Life;
        public string KingName;
        public string AllianceName;
        public int Title;
    }

    public void SetThis(KingDetailData data, List<KingDetailButtonController.KingDetailButtonConfig> configList)
    {
        m_KingInfo = data;

        if (m_KingInfo.Title > 0)
        {
            m_TitleSprite.gameObject.SetActive(true);
            m_TitleSprite.spriteName = m_KingInfo.Title.ToString();
        }
        else
        {
            m_TitleSprite.gameObject.SetActive(false);
        }

        //Set right part data.
        m_AttackLabel.text = m_KingInfo.Attack.ToString();
        m_ProtectLabel.text = m_KingInfo.Protect.ToString();
        m_LifeLabel.text = m_KingInfo.Life.ToString();
        m_LevelLabel.text = m_KingInfo.Level.ToString();
        m_BattleValue.text = m_KingInfo.BattleValue.ToString();
        m_MoneyLabel.text = m_KingInfo.Money.ToString();

        m_JunxianLabel.text = m_KingInfo.JunxianRank < 0 ? "无军衔" : m_KingInfo.Junxian;

        m_KingNameLabel.text = m_KingInfo.KingName;

        m_AllianceNameLabel.text = (m_KingInfo.AllianceName == "无") ? "无联盟" : ("<" + m_KingInfo.AllianceName + ">");

        //Set right part buttons.
        while (m_Grid.transform.childCount != 0)
        {
            var child = m_Grid.transform.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }

        if (configList != null)
        {
            for (int i = 0; i < configList.Count; i++)
            {
                var temp = Instantiate(m_ButtonPrefab) as GameObject;
                var controller = temp.GetComponent<KingDetailButtonController>();

                controller.SetThis(configList[i]);
                TransformHelper.ActiveWithStandardize(m_Grid.transform, temp.transform);
            }

            m_Grid.Reposition();
        }
    }

    public GameObject TopLeftAnchor;

    void Awake()
    {
        MainCityUI.setGlobalBelongings(gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY);
        MainCityUI.setGlobalTitle(TopLeftAnchor, "玩家信息", 0, 0);
    }

    void Start()
    {
        m_ScaleEffectController.OpenCompleteDelegate = EndDelegate;

        //Close guide.
        CityGlobalData.m_isRightGuide = true;
    }

    void OnCloseWindow()
    {
        Destroy(gameObject);
    }

    public void OnCloseClick()
    {
        m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
    }

    public void PlayerLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        m_PlayerModel = Instantiate(p_object) as GameObject;

        TransformHelper.ActiveWithStandardize(m_PlayerParent.transform, m_PlayerModel.transform);

        m_PlayerModel.name = p_object.name;

        m_PlayerModel.GetComponent<NavMeshAgent>().enabled = false;

        m_PlayerModel.GetComponent<Animator>().Play("zhuchengidle");

        m_PlayerModel.transform.rotation = new Quaternion(0, 163, 0, 0);
    }

    public void EndDelegate()
    {
        //        Debug.Log("===1");

        Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + m_KingInfo.RoleID),
                                PlayerLoadCallBack);
    }
}
