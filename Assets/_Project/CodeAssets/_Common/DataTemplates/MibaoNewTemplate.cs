using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MibaoNewTemplate : XmlLoadManager 
{	
	public static List<MibaoNewTemplate> templates = new List<MibaoNewTemplate>();
	public int id;
	public int nameID;
	public int descID;
	public int pinzhi;
	public int color;
	public int iconID;
	public int suipianID;
	public int jinjieNum;
	public int gongji;
	public int fangyu;
	public int shengming;
	public string pinzhiName;
	public string showName;
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MiBaoNew.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
		
		//		Debug.Log ("j加载了ibaoSuiPian.xml");
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){
		{
			templates.Clear();
		}
		
		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "MiBaoNew" );
			
			if( !t_has_items ){
				break;
			}
			
			MibaoNewTemplate t_template = new MibaoNewTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nameID = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.descID = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.pinzhi = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.color = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.iconID = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.suipianID = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.jinjieNum = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();

				t_reader.MoveToNextAttribute();
				t_template.gongji = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.fangyu = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.shengming= int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();

				t_reader.MoveToNextAttribute();
				t_template.pinzhiName = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.showName = t_reader.Value;
		}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static MibaoNewTemplate GetTemplateByID (int skillId)
	{
		foreach (MibaoNewTemplate template in templates)
		{
			if (template.id == skillId)
			{
				return template;
			}
		}
		return null;
	}

	public static string GetPinzhiAllArr (int pinzhi)
	{
		int gongji = 0;
		int fangyu = 0;
		int shengming = 0;
		int bIndex = pinzhi * 9;
		int eIndex = (pinzhi + 1) * 9;
		for(int i = bIndex; i < eIndex; i ++)
		{
			gongji += templates[i].gongji;
			fangyu += templates[i].fangyu;
			shengming += templates[i].shengming;
		}

		string tempString = "";
		bool nextLink = false;
		if(gongji > 0)
		{
			tempString = "攻击提升：" + MyColorData.getColorString(4, gongji);
			nextLink = true;
		}
		if(fangyu > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "防御提升：" + MyColorData.getColorString(4, fangyu);
			nextLink = true;
		}
		if(shengming > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "生命提升：" + MyColorData.getColorString(4, shengming);
			nextLink = true;
		}
		return tempString;
	}
}
