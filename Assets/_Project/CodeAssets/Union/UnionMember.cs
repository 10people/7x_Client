using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class UnionMember : MonoBehaviour
{
	public UnionMemberControllor controllor;

	public UILabel labelJunxian;

	public UILabel labelLevel;

	public UILabel labelName;

	public UILabel labelGrade;

	public UILabel labelShengwang;


	[HideInInspector] public UnionMemberDate memberDate;


	public void refreshDate(UnionMemberDate _memberDate)
	{
		memberDate = _memberDate;

		BaiZhanTemplate template = BaiZhanTemplate.getBaiZhanTemplateById(1);//memberDate.junXian);

		labelJunxian.text = NameIdTemplate.getNameIdTemplateByNameId(template.templateName).Name;

		labelLevel.text = memberDate.level + "";

		labelName.text = memberDate.memberName;

		labelGrade.text = memberDate.grade;

		labelShengwang.text = memberDate.shengWang + "";
	}

	public void OnPress(bool pressed)
	{
		if(pressed)
		{
			controllor.OnHideMenu();
		}
	}

	public void OnClick()
	{
		controllor.OnSelect(this);
	}

}
