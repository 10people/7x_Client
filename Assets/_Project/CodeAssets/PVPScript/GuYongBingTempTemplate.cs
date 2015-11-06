using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class GuYongBingTempTemplate : XmlLoadManager {

	//<GuYongBing id="401001" icon="1" zhiye="11" renshu="2" modelId="2002" name="701001" 
	//description="701001" needLv="1" quality="1" level="1" gongjiType="21" gongji="28" 
	//fangyu="17" shengming="1800" wqSH="0" wqJM="50" wqBJ="0" wqRX="0" jnSH="0" jnJM="0" 
	//jnBJ="0" jnRX="0" skills="211001,211002" skill1="211001" skill2="211002" skill3="0" 
	//skill4="0" skill5="0" power="375" />


	public int id;
	
	
	public int icon;
	
	public int zhiye;

	public int renshu;

	public int modelId;

	public int m_name;
	
	
	public int description;
	
	public int needLv;
	
	public int quality;
	
	
	public int level;
	
	public int gongjiType;
	
	public int gongji;
	
	public int fangyu;
	
	public int shengming;
	
	public int wqSH;
	
	public int wqJM;
	
	
	public int wqBJ;
	
	public int wqRX;
	
	public int jnSH;
	
	public int jnJM;
	
	public int jnBJ;
	
	public int jnRX;

	public string skills;

	public int skill1;
	
	public int skill2;
	
	public int skill3;
	
	public int skill4;
	
	
	public int skill5;
	
	public int power;

	public int profession;

	public int type;
	
	
	private static List<GuYongBingTempTemplate> templates = new List<GuYongBingTempTemplate>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "GuYongBing.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;
	
	public static void CurLoad(ref WWW www, string path, Object obj){
		if ( obj == null) {
			Debug.LogError ( "Asset Not Exist: " + path );
			
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
			t_has_items = t_reader.ReadToFollowing( "GuYongBing" );
			
			if( !t_has_items ){
				break;
			}
			
			GuYongBingTempTemplate t_template = new GuYongBingTempTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.zhiye = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.renshu = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.modelId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.m_name = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.description = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.needLv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.quality = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongjiType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongji = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fangyu = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shengming = int.Parse( t_reader.Value );
				
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
				t_template.skills = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.skill1 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill2 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill3 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill4 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill5 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.power = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.profession = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static bool HaveTemplateById( int p_id ){
		{
			ProcessAsset();
		}

		for( int i = 0; i < templates.Count; i++ ){
			GuYongBingTempTemplate t_item = templates[ i ];
			
			if( t_item.id == p_id ){
				
				return true;
			}
		}

		return false;
	}

	public static GuYongBingTempTemplate GetGuYongBingTempTemplate_By_id( int m_id ){
		{
			ProcessAsset();
		}

		for( int i = 0; i < templates.Count; i++ ){
			GuYongBingTempTemplate t_item = templates[ i ];
			
			if( t_item.id == m_id ){

				return t_item;
			}
		}
		
		Debug.LogError( "m_id "+ m_id + " : not found." );
		
		return null;
	}



}
