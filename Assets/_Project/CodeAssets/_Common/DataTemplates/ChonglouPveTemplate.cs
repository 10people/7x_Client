using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

//id="400001" layer="1" Name="420001" Desc="420001" monarchLevel="0" awardId="810101=100" firstAwardID="810101=100" npcId="400010"
//	power="0" icon="0" soundId="300101" sceneId="100" time="120" maxTime="3" RenWuLimit="0" configId="300101" frontPoint="0" 
//		wanfaType="休闲模式" recZhanli="3000" recMibaoSkill="250101" />

public class ChonglouPveTemplate : XmlLoadManager {

	public int id;

	public int layer;
	
	public int Name;
	
	public int Desc;
	
	public int monarchLevel;
	
	public string awardId;
	
	public string firstAwardID;

	public int npcId;
	
	public int power;
	
	public int icon;
	
	public int soundId;
	
	public int sceneId;
	
	public int time;
	
	public int maxTime;

	public string RenWuLimit;
	
	public int  configId;
	
	public int frontPoint;
	
	public string wanfaType;
	
	public int recZhanli;
	
	public string recMibaoSkill;

	public int previewable;

	public string awardShow;
	private static List<ChonglouPveTemplate> templates = new List<ChonglouPveTemplate>();
	
	public void Log(){
		Debug.Log( "ChonglouPveTemplate-  id: " + id +
		          " bigName: " + layer + 
		          " smaName: " + Name );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ChonglouPveTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	private static TextAsset m_templates_text = null;
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		if ( obj == null ) {
			Debug.LogError ("Asset Not Exist: " + path);
			
			return;
		}
		
		m_templates_text = obj as TextAsset;
	}
	
	private static void ProcessAsset(){
		if( templates.Count > 0 ) {
			return;
		}
		
		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );
			
			return;
		}
		
		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "ChonglouPveTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			ChonglouPveTemplate t_template = new ChonglouPveTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.layer = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.Name = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.Desc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.monarchLevel = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.awardId =  t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.firstAwardID =  t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.npcId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.power = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.soundId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.sceneId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.time = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.maxTime = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.RenWuLimit =   t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.configId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.frontPoint = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wanfaType =  t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.recZhanli =  int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.recMibaoSkill =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.previewable =  int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.awardShow =  t_reader.Value ;


			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
		
		{
			m_templates_text = null;
		}
	}
	public static ChonglouPveTemplate Get_QCL_PVETemplate_By_Layer( int Layer ){
		{
			ProcessAsset();	
		}
		
		for( int i = 0; i < templates.Count; i++ ){
			ChonglouPveTemplate t_item = templates[ i ];
			
			if( t_item.layer == Layer ){
				return t_item;
			}
		}
		
		Debug.LogError( "ChonglouPveTemplate not found: " + Layer  );
		
		return null;
	}
	public static List<int > Get_Key_Layer(  ){
		{
			ProcessAsset();	
		}
		List<int > KeyList = new List<int> ();
		for( int i = 0; i < templates.Count; i++ ){
			ChonglouPveTemplate t_item = templates[ i ];
			
			if( t_item.previewable == 1 ){
				KeyList.Add(t_item.layer);
			}
		}

		return KeyList;
	}
}
