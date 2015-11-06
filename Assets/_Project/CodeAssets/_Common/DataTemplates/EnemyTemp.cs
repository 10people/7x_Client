using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class EnemyTemp : XmlLoadManager
{
	//<EnemyTemp id="101011" icon="1" zhiye="11" name="401011" description="401011" quality="1" level="1" gongjiType="11" gongji="59" 
	//fangyu="18" shengming="475" shuxing1="0" shuxing2="0" shuxing3="0" skill1="0" skill2="0" skill3="0" skill4="0" skill5="0" 
	//shiBingId="0" power="855" />


	public int id;

	public int icon;

	public int zhiye;

	public int enemyName;

	public int description;

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

	public int skill1;

	public int skill2;

	public int skill3;

	public int skill4;

	public int skill5;

	public int shiBingId;

	public string YuanSu;

	public int power;


	private static List<EnemyTemp> m_templates = new List<EnemyTemp>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "EnemyTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;

	public static void CurLoad( ref WWW p_www, string p_path, Object p_object ){
		if( p_object == null ) {
			Debug.LogError( "Asset Not Exist: " + p_path );

			return;
		}

		m_templates_text = p_object as TextAsset;
	}

	private static void ProcessAsset(){
		if( m_templates.Count > 0 ) {
			return;
		}

		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );

			return;
		}

		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );

		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "EnemyTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			EnemyTemp t_template = new EnemyTemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.zhiye = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.enemyName = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.description = int.Parse( t_reader.Value );
				
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
				// skills
				
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
				t_template.YuanSu = t_reader.Value;
				
				//_template.shiBingId = int.Parse(_element.GetAttribute("shiBingId"));
				
				t_reader.MoveToNextAttribute();
				t_template.power = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			m_templates.Add(t_template);
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static EnemyTemp getEnemyTempById( int id ){
		{
			ProcessAsset();
		}

		foreach(EnemyTemp template in m_templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get EnemyTemp with id " + id);
		
		return null;
	}

	public void Log(){
		Debug.Log( "EnemyTemp id: " + id +
		          " icon: " + icon + 
		          " zhiye: " + zhiye +
		          " qulaity: " + quality +
		          " level: " + level + 
		          " skill1: " + skill1 + 
		          " skill2: " + skill2 + 
		          " YuanSu: " + YuanSu +
		          " power: " + power );
	}
}
