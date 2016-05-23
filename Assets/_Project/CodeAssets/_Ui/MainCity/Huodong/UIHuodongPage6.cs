using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIHuodongPage6 : MYNGUIPanel 
{
	public UILabel m_labelDis0;
	public UILabel m_labelDis1;
	public UILabel m_labelDis2;
	public GameObject m_objCopy;
	public ErrorMessage m_Info;
	void Start () 
	{

	}
	
	void OnDestroy()
	{

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void setData(ErrorMessage data)
	{
		m_Info = data;

		string[] tempData = m_Info.errorDesc.Split(new string[]{"#=#=#"}, System.StringSplitOptions.None);
		m_labelDis0.text = tempData[0];
		m_labelDis1.text = "官方QQ群：[ff0000]" + tempData[1] + "[-]";
		m_labelDis2.text = "官方QQ群：" + tempData[1];
		DeviceHelper.SetClipBoard(tempData[1]);
	}
	
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("quedingCopy") != -1)
		{
			m_objCopy.SetActive(false);
		}
		else if(ui.name.IndexOf("Copy") != -1)
		{
			m_objCopy.SetActive(true);
		}
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
}
