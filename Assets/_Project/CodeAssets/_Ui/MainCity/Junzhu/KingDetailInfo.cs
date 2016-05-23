using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
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

    public static Dictionary<int, string> TransferNation = new Dictionary<int, string>()
    {
        {1, "齐"}, {2, "楚"}, {3, "燕"}, {4, "韩"}, {5, "赵"}, {6, "魏"}, {7, "秦"},
    };

    /// <summary>
    /// Do not set this if not in rank sys.
    /// </summary>
    public KingDetailEquipInfo m_KingDetailEquipInfo;
    public KingDetailMibaoInfo m_KingDetailMibaoInfo;
    public ScaleEffectController m_ScaleEffectController;

    public UILabel m_LevelLabel;
    public UILabel m_JunxianLabel;
    public UILabel m_ScoreLabel;
    public UILabel m_BattleValue;
    public UILabel m_KingNameLabel;
    public UILabel m_AllianceNameLabel;
    public UILabel m_NationLabel;
    public GameObject m_PlayerParent;
    public GameObject m_PlayerModel;
    public GameObject m_PlayerLeft;
    public GameObject m_PlayerRight;

    public UISprite m_TitleSprite;

    public UISprite m_VotedSprite;

    public UIGrid m_Grid;
    public GameObject m_ButtonPrefab;

    public JunZhuInfo m_KingInfo;

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
        public int Nation;
    }

    public void SetThis(JunZhuInfo data, List<KingDetailButtonController.KingDetailButtonConfig> configList)
    {
        m_KingInfo = data;

        if (m_KingInfo.chenghao > 0)
        {
            m_TitleSprite.gameObject.SetActive(true);
            m_TitleSprite.spriteName = m_KingInfo.chenghao.ToString();
        }
        else
        {
            m_TitleSprite.gameObject.SetActive(false);
        }

        //Set right part data.
        m_LevelLabel.text = "Lv " + m_KingInfo.level;
        m_BattleValue.text = m_KingInfo.zhanli.ToString();
        m_ScoreLabel.text = m_KingInfo.gongjin.ToString();

        m_JunxianLabel.text = m_KingInfo.junxianRank < 0 ? "无军衔" : m_KingInfo.junxian;

        m_KingNameLabel.text = m_KingInfo.name;

        m_AllianceNameLabel.text = (string.IsNullOrEmpty(m_KingInfo.lianMeng) || m_KingInfo.lianMeng == "无") ? "无联盟" : ("<" + m_KingInfo.lianMeng + ">");

        if (TransferNation.ContainsKey(m_KingInfo.guojiaId))
        {
            m_NationLabel.text = TransferNation[m_KingInfo.guojiaId] + "国";
        }

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

        m_KingDetailEquipInfo.m_BagItemDic = m_KingInfo.equip.items.Where(item => item.buWei > 0).ToDictionary(item => TransferBuwei(item.buWei));
        m_KingDetailEquipInfo.m_BagItemDic = m_KingInfo.equip.items.Where(item => item.buWei > 0).ToDictionary(item => TransferBuwei(item.buWei));
        m_KingDetailMibaoInfo.m_MibaoInfoResp = m_KingInfo.mibaoInfoResp;

        m_KingDetailMibaoInfo.setDataMiBao();
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

        var nav = m_PlayerModel.GetComponent<NavMeshAgent>();
        if (nav != null)
        {
            nav.enabled = false;
        }

        m_PlayerModel.GetComponent<Animator>().Play("zhuchengidle");

        m_PlayerModel.transform.rotation = new Quaternion(0, 163, 0, 0);
    }

    public void EndDelegate()
    {
        Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + m_KingInfo.roleId),
                                PlayerLoadCallBack);
    }
}
