using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class guyongBing : MonoBehaviour {

	[HideInInspector] public GuYongBing m_guyongBing;
    //public UISprite GYb_Type;
    //public UISprite GYb_Icon;



	public void init()
	{
//		GuYongBingTempTemplate mGuYongBingTempTemplate = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id (m_guyongBing.id);

		//GYb_Type.spriteName = "";//mGuYongBingTempTemplate.icon.toString();
		//GYb_Icon.spriteName = "";//mGuYongBingTempTemplate.name.ToString();
	}
}
