using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MiBaoSkillTemp : XmlLoadManager {

//	d="1" needNum="3" lv="1" skill="250101" skillDesc="391001" skill2="251201" skill2Desc="395001" nameId="651001" briefDesc="392001" detailDesc="394001" 
//		icon="250101" />
//		<MibaoSkill 
	public int id;
	
	public int needNum;

	public int lv;

	public int skill;
	
	public int skillDesc;

	public int skill2;

	public int skill2Desc;

	public int nameId;

	public int briefDesc;

	public int detailDesc;

	public string icon;

	public static List<MiBaoSkillTemp> templates = new List<MiBaoSkillTemp>();
	
	
	public void Log(){
		Debug.Log( "MiBaoSkillTemp.Log( id: " + id +
		          " awardId : " );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MibaoSkill.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );

//		Debug.Log ("j加载了ibaoSuiPian.xml");
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
			t_has_items = t_reader.ReadToFollowing( "MibaoSkill" );
			
			if( !t_has_items ){
				break;
			}
			
			MiBaoSkillTemp t_template = new MiBaoSkillTemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.needNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.lv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.skillDesc =  int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.skill2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill2Desc = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.briefDesc = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.detailDesc = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.icon = t_reader.Value;


			}

			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static MiBaoSkillTemp getMiBaoSkillTempByZuHe_Pinzhi(int mzuhe,int pz)
	{
		foreach( MiBaoSkillTemp template in templates )
		{
//			if( template.zuhe == mzuhe&& template.pinzhi == pz)
//			{
//				return template;
//			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoSkillTemp with tempid " + mzuhe+pz);
		
		return null;
	}
	public static MiBaoSkillTemp getMiBaoSkillTempBy_id(int id)
	{
		foreach( MiBaoSkillTemp template in templates )
		{
			if( template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoSkillTemp with tempid " + id);
		
		return null;
	}
	public static MiBaoSkillTemp getMiBaoSkillTempBy_pz_id(int pz,int id)
	{
		foreach( MiBaoSkillTemp template in templates )
		{
			//Debug.Log("template.skill = "+template.skill +"template.pinzhi = "+template.pinzhi);
			if( template.skill == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoSkillTemp with id and pz " + id+"pingz "+pz);
		
		return null;
	}

	public static MiBaoSkillTemp getMiBaoSkillTempByZuHeId (int zuHeId)
	{
		foreach( MiBaoSkillTemp template in templates )
		{
			//Debug.Log("template.skill = "+template.skill +"template.pinzhi = "+template.pinzhi);
			if( template.id == zuHeId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get getMiBaoSkillTempByZuHeId with zuHeId " + zuHeId);
		
		return null;
	}
}
