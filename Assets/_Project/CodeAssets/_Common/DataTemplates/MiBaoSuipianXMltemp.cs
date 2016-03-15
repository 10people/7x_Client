using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MiBaoSuipianXMltemp : XmlLoadManager {
	
	
	//<AwardTemp id="800001" awardId="1" itemId="111001" itemType="0" itemNum="1" weight="100" />
	
	public int id;
	
	public int m_name;

	public int funDesc;

	public int icon;

	public int initialStar;

	public int hechengNum;

	public int expId;

	public int money;

	
	public int fenjieNum;
	
	public int tempId;
	
	public int recyclePrice;

	public string mibaoPath;
	public static List<MiBaoSuipianXMltemp> templates = new List<MiBaoSuipianXMltemp>();
	
	
	public void Log(){
		Debug.Log( "MiBaoSuipianXMltemp.Log( id: " + id +
		          " awardId : " );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MibaoSuiPian.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "MibaoSuiPian" );
			
			if( !t_has_items ){
				break;
			}
			
			MiBaoSuipianXMltemp t_template = new MiBaoSuipianXMltemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_name = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.funDesc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.initialStar = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.hechengNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.money = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.fenjieNum = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.tempId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.recyclePrice = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.mibaoPath =  t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static MiBaoSuipianXMltemp getMiBaoSuipianXMltempBytempid(int tempid)
	{
		foreach( MiBaoSuipianXMltemp template in templates )
		{
			if( template.tempId == tempid )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoSuipianXMltemp with tempid " + tempid);
		
		return null;
	}

	public static MiBaoSuipianXMltemp getMiBaoSuipianXMltempById(int id)
	{
		foreach( MiBaoSuipianXMltemp template in templates )
		{
			if( template.id == id )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoSuipianXMltemp with id " + id);
		
		return null;
	}
	
//	public static MiBaoSuipianXMltemp getMiBaoSuipianXMltemp_By_expId( int expId_id,int lv )
//	{
//		foreach( MiBaoSuipianXMltemp template in templates )
//		{
//			if( template.expId == expId_id &&template.level == lv)
//			{
//				return template;
//			}
//		}
		
//		Debug.LogError( "MiBaoSuipianXMltemp not found with award id: " + expId_id );
//		
//		return null;
//	}
}
