using UnityEngine;
using System.Collections;

public class EnterSkillItems : ChangeLayer
{
    void OnClick()
    {
        CityGlobalData.m_skillType = int.Parse(this.gameObject.name);

        base.Change();
    }
}
