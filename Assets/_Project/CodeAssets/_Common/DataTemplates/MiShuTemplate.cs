using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MishuTemplate : XmlLoadManager 
{	
	public static List<MishuTemplate> templates = new List<MishuTemplate>();
	public int id;
	public string pinzhiName;
	public string name;
	public int iconID;
	public int color;
	public int pinzhi;

	public int gongji;
	public int fangyu;
	public int shengming;
	public int wqSH;
	public int wqJM;
	public int wqBJ;
	public int wqRX;
	public int jnSH;
	public int jnJM;
	public int jnBJ;
	public int jnRX;
	public string tileLabel;
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Mishu.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "Mishu" );
			
			if( !t_has_items ){
				break;
			}
			
			MishuTemplate t_template = new MishuTemplate();
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.pinzhiName = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.name = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.iconID = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.color = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.pinzhi = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.gongji = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.fangyu = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.shengming = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.wqSH = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.wqJM = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.wqBJ = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.wqRX = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.jnSH = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.jnJM = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.jnBJ = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.jnRX = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.tileLabel = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static MishuTemplate GetTemplateByID (int skillId)
	{
		foreach (MishuTemplate template in templates)
		{
			if (template.id == skillId)
			{
				return template;
			}
		}
		return null;
	}
}
