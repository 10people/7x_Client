using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class LianMengKeJiTemplate : XmlLoadManager {

//	<LianMengKeJi id="101001" name="攻击" level="1" shuYuanlvNeeded="1" lvUpValue="250" desc="增加君主**点攻击" card="1" type="101" 
//		value1="20" value2="-1" />

	public int id;

	public string name;

	public int  level;

	public int shuYuanlvNeeded;

	public int  lvUpValue;

	public string desc;

	public int card;

	public int type;

	public int  value1;

	public int  value2;

	public int  Icon;

	public static List<LianMengKeJiTemplate> templates = new List<LianMengKeJiTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianMengKeJi.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LianMengKeJi" );
			
			if( !t_has_items ){
				break;
			}
			
			LianMengKeJiTemplate t_template = new LianMengKeJiTemplate();
			
			{

				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.name = t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.shuYuanlvNeeded = int.Parse( t_reader.Value );
								
				t_reader.MoveToNextAttribute();
				t_template.lvUpValue = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.desc =  t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.card = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.value1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.value2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.Icon = int.Parse( t_reader.Value );
				
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static LianMengKeJiTemplate GetLianMengKeJiTemplate_by_Type_And_Level(int type, int mLevel)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].type == type&& templates[i].level == mLevel)
			{
				return templates[i];
			}
		}
		return null;
		
	}
	public static List<LianMengKeJiTemplate> mLianMengKeJiTemplateList = new List<LianMengKeJiTemplate> ();
	public static List<LianMengKeJiTemplate> GetLianMengKeJiTemplate_by_type()
	{
		mLianMengKeJiTemplateList.Clear ();
		for (int i = 0; i < templates.Count; i++)
		{
			bool save = false ;
			for (int j = 0; j < mLianMengKeJiTemplateList.Count; j++)
			{
				if(mLianMengKeJiTemplateList[j].type == templates[i].type)
				{
					save = true;
					break;
				}
			}
			if(!save)
			{
				mLianMengKeJiTemplateList.Add( templates[i]);
			}
		}
		return mLianMengKeJiTemplateList;	
	}
}
