using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

public class ABBuyItemWindowManager : MonoBehaviour
{
    public Transform m_IconParent;

    public UILabel m_CostLabel;
    public UILabel m_TotalLabel;

    public enum Type
    {
        Blood,
        Summon
    }
    private Type m_type;

    public void SetThis(Type p_type, int p_costNum, int p_totalNum)
    {
        m_type = p_type;
        costNum = p_costNum;

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
        m_TotalLabel.text = p_totalNum.ToString();
    }

    private int itemID;
    private int costNum;

    private void OnIconLoadedCallBack(ref WWW www, string path, Object prefab)
    {
        var ins = Instantiate(prefab) as GameObject;
        TransformHelper.ActiveWithStandardize(m_IconParent, ins.transform);
        var manager = ins.GetComponent<IconSampleManager>();
        manager.SetIconByID(itemID, "", 10);
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
            CommonBuy.Instance.ShowRecharge();
        }

        OnCloseWindowClick();
    }

    public void OnCloseWindowClick()
    {
        gameObject.SetActive(false);
    }
}
