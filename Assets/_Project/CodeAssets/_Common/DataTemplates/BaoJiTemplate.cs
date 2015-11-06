using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BaoJiTemplate : XmlLoadManager {

	public int id;

	public int type;

	public  List<int> beishu = new List<int>();

	public  List<int> gailv = new List<int>(); 

	public static List<BaoJiTemplate> templates = new List<BaoJiTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "BaoJi.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){
		{
			templates.Clear();
		
			ClearData();
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
			t_has_items = t_reader.ReadToFollowing( "BaoJi" );
			
			if( !t_has_items ){
				break;
			}
			
			BaoJiTemplate t_template = new BaoJiTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				{
					string tempBeiShu = t_reader.Value;
					
					string[] tempBeiShuStr = tempBeiShu.Split(new char[]{','});
					
					foreach( string tempString in tempBeiShuStr ){
						t_template.beishu.Add( int.Parse( tempString ) );
					}
				}

				t_reader.MoveToNextAttribute();
				{
					string tempGaiLv = t_reader.Value;
					
					string[] tempGaiLvStr = tempGaiLv.Split(new char[]{','});
					
					foreach( string tempString in tempGaiLvStr ){
						t_template.gailv.Add( int.Parse(tempString) );
					}
				}
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static void ClearData()
	{
		foreach(BaoJiTemplate template in templates)
		{
			template.beishu.Clear();
			template.gailv.Clear();
		}
	}

	public static int GetBaoJiNumByRandom(int tempRandom)
	{
		return templates[0].beishu[tempRandom];
	}
}
