using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIHuodongPage7 : MYNGUIPanel 
{
	public UILabel m_labelDis0;
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
		m_labelDis0.text = m_Info.errorDesc;
	}
	
	public override void MYClick(GameObject ui)
	{

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
