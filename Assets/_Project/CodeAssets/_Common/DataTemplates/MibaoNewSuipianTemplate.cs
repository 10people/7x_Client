using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MibaoNewSuipianTemplate : XmlLoadManager 
{	
	public static List<MibaoNewSuipianTemplate> templates = new List<MibaoNewSuipianTemplate>();
	public int id;
	public int nameID;
	public int descID;
	public int pinzhi;
	public int color;
	public int iconID;
	public int mibaoID;
	public string guanqiaID;
	public string pinzhiName;
	public string showName;
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MiBaoNewSuiPian.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
		
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
			t_has_items = t_reader.ReadToFollowing( "MiBaoNewSuiPian" );
			
			if( !t_has_items ){
				break;
			}
			
			MibaoNewSuipianTemplate t_template = new MibaoNewSuipianTemplate();
			
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
				t_template.mibaoID = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.guanqiaID = t_reader.Value;

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
	
	public static MibaoNewSuipianTemplate GetTemplateByID (int skillId)
	{
		foreach (MibaoNewSuipianTemplate template in templates)
		{
			if (template.id == skillId)
			{
				return template;
			}
		}
		return null;
	}
}
