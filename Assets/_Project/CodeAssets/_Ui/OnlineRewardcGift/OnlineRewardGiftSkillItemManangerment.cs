using UnityEngine;
using System.Collections;

public class OnlineRewardGiftSkillItemManangerment : MonoBehaviour
{
    public UILabel m_labName;
    public UILabel m_labDes;
    public UISprite m_SkillIcon;

    public void ShowInfo(OnlineRewardcGiftDayManagerment.SkillsInfo skillinfo)
    {
        m_labName.text = MyColorData.getColorString(22, LanguageTemplate.GetText(int.Parse(skillinfo.name)));
        m_labDes.text = LanguageTemplate.GetText(int.Parse(skillinfo.des));
        m_SkillIcon.spriteName = skillinfo.icon.ToString();
    }
}
