using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HeroGrowTemplate : XmlLoadManager
{
	//<HeroGrow id="30001" heroId="1" star="0" wqSH="0" wqJM="0" wqBJ="0" 
	//wqRX="0" jnSH="0" jnJM="0" jnBJ="0" jnRX="0" gongji="80" fangyu="60" 
	//shengming="1600" gongjijiacheng="0" fangyujiacheng="0" 
	//shengmingjiacheng="0" chengzhang="1" skills="" skill1="0" skill2="0" 
	//skill3="0" skill4="0" skill5="0" shiBingId="0" starId="0" />


	public int id;

	public int heroId;
	
	public int star;

	public int wqSH;

	public int wqJM;

	public int wqBJ;

	public int wqRX;
	
	public int jnSH;
	
	public int jnJM;
	
	public int jnBJ;
	
	public int jnRX;
	
	public int gongji;
	
	public int fangyu;
	
	public int shengming;
	
	public float gongjijiacheng;
	
	public float fangyujiacheng;
	
	public float shengmingjiacheng;
	
	public float chengzhang;

	public string skills;

	public int skill1;

	public int skill2;

	public int skill3;

	public int skill4;

	public int skill5;

	public int shiBingId;

	public int starId;


	private static List<HeroGrowTemplate> templates = new List<HeroGrowTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HeroGrow.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;
	
	public static void CurLoad(ref WWW www, string path, Object obj){
		if ( obj == null) {
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
			t_has_items = t_reader.ReadToFollowing( "HeroGrow" );
			
			if( !t_has_items ){
				break;
			}
			
			HeroGrowTemplate t_template = new HeroGrowTemplate();
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.heroId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.star = int.Parse( t_reader.Value );
				
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
				t_template.gongji = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.fangyu = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.shengming = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongjijiacheng = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fangyujiacheng = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shengmingjiacheng = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.chengzhang = float.Parse( t_reader.Value );
				
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
				t_template.shiBingId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.starId = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static bool HaveTemplateWithId( int p_id ){
		{
			ProcessAsset();
		}

		foreach( HeroGrowTemplate template in templates ){
			if( template.id == p_id ) {
				return true;
			}
		}

		return false;
	}

	public static HeroGrowTemplate getTemplateById( int id ){
		{
			ProcessAsset();
		}

		foreach(HeroGrowTemplate template in templates){
			if(template.id == id){
				return template;
			}
		}

		Debug.LogError("XML ERROR: Can't get HeroGrowTemplate with id " + id);

		return null;
	}

	public static HeroGrowTemplate getTemplateByHeroIdAndStar(int heroId, int star){
		{
			ProcessAsset();
		}

		foreach(HeroGrowTemplate template in templates)
		{
			if(template.heroId == heroId && template.star == star)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HeroGrowTemplate with heroId " + heroId + ", and star " + star);
		
		return null;
	}

}
