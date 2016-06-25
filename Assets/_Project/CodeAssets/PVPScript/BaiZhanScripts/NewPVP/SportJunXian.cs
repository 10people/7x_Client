using UnityEngine;
using System.Collections;

public class SportJunXian : MonoBehaviour {

	[HideInInspector]
	public BaiZhanTemplate m_sportTemp;

	public UISprite m_roomIcon;
	public UISprite m_junXianIcon;
	public UILabel m_junXian;

	public void InItJunXianRoom ()
	{
		UIDragObject dragObj = m_roomIcon.GetComponent<UIDragObject> ();
		dragObj.enabled = !QXComData.CheckYinDaoOpenState (100200);

		m_roomIcon.spriteName = m_sportTemp.icon.ToString ();
		m_junXianIcon.spriteName = "junxian" + m_sportTemp.icon.ToString ();
		m_junXian.text = NameIdTemplate.GetName_By_NameId (m_sportTemp.templateName);
		BoxCollider roomBox = m_roomIcon.GetComponent<BoxCollider> ();
		string[] posStr = SportPage.m_instance.M_RoomBoxDic [m_sportTemp.jibie].Split (':');
		Vector3 boxSize = roomBox.size;
		boxSize.x = float.Parse (posStr [0]);
		boxSize.y = float.Parse (posStr [1]);
		roomBox.size = boxSize;
	}
}
