using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class GongJiTypeTemplate : XmlLoadManager
{
	//<GongjiType typeId="1" moveSpeed="6" attackSpeed="1" attackRange="4" shiyeRange="12" 
	//baojiX="10" baojiY="2" jnbaojiX="10" jnbaojiY="2" />

	public int typeId;

	public float moveSpeed;

	public int attackSpeed;

	public int attackRange;

	public int shiyeRange;

	public int baojiX;

	public int baojiY;

	public int jnbaojiX;

	public int jnbaojiY;


	public static List<GongJiTypeTemplate> templates= new List<GongJiTypeTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "GongjiType.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "GongjiType" );
			
			if( !t_has_items ){
				break;
			}
			
			GongJiTypeTemplate t_template = new GongJiTypeTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.typeId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.moveSpeed = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.attackSpeed = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.attackRange = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shiyeRange = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.baojiX = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.baojiY = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jnbaojiX = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.jnbaojiY = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static GongJiTypeTemplate getGongJiTypeTemplateByTypeId(int typeId)
	{
		foreach(GongJiTypeTemplate template in templates)
		{
			if(template.typeId == typeId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get GongJiTypeTemplate with typeId " + typeId);
		
		return null;
	}

}
