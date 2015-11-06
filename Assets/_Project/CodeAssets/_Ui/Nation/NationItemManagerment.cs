using UnityEngine;
using System.Collections;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NationItemManagerment : MonoBehaviour
{

    public UILabel m_LabelRank;
    public UILabel m_LabelNation;
    public UILabel m_LabelShengWang;

    public UISprite m_SpriteBack;

    public void ShowInfo(GuojiaRankInfo info)
    { 
        //if (JunZhuData.Instance().m_junzhuInfo.guoJiaId == info.guojiaId)
        //{
        //    m_SpriteBack.spriteName = "bg1";
        //}
        //else if (info.rank % 2 == 0)
        //{
        //    m_SpriteBack.spriteName = "bg2";
        //}
        m_LabelRank.text = (info.rank + 1).ToString();
        m_LabelNation.text = NameIdTemplate.GetName_By_NameId(info.guojiaId); ;
        m_LabelShengWang.text = info.shengwang.ToString();
    }
   


}
