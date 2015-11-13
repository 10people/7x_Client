using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MiBaoXmlTemp : XmlLoadManager {


	
	//<AwardTemp id="800001" awardId="1" itemId="111001" itemType="0" itemNum="1" weight="100" />
	
	public int id;
	
	public int tempId;
	
	public int nameId;
	
	public int descId;
	
	public int icon;
	
	public int initialStar;

	public float initialGrow;

	public int pinzhi;

	public int zuheId;

	public int dengji;

	public float gongji;

	public float fangyu;

	public float shengming;

	public int gongjiRate;

	public int fangyuRate;

	public int shengmingRate;
	
	public int wqSH;

	public int wqJM;

	public int wqBJ;

	public int wqRX;

	public int jnSH;

	public int jnJM;

	public int jnBJ;

	public int jnRX;

	public int skill;

	public int maxLv;

	public int expId;

	public int suipianId;

	public int unlockType;

	public int unlockValue;

	public static List<MiBaoXmlTemp> templates = new List<MiBaoXmlTemp>();
	
	
	public void Log(){
		Debug.Log( "MiBaoXmlTemp.Log( id: " + id +
		          " awardId : " );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MiBao.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing( "MiBao" );
			
			if( !t_has_items ){
				break;
			}
			
			MiBaoXmlTemp t_template = new MiBaoXmlTemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.tempId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.descId = int.Parse( t_reader.Value );


				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.initialStar = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.initialGrow = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.pinzhi = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.zuheId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.dengji = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongji = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fangyu = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shengming = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongjiRate = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fangyuRate = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.shengmingRate = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.wqSH = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqJM = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqBJ = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqRX = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.jnSH = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jnJM = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jnBJ = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jnRX = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.maxLv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.expId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.suipianId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.unlockType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.unlockValue = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static MiBaoXmlTemp getMiBaoXmlTempById(int id)
	{
		foreach( MiBaoXmlTemp template in templates )
		{
			if( template.id == id )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoXmlTemp with id " + id);
		
		return null;
	}
	public static MiBaoXmlTemp getMiBaoXmlTempByPinZhi(int Pinz)
	{
		foreach( MiBaoXmlTemp template in templates )
		{
			if( template.pinzhi == Pinz )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoXmlTemp with id " + Pinz);
		
		return null;
	}
	public static MiBaoXmlTemp getMiBaoXmlTemp_By_tempId( int tempId_id )
	{
		foreach( MiBaoXmlTemp template in templates )
		{
			if( template.tempId == tempId_id )
			{
				return template;
			}
		}
		
		Debug.LogError( "MiBaoXmlTemp not found with award id: " + tempId_id );
		
		return null;
	}
}
