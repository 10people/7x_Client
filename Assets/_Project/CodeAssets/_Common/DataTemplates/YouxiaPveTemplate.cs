using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class YouxiaPveTemplate : XmlLoadManager 
{
	//<YouxiaPveTemp id="300101" bigName="300100" bigDesc="300100" bigId="1" smaName="300101" 
	//smaDesc="300101" smaId="1" monarchLevel="10" awardId="810101=100" npcId="301010" power="1000" 
	//icon="0" soundId="300101" sceneId="300101" time="240" maxTime="0" RenWuLimit="0" 
	//configId="300101" />

	public int id;

	public int bigName;

	public int bigDesc;

	public int bigId;

	public int smaName;

	public int smaDesc;

	public int smaId;

	public int monarchLevel;

	public string awardId;

	public int npcId;

	public int power;

	public int icon;

	public int soundId;

	public int sceneId;

	public int time;

	public int maxTime;

	public int RenWuLimit;

	public int configId;

	public string openDay;

	public int frontPoint;

	public int maxNum; // wanfaType="百战" recZhanli="1" recMibaoSkill="250101" vicConDescID="1525" />

	public string wanfaType;

	public int recZhanli;

	public string recMibaoSkill;

	public int vicConDescID;

	public string awardShow;

	public static List<YouxiaPveTemplate> templates = new List<YouxiaPveTemplate>();


	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "YouxiaPveTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj)
	{
		templates.Clear();

		XmlReader t_reader = null;
		
		if (obj != null)
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create(new StringReader(t_text_asset.text));
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else
		{
			t_reader = XmlReader.Create(new StringReader(www.text));
		}
		
		bool t_has_items = true;
		
		do
		{
			t_has_items = t_reader.ReadToFollowing("YouxiaPveTemp");
			
			if (!t_has_items)
			{
				break;
			}
			
			YouxiaPveTemplate t_template = new YouxiaPveTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.bigName = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.bigDesc = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.bigId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.smaName = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.smaDesc = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.smaId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.monarchLevel = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.awardId = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.npcId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.power = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.soundId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.sceneId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.time = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.maxTime = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.RenWuLimit = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.configId = int.Parse(t_reader.Value);


				t_reader.MoveToNextAttribute();
				t_template.openDay = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.frontPoint = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.maxNum = int.Parse(t_reader.Value);


				t_reader.MoveToNextAttribute();
				t_template.wanfaType = t_reader.Value;
			

				t_reader.MoveToNextAttribute();
				t_template.recZhanli = int.Parse(t_reader.Value);
			

				t_reader.MoveToNextAttribute();
				t_template.recMibaoSkill = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.vicConDescID = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.awardShow = t_reader.Value;

				templates.Add(t_template);
			}
		}
		while (t_has_items);
	}

	public static YouxiaPveTemplate getYouXiaPveTemplateById( int id )
	{
		foreach( YouxiaPveTemplate template in templates ){
			if( template.id == id )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get YouxiaPveTemplate with id " + id);
		
		return null;
	}
	public static YouxiaPveTemplate getYouXiaPveTemplateBy_BigId( int bigid )
	{
		foreach( YouxiaPveTemplate template in templates ){
			if( template.bigId == bigid )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get YouxiaPveTemplate with bigid " + bigid);
		
		return null;
	}

	public static List<YouxiaPveTemplate> getYouXiaPveTemplateListBy_BigId( int bigid )
	{
		List<YouxiaPveTemplate> mYouxiaPveTemplateList = new List<YouxiaPveTemplate> ();

		foreach( YouxiaPveTemplate template in templates ){

			if( template.bigId == bigid )
			{
				mYouxiaPveTemplateList.Add( template);
			}
		}
		
		return mYouxiaPveTemplateList;
	}
}
