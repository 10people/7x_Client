using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class UnionMemberControllor : MonoBehaviour
{
	public UnionDetailUIControllor controllor;

	public UILabel labelName;

	public UILabel labelShengWang;

	public UILabel labelRenKou;

	public UILabel labelLevel;

	public UILabel labelMemberNum;

	public GameObject memberTemple;

	public GameObject memberMenu;


	private List<GameObject> memberList = new List<GameObject>();

	private UnionMemberDate focusMember;

	private string tempStrTransfer = "";

	private string tempStrAdvance = "";

	private string tempStrDemotion = "";

	private string tempStrRemove = "";


	public void refreshDate()
	{
		memberMenu.SetActive(false);

		labelName.text = controllor.unionDate.unionName;

		labelShengWang.text = controllor.unionDate.shengwang + "";

		labelRenKou.text = controllor.unionDate.renkou + "";

		labelLevel.text = controllor.unionDate.level + "";

		labelMemberNum.text = controllor.unionDate.member + "";

		foreach(GameObject gc in memberList)
		{
			Destroy(gc);
		}

		memberList.Clear();

		memberTemple.SetActive(false);

		for(int i = 0; controllor.unionDate.members != null && i < controllor.unionDate.members.Count; i++)
		{
			GameObject memberObject = (GameObject)Instantiate(memberTemple);

			memberObject.SetActive(true);

			memberObject.transform.parent = memberTemple.transform.parent;

			memberObject.transform.localScale = memberTemple.transform.localScale;

			memberObject.transform.localPosition = memberTemple.transform.localPosition + new Vector3(0, -100 * i, 0);

			UnionMember member = (UnionMember)memberObject.GetComponent("UnionMember");

			member.refreshDate(controllor.unionDate.members[i]);

			memberList.Add(memberObject);
		}
	}

	public void OnSelect(UnionMember member)
	{
		int id = (int)JunZhuData.Instance().m_junzhuInfo.id;

		if(controllor.unionDate.leaderId == id)
		{
			OnShowMenu(member);
		}
		else
		{
			focusMember = member.memberDate;

			OnShowMemberDetail();
		}
	}

	private void OnShowMenu(UnionMember member)
	{
		focusMember = member.memberDate;

		memberMenu.SetActive(true);

		float y = member.transform.localPosition.y + member.transform.parent.localPosition.y;

		memberMenu.transform.localPosition += new Vector3(0, y - memberMenu.transform.localPosition.y, 0);
	}

	public void OnHideMenu()
	{
		focusMember = null;

		memberMenu.SetActive(false);
	}

	public void OnShowHintTransfer()
	{
		if(tempStrTransfer.Length == 0)
		{
			tempStrTransfer = controllor.hintControllor.labelTransfer.text;
		}

		controllor.hintControllor.labelTransfer.text = tempStrTransfer + focusMember.memberName;

		controllor.OnShowHint(UnionHintControllor.HintType.Transfer, focusMember);
	}

	public void OnShowHintAdvance()
	{
		if(tempStrAdvance.Length == 0)
		{
			tempStrAdvance = controllor.hintControllor.labelAdvance.text;
		}
		
		controllor.hintControllor.labelAdvance.text = tempStrAdvance + focusMember.memberName;
		
		controllor.OnShowHint(UnionHintControllor.HintType.Advance, focusMember);
	}

	public void OnShowHintDemotion()
	{
		if(tempStrDemotion.Length == 0)
		{
			tempStrDemotion = controllor.hintControllor.labelDemotion.text;
		}
		
		controllor.hintControllor.labelDemotion.text = tempStrDemotion + focusMember.memberName;
		
		controllor.OnShowHint(UnionHintControllor.HintType.Demotion, focusMember);
	}

	public void OnShowHintRemove()
	{
		if(tempStrRemove.Length == 0)
		{
			tempStrRemove = controllor.hintControllor.labelRemove.text;
		}
		
		controllor.hintControllor.labelRemove.text = tempStrRemove + focusMember.memberName;
		
		controllor.OnShowHint(UnionHintControllor.HintType.Remove, focusMember);
	}

	public void OnShowMemberDetail()
	{
		controllor.OnShowMemberDetail(focusMember);
	}

	public void OnClose()
	{
		controllor.OnShowNoticeLayer();
	}

}
