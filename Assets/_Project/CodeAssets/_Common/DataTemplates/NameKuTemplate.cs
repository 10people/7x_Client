using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class NameKuTemplate : XmlLoadManager {

	public int type;

	public string m_name;

	public static List<string> nameList1 = new List<string> ();

	public static List<string> nameList2 = new List<string> ();

	public static List<string> nameList3 = new List<string> ();

	public static List<NameKuTemplate> nameKuTemplates = new List<NameKuTemplate> ();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "NameKu.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	public static void CurLoad( ref WWW www, string path, Object obj ){
		{
			nameKuTemplates.Clear ();
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
			t_has_items = t_reader.ReadToFollowing( "NameKu" );
			
			if( !t_has_items ){
				break;
			}
			
			NameKuTemplate t_template = new NameKuTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_name = t_reader.Value;
			}
			
			//			t_template.Log();
			
			nameKuTemplates.Add( t_template );
		}
		while( t_has_items );

		
		nameList1.Clear ();
		nameList2.Clear ();
		nameList3.Clear ();
		GetNameListByType (1);
		
		GetNameListByType (2);
		
		GetNameListByType (3);
	}

	//通过type查找姓名
	public static void GetNameListByType (int n_type) {

		for( int i = 0; i < nameKuTemplates.Count; i++ ){

			NameKuTemplate nameKu = nameKuTemplates[ i ];

			if( nameKu.type == n_type ){
				
				switch (n_type) {

				case 1:

					nameList1.Add (nameKu.m_name);

					break;

				case 2:

					nameList2.Add (nameKu.m_name);

					break;

				case 3:

					nameList3.Add (nameKu.m_name);

					break;
				}
			}
		}
		
//		Debug.Log( "随机名字库");
	}
}
