using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile.protobuf;

public class ControlOrderLvTemplate : XmlLoadManager
{
	//<BeatDownLv id="100" player="0" boss="0" hero="0" soldier="1" playerSkill="0" playerAttack="0" 
	//bossSkill="0" bossAttack="0" heroSkill="0" heroAttack="0" soldierSkill="1" soldierAttack="1" />

	public int id;

	public int player;

	public int boss;

	public int hero;

	public int soldier;

	public int playerSkill;

	public int playerAttack;

	public int bossSkill;

	public int bossAttack;

	public int heroSkill;

	public int heroAttack;

	public int soldierSkill;

	public int soldierAttack;


	public static List<ControlOrderLvTemplate> templates;


	public static void LoadTemplates(EventDelegate.Callback p_callback = null )
	{
		if(templates == null) templates = new List<ControlOrderLvTemplate>();
		
		else templates.Clear();
		
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ControlOrderLv.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj )
	{
		XmlReader t_reader = null;
		
		if( obj != null )
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
		}
		else
		{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "ControlOrderLv" );
			
			if( t_has_items == false ) break;
			
			ControlOrderLvTemplate t_template = new ControlOrderLvTemplate();

			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.player = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.boss = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.hero = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.soldier = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.playerSkill = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.playerAttack = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.bossSkill = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.bossAttack = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.heroSkill = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.heroAttack = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.soldierSkill = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.soldierAttack = int.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static bool haveBeatDownTemplateById(int id)
	{
		foreach(ControlOrderLvTemplate template in templates)
		{
			if(template.id == id)
			{
				return true;
			}
		}
		
		return false;
	}

	public static ControlOrderLvTemplate getBeatDownTemplateById(int id)
	{
		foreach(ControlOrderLvTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}

		Debug.LogError ("THERE IS NO ControlOrderLvTemplate WITH ID " + id);

		return null;
	}

	public static bool getCantrolableById(int templateId, AIdata.AttributeType controlType, BaseAI node)
	{
		ControlOrderLvTemplate template = getBeatDownTemplateById (templateId);

		int contrlLv = 0;

		if(node.nodeData.nodeType == NodeType.PLAYER)
		{
			KingControllor king = (KingControllor)node;

			if(king.isPlayingAttack()) contrlLv = template.playerAttack;

			else if(king.isPlayingSkill()) contrlLv = template.playerSkill;

			else contrlLv = template.player;
		}
		else if(node.nodeData.nodeType == NodeType.BOSS)
		{
			if(node.isPlayingAttack()) contrlLv = template.bossAttack;
			
			else if(node.isPlayingSkill()) contrlLv = template.bossSkill;
			
			else contrlLv = template.boss;
		}
		else if(node.nodeData.nodeType == NodeType.HERO)
		{
			if(node.isPlayingAttack()) contrlLv = template.heroAttack;
			
			else if(node.isPlayingSkill()) contrlLv = template.heroSkill;
			
			else contrlLv = template.hero;
		}
		else if(node.nodeData.nodeType == NodeType.SOLDIER)
		{
			if(node.isPlayingAttack()) contrlLv = template.soldierAttack;
			
			else if(node.isPlayingSkill()) contrlLv = template.soldierSkill;
			
			else contrlLv = template.soldier;
		}

		float defenderControl = node.nodeData.GetAttribute (controlType);

		if(node.nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_ArmorMax) > 0 && node.nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_Armor) <= 0)
		{
			if(node.nodeData.nodeType == NodeType.BOSS)
			{
				if(node.isPlayingAttack())
				{
					if(controlType == AIdata.AttributeType.ATTRTYPE_ReductionBTACDown)
					{
						return false;
					}
				}
			}
			defenderControl -= 1;
		}

		if (contrlLv - defenderControl > 0) return true;

		return false;
	}

}
