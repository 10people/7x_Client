using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class UnionApplyAvatar : MonoBehaviour
{
	public UnionApplysControllor controllor;

	public UILabel labelJunXian;

	public UISprite spriteAvatar;

	public UILabel labelLevel;

	public UILabel labelName;


	private UnionMemberDate memberDate;


	public void refreshDate(UnionMemberDate _memberDate)
	{
		memberDate = _memberDate;

		BaiZhanTemplate template = BaiZhanTemplate.getBaiZhanTemplateById(1);//memberDate.junXian);

		labelJunXian.text = NameIdTemplate.getNameIdTemplateByNameId(template.templateName).Name;

		labelLevel.text = memberDate.level + "";

		labelName.text = memberDate.memberName;
	}

	public void OnAgree()
	{
		controllor.OnAgree(memberDate);
	}

	public void OnInvite()
	{
		controllor.OnInvite(memberDate);
	}

}
