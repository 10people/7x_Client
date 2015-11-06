using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LabelTemplate : XmlLoadManager
{
	//<LabelTemplate tempId="80001" icon="1" labelType="1" labelName="5001" description="5001" heroID="51" effectId="0" />

	public int tempId;

	public int icon;

	public int labelType;

	public int labelName;

	public int description;

	public int heroID;

	public int effectId;


	public static List<LabelTemplate> templates = new List<LabelTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LabelTemplate.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LabelTemplate" );
			
			if( !t_has_items ){
				break;
			}
			
			LabelTemplate t_template = new LabelTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.tempId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.labelType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.labelName = int.Parse( t_reader.Value );

//				description="5001" heroID="501" effectId="0" />
				t_reader.MoveToNextAttribute();
				t_template.description = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.heroID = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.effectId = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static LabelTemplate getLabelTemplateByLabelTempId(int tempId)
	{
		foreach(LabelTemplate template in templates)
		{
			if(template.tempId == tempId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get LabelTemplate with tempId " + tempId);
		
		return null;
	}

}
