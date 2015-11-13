using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MiBaoSkillTemp : XmlLoadManager {

//	<MibaoSkill zuhe="1" pinzhi="2" skill="250101" shuxingDesc="391001" skill2="251201" 
//		desc1="399001" value1="10%" desc2="0" value2="20%" desc3="0" value3="30%" desc4="0"
//			value4="0" nameId="651001" zuheDesc="392001" SkillSummary="393001" SkillDetail="394001" icon="250101" />
	public int zuhe;
	
	public int pinzhi;
	
	public int skill;
	
	public int shuxingDesc;

	public string skill2;

	public int desc1;

	public string value1;

	public int desc2;

	public string value2;

	public int desc3;

	public string value3;

	public int desc4;

	public string value4;

	public int nameId;

	public int zuheDesc;

	public int SkillSummary;

	public int SkillDetail;

	public int icon;

	public static List<MiBaoSkillTemp> templates = new List<MiBaoSkillTemp>();
	
	
	public void Log(){
		Debug.Log( "MiBaoSkillTemp.Log( id: " + zuhe +
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
				t_template.zuhe = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.pinzhi = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.shuxingDesc = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.skill2 =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.desc1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.value1 = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.desc2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.value2 = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.desc3 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.value3 = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.desc4 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.value4 = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.zuheDesc = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.SkillSummary = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.SkillDetail = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );

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
			if( template.zuhe == mzuhe&& template.pinzhi == pz)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoSkillTemp with tempid " + mzuhe+pz);
		
		return null;
	}
	public static MiBaoSkillTemp getMiBaoSkillTempBy_id(int id)
	{
		foreach( MiBaoSkillTemp template in templates )
		{
			if( template.skill == id)
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
			if( template.zuhe == zuHeId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get getMiBaoSkillTempByZuHeId with zuHeId " + zuHeId);
		
		return null;
	}
}
