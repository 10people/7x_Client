using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class TalentTemplate : XmlLoadManager
{
	public int id;

	public int type;

	public string tianfuName;
	
	public string funDesc;
	
	public int maxLv;

	public int arrID;

	public int expID;

	public List<int> listFrontPoint = new List<int>();
	
	public int FrontPointLv;

	public int UPDianshu;
	
	public static List<TalentTemplate> templates= new List<TalentTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Talent.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
		
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing( "Talent" );
			
			if( !t_has_items ){
				break;
			}
			
			TalentTemplate t_template = new TalentTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.tianfuName = t_reader.Value;


				t_reader.MoveToNextAttribute();
				t_template.funDesc = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.maxLv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.arrID = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.expID = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				string[] tempString = t_reader.Value.Split(',');
				for(int i = 0; i < tempString.Length; i ++)
				{
					t_template.listFrontPoint.Add(int.Parse(tempString[i]));
				}

				t_reader.MoveToNextAttribute();
				t_template.FrontPointLv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.UPDianshu = int.Parse( t_reader.Value );
			}
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static TalentTemplate getSkillTemplateById(int id)
	{
		foreach(TalentTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get SkillTemplate with id " + id);
		
		return null;
	}
}
