using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FunctionUnlock : XmlLoadManager
{
	//<EffectIdGroup Id="1" Group="1000|1010" Count="2" Time="0.8" />
	public int id;

	public string des1;

	public string spriteName;

	public string des2;

	public int rank;

	public int desID;
	
	public static List<FunctionUnlock> templates = new List<FunctionUnlock>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FunctionUnlock.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj )
	{
		templates.Clear();
		
		XmlReader t_reader = null;
		
		if( obj != null )
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else
		{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "FunctionUnlock" );
			
			if( !t_has_items ) break;
			
			{
				FunctionUnlock template = new FunctionUnlock();
				
				t_reader.MoveToNextAttribute();
				template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				template.des1 = t_reader.Value;

				t_reader.MoveToNextAttribute();
				template.spriteName = t_reader.Value;

				t_reader.MoveToNextAttribute();
				template.des2 = t_reader.Value;

				t_reader.MoveToNextAttribute();
				template.rank = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				template.desID = int.Parse(t_reader.Value);

				templates.Add( template );
			}
		}
		while( t_has_items );
	}
	
	public static FunctionUnlock getGroudById(int effectId)
	{
		foreach(FunctionUnlock eig in templates)
		{
			if(eig.id == effectId)
			{
				return eig;
			}
		}
		return null;
	}
	
}
