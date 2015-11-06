using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillDescribe : MonoBehaviour 
{
	public int m_skillType;

	public UILabel m_SkillTile;

	public UILabel m_SkillDis;
	
	public List<UILabel> m_labelList = new List<UILabel>();

	public List<UILabel> m_labelNameList = new List<UILabel>();

	public List<UISprite> m_skillIconList = new List<UISprite>();

	void OnEnable()
	{
		List<SkillTemplate> tempLateList = SkillTemplate.GetSkillOfJunZhu(m_skillType);

		for(int i = 0; i< m_labelList.Count;i++)
		{
			m_labelList[i].text = DescIdTemplate.GetDescriptionById(tempLateList[i].funDesc);
		}

		for(int i = 0;i < m_labelNameList.Count;i ++)
		{
			m_labelNameList[i].text = NameIdTemplate.GetName_By_NameId(tempLateList[i].skillName);
		}

		for(int i = 0;i < m_skillIconList.Count;i ++)
		{
			m_skillIconList[i].spriteName = tempLateList[i].id.ToString();
		}
	}
}
