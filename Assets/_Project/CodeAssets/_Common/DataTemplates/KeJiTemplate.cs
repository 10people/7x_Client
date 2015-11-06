using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class KeJiTemplate : XmlLoadManager
{
	//<Keji id="1" name="1001" description="1001" level="1" kejiType="1" dataType="1" value="1" limitLevel="1" preId="0" 
	//posId="2" needItemId="900006,900001" needItemNum="100,2" costTime="10" />

	public int id;

	public int nameId;

	public int description;

	public int level;

	public int kejiType;

	public int dataType;

	public int value;

	public int limitLevel;

	public List<int> preId;

	public int posId;

	public List<int> needItemId;

	public List<int> needItemNum;

	public int costTime;


	public static List<KeJiTemplate> templates = new List<KeJiTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Keji.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "Keji" );
			
			if( !t_has_items ){
				break;
			}
			
			KeJiTemplate t_template = new KeJiTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.description = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.kejiType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.dataType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.value = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.limitLevel = int.Parse( t_reader.Value );

				{
					t_template.preId = new List<int>();
					
					t_reader.MoveToNextAttribute();
					string strPreId = t_reader.Value;
					
					string[] preIds = strPreId.Split(',');
					
					foreach(string s in preIds){
						t_template.preId.Add(int.Parse(s));
					}
				}

				t_reader.MoveToNextAttribute();
				t_template.posId = int.Parse( t_reader.Value );

				{
					t_template.needItemId = new List<int>();
					
					t_reader.MoveToNextAttribute();
					string needItemIdStr = t_reader.Value;
					
					string[] needItemIdList = needItemIdStr.Split(',');
					
					foreach(string needItemId in needItemIdList){
						t_template.needItemId.Add(int.Parse(needItemId));
					}
				}

				{
					t_template.needItemNum = new List<int>();
					
					t_reader.MoveToNextAttribute();
					string needItemNumStr = t_reader.Value;
					
					string[] needItemNumList = needItemNumStr.Split(',');
					
					foreach(string needItemNum in needItemNumList){
						t_template.needItemNum.Add(int.Parse(needItemNum));
					}
				}

				t_reader.MoveToNextAttribute();
				t_template.costTime = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static KeJiTemplate getKeJiTemplateByTypeAndLevel(int type, int level)
	{
		foreach(KeJiTemplate template in templates)
		{
			if(template.kejiType == type && template.level == level)
			{
				return template;
			}
		}

		Debug.LogError("XML ERROR: Can't get KeJiTemplate with type " + type + " and level " + level);

		return null;
	}

	public static KeJiTemplate getKeJiTemplateById(int id)
	{
		foreach(KeJiTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get KeJiTemplate with id " + id);
		
		return null;
	}

}

