using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class _AllMember : MonoBehaviour {

	public MemberInfo mMemberInfo;

	public UILabel m_name;

	public UILabel m_Level;

	public UILabel m_Zhiwei;

	public UILabel m_Contrbiution;

	public void init()
	{
		m_name.text = mMemberInfo.name;

		m_Level.text = mMemberInfo.level.ToString ();

		if(mMemberInfo.identity == 0)
		{
			m_Zhiwei.text = "盟员";
		}
		if(mMemberInfo.identity == 1)
		{
			m_Zhiwei.text = "副盟主";
		}
		if(mMemberInfo.identity == 2)
		{
			m_Zhiwei.text = "盟主";
		}
		m_Contrbiution.text = mMemberInfo.contribution.ToString ();
	}
	public void ShowBtnList()
	{

	}
}
