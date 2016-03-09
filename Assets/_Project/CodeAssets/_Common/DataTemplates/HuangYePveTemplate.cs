//#define DEBUG_TEMPLATE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HuangYePveTemplate : XmlLoadManager {
//	<HuangyePve id="100001" lv="1" nameId="720001" descId="720001" icon="720001" condition="4" openCost="5000" npcId="201010" award="700101=100,700101=100" rank1Award="180" 
	//rank2Award="160" rank3Award="140" rank4Award="120" rank5Award="100" fastAward="30" killAward="50" soundId="100001" sceneId="406" power="284921" configId="1" positionX="1"
	//	positionY="2" />
//		<

	public int id;
	
	public int lv;
	
	public int nameId;
	
	public int descId;
	
	public int icon;
	
	public string condition;
	
	public int openCost;
	
	public int npcId;
	
	public string award;

	public int rank1Award;

	public int rank2Award;

	public int rank3Award;

	public int rank4Award;

	public int rank5Award;

	public string fastAward;

	public int killAward;

	public int soundId;

	public int sceneId;

	public int power;

	public int configId;

	public int positionX;

	public int positionY; //pveId="101509" paraK="2193" />

	public int pveId;

	public int paraK;

	public string recMibaoSkill;

	public static List<HuangYePveTemplate> templates = new List<HuangYePveTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuangyePve.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	public static void CurLoad( ref WWW p_www, string p_path, Object p_object ){
		{
			templates.Clear ();
		}

		XmlReader t_reader = null;
		
		if(p_object != null){
			TextAsset t_text_asset = p_object as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( p_www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "HuangyePve" );
			
			if( !t_has_items ){
				break;
			}
			
			HuangYePveTemplate t_template = new HuangYePveTemplate();
			
			{

				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.lv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.descId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.condition =  t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.openCost = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.npcId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.award =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.rank1Award = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.rank2Award = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.rank3Award = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.rank4Award = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.rank5Award = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.fastAward =  t_reader.Value ;
			
				t_reader.MoveToNextAttribute();
				t_template.killAward = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.soundId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.sceneId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.power = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.configId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.positionX = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.positionY = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.pveId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.paraK = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.recMibaoSkill =  t_reader.Value ;
				#if DEBUG_TEMPLATE
				Log( t_template );
				#endif
			}
			
			templates.Add(t_template);
		}
		while( t_has_items );
	}

	public static void Log( HuangYePveTemplate p_data ){

	}

	
	public static HuangYePveTemplate getHuangYePveTemplatee_byid(int id)
	{
		foreach(HuangYePveTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HuangYePveTemplate with id " + id);
		
		return null;
	}
}
