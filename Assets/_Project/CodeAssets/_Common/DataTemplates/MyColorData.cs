using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MyColorData : XmlLoadManager
{
	public int id;
	
	public string color;

	public static List<MyColorData> templates= new List<MyColorData>();


	private static MyColorData m_instance = null;

	public static MyColorData Instance(){
		if( m_instance == null ){
			m_instance = new MyColorData();
		}

		return m_instance;
	}


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad( PathManager.GetUrl( m_LoadPath + "MyColorData.xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj){
		if( templates.Count > 0 ){
			return;
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
			t_has_items = t_reader.ReadToFollowing( "MyColorData" );
			
			if( !t_has_items ){
				break;
			}
			
			MyColorData t_template = new MyColorData();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.color = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

//		Color a = System.Drawing.ColorTranslator.FromHtml("#ff0000");
	}

	public static MyColorData getNameIdMyColorDataId(int nameId)
	{
		foreach( MyColorData template in templates )
		{
			if(template.id == nameId)
			{
				return template;
			}
		}
		return null;
	}

	public static string getColor( int id ){
		MyColorData t_color = getNameIdMyColorDataId( id );

		if( t_color != null ){
			return t_color.color;
		}
		else{
			return "000000";
		}
	}

	public static string getColorString(int id, string data)
	{
		MyColorData template = getNameIdMyColorDataId(id);
		return "[" + template.color + "]" + data + "[-]";
	}

	public static string getColorString(int id, int data)
	{
		MyColorData template = getNameIdMyColorDataId(id);
		return "[" + template.color + "]" + data + "[-]";
	}

	public static void getColorString(int id, UILabel data)
	{
		if(data = null)
		{
			return;
		}
		data.text = getColorString(id, data.text);
	}

	public static void getColorString(int id, GameObject data)
	{
		getColorString(id, data.GetComponent<UILabel>());
	}
}
