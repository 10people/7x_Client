using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

public class ABBuyItemWindowManager : MonoBehaviour
{
    public Transform m_IconParent;

    public UILabel m_CostLabel;
    public UILabel m_RemainingTimesLabel;
    public UILabel m_NameLabel;
    public UILabel m_DescLabel;
    public UILabel m_TitleLabel;

    public GameObject VipObject;
    public UISprite VipSprite;

    public UISprite OkSprite;

    public enum Type
    {
        Blood,
        Summon
    }
    private Type m_type;

    public void SetThis(Type p_type, int p_costNum, int p_remainingTimes)
    {
        m_type = p_type;
        costNum = p_costNum;
        remainingTimes = p_remainingTimes;

        if (p_type == Type.Blood)
        {
            itemID = 910012;
        }
        else if (p_type == Type.Summon)
        {
            itemID = 910011;
        }

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconLoadedCallBack);

        m_CostLabel.text = p_costNum.ToString();
        if (p_remainingTimes >= 0)
        {
            m_RemainingTimesLabel.text = "剩余" + ColorTool.Color_Red_FF0000 + p_remainingTimes + "[-]" + "次";
            m_RemainingTimesLabel.gameObject.SetActive(true);
        }
        else
        {
            m_RemainingTimesLabel.gameObject.SetActive(false);
        }
        m_NameLabel.text = NameIdTemplate.getNameIdTemplateByNameId(itemID).Name;
        m_DescLabel.text = DescIdTemplate.getDescIdTemplateByNameId(itemID).description;
        m_TitleLabel.text = "购买" + NameIdTemplate.getNameIdTemplateByNameId(itemID).Name;

        switch (p_type)
        {
            case Type.Blood:
                {
                    int vip = VipTemplate.templates.Where(item => item.ABBlood > 0).OrderBy(item => item.lv).First().lv;

                    VipObject.SetActive(true);
                    VipSprite.spriteName = "v" + vip;

                    if (JunZhuData.Instance().m_junzhuInfo.vipLv >= vip && p_remainingTimes == 0)
                    {
                        OkSprite.color = new Color(.5f, .5f, .5f, 1);
                    }
                    else
                    {
                        OkSprite.color = new Color(1, 1, 1, 1);
                    }

                    break;
                }
            case Type.Summon:
                {
                    OkSprite.color = new Color(1, 1, 1, 1);

                    VipObject.SetActive(false);

                    break;
                }
            default:
                {
                    VipObject.SetActive(false);

                    break;
                }
        }
    }

    private int itemID;
    private int costNum;
    private int remainingTimes;

    private void OnIconLoadedCallBack(ref WWW www, string path, Object prefab)
    {
        while (m_IconParent.childCount != 0)
        {
            var child = m_IconParent.GetChild(0);
            Destroy(child.gameObject);
            child.parent = null;
        }

        var ins = Instantiate(prefab) as GameObject;
        TransformHelper.ActiveWithStandardize(m_IconParent, ins.transform);
        var manager = ins.GetComponent<IconSampleManager>();
        manager.SetIconByID(itemID, "", 10);
        manager.SetIconPopText();
    }

    public void OnBuy1Click()
    {
        DoBuy(1);
    }

    public void OnBuy3Click()
    {
        DoBuy(3);
    }

    public void OnBuy5Click()
    {
        DoBuy(5);
    }

    private void DoBuy(int num)
    {
        switch (m_type)
        {
            case Type.Blood:
                {
                    int vip = VipTemplate.templates.Where(item => item.ABBlood > 0).OrderBy(item => item.lv).First().lv;

                    if (JunZhuData.Instance().m_junzhuInfo.vipLv < vip)
                    {
                        CommonBuy.Instance.ShowVIP();
                        OnCloseWindowClick();
                        return;
                    }
                    else if (remainingTimes == 0)
                    {
                        ClientMain.m_UITextManager.createText("今日可购买药水次数已用尽");
                        OnCloseWindowClick();
                        return;
                    }
                    break;
                }
            case Type.Summon:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }

        if (JunZhuData.Instance().m_junzhuInfo.yuanBao >= costNum * num)
        {
            BuyXuePingReq temp = new BuyXuePingReq()
            {
                code = 10000 + num
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, temp);
            byte[] t_protof;
            t_protof = tempStream.ToArray();

            if (m_type == Type.Blood)
            {
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_BUY_XueP, ref t_protof);
            }
            else if (m_type == Type.Summon)
            {
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_BUY_Summon, ref t_protof);
            }
        }
        else
        {
            CommonBuy.Instance.ShowIngot();
            OnCloseWindowClick();
        }
    }

    public void OnCloseWindowClick()
    {
        gameObject.SetActive(false);
    }
}
