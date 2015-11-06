using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HeroProtoTypeTemplate : XmlLoadManager
{
	//<HeroProtoType  tempId="700513" heroId="513" icon="513" heroType="11" 
	//heroName="10513" description="10513" quality="5" gongjiType="11" sex="1" 
	//country="1" label="80204,82404,83001" yuansu="2,3,4" jingpoId="930513" />

	public int tempId;

	public int heroId;

	public int icon;

	public int modelId;

	public int heroType;

	public int heroName;

	public int description;

	public int quality;

	public int gongjiType;

	public int sex;

	public int country;

	public List<int> label;

	public List<int> yuansu;

	public int jingpoId;

	
	public static List<HeroProtoTypeTemplate> templates = new List<HeroProtoTypeTemplate>();


	public void Log()
	{
		Debug.Log( "HeroProtoTypeTemplate( tempId: " + tempId +
		          " heroId: " + heroId +
		          " icon: " + icon +
		          " modelId: " + modelId +
		          " heroType: " + heroType +
		          " heroName: " + heroName +
		          " description: " + description +
		          " quality: " + quality );
	}


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HeroProtoType.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "HeroProtoType" );
			
			if( !t_has_items ){
				break;
			}
			
			HeroProtoTypeTemplate t_template = new HeroProtoTypeTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.tempId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.heroId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.modelId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.heroType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.heroName = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.description = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.quality = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.gongjiType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.sex = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.country = int.Parse( t_reader.Value );

				t_template.label = new List<int>();

				t_reader.MoveToNextAttribute();
				string strLabel = t_reader.Value;
				
				string[] labels = strLabel.Split(',');
				
				foreach(string s in labels)
				{
					t_template.label.Add(int.Parse(s));
				}
				
				t_template.yuansu = new List<int>();

				t_reader.MoveToNextAttribute();
				string strYuanSu = t_reader.Value;

				string[] yuansus = strYuanSu.Split(',');
				
				foreach(string s in yuansus)
				{
					int yuansuId = int.Parse(s);
					
					t_template.yuansu.Add(yuansuId);
				}

				t_reader.MoveToNextAttribute();
				t_template.jingpoId = int.Parse( t_reader.Value );
				
				templates.Add( t_template );
			}
		}
		while( t_has_items );
	}
	//通过tempId得到iconId
	public static int GetHeroProtoIconIdBytempId(int tempId)
	{
		foreach(HeroProtoTypeTemplate template in templates)
		{
			if(template.tempId == tempId)
			{
				return template.icon;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HeroIconId with tempId " + tempId);

		return tempId;
	} 
	//通过tempId得到精魄品质Id
	public static int GetHeroProtoJingPoPinZhiBytempId(int tempId)
	{
		foreach(HeroProtoTypeTemplate template in templates)
		{
			if(template.tempId == tempId)
			{
				return template.quality;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HeroJingPinZhi with tempId " + tempId);
		
		return tempId;
	} 
	//通过tempId得到HeroNameId
	public static int GetHeroProtoHeroNameBytempId(int tempId)
	{
		foreach(HeroProtoTypeTemplate template in templates)
		{
			if(template.tempId == tempId)
			{
				return template.heroName;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HeroName with tempId " + tempId);
		
		return tempId;
	} 

	public static HeroProtoTypeTemplate getHeroProtoTypeByTempId(int tempId)
	{
		foreach(HeroProtoTypeTemplate template in templates)
		{
			if(template.tempId == tempId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HeroProtoType with tempId " + tempId);
		
		return null;
	}

	public static HeroProtoTypeTemplate getHeroProtoTypeByHeroId(int heroId)
	{
		foreach(HeroProtoTypeTemplate template in templates)
		{
			if(template.heroId == heroId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HeroProtoType with heroId " + heroId);
		
		return null;
	}

}
