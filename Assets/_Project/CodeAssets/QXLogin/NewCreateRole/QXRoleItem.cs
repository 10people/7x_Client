using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXRoleItem : MonoBehaviour {
	//148,124
	public UISprite m_roleIcon;

	public GameObject m_lock;

	public bool M_Active = false;//是否解锁

	public void InItRole (QXSelectRolePage.SelectType tempType,int tempRoleId,int curId,List<int> tempList)
	{
		M_Active = tempType == QXSelectRolePage.SelectType.UNLOCK_ROLE ? (tempList.Contains (tempRoleId) ? true : false) : true;
	
		m_lock.SetActive (!M_Active);

		bool active = tempRoleId == curId ? true : false;
		m_roleIcon.spriteName = QXSelectRolePage.m_instance.M_RoleDic[tempRoleId][active ? 2 : 1];
		m_roleIcon.SetDimensions (292,active ? 148 : 124);
		m_roleIcon.transform.localPosition = new Vector3 (0,active ? 10 : -2,0);
		m_lock.transform.localPosition = new Vector3 (-100,active ? -50 : -38,0);
	}
}
