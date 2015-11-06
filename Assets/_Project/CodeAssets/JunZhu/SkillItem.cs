using UnityEngine;
using System.Collections;

public class SkillItem : MonoBehaviour
{
    public delegate void SkillClick(SkillItem tempSkill);

    public event SkillClick m_skillClick;

    public UISprite m_skill;

    public int m_index;

    [HideInInspector]
    public bool m_isCheck = false;

    void OnClick()
    {
        if (m_skillClick != null)
        {
            m_skillClick(this.GetComponent<SkillItem>());
        }
    }
}
